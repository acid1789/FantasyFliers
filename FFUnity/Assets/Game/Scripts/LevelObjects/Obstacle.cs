using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {

    public bool Ordered = true;             // True if this obstacle has to be cleared in the order it is placed in the scene
    public bool Required = true;            // True if this obstacle is required to be cleared
    public bool MergeWithPrevious = false;  // True if either this OR the previous obstacle count as the same obstacle in the sequence
    public bool ResetsWithLap = true;       // True if this obstacle gets reset on each new lap
    public bool StartLine = false;          // True if this obstacle is counted as the starting line
    public bool FinishLine = false;         // True if this obstacle is counted as the finsih line
    public int ScoreMin;                    // Minmum amount added to score upon clearing
    public int ScoreMax;                    // Maximum amount added to score upon clearing

    int _orderID;

    protected bool _finished;

    // Use this for initialization
    void Start () {
        _finished = false;

    }
	
	// Update is called once per frame
	void Update () {
	
	}
    
    protected virtual void ObstacleCompleted()
    {
        transform.parent.SendMessage("OnObstacleCompleted", this);
    }

    // Called by FlightCoure when this obstacle has been completed and accepted as complete
    public virtual void Finish()
    {
        _finished = true;
    }

    public virtual void Reset()
    {
        _finished = false;
    }

    public int OrderID
    {
        get { return _orderID; }
        set { _orderID = value; }
    }

    public int Score
    {
        get { return Random.Range(ScoreMin, ScoreMax); }
    }
}
