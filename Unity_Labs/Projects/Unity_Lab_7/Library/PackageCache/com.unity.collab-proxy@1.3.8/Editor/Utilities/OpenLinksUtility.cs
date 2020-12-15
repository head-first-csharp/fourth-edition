using System;
using UnityEditor.Connect;

namespace Unity.Cloud.Collaborate.Utilities
{
    internal static class OpenLinksUtility
    {
        public static void OpenMembersLink()
        {
            string url;
            var config = UnityConnect.instance.configuration;
            switch (config)
            {
                case "development": url = "https://dev-developer.cloud.unity3d.com/orgs/{0}/projects/{1}/users"; break;
                case "staging": url = "https://staging-developer.cloud.unity3d.com/orgs/{0}/projects/{1}/users"; break;
                case "production": url = "https://developer.cloud.unity3d.com/orgs/{0}/projects/{1}/users"; break;
                default:
                    UnityEngine.Debug.LogError($"Unexpected connection configuration {config}"); return;
            }

            // url = url.Replace("%%ORGID%%", UnityConnect.instance.projectInfo.organizationId).Replace("%%UPID%%", UnityConnect.instance.projectInfo.projectGUID);
            url = string.Format(url, UnityConnect.instance.projectInfo.organizationId, UnityConnect.instance.projectInfo.projectGUID);
            UnityConnect.instance.OpenAuthorizedURLInWebBrowser(url);
        }
    }
}
