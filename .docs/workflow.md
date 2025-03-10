[<< Back](../README.md)

## Workflow
The software automated software pipeline(s).

### Introduction
The current platform of choice to run workflows on is GitHub Workflows. See the GitHib Workflow file(s) in `<REPO_ROOT>/.github/workflows`.

## Workflows
### Containerization
This workflow simply builds a Dockerfile in the repository root and push it to the correct image in Docker Hub based on the environment variables specified in the [.env](./../.env) file. This workflow also updates that image's description to match that of our README.md file.
### Nuget
TODO: Support and document Nuget. Also add nuget to the main readme page.

[<< Back](../README.md)