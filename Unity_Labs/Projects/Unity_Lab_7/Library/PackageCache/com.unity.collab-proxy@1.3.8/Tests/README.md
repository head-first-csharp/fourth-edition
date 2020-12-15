# Unity Source Control Tests
This project contains the tests for the window/UI of this collab client.

## Overview
This is the structure of the project:
```none
<root>
  ├── .tests.json
  └── Editor/
      ├── Unity.CollabProxy.EditorTests.asmdef
      ├── Components/
      ├── Models/
      ├── Ppresenters/
      ├── Scenario/
      ├── UserInterface/
      └── Views/
```

Each directory features tests and mock classes for classes in the editor code.

## Tests
To run the tests, use the Unity Test Runner from within the Unity Editor. Unity Test Runner documentation is [here](https://docs.unity3d.com/Manual/testing-editortestsrunner.html).

## Adding a Test
While 100% coverage is hard to achieve, tests should be added with each new feature to ensure coverage either remains constant or increases.

With that out of the way, tests are in the typical C# format with a function with a `[Test]` decorator. Below is an example of a test taken from `Editor/Models/ChangesModelTests.cs`
```csharp
[Test]
public void ChangesModel_NullSourceControlEntries_EmptyResultLists()
{
    var model = new TestableChangesModel();
    model.UpdateChangeList(new List<IChangeEntry>());

    var fullList = model.GetAllEntries();
    Assert.AreEqual(1, fullList.Count);
    Assert.IsTrue(fullList[0].All);

    Assert.AreEqual(0, model.GetToggledEntries().Count);
    Assert.AreEqual(0, model.GetUntoggledEntries().Count);

    Assert.AreEqual(0, model.ToggledCount);
}
```
For documentation on the testing library, look at the NUnit [documentation](https://github.com/nunit/docs/wiki/NUnit-Documentation) over at GitHub. Unity Test Runner is a superset of NUnit and the documentation for that is [here](https://docs.unity3d.com/Manual/testing-editortestsrunner.html).

To access private/internal classes, creating a subclass and marking the parent fields as protected/internal will allow them to be used in testing.
