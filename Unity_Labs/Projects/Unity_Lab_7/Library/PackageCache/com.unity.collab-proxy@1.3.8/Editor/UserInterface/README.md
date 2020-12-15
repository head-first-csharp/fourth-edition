# Collaborate User Interface
This directory contains the logic to present the collaborate UI.

## Overview
This is the structure of the directory:
```none
<root>
  ├── TestWindows/
  ├── Bootstrap.cs
  ├── CollaborateWindow.cs
  ├── ToolbarButton.cs
  └── WindowCache.cs
```
The `TestWindows/` directory contains testing windows and is not present in release builds.

`Bootstrap.cs` provides the code to initialize the toolbar button on start up.

`CollaborateWindow.cs` is the entry point for the user interface. It spawns a EditorWindow and sets up the UI.

`ToolbarButton.cs` contains the code to create, update, and handle the collaborate button in the toolbar.

`WindowCache.cs` provides a collection of fields that are preserved during domain reload and editor restart. Some
examples are the the current commit message and the currently selected items for the simple UI/UX. Any data that would
impact UX if lost during reload or exit, should be saved in here.
