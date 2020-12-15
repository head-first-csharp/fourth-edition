using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace Unity.Cloud.Collaborate.Common
{
    internal class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        static T s_Instance;
        public static T Instance
        {
            get
            {
                if (s_Instance == null)
                    CreateAndLoad();
                return s_Instance;
            }
        }

        protected ScriptableObjectSingleton()
        {
            if (s_Instance != null)
            {
                Debug.LogError("Singleton already exists!");
            }
            else
            {
                s_Instance = this as T;
                Assert.IsFalse(s_Instance == null);
            }
        }

        static void CreateAndLoad()
        {
            Assert.IsTrue(s_Instance == null);

            var filePath = GetFilePath();
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                InternalEditorUtility.LoadSerializedFileAndForget(filePath);
            }

            if (s_Instance == null)
            {
                var inst = CreateInstance<T>() as ScriptableObjectSingleton<T>;
                Assert.IsFalse(inst == null);
                inst.hideFlags = HideFlags.HideAndDontSave;
                inst.Save();
            }

            Assert.IsFalse(s_Instance == null);
        }

        protected void Save()
        {
            if (s_Instance == null)
            {
                Debug.LogError("Cannot save singleton, no instance!");
                return;
            }

            var locationFilePath = GetFilePath();
            var directoryName = Path.GetDirectoryName(locationFilePath);
            if (directoryName == null) return;
            Directory.CreateDirectory(directoryName);
            InternalEditorUtility.SaveToSerializedFileAndForget(new Object[]{ s_Instance }, locationFilePath, true);
        }

        [CanBeNull]
        static string GetFilePath()
        {
            var attr = typeof(T).GetCustomAttributes(true)
                                .OfType<LocationAttribute>()
                                .FirstOrDefault();
            return attr?.FilePath;
        }
    }
}
