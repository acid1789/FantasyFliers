using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountdownUI : MonoBehaviour {

    float _timer = 3;

	// Use this for initialization
	void Start ()
    {
        _timer = 3;	
	}
	
	// Update is called once per frame
	void Update ()
    {
        _timer -= Time.deltaTime;

        // Update text to the timer	
        int number = Mathf.CeilToInt(_timer);
        Text txt = transform.FindChild("Counter").GetComponent<Text>();
        if( number > 0 )
            txt.text = number.ToString();
        else
            txt.text = "GO!";
	}

    public bool CanGo
    {
        get { return (_timer <= 0); }
    }

    public bool IsDone
    {
        get { return (_timer <= -1.0f); }
    }
}
