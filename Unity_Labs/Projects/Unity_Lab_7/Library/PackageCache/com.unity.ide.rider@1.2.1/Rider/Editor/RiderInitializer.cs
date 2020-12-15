using System;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Packages.Rider.Editor
{
    internal class RiderInitializer
    {
      public void Initialize(string editorPath)
      {
        var assembly = EditorPluginInterop.EditorPluginAssembly;
        if (EditorPluginInterop.EditorPluginIsLoadedFromAssets(assembly))
        {
          Debug.LogError($"Please delete {assembly.Location}. Unity 2019.2+ loads it directly from Rider installation. To disable this, open Rider's settings, search and uncheck 'Automatically install and update Rider's Unity editor plugin'.");
          return;
        }

        var relPath = "../../plugins/rider-unity/EditorPlugin";
        if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX)
          relPath = "Contents/plugins/rider-unity/EditorPlugin";
        var baseDir = Path.Combine(editorPath, relPath);
        var dllFile = new FileInfo(Path.Combine(baseDir, $"{EditorPluginInterop.EditorPluginAssemblyName}.dll"));
        
        if (!dllFile.Exists)
          dllFile = new FileInfo(Path.Combine(baseDir, $"{EditorPluginInterop.EditorPluginAssemblyNameFallback}.dll"));

        if (dllFile.Exists)
        {
          var bytes = File.ReadAllBytes(dllFile.FullName); 
          assembly = AppDomain.CurrentDomain.Load(bytes); // doesn't lock assembly on disk
          // assembly = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(dllFile.FullName)); // use this for external source debug
          if (PluginSettings.SelectedLoggingLevel >= LoggingLevel.TRACE)
            Debug.Log($"Rider EditorPluging loaded from {dllFile.FullName}");
          
          EditorPluginInterop.InitEntryPoint(assembly);
        }
        else
        {
          Debug.Log($"Unable to find Rider EditorPlugin {dllFile.FullName} for Unity ");
        }
      }
    }
}
