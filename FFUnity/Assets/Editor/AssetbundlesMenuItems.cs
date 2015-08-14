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

    
}
