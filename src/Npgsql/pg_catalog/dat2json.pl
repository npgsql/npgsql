#!/usr/bin/env perl
use strict;
use warnings;
use File::Basename;
use MIME::Base64;

# sudo apt-get install libwww-perl
use LWP::UserAgent;

# sudo apt-get install libjson-perl
use JSON;

# sudo apt-get install libnet-github-perl
use Net::GitHub::V4;

# Update this once in a while if there is a
# newer PostgreSQL major release
use constant LATEST_KNOWN_POSTGRES_MAJOR => 14;

# The URL template in sprintf format where parameter index 1 is the file name and parameter index 2 is the branch
#use constant URL_TEMPLATE => 'https://git.postgresql.org/gitweb/?p=postgresql.git;a=blob_plain;f=src/include/catalog/%1$s;hb=refs/heads/%2$s';
use constant URL_TEMPLATE => 'https://raw.githubusercontent.com/postgres/postgres/%2$s/src/include/catalog/%1$s';

# The template for the stable PostgreSQL git branch to pull from in sprintf format
use constant BRANCH_TEMPLATE => 'REL_%u_STABLE';

# The the names of the catalog initialization files the in the PostgreSQL repository
use constant PG_RANGE_CATALOG_FILE_NAME => 'pg_range.dat';
use constant PG_TYPE_CATALOG_FILE_NAME => 'pg_type.dat';

# Positive lists of the keys we are interested in so that we can trim down our generated json files to the bare minimum.
# This avoids creating PRs for .dat file changes that don't really affect our behavior
use constant PG_RANGE_KEYS => qw(rngtypid rngsubtype rngmultitypid);
use constant PG_TYPE_KEYS => qw(oid typname typtype);

die "Please pass the access token" unless defined $ARGV[0];
run();

sub run {
    my $access_token = shift(@ARGV);
    my ($pg_range, $pg_type, $latestBranch, $pg_range_url, $pg_type_url, $httpResponse);
    my $ua = LWP::UserAgent->new;
    $ua->agent("Npgsql catalog updater");

    # Try to get the .dat files latest stable PostgreSQL release by trying urls while incrementing the major version
    # starting at LATEST_KNOWN_POSTGRES_MAJOR
    for (my $i = LATEST_KNOWN_POSTGRES_MAJOR;$i < 100;$i++) {
        my $currentBranch = sprintf(BRANCH_TEMPLATE, $i);
        $pg_range_url = sprintf(URL_TEMPLATE, PG_RANGE_CATALOG_FILE_NAME, $currentBranch);
        my $req = HTTP::Request->new(GET => $pg_range_url);
        $httpResponse = $ua->request($req);
        last unless $httpResponse->is_success;
        $pg_range = $httpResponse->decoded_content;

        $pg_type_url = sprintf(URL_TEMPLATE, PG_TYPE_CATALOG_FILE_NAME, $currentBranch);
        $req = HTTP::Request->new(GET => $pg_type_url);
        $httpResponse = $ua->request($req);
        last unless $httpResponse->is_success;
        $pg_type = $httpResponse->decoded_content;
        $latestBranch = $currentBranch;
    }

    die "Failed to get ${\PG_RANGE_CATALOG_FILE_NAME} from $pg_range_url: ${\$httpResponse->status_line}" unless defined $pg_range;
    die "Failed to get ${\PG_TYPE_CATALOG_FILE_NAME} from $pg_type_url: ${\$httpResponse->status_line}" unless defined $pg_type;

    $pg_type = filter_dat_entries($pg_type, PG_TYPE_KEYS);
    $pg_range = filter_dat_entries($pg_range, PG_RANGE_KEYS);

    my $json = JSON->new->allow_nonref;
    $json->canonical();

    my $newJson = $json->pretty->encode( { pg_range => $pg_range, pg_type => $pg_type } );
    my $catalogs_json_path = sprintf("%s/catalogs.json", dirname(__FILE__));
    open(my $fh, '+<:raw', $catalogs_json_path) or die "Failed to open '$catalogs_json_path': $!";
    read $fh, my $oldJson, -s $fh;
    if ($newJson ne $oldJson) {
        print "Detected changes in $latestBranch.\n";
        truncate $fh, 0;
        seek $fh, 0, 0;
        print $fh $newJson;
        commit_and_pr($access_token, $latestBranch, encode_base64($newJson, ''));
    }
    else {
        print "The file '$catalogs_json_path' is up to date.\n";
    }
    close($fh);
}

sub filter_dat_entries {
    my $datArray = eval(shift);
    my @wantedKeys = @_;

    for my $i (0 .. $#$datArray) {
        # We don't use hash slicing here since this adds missing keys with null values
        foreach my $key (keys %{${$datArray}[$i]}) {
            delete ${${$datArray}[$i]}{$key} unless (grep $_ eq $key, @wantedKeys);
        }
    }
    return $datArray;
}

sub commit_and_pr {
    my ($access_token, $postgresBranch, $encoded_json) = @_;

    my $gh = Net::GitHub::V4->new( access_token => $access_token );
    my $repositoryOwner = 'Brar';
    my $repository = 'Npgsql';
    my $currentBranch = 'main';
    if (defined($ENV{GITHUB_REPOSITORY_OWNER})) {
        $repositoryOwner = $ENV{GITHUB_REPOSITORY_OWNER};
    }
    if (defined($ENV{GITHUB_REPOSITORY})) {
        $repository = $ENV{GITHUB_REPOSITORY};
    }
    if (defined($ENV{GITHUB_REF_NAME})) {
        $currentBranch = $ENV{GITHUB_REF_NAME};
    }

    # First get the current repository's id and the HEAD which we need to create a branch
    my $result = $gh->query(<<'QUERY_END', { 'owner' => $repositoryOwner, 'repository' => $repository, 'branch' => $currentBranch });
query GetRepositoryInfoForBranching($owner: String!, $repository: String!, $branch: String!) {
  repository(owner: $owner, name: $repository) {
    id
    ref(qualifiedName: $branch) {
      target {
        ... on Commit {
          history(first: 1) {
            nodes {
              oid
            }
          }
        }
      }
    }
  }
}
QUERY_END
    handle_graphql_error($result);

    my $headOid = ${${${${${${${$$result{'data'}}{'repository'}}{'ref'}}{'target'}}{'history'}}{'nodes'}}[0]}{'oid'};
    my $repositoryId = ${${$$result{'data'}}{'repository'}}{'id'};

    # Then get all update_catalog/* branches to find the next available branch name
    # This may cause race conditions if two GitHub actions jobs run at the same
    # time but let's see whether this really happens in practice
    $result = $gh->query(<<'QUERY_END', { 'owner' => $repositoryOwner, 'repository' => $repository });
query GetUpdateCatalogBranches($owner: String!, $repository: String!) {
  repository(owner: $owner, name: $repository) {
    refs(
      refPrefix: "refs/heads/"
      first: 100
      query: "update_catalog/"
    ) {
      nodes {
        name
      }
    }
  }
}
QUERY_END
    handle_graphql_error($result);
    
    my @updateCatalogBranches = @{${${${$$result{'data'}}{'repository'}}{'refs'}}{'nodes'}};
    my $nextBranchIndex = 1;
    foreach(@updateCatalogBranches) {
        my $branchIndex = 0 + ((split(/\//, $$_{'name'}))[1]);
        if ($nextBranchIndex <= $branchIndex) {
            $nextBranchIndex = 1 + $branchIndex;
        }
    }
    my $updateCatalogBranch = sprintf('refs/heads/update_catalog/%04u', $nextBranchIndex);

    # Then create a new branch to later create a pull request from it
    $result = $gh->query(<<'QUERY_END', { 'repository_id' => $repositoryId, 'update_catalog_branch' => $updateCatalogBranch, 'head_oid' => $headOid });
mutation CreateUpdateCatalogBranch($repository_id: ID!, $update_catalog_branch: String!, $head_oid: GitObjectID!) {
  createRef(
    input: {repositoryId: $repository_id, name: $update_catalog_branch, oid: $head_oid}
  ) {
    ref {
      name
    }
  }
}
QUERY_END
    handle_graphql_error($result);

    my $updateCatalogBranchName = ${${${$$result{'data'}}{'createRef'}}{'ref'}}{'name'};
    print "Created branch $updateCatalogBranchName from $currentBranch in $repositoryOwner/$repository\n";

    # Then get the head of the newly created branch to append the commit to it
    $result = $gh->query(<<'QUERY_END', { 'owner' => $repositoryOwner, 'repository' => $repository, 'branch' => $updateCatalogBranchName });
query GetExpectedHeadOid($owner:String!, $repository:String!, $branch:String!) {
  repository(owner: $owner, name: $repository) {
    ref(qualifiedName: $branch) {
      target {
        ... on Commit {
          history(first: 1) {
            nodes {
              oid
            }
          }
        }
      }
    }
  }
}
QUERY_END
    handle_graphql_error($result);

    my $expectedHeadOid = ${${${${${${${$$result{'data'}}{'repository'}}{'ref'}}{'target'}}{'history'}}{'nodes'}}[0]}{'oid'};
    
    # Then add a commit with the new catalogs.json file
    my $commitAndPrTitle = "Update catalogs.json from PostgreSQL's $postgresBranch branch";
    my $commitInput = {
        'branch' => {
            'repositoryNameWithOwner' => "$repositoryOwner/$repository",
            'branchName' => $updateCatalogBranchName
        },
        'message' => {
            'headline' => $commitAndPrTitle
        },
        'fileChanges' => {
            'additions' => [
                {
                    'path'     => 'src/Npgsql/pg_catalog/catalogs.json',
                    'contents' => $encoded_json
                }
            ]
        },
        'expectedHeadOid' => $expectedHeadOid };

    $result = $gh->query(<<'QUERY_END', {'input' => $commitInput });
mutation CommitJsonUpdate($input:CreateCommitOnBranchInput!) {
  createCommitOnBranch(input: $input) {
    commit {
      url
    }
  }
}
QUERY_END
    handle_graphql_error($result);

    my $commitUrl = ${${${$$result{'data'}}{'createCommitOnBranch'}}{'commit'}}{'url'};

    print "Added commit $commitUrl\n";

    # Finally create a pull request to the original branch
    $result = $gh->query(<<'QUERY_END', { 'repository_id' => $repositoryId, 'head_branch' => $updateCatalogBranchName, 'base_branch' => $currentBranch, 'title' => $commitAndPrTitle });
mutation CreatePullRequest($repository_id: ID!, $base_branch: String!, $head_branch: String!, $title: String!) {
  createPullRequest(
    input: {repositoryId: $repository_id, baseRefName: $base_branch, headRefName: $head_branch, title: $title}
  ) {
    pullRequest {
      url
    }
  }
}
QUERY_END
    handle_graphql_error($result);

    my $prUrl = ${${${$$result{'data'}}{'createPullRequest'}}{'pullRequest'}}{'url'};

    print "Created pull request $prUrl\n";
}

sub handle_graphql_error {
    my $result = shift;

    if (defined($$result{'message'})) {
        die "API query failed: $$result{'message'}";
    }
    elsif (defined(${${$$result{'errors'}}[0]}{'message'})) {
        die "API query failed: ${${$$result{'errors'}}[0]}{'message'}";
    }
}
