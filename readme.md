# ChangeLog Generator

It is a Git and Azure DevOps-based NET Core changelog generator:

  - Use Git to get the commit history
  - Use Azure DevOps RestAPI to get related work items
  - Use Hangfire to process background jobs
  - Generate build history based on markdown files

## Interfaces
#### Tasks
``` GET /tasks ``` - Returns a list of active and queued tasks
``` DELETE /tasks/{taskId} ``` - Remove the task from the task list 
#### Generate
``` GET /generate?branch={branch}&from={fromTag}&to={toTag}``` - Generate changelog for specific tags
``` GET /generate?branch={branch} ``` - Generate changelog for the latest release

## Markdown template
#### Variables
- ```@@TEMPLATE@@``` - newly generated changelog will be inserted right after the variable
- ```@@BUILD_VERSION@@``` - build version
- ```@@WORK_ITEMS@@``` - List of associated work items
- ```@@NOT_LINKED_CHANGES@@``` - List of commits without any related items
#### Template example
```$
# My Program Changelog
(c) UserName

@@TEMPLATE@@
## Version @@BUILD_VERSION@@
### Changes
@@WORK_ITEMS@@
### Additional changes (without work items)
@@NOT_LINKED_CHANGES@@
```

## Settings
```$
"GenerateToPath": "\Changelog"
```

### Todos

License
----
MIT