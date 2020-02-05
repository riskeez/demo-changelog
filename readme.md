# ChangeLog Generator

It is a Git, Azure DevOps and NET Core changelog generator:

  - Use Git to get the commit history
  - Use Azure DevOps RestAPI to get related work items
  - Generate build history based on markdown files

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