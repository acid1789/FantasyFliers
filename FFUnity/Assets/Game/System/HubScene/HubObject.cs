using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HubObject : MonoBehaviour
{
    public RectTransform UICanvas;

    // Use this for initialization
    void Start()
    {
        Button exitButton = UICanvas.FindChild("Exit").GetComponent<Button>();
        exitButton.onClick.AddListener(ExitButton);

        Button timeTrialButton = UICanvas.FindChild("Time Trial Button").GetComponent<Button>();
        timeTrialButton.onClick.AddListener(TimeTrialButton);

        Button raceButton = UICanvas.FindChild("RaceButton").GetComponent<Button>();
        raceButton.onClick.AddListener(RaceButton);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ExitButton()
    {
        Application.Quit();
    }

    void TimeTrialButton()
    {
        Application.LoadLevel("Test Flight 1");
    }

    void RaceButton()
    {
        Globals.LevelBundle = "course_ronslevel";
        Globals.LevelName = "RonsLevel";

        Globals.Drones = new Globals.AssetLoadInfo[1];
        Globals.Drones[0] = new Globals.AssetLoadInfo("drone_hammerhead", "Hammerhead");

        Globals.Cameras = new Globals.AssetLoadInfo[1];
        Globals.Cameras[0] = new Globals.AssetLoadInfo("camera_chasecamera", "ChaseCamera");

        Application.LoadLevel("LevelLoader");
    }
}
