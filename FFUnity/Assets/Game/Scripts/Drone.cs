using UnityEngine;
using System.Collections;

public class Drone : MonoBehaviour {

    public Transform[] PropLocations;
    public GameObject Prop;

    public float ChaseDistance = 50;            // Distance the chase camera stays behind the drone
    
    GameObject[] _props;
    
	// Use this for initialization
	void Start ()
    {
        // Spawn props
        _props = new GameObject[PropLocations.Length];
        for (int i = 0; i < PropLocations.Length; i++)
        {
            _props[i] = (GameObject)GameObject.Instantiate(Prop, PropLocations[i].position, PropLocations[i].rotation);
            _props[i].transform.parent = transform;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        // Rotate props
        float rotateSpeed = Mathf.PI * 200;
        int frontHalf = _props.Length / 2;
        for( int i = 0; i < frontHalf; i++ )
            _props[i].transform.Rotate(Vector3.up, rotateSpeed);
        for( int i = frontHalf; i < _props.Length; i++ )
            _props[i].transform.Rotate(Vector3.up, -rotateSpeed);        
	}
}
