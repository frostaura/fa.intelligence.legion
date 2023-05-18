[<< Back](../README.md)

# Getting Started
## Description
This page describes how to set up Windows and Mac environments for working with this repo.

In addition, this directory provides various conda environments that can be used cross-platform to work with various libraries and versioning restrictions that may come with them. For this you can refer to the [conda](./conda/README.md) page.

## Getting Started
### Local
- Python >= 3.7 required.
- [Setup your environment.](./environments/README.md)

To build the Python package locally, from the root of the project, run `python -m build` (Legacy way: `python setup.py sdist bdist_wheel`).
### PIP Installation
```
pip install -U --no-cache-dir frostaura
````
#### Example Usage (See [all the examples here](https://github.com/faGH/fa.intelligence.notebooks/tree/main/examples).)
```
from frostaura import (models,
                       data_access,
                       engines,
                       managers)

html_data_access = data_access.HtmlDataAccess()
engine = engines.FinvizAssetValuationEngine(html_data_access=html_data_access)

vars(engine.valuate(symbol='AAPL', company_name='Apple Inc.'))
```
### MiniForge (conda)
#### Windows
- Install Miniforge from [their GitHub page](https://github.com/conda-forge/miniforge). In our case [Miniforge3-Windows-x86_64](https://github.com/conda-forge/miniforge/releases/latest/download/Miniforge3-Windows-x86_64.exe).
  - Be sure to tick the option to add MiniForge to your system PATH.
#### Apple Silicon
- Install Homebrew (https://brew.sh)
- Install XCode
- Install XCode command line tools: `xcode-select --install`
- Install Miniforge via Homebrew: `brew install miniforge`

### Next Steps
- Install [Python 3.X from here](https://www.python.org/downloads/).
- Install [Visual Studio Code from here](https://code.visualstudio.com/download).
- Install Jupiter with the command `conda install -y jupyter` via a fresh terminal session.
- [Start setting up conda environments of your choice](./conda/README.md).
- Start Experimenting
  - With Visual Studio Code, open the root of this repo (folder). Specifically the folder / directory where '.vscode' lives.
    - Install the extensions that VS Code suggests if you like. This should be an automated prompt.
  - [Run this notebook to check library versions post-setup](./scripts/check_version.ipynb).
  - Now you can run any notebooks just like you did the above. Remember to choose your environment that you registered in the setup stage, as your Python interpreter of choice.

[<< Back](../README.md)