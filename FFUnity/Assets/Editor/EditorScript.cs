using UnityEngine;
using UnityEditor;

public class EditorScript
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles("AssetBundles");
    }

    [MenuItem("Assets/Get AssetBundle names")]
    static void GetNames()
    {
        var names = AssetDatabase.GetAllAssetBundleNames();
        foreach (var name in names)
            Debug.Log("AssetBundle: " + name);
    }
}