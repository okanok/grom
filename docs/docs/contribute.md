# Contributing to Grom

## Bugs

Bugs can be reported on the Bug category of Groms GitHub discussions [page](https://github.com/okanok/grom/discussions/categories/bugs).
Please make sure to provide as much information and context as possible.

If the problem is an unsolved bug then a contributor can make a new Issue for the projects board. 
The issue should have a link to the discussion, a description of the problem and if possible a preferred solution to the bug.

The bug can be picked up by anyone and a PR for the fix can be created to the development branch. 
The fix should be released in the next release.

## Feature requests

Features can be requested [here](https://github.com/okanok/grom/discussions/categories/feature-requests).
A contributor can then create a issue for anyone to pick up.


## Pull requests

Anyone is free and encouraged to create a feature branch and implement something themselves. 
Whne you think the feature is done PR to develop can be created. 
The PR should describe briefly what the changes do and link to the relevant issue ticket. 
If there is no issue try to explain it in the PR description.
We also expect integration tests to be added with every feature.

## Release process

There is no set release cycle for now since the project is still small with only one contributor.
A new release will be created when enough new features have been added or when there are bug fixes.

To release first a release branch needs to be created from develop. 
When the branch has all the features and code needed for the release a PR can be created to main.
This PR should trigger the release stage of the build pipeline.
When the maintainer approves the release the new version will be added to NuGet.