# fa.intelligence.legion
![Picture of Legion](https://github.com/faGH/fa.intelligence.legion/blob/main/src/icon.png?raw=true)
## Description
FrostAura's Legion is a dotnet-based state-of-the-art multi-agent LLM framework.

## Status
| Project | Status | Platform
| --- | --- | --- |
| FrostAura Legion Containerization | [![Containerization Workflow](https://github.com/faGH/fa.intelligence.legion/actions/workflows/containerization_workflow.yml/badge.svg)](https://github.com/faGH/fa.intelligence.legion/actions/workflows/containerization_workflow.yml) | GitHub Actions
| FrostAura Legion Docker | [![Docker Badge](https://dockeri.co/image/frostaura/legion)](https://dockeri.co/image/frostaura/legion) | Docker Hub 

## Getting Started
### Requirements for Development
- Dotnet 7 SDK
- Visual Studio / Visual Studio Code (with the C# extension)
### Just Run Legion
For this option, [Docker](https://docs.docker.com/get-docker/) should be installed and running on the host computer then, in the root of the project, open a terminal and start Iluvatar using Docker Compose via a single command.
```bash
docker-compose up
```
Ensure that you set the required variables in order for the docker-compose to work. See the [docker-compose.yml](https://github.com/faGH/fa.intelligence.iluvatar/blob/main/docker-compose.yml) file for more.

## Documentation
| Content | Description
| -- | -- |
| [Repo Structure](.docs/repo_structure.md) | The structuring of the repo.
| [Getting Started](.docs/getting_started.md) | Start using the repo.
| [Design](.docs/design.md) | The software architecture diagram(s) and design(s).
| [Workflow](.docs/workflow.md) | The software automated software pipeline(s).
| [Support & Contribute](.docs/support_contribute.md) | Basic queries, constributing to the repo and supporting the team(s) working on this open-source repo.
