# fa.intelligence.zeus
![Picture of Zeus](https://github.com/faGH/fa.intelligence.zeus/blob/main/src/icon.png?raw=true)
## Description
The autonomous general purpose, multi-modal agent with a Telegram bot interface.

## Status
| Project | Status | Platform
| --- | --- | --- |
| FrostAura Zeus Pipeline | [![Package FrostAura Zeus Workflow](https://github.com/faGH/fa.intelligence.zeus/actions/workflows/package_zeus_workflow.yml/badge.svg)](https://github.com/faGH/fa.intelligence.zeus/actions/workflows/package_zeus_workflow.yml) | GitHub Actions
| FrostAura Zeus PIP Package | [![PyPI version](https://badge.fury.io/py/frostaura.intelligence.zeus.svg)](https://badge.fury.io/py/frostaura.intelligence.zeus) | PYPI
| FrostAura Zeus Containerization | [![Containerization Workflow](https://github.com/faGH/fa.intelligence.zeus/actions/workflows/containerization_workflow.yml/badge.svg)](https://github.com/faGH/fa.intelligence.zeus/actions/workflows/containerization_workflow.yml) | GitHub Actions
| FrostAura Zeus Docker | [![Docker Badge](https://dockeri.co/image/frostaura/zeus)](https://dockeri.co/image/frostaura/bifrost) | Docker Hub

## Getting Started
### Requirements
- [Install Mini Conda](https://docs.conda.io/en/latest/miniconda.html)
### Setup for Development
In the root of the project, open a terminal and create a conda environment for Zeus to run in with the dependencies it needs.
```bash
conda create --name zeus python=3.8
```
Now let's activate the environment.
```bash
conda activate zeus
```
Install the Dependencies Into The Environment
```
python -m pip install -r requirements.txt
```
Finally, run the Zeus program.
```bash
python src/app.py
```
### Just Run Zeus
For this option, [Docker](https://docs.docker.com/get-docker/) should be installed and running on the host computer then, in the root of the project, open a terminal and start Zeus using Docker Compose via a single command.
```bash
docker-compose up
```

## Documentation
| Content | Description
| -- | -- |
| [Repo Structure](.docs/repo_structure.md) | The structuring of the repo.
| [Getting Started](.docs/getting_started.md) | Start using the repo.
| [Design](.docs/design.md) | The software architecture diagram(s) and design(s).
| [Workflow](.docs/workflow.md) | The software automated software pipeline(s).
| [Support & Contribute](.docs/support_contribute.md) | Basic queries, constributing to the repo and supporting the team(s) working on this open-source repo.