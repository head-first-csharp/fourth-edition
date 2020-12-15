# Code Editor Package for Visual Studio

## [2.0.2] - 2020-05-27

Added support for solution folders.
Only bind the messenger when the VS editor is selected.
Warn when unable to create the messenger.
Fixed an initialization issue triggering legacy code generation.
Allow package source in assembly to be generated when referenced from asmref.


## [2.0.1] - 2020-03-19

When Visual Studio installation is compatible with C# 8.0, setup the language version to not prompt the user with unsupported constructs. (So far Unity only supports C# 7.3).
Use Unity's TypeCache to improve project generation speed.
Properly check for a managed assembly before displaying a warning regarding legacy PDB usage.
Add support for selective project generation (embedded, local, registry, git, builtin, player).


## [2.0.0] - 2019-11-06

- Improved Visual Studio and Visual Studio for Mac automatic discovery
- Added support for the VSTU messaging system (start/stop features from Visual Studio)
- Added support for solution roundtrip (preserves references to external projects and solution properties)
- Added support for VSTU Analyzers (requires Visual Studio 2019 16.3, Visual Studio for Mac 8.3)
- Added a warning when using legacy pdb symbol files.
- Fixed issues while Opening Visual Studio on Windows
- Fixed issues while Opening Visual Studio on Mac

## [1.1.1] - 2019-05-29

Fix Bridge assembly loading with non VS2017 editors

## [1.1.0] - 2019-05-27

Move internal extension handling to package.

## [1.0.11] - 2019-05-21

Fix detection of visual studio for mac installation.

## [1.0.10] - 2019-05-04

Fix ignored comintegration executable


## [1.0.9] - 2019-03-05

Updated MonoDevelop support, to pass correct arguments, and not import VSTU plugin
Use release build of COMIntegration for Visual Studio


## [1.0.7] - 2019-04-30

Ensure asset database is refreshed when generating csproj and solution files.

## [1.0.6] - 2019-04-27

Add support for generating all csproj files.

## [1.0.5] - 2019-04-18

Fix relative package paths.
Fix opening editor on mac.

## [1.0.4] - 2019-04-12

- Fixing null reference issue for callbacks to AssetPostProcessor.
- Ensure Path.GetFullPath does not get an empty string.

## [1.0.3] - 2019-01-01

### This is the first release of *Unity Package visualstudio_editor*.

Using the newly created api to integrate Visual Studio with Unity.
