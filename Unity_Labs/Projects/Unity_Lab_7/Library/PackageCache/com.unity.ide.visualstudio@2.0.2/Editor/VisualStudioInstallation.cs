/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.IO;
using Microsoft.Win32;
using Unity.CodeEditor;
using IOPath = System.IO.Path;

namespace Microsoft.Unity.VisualStudio.Editor
{
	internal class VisualStudioInstallation
	{
		public string Name { get; set; }
		public string Path { get; set; }
		public Version Version { get; set; }
		public bool IsPrerelease { get; set; }

		public bool SupportsAnalyzers
		{
			get
			{
				if (VisualStudioEditor.IsWindows)
					return Version >= new Version(16, 3);

				if (VisualStudioEditor.IsOSX)
					return Version >= new Version(8, 3);

				return false;
			}
		}

		public bool SupportsCSharp8
		{
			get
			{
				if (VisualStudioEditor.IsWindows)
					return Version >= new Version(16, 0);

				if (VisualStudioEditor.IsOSX)
					return Version >= new Version(8, 2);

				return false;
			}
		}

		private static string ReadRegistry(RegistryKey hive, string keyName, string valueName)
		{
			try
			{
				var unitykey = hive.OpenSubKey(keyName);

				var result = (string)unitykey?.GetValue(valueName);
				return result;
			}
			catch (Exception)
			{
				return null;
			}
		}

		// We only use this to find analyzers, we do not need to load this assembly anymore
		private string GetBridgeLocation()
		{
			if (VisualStudioEditor.IsWindows)
			{
				// Registry, using legacy bridge location
				var keyName = $"Software\\Microsoft\\Microsoft Visual Studio {Version.Major}.0 Tools for Unity";
				const string valueName = "UnityExtensionPath";

				var bridge = ReadRegistry(Registry.CurrentUser, keyName, valueName);
				if (string.IsNullOrEmpty(bridge))
					bridge = ReadRegistry(Registry.LocalMachine, keyName, valueName);

				return bridge;
			}

			if (VisualStudioEditor.IsOSX)
			{
				// Environment,  useful when developing UnityVS for Mac 
				var bridge = Environment.GetEnvironmentVariable("VSTUM_BRIDGE");
				if (!string.IsNullOrEmpty(bridge) && File.Exists(bridge))
					return bridge;

				const string addinBridge = "Editor/SyntaxTree.VisualStudio.Unity.Bridge.dll";
				const string addinName = "MonoDevelop.Unity";

				// user addins repository
				var localAddins = IOPath.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.Personal),
					$"Library/Application Support/VisualStudio/${Version.Major}.0" + "/LocalInstall/Addins");

				// In the user addins repository, the addins are suffixed by their versions, like `MonoDevelop.Unity.1.0`
				// When installing another local user addin, MD will remove files inside the folder
				// So we browse all VSTUM addins, and return the one with a bridge, which is the one MD will load
				if (Directory.Exists(localAddins))
				{
					foreach (var folder in Directory.GetDirectories(localAddins, addinName + "*", SearchOption.TopDirectoryOnly))
					{
						bridge = IOPath.Combine(folder, addinBridge);
						if (File.Exists(bridge))
							return bridge;
					}
				}

				// Check in Visual Studio.app/
				// In that case the name of the addin is used
				bridge = IOPath.Combine(Path, $"Contents/Resources/lib/monodevelop/AddIns/{addinName}/{addinBridge}");
				if (File.Exists(bridge))
					return bridge;
			}

			return null;
		}

		public string[] GetAnalyzers()
		{
			var bridge = GetBridgeLocation();

			if (!string.IsNullOrEmpty(bridge))
			{
				var baseLocation = IOPath.Combine(IOPath.GetDirectoryName(bridge), "..");
				var analyzerLocation = IOPath.GetFullPath(IOPath.Combine(baseLocation, "Analyzers"));

				if (Directory.Exists(analyzerLocation))
					return Directory.GetFiles(analyzerLocation, "*Analyzers.dll", SearchOption.AllDirectories);
			}

			// Local assets
			// return FileUtility.FindPackageAssetFullPath("Analyzers a:packages", ".Analyzers.dll");
			return Array.Empty<string>();
		}

		public CodeEditor.Installation ToCodeEditorInstallation()
		{
			return new CodeEditor.Installation() { Name = Name, Path = Path };
		}
	}
}
