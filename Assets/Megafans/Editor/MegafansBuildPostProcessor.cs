using System.IO;
using UnityEngine;
using UnityEditor;
#if UNITY_IOS
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class MegafansBuildPostProcessor
{

    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string path)
    {

        if (buildTarget == BuildTarget.iOS)
        {

            string plistPath = path + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            PlistElementDict rootDict = plist.root;

            Debug.Log(">> Automation, plist ... <<");

            rootDict.SetString("GADApplicationIdentifier", "ca-app-pub-8665372835563609~1765013376");
            rootDict.SetString("NSPhotoLibraryUsageDescription", "Send photos to help resolve app issues");
            rootDict.SetString("NSLocationAlwaysUsageDescription","Used By OneSignal for push notification");
            rootDict.SetString("NSLocationWhenInUseUsageDescription","Used By OneSignal for push notification");
            

            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}
#endif