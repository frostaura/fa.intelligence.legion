[<< Back](../README.md)

## Repo Structure
Below are brief descriptions of the layout structures of the repository.

### Repository Structure
- `/.docs` - In-repo documentation reside here.
  - `/designs` - Specifically draw.io diagrams that are referenced in order docs as well as design-related docs.
    - `<DESIGN_NAME>.drawio.png` - In order for the draw.io extension to work, the drawio suffix is required.
    - `design.<DESIGN_NAME>.md` - Design docs with a prefix.
  - `<DOCUMENT_NAME>.md` - Design documentation that typically refers to a design diagram or more, as per above.
- `/.github` - GitHub-reserved content.
  - `/workflows` - GitHub pipeline workflows.
    - `<WORKFLOW_NAME>_workflow.yml` - Naming convention to follow for workflows.
- `/.vscode` - VS Code workspace settings.
  - `settings.json` - Shared workspace settings.
  - `extensions.json` - Recommended extensions to be installed when opening the project in VS Code.
- `/src` - The parent container for the project code.
  - `...` - TODO: Map out the project structure here.
- `.gitignore` - The file that deterines which artefacts to exclude from source control.
- `docker-compose.yml` - The container services configuration for running the project.
- `Dockerfile` - The docker container build configuration for the project.
- `LICENSE` - The license agreement for usage of the project and source code.
- `README.md` - The root of the project documentation.

[<< Back](../README.md)