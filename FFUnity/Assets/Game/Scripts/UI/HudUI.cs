using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HudUI : MonoBehaviour {

    protected Transform _safeArea;
    protected Transform _fpsCounter;
    protected Transform _speedUI;

    float _frameTime;

	// Use this for initialization
	void Start ()
    {
        Application.targetFrameRate = -1;
        _frameTime = 0;

        // Init common pointers
        _safeArea = transform.FindChild("Safe Area");               
        _fpsCounter = _safeArea.FindChild("FPSCounter");
        _speedUI = _safeArea.FindChild("SpeedDisplay");

        // Call virtual init
        Init();	
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Update FPS counter
        UpdateFPSCounter();

        // Call virtual update
        DoUpdate();
    }

    protected virtual void Init()
    {

    }

    protected virtual void DoUpdate()
    {
        if (_speedUI != null)
        {
            float speed = Globals.FlightController.Speed;
            float kilometersPerHour = speed * 3.6f;
            _speedUI.GetComponent<Text>().text = string.Format("Speed: {0:0.0} m/s\nSpeed: {1:0.0} km/h", speed, kilometersPerHour);
        }
    }

    protected void UpdateFPSCounter()
    {
        if (_fpsCounter != null)
        {
            _frameTime += (Time.deltaTime - _frameTime);

            float dt = _frameTime;
            float fps = 1.0f / dt;
            string fpsText = string.Format("FPS: {0:0.} - {1:0.000}ms", fps, dt * 1000.0f);
            Text txt = _fpsCounter.GetComponent<Text>();
            txt.text = fpsText;
        }
    }

    public virtual void StartFlight()
    {
    }

    public virtual void StopFlight()
    {
    }
}
