---
Title: Getting Started
Published: 12/23/2016
Tags: Introduction
---
# Getting Started

[Wyam](http://wyam.io) is a "static content toolkit" based on Roslyn, the .NET compiler.
[This](http://github.com/wekempf/wyam-starter) GitHub repository makes it easy to get started using
Wyam with [GitHub Pages](http://pages.github.com).

## Export the Repo

From a PowerShell prompt, execute the following (replacing 'blog' with another path for exporting to
if you prefer):

```PowerShell
$dir='blog'; git clone git@github.com:wekempf/wyam-starter.git $dir; Remove-Item (Join-Path $dir '.git') -Recurse -Force
```

This exports the repository to the directory you specify ('blog' in the example above).

## Initialize the Repo

Change directories into the exported repo and execute the following commands:

```PowerShell
git init .
git add .
git commit -m "Initial creation"
```

This initializes the directory as a git repo and checks all of the exported files into it.

## Create a GitHub Repo

Follow the [directions](https://help.github.com/articles/create-a-repo/) to create a repository on GitHub.
Do not select 'Initialize this repository with a README.' We want an empty repo to be created. Once the
repository is created follow the instructions under '…or push an existing repository from the command line'
to push the contents of the local repository to the GitHub repository.

## Create a Personal Access Token

In order for the build scripts to be able to push the results to the GitHub Pages associated with your
repository we'll need to have a "personal access token", as per the [documentation](https://help.github.com/articles/creating-an-access-token-for-command-line-use/).
Under Scopes select Repo to provide full acccess to the repository from the build scripts. Be sure to copy
the generated token to somewhere safe. We'll encrypt it for use in our AppVeyor powered build later on.

## Create an AppVeyor build

If you don't already have an [AppVeyor](http://appveyor.com) account, create one. Add a project in AppVeyor
for your GitHub repo.

## Encrypt Access Token

Go to https://ci.appveyor.com/tools/encrypt to encrypt the Personal Access Token you created earlier. Modify
the appveyor.yml file in the root of the repository, replacing the encrypted GitHubPersonalAccessToken with
the encrypted value you just generated.

## Modify Build Files

Modify the build.cake file in the root of the repository, setting the gitPagesRepo to point at your own GitHub
repository. Note that you **must** use https and not ssh. In addition, you need to specify your username and
the access token. For example, if the https url is `https://github.com/user/blog.git` you'll need to
specify `"https://user:" + EnvironmentVariable("GitHubPersonalAccessToken") + "@github.com/user/blog"` in
the build.cake file.

You'll also need to modify appveyor.yml to use your name and email address for commits.

## Commit and Push the Changes

Commit and push your changes to the GitHub repository using the following command:

```PowerShell
git add .
git commit -m "Modified build scripts"
git push
```

This should kick off a build on AppVeyor automatically, and if the build scripts were properly Modified
this will publish the results on your repository's GitHub Pages set.