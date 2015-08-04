using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class AssetbundlesMenuItems
{
	const string kSimulateAssetBundlesMenu = "AssetBundles/Simulate AssetBundles";

	[MenuItem(kSimulateAssetBundlesMenu)]
	public static void ToggleSimulateAssetBundle ()
	{
		AssetBundleManager.SimulateAssetBundleInEditor = !AssetBundleManager.SimulateAssetBundleInEditor;
	}

	[MenuItem(kSimulateAssetBundlesMenu, true)]
	public static bool ToggleSimulateAssetBundleValidate ()
	{
		Menu.SetChecked(kSimulateAssetBundlesMenu, AssetBundleManager.SimulateAssetBundleInEditor);
		return true;
	}
	
	[MenuItem ("AssetBundles/Build AssetBundles")]
	static public void BuildAssetBundles ()
	{
		BuildScript.BuildAssetBundles(false);
	}

    [MenuItem("AssetBundles/Re-Build AssetBundles")]
    static public void RebuildAssetBundles()
    {
        BuildScript.BuildAssetBundles(true);
    }

    [MenuItem ("AssetBundles/Build Player")]
	static void BuildPlayer ()
	{
		BuildScript.BuildPlayer();
	}

    static void CopyDirectory(string sourceDir, string dest)
    {
        string src = sourceDir.Replace('\\', '/');
        string[] dirs = src.Split('/');
        string path = "";
        foreach (string dir in dirs)
        {
            path += dir + "/";
            if( !Directory.Exists(dest + path) )
                Directory.CreateDirectory(dest + path);
        }
        
        string[] files = Directory.GetFiles(sourceDir);
        foreach (string file in files)
        {
            File.Copy(file, dest + sourceDir + "/" + Path.GetFileName(file), true);
        }
    }
    
    [MenuItem("Build/Build All Targets")]
    public static void BuildGame()
    {
        string exeName = "DynastyDrones";
        string[] levels = new string[] 
        {
            "Assets/Game/System/Startup Scene/Startup.unity",
            "Assets/Game/System/LevelLoader/LevelLoader.unity",
            "Assets/Game/System/HubScene/HubScene.unity"
        };

        // Build windows
        BuildScript.BuildAssetBundlesForPlatform(BuildTarget.StandaloneWindows);        
        BuildPipeline.BuildPlayer(levels, "Builds/Windows/" + exeName + ".exe", BuildTarget.StandaloneWindows, BuildOptions.None);
        string winBuildDataPath = "Builds/Windows/" + exeName + "_Data/StreamingAssets/";
        CopyDirectory("AssetBundles/Windows", winBuildDataPath);        

        // Build Android
        BuildScript.BuildAssetBundlesForPlatform(BuildTarget.Android);
        CopyDirectory("AssetBundles/Android", "Assets/StreamingAssets/");
        string apk = "Builds/Android/" + exeName + ".apk";
        BuildPipeline.BuildPlayer(levels, apk, BuildTarget.Android, BuildOptions.None);
        FileUtil.DeleteFileOrDirectory("Assets/StreamingAssets/AssetBundles/");
        File.Delete("Assets/StreamingAssets/AssetBundles.meta");
    }
    
    static string adbLocation;
    static string bundleIdent = PlayerSettings.bundleIdentifier;

    [MenuItem("Build/PushRun To Android %i")]
    public static void PushToAndroid()
    {
        string apkLocation = PlayerPrefs.GetString("APK location");
        if (string.IsNullOrEmpty(apkLocation) || !File.Exists(apkLocation))
            apkLocation = EditorUtility.OpenFilePanel("Find APK", Environment.CurrentDirectory, "apk");

        if (string.IsNullOrEmpty(apkLocation) || !File.Exists(apkLocation))
        {
            Debug.LogError("Cannot find .apk file.");
            return;
        }
        PlayerPrefs.SetString("APK location", apkLocation);

        adbLocation = PlayerPrefs.GetString("Android debug bridge location");
        if (string.IsNullOrEmpty(apkLocation) || !File.Exists(adbLocation))
            adbLocation = EditorUtility.OpenFilePanel("Android debug bridge", Environment.CurrentDirectory, "exe");
        if (string.IsNullOrEmpty(apkLocation) || !File.Exists(adbLocation))
        {
            Debug.LogError("Cannot find adb.exe.");
            return;
        }
        PlayerPrefs.SetString("Android debug bridge location", adbLocation);

        ProcessStartInfo info = new ProcessStartInfo
        {
            FileName = adbLocation,
            Arguments = string.Format("install -r \"{0}\"", apkLocation),
            WorkingDirectory = Path.GetDirectoryName(adbLocation),
        };
        Process adbPushProcess = Process.Start(info);
        if (adbPushProcess != null)
        {
            adbPushProcess.EnableRaisingEvents = true;
            adbPushProcess.Exited += RunApp;
        }
        else
        {
            Debug.LogError("Error starting adb");
        }
    }

    public static void RunApp(object o, EventArgs args)
    {
        ProcessStartInfo info = new ProcessStartInfo
        {
            FileName = adbLocation,
            Arguments = string.Format("shell am start -n " + bundleIdent + "/com.unity3d.player.UnityPlayerActivity"),
            WorkingDirectory = Path.GetDirectoryName(adbLocation),
        };

        Debug.Log(adbLocation + " " + info.Arguments);

        Process.Start(info);
    }
}
