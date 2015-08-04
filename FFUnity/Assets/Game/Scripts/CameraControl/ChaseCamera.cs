using UnityEngine;
using System.Collections;

public class ChaseCamera : GameCamera {

    public float TravelSpeed = 100;
    public float RotateSpeed = 100;

    [Tooltip("Angle that the camera is raised to look down onto the drone (in degrees)")]
    public float ElevationAngle = 10;       

    Drone _drone;
    Vector3 _targetPosition;
    Quaternion _targetRotation;

	// Use this for initialization
	void Start ()
    {	
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Adjust target to be behind the drone
        SetTarget();

        // Adjust to the target	
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, TravelSpeed);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, RotateSpeed);
    }

    public override void Setup(FlightCourse fc, Drone playerDrone)
    {
        base.Setup(fc, playerDrone);

        _drone = playerDrone;
        //transform.SetParent(playerDrone.transform);

        // CalculateTarget
        SetTarget();

        // Snap to target
        transform.position = _targetPosition;
        transform.rotation = _targetRotation;
    }

    void SetTarget()
    {
        Vector3 droneDir = Quaternion.AngleAxis(ElevationAngle, _drone.transform.right) * _drone.transform.forward;
        _targetPosition = _drone.transform.position + (droneDir * -_drone.ChaseDistance);         // Negative distance to go behind the drone instead of in front
        _targetRotation = Quaternion.LookRotation(droneDir, Vector3.up);
    }
}
