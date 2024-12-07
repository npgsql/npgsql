# Npgsql Release Checklist

## Release the actual version

* Check out the git branch which represents the version you want to release. For a patch version, this will be e.g. `hotfix/9.0.2`; for a major version, this will be `main`.
* Verify that the version is correct inside `Directory.Build.props`.
* Do a git tag for the version (`git tag v9.0.2`) and push it to origin (`git push origin v9.0.2`).
* Go to the repository's [Actions tab](https://github.com/npgsql/npgsql/actions) and wait for the build to complete.
* If all goes well, you'll need to approve the deployment (this represents the push to nuget.org). If there's some sort of build failure, you can fix it, and then re-tag (`git tag -f v9.0.2`) and force-push (`git push -f upstream v9.0.2`).

## Post-release actions

* If you released a patch version, you need to create the new hotfix branch and remove the old one (both locally and from `origin`): `git checkout -b hotfix/9.0.3`.
* Edit `Directory.Build.props` to bump the new version, and commit (`git commit -a -m "Bump version to 9.0.3"`).
* At this point, push the new hotfix branch and remove the old one:

```console
git push -u upstream hotfix/9.0.3

git branch -D hotfix/9.0.2
git push upstream :hotfix/9.0.2
```

* If you're releasing a major version, the steps are the same except that there's no old hotfix branch to remove, and the version needs to be updated in `main` (and pushed).

## Github milestone and release

* Go to the [Milestones page](https://github.com/npgsql/npgsql/milestones) of the repo.
* Create a milestone for the next version (e.g. 9.0.3).
* If there are any open issues in the version milestone which you just published, and those weren't actually completed, move them to the new milestone for the next version (or to the backlog). The released milestone should contain only closed issues.
* Edit the released milestone and set the date to today, just to have a record which version was published when.
* Closed the released milestone.

* Go to the [Releases page](https://github.com/npgsql/npgsql/releases) of the repo.
* Click "Draft a new release", select the git tag you just released, and give the release the title "v9.0.2".
* Write a short text describing the changes; add a link to the closed issue list of the milestone ([example](https://github.com/npgsql/npgsql/milestone/125?closed=1)), and click "generate release notes". For a major version, link to the release notes page in our conceptual docs.
* Publish the release.

## Update API documentation

[Our documentation site](https://www.npgsql.org) automatically publishes API docs for Npgsql and EFCore.PG from the `docs` branches in each of those two repos. After publishing a major version, you'll need to re-point that branch to the new version, so that API docs for that version are generated (`git reset --hard v9.0.0`). You'll then need to trigger a docs rebuild (you can just wait for the next time some small note needs to be updated etc.).
