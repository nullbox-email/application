# Versioning

This repo uses GitVersion to calculate SemVer versions from git history.

We use two mechanisms to control version bumps:

1. Tags: best for major (and optionally minor) release anchors.
2. Commit messages: optional, for forcing a bump (major/minor/patch) without creating a tag.

Important: use full SemVer tags (vX.Y.Z). Do not use v1.0. Use v1.0.0.

## How versions normally move

- On `main`, versions automatically increase (typically patch: X.Y.Z -> X.Y.(Z+1)) on each new commit after the latest version anchor.
- Feature branches usually produce pre-release versions (for example `1.2.3-feature-name.4`), which are not meant to be deployed as releases.

The "version anchor" is typically the most recent SemVer tag reachable from the current commit.

## Option 1: Bump using tags (recommended)

### Patch bump (rare to tag)
Normally patch bumps happen automatically from new commits on `main`. You typically do not tag patches unless you want an explicit release point.

Example patch tag:

```bash
git checkout main
git pull
git tag -a v1.0.3 -m "Release 1.0.3"
git push origin v1.0.3
````

### Minor bump (tagged)

Tag the next minor version on `main`:

```bash
git checkout main
git pull
git tag -a v1.1.0 -m "Release 1.1.0"
git push origin v1.1.0
```

After this, new commits on `main` will produce `1.1.1`, `1.1.2`, ...

### Major bump (tagged)

Tag the next major version on `main`:

```bash
git checkout main
git pull
git tag -a v2.0.0 -m "Release 2.0.0"
git push origin v2.0.0
```

After this, new commits on `main` will produce `2.0.1`, `2.0.2`, ...

### Notes on tags

List tags:

```bash
git tag --list "v*" --sort=-version:refname | head -n 20
```

Delete a tag locally and on origin (be careful, this rewrites release history):

```bash
git tag -d v1.0.0
git push origin :refs/tags/v1.0.0
```

## Option 2: Bump using commit messages (optional)

GitVersion can bump versions based on special markers in commit messages.

Marker format:

* `+semver: major` (or `+semver: breaking`)
* `+semver: minor` (or `+semver: feature`)
* `+semver: patch` (or `+semver: fix`)
* `+semver: none` (or `+semver: skip`) to prevent a bump

### Do I add this to every commit?

No.

Recommended: only add `+semver:` to the merge commit (or PR merge title) when you want to force a bump.

### Patch bump using a commit message

If your default behaviour is not patch, or you want to force patch on a merge commit:

Merge commit message example:

```text
Merge pull request #123 from feature/something

+semver: patch
```

Or a single-line commit message:

```bash
git commit -m "Merge feature/something +semver: patch"
```

### Minor bump using a commit message

Merge commit message:

```text
Merge pull request #456 from feature/new-api

+semver: minor
```

Or:

```bash
git commit -m "Introduce new API +semver: minor"
```

### Major bump using a commit message

Merge commit message:

```text
Merge pull request #789 from feature/breaking-change

+semver: major
```

Or:

```bash
git commit -m "Breaking change to auth flow +semver: major"
```

### Prevent bump on a merge commit

```text
Merge pull request #999 from docs/typos

+semver: none
```

## Suggested policy

* Use tags for major releases (v2.0.0, v3.0.0, ...).
* Let patch increments happen automatically on `main`.
* Use `+semver: minor` (on merge commits) only when you want to intentionally bump minor without creating a tag.
* Avoid tagging patch versions unless you need a pinned release marker.

## Quick examples

Major via tag:

```bash
git tag -a v2.0.0 -m "Release 2.0.0"
git push origin v2.0.0
```

Minor via merge message:

```text
+semver: minor
```

Patch is automatic on main (no action needed).