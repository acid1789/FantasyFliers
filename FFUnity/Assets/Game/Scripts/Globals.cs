using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class Globals
{
    public class AssetLoadInfo
    {
        public string BundleName;
        public string AssetName;
        public GameObject Prefab;

        public AssetLoadInfo(string bundleName, string assetName)
        {
            BundleName = bundleName;
            AssetName = assetName;
        }
    }

    public static string LevelBundle;
    public static string LevelName;
    
    public static AssetLoadInfo[] Drones;
    public static AssetLoadInfo[] Cameras;

    public static FlightController FlightController;
    public static NetworkManager Network;
    public static Inventory Inventory;

    #region UI
    public static GameObject UI_Countdown;
    public static GameObject UI_Hud_TimeTrial;
    public static GameObject UI_ScoreScreen;
    public static GameObject UI_SignIn;
    #endregion
    
#if UNITY_EDITOR
    public static void PlayInEditor()
    {
        if (Globals.Drones == null || Globals.Drones[0] == null || Globals.Drones[0].Prefab == null)
        {
            Globals.Drones = new Globals.AssetLoadInfo[2];
            Globals.Drones[0] = new Globals.AssetLoadInfo(null, null);
            Globals.Drones[0].Prefab = (GameObject)AssetDatabase.LoadMainAssetAtPath("Assets/Game/Drones/Hammerhead/Hammerhead.prefab");
            Globals.Drones[1] = new Globals.AssetLoadInfo(null, null);
            Globals.Drones[1].Prefab = (GameObject)AssetDatabase.LoadMainAssetAtPath("Assets/Game/Drones/RonsTestDrone/RonsDrone.prefab");
        }

        if (Globals.Cameras == null)
        {
            Globals.Cameras = new Globals.AssetLoadInfo[1];
            Globals.Cameras[0] = new Globals.AssetLoadInfo(null, null);
            Globals.Cameras[0].Prefab = (GameObject)AssetDatabase.LoadMainAssetAtPath("Assets/Game/Cameras/ChaseCamera.prefab");
        }

        if (Globals.FlightController == null)
        {
            Globals.FlightController = new ArcadeFlightController();
            Globals.FlightController.SetAxisInput(new DualVirtualJoysticks());
            if(Globals.FlightController.GetAxisInput().GetUIPrefab() == null)
                Globals.FlightController.GetAxisInput().SetUIPrefab((GameObject)AssetDatabase.LoadMainAssetAtPath("Assets/Game/UI/UI_DualVirtualJoysticks.prefab"));
        }

        if (Globals.UI_Countdown == null)
            Globals.UI_Countdown = (GameObject)AssetDatabase.LoadMainAssetAtPath("Assets/Game/UI/UI_Countdown.prefab");
        if( Globals.UI_Hud_TimeTrial == null )
            Globals.UI_Hud_TimeTrial = (GameObject)AssetDatabase.LoadMainAssetAtPath("Assets/Game/UI/UI_Hud_TimeTrial.prefab");
        if( Globals.UI_ScoreScreen == null )
            Globals.UI_ScoreScreen = (GameObject)AssetDatabase.LoadMainAssetAtPath("Assets/Game/UI/UI_ScoreScreen.prefab");
        if (Globals.UI_SignIn == null )
            Globals.UI_SignIn = (GameObject)AssetDatabase.LoadMainAssetAtPath("Assets/Game/UI/UI_SignIn.prefab");
    }
#endif
}
