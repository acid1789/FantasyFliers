using UnityEngine;
using System.Collections;

public class LootMarker : Obstacle {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    protected override void ObstacleCompleted()
    {
        base.ObstacleCompleted();
        GetComponent<Renderer>().enabled = false;
    }

    void TriggerVolumeEntered(TriggerEventArgs args)
    {
        if (_finished)
            return;
        ObstacleCompleted();
    }
}
