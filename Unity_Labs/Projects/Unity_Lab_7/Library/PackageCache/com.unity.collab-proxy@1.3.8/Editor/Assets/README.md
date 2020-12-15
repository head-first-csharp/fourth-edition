# Assets
This directory contains the non-code assets for the Collaborate UI.

## Overview

```none
<root>
  ├── Icons/
  ├── Layouts/
  ├── Styles/
  ├── StringAssets.cs
  └── UiConstants.cs
```

- `StringAssets.cs` contains all the static string resources for the package.
- `UiConstants.cs` conatins a number of static values used to control the UI.

## Editing
USS and UXML files are inspired by the respective CSS and XML files. USS is a non-struct subset of CSS and each View and
Component has the option of having its own USS and/or UXML file. For USS, to specifiy dark vs light style, prepend a `.dark`
or `.light` to the line. For example:

```css
.dark .divider-vertical {
    background-color: var(--divider-dark);
}

.light .divider-vertical {
    background-color: var(--divider-light);
}
```

Documentation about the two file types is provided within the Unity documentation for UiElements:
https://docs.unity3d.com/2020.1/Documentation/Manual/UIElements.html

In general each Component and View will have its own layout file. When adding new components with their uxml factories
the UIElements schema will need to updated within the editor if you want auto completion in your editor. Click
`Assets/Update UIElements Schema` in the Editor to update the definitions.
