using UnityEditor;
using System.IO;
using UnityEditor.Callbacks;
using Admix.AdmixCore.Editor;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace Admix.WebView
{
    public class AdmixBuildPostProcessor
    {
        [PostProcessBuild(700)]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            MaterialBuildHelper.TurnOnPlacementsTextures();
#if UNITY_IOS
            if (target != BuildTarget.iOS)
            {
                return;
            }
            string projPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
            var proj = new PBXProject();
            proj.ReadFromString(File.ReadAllText(projPath));
#if UNITY_2019_1_OR_NEWER
            string targetGuid = proj.GetUnityMainTargetGuid();
            proj.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-ObjC");
            proj.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
            File.WriteAllText(projPath, proj.WriteToString());
            
            targetGuid = proj.GetUnityFrameworkTargetGuid();
            proj.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-ObjC");
            proj.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
            File.WriteAllText(projPath, proj.WriteToString());
#else
            string targetGuid = proj.ProjectGuid();
            proj.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-ObjC");
            proj.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");

            targetGuid = proj.TargetGuidByName("Unity-iPhone");
            proj.AddBuildProperty(targetGuid, "OTHER_LDFLAGS", "-ObjC");
            proj.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
            File.WriteAllText(projPath, proj.WriteToString());
            #endif
#endif
        }
    }
}
