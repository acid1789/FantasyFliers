using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelLoader : BaseLoader
{
    // Use this for initialization
    IEnumerator Start()
    {
        List<string> loadedBundles = new List<string>();

        yield return StartCoroutine(Initialize());

        // Load drones
        foreach (Globals.AssetLoadInfo drone in Globals.Drones)
        {
            yield return StartCoroutine(Load(drone.BundleName, drone.AssetName));
            drone.Prefab = LastLoadedAsset;
            loadedBundles.Add(drone.BundleName);
        }

        // Load cameras
        foreach (Globals.AssetLoadInfo cam in Globals.Cameras)
        {
            yield return StartCoroutine(Load(cam.BundleName, cam.AssetName));
            cam.Prefab = LastLoadedAsset;
            loadedBundles.Add(cam.BundleName);
        }

        // Load level.
        yield return StartCoroutine(LoadLevel(Globals.LevelBundle, Globals.LevelName, true));
        loadedBundles.Add(Globals.LevelBundle);

        // Unload assetBundles.
        foreach (string bundle in loadedBundles)
        {
            AssetBundleManager.UnloadAssetBundle(bundle);
        }
    }
}
