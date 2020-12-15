using System;
using UnityEditor;
using UnityEngine;

namespace Packages.Rider.Editor
{
  public class RiderScriptEditorData : ScriptableSingleton<RiderScriptEditorData>
  {
    [SerializeField] internal bool hasChanges = true; // sln/csproj files were changed 
    [SerializeField] internal bool shouldLoadEditorPlugin;
    [SerializeField] internal bool initializedOnce;
    [SerializeField] internal Version editorBuildNumber;
    [SerializeField] internal RiderPathLocator.ProductInfo productInfo;

    public void Init()
    {
      if (editorBuildNumber == null)
      {
        Invalidate(RiderScriptEditor.CurrentEditor);
      }
    }

    public void Invalidate(string editorInstallationPath)
    {
      editorBuildNumber = RiderPathLocator.GetBuildNumber(editorInstallationPath);
      productInfo = RiderPathLocator.GetBuildVersion(editorInstallationPath);
      if (editorBuildNumber == null)
        shouldLoadEditorPlugin = false;

      shouldLoadEditorPlugin = editorBuildNumber >= new Version("191.7141.156");
    }
  }
}