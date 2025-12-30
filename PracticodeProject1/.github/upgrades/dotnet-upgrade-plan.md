# .NET 9.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 9.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 9.0 upgrade.
3. Upgrade fib.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

Table below contains projects that do belong to the dependency graph for selected projects and should not be included in the upgrade.

| Project name           | Description      |
|:-----------------------------------------------|:---------------------------:|
| (none) |         |

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name          | Current Version | New Version | Description                   |
|:----------------------------------------------|:---------------:|:-----------:|:---------------------------------------------------|
| System.Buffers      | 4.5.1     |             | Package functionality included with new framework  |
| System.Memory     | 4.5.5         |    | Package functionality included with new framework  |
| System.Numerics.Vectors       | 4.5.0|        | Package functionality included with new framework  |
| System.Runtime.CompilerServices.Unsafe        | 4.5.3   | 6.1.2       | Recommended for .NET 9.0       |

### Project upgrade details

This section contains details about each project upgrade and modifications that need to be done in the project.

#### fib.csproj modifications

Project properties changes:
  - Project file needs to be converted to SDK-style
  - Target framework should be changed from `net472` to `net9.0`

NuGet packages changes:
  - System.Buffers should be removed (*functionality included with new framework*)
  - System.Memory should be removed (*functionality included with new framework*)
- System.Numerics.Vectors should be removed (*functionality included with new framework*)
  - System.Runtime.CompilerServices.Unsafe should be updated from `4.5.3` to `6.1.2` (*recommended for .NET 9.0*)
