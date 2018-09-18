#!/usr/bin/pwsh

###########################
## Set version variables ##
###########################

$env:TruncatedSha1 = $env:BUILD_SOURCEVERSION.subString(0, 9);

if ($env:TAGGED_COMMIT -eq 'true' -and $env:BUILD_SOURCEBRANCHNAME -match '^v\d+\.\d+\.\d+(-(\w+))?') {
    if ($matches[2]) {
        Write-Host "Pre-release tag detected ($env:APPVEYOR_REPO_TAG_NAME), version suffix set to $($matches[2]).";
        $env:VersionSuffix = $matches[2];
    } else {
        Write-Host "Release tag detected ($env:APPVEYOR_REPO_TAG_NAME), no version suffix will be set.";
    }
    $env:deploy_github_release = $True;
} else {
    # Set which myget feed we deploy to
    if ($env:BUILD_SOURCEBRANCHNAME.StartsWith("hotfix/")) {
        $env:deploy_myget_stable = $True;
    } else {
        $env:deploy_myget_unstable = $True;
    }

    Write-Host "Setting version suffix to $env:VersionSuffix";
    $env:VersionSuffix = "ci.$env:VersionSuffix+sha.$env:TruncatedSha1";
}
