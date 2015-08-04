using UnityEngine;
using System.Collections;

public class TriggerEventArgs
{
    public GameObject Trigger;
    public Collider Other;

    public TriggerEventArgs(GameObject trigger, Collider other)
    {
        Trigger = trigger;
        Other = other;
    }
}

public class TriggerScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        TriggerEventArgs args = new TriggerEventArgs(gameObject, other);
        SendMessageUpwards("TriggerVolumeEntered", args);
    }

    void OnTriggerExit(Collider other)
    {
        TriggerEventArgs args = new TriggerEventArgs(gameObject, other);
        SendMessageUpwards("TriggerVolumeExited", args);
    }
}
