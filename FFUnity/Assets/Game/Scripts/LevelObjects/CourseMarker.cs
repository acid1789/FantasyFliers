using UnityEngine;
using System.Collections;

public class CourseMarker : Obstacle {

    bool _frontEntered = false;
    bool _frontExited = false;
    bool _backEntered = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    }

    void TriggerVolumeEntered(TriggerEventArgs args)
    {
        if(_finished )
            return;

        switch ( args.Trigger.name )
        {
            case "Pole1":
                _frontEntered = true;
                _frontExited = false;
                _backEntered = false;            
                break;
            case "Pole2":
                _backEntered = true;
                break;
        }
    }

    void TriggerVolumeExited(TriggerEventArgs args)
    {
        if(_finished ) 
            return;

        switch (args.Trigger.name)
        {
            case "Pole1":
                _frontExited = true;
                break;
            case "Pole2":
                if( _frontEntered && _frontExited && _backEntered )
                    ObstacleCompleted();
                break;
        }
    }
}
