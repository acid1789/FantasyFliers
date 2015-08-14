using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

public class BuildMenu
{
    static void CopyDirectory(string sourceDir, string dest)
    {
        string src = sourceDir.Replace('\\', '/');
        string[] dirs = src.Split('/');
        string path = "";
        foreach (string dir in dirs)
        {
            path += dir + "/";
            if (!Directory.Exists(dest + path))
                Directory.CreateDirectory(dest + path);
        }

        string[] files = Directory.GetFiles(sourceDir);
        foreach (string file in files)
        {
            File.Copy(file, dest + sourceDir + "/" + Path.GetFileName(file), true);
        }
    }

    [MenuItem("Build/Build All Targets", false, 1)]
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

    [MenuItem("Build/Build Windows", false, 2)]
    public static void BuildWindows()
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
    }

    [MenuItem("Build/Build Android", false, 3)]
    public static void BuildAndroid()
    {
        string exeName = "DynastyDrones";
        string[] levels = new string[]
        {
            "Assets/Game/System/Startup Scene/Startup.unity",
            "Assets/Game/System/LevelLoader/LevelLoader.unity",
            "Assets/Game/System/HubScene/HubScene.unity"
        };

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

    [MenuItem("Build/PushRun To Android %i", false, 100)]
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

    static ItemTemplate[] FetchItemTemplates()
    {
        bool buildError = false;
        Dictionary<int, ItemTemplate> templates = new Dictionary<int, ItemTemplate>();

        UnityEngine.Object[] objs = Resources.FindObjectsOfTypeAll(typeof(GameObject));
        foreach (GameObject go in objs)
        {
            ItemTemplate template = go.GetComponent<ItemTemplate>();
            if (template != null)
            {
                if (templates.ContainsKey(template.TemplateID))
                {
                    buildError = true;
                    Debug.Log(string.Format("Duplicate ItemTemplate ID Found {0}: {1} - {2}", template.TemplateID, template.gameObject.name, templates[template.TemplateID].gameObject.name));
                }
                templates[template.TemplateID] = template;
            }
        }

        if (buildError)
            throw new Exception("Errors with Item Template IDs");

        List<ItemTemplate> templs = new List<ItemTemplate>();
        foreach (ItemTemplate templ in templates.Values)
            templs.Add(templ);
        return templs.ToArray();
    }

    static FlightCourse[] FetchFlightCourses()
    {
        bool buildError = false;
        Dictionary<int, FlightCourse> flightCourses = new Dictionary<int, FlightCourse>();

        UnityEngine.Object[] objs = Resources.FindObjectsOfTypeAll(typeof(GameObject));
        foreach (GameObject go in objs)
        {
            FlightCourse fc = go.GetComponent<FlightCourse>();
            if (fc != null)
            {
                if (flightCourses.ContainsKey(fc.CourseID))
                {
                    buildError = true;
                    Debug.Log(string.Format("Duplicate FlightCourse ID Found {0}: {1} - {2}", fc.CourseID, fc.gameObject.name, flightCourses[fc.CourseID].gameObject.name));
                }
                flightCourses[fc.CourseID] = fc;
            }
        }

        if (buildError)
            throw new Exception("Errors with flight course IDs");

        List<FlightCourse> fcs = new List<FlightCourse>();
        foreach( FlightCourse fc in flightCourses.Values )
            fcs.Add(fc);
        return fcs.ToArray();
    }

    static void ValidateLootTables(FlightCourse[] flightCourses)
    {
        Dictionary<int, LootTable> lootTables = new Dictionary<int, LootTable>();
        foreach (FlightCourse fc in flightCourses)
        {
            // Validate the loot table
            if (fc.LootTable == null)
            {
                string err = string.Format("Flight Course ({0}) has no loot table", fc.gameObject.name);
                Debug.Log(err);
                throw new Exception(err);
            }
            else if (lootTables.ContainsKey(fc.LootTable.TableID))
            {
                string err = string.Format("Duplicate loot table ID found ({0}): {1} - {2}", fc.LootTable.TableID, fc.LootTable.gameObject.name, lootTables[fc.LootTable.TableID].gameObject.name);
                Debug.Log(err);
                throw new Exception(err);
            }
            else
            {
                lootTables[fc.LootTable.TableID] = fc.LootTable;
                if (fc.LootTable.MarkerBonus + fc.LootTable.BaseDropChance > 100)
                {
                    throw new Exception(string.Format("LootTable ({0}:{1}) BaseDropChance + MarkerBonus exceeds 100%", fc.LootTable.TableID, fc.LootTable.gameObject.name));
                }

                if (fc.LootTable.Ranges.Length <= 0)
                {
                    string err = string.Format("LootTable ({0}:{1}) has no ranges", fc.LootTable.TableID, fc.LootTable.gameObject.name);
                    Debug.Log(err);
                    throw new Exception(err);
                }
                else
                {
                    float totalPercentage = 0;
                    foreach (LootRange range in fc.LootTable.Ranges)
                    {
                        totalPercentage += range.Percentage;
                    }
                    if (totalPercentage > 100)
                    {
                        string err = string.Format("LootTable ({0}:{1}) ranges add up to more than 100%", fc.LootTable.TableID, fc.LootTable.gameObject.name);
                        Debug.Log(err);
                        throw new Exception(err);
                    }
                }
            }
        }
    }

    static NetworkManager ConnectToServer()
    {
        NetworkManager nm = new NetworkManager("192.168.1.200", 1255);
        DateTime connectStart = DateTime.Now;
        while (!nm.Connected)
        {
            System.Threading.Thread.Sleep(500);
            if ((DateTime.Now - connectStart).TotalSeconds > 30)
            {
                Debug.Log("Connection to server timed out");
                return null;
            }
        }
        return nm;
    }

    [MenuItem("Build/Push Data to Server", false, 200)]
    public static void PushDataToServer()
    {
        // Get all the item templates
        ItemTemplate[] itemTemplates = FetchItemTemplates();

        // Get the flight courses
        FlightCourse[] flightCourses = FetchFlightCourses();

        // Do loot table validation
        ValidateLootTables(flightCourses);

        // Connect to the server
        NetworkManager nm = ConnectToServer();
        if( nm == null )
            throw new Exception("Failed to connect to server");

        // Push data to server
        foreach (ItemTemplate template in itemTemplates)
        {
            nm.SetItemTemplate(template);
        }

        foreach (FlightCourse fc in flightCourses)
        {
            // Calculate max score & loot markers
            int score = 0;
            int lootMarkers = 0;
            foreach (Transform child in fc.transform)
            {
                Obstacle ob = child.GetComponent<Obstacle>();
                if (ob != null)
                    score += ob.ScoreMax;
                LootMarker lm = child.GetComponent<LootMarker>();
                if (lm != null)
                    lootMarkers++;
            }

            // Send course info to the server
            nm.SetCourseInfo(fc.CourseID, fc.ParTimeSeconds, lootMarkers, score, (int)fc.GameMode, fc.TimeScoreBase, fc.TimeScoreModifier, fc.LootTable.TableID);
            
            // Send loot table to the server
            nm.SetLootTable(fc.LootTable);
        }        

        // Close server connection
        nm.Destroy();
    }

    [MenuItem("Build/Reload Server Data", false, 201)]
    public static void ReloadServerData()
    {
        // Connect to the server
        NetworkManager nm = ConnectToServer();
        if (nm == null)
            throw new Exception("Failed to connect to server");

        nm.ReloadServerData();
        nm.Destroy();
    }

    [MenuItem("Build/Generate Template Map", false, 202)]
    public static void GenerateTemplateMap()
    {
        ItemTemplate[] templates = FetchItemTemplates();

        FileStream fs = File.Open("Assets/Game/Loot/Templates/TemplateMap.txt", FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);

        foreach (ItemTemplate template in templates)
        {
            string templateLine = template.TemplateID + "," + template.gameObject.name;
            sw.WriteLine(templateLine);
        }

        sw.Close();
    }
}
