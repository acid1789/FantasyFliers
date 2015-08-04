using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class TimeTrialHudUI : HudUI {

    Transform _flightTimeUI;

    bool _flightStarted;
    DateTime _flightStartTime;

    protected override void Init()
    {
        base.Init();

        _flightTimeUI = _safeArea.FindChild("FlightTime");

        _flightStarted = false;
    }

    protected override void DoUpdate()
    {
        base.DoUpdate();

        if (_flightStarted)
        {            
            if (_flightTimeUI != null)
            {
                TimeSpan span = DateTime.Now - _flightStartTime;
                Text txt = _flightTimeUI.GetComponent<Text>();
                txt.text = string.Format("{0:00.}:{1:00.}:{2:00}", span.Minutes, span.Seconds, span.Milliseconds * 0.1f);
            }
        }
    }

    public override void StartFlight()
    {
        base.StartFlight();

        _flightStartTime = DateTime.Now;
        _flightStarted = true;
    }

    public override void StopFlight()
    {
        base.StopFlight();

        _flightStarted = false;
    }
}
