using UnityEngine;
using System.Collections;

public class CountdownTimer : MonoBehaviour{

    [Tooltip("Time in seconds the timer will be counting down from")]
    public float timeLimit;

    private float timeRemaining = 0f;
    private bool timerExpired = false;

	// Use this for initialization
	void Start ()
    {
        timeRemaining = timeLimit;	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (timeRemaining > 0)
            timeRemaining -= Time.deltaTime;
        else
            timerExpired = true;
	
	}

    public bool IsTimerExpiered()
    {
        return timerExpired;
    }

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }
}
