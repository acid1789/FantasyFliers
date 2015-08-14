using UnityEngine;
using System.Collections;

public class FlightController
{
    protected Drone _drone;
    protected AxisInput _input;

    public FlightController()
    {

    }

    // Set the axis input handler
    public virtual void SetAxisInput(AxisInput axisInput)
    {
        _input = axisInput;
    }

    // Gets the axis input handler
    public virtual AxisInput GetAxisInput()
    {
        return _input;
    }

    // Set the drone that this flight controller controls
    public virtual void SetDrone(Drone drone)
    {
        _drone = drone;
    }

    // Reset anything in the flight controller for the beginning of flight
    public virtual void Reset()
    {
        _input.Zero();
    }

    // Called every frame by the FlightCourse to apply flight control
    public virtual void Update()
    {
        _input.Update();
    }

    public virtual void FixedUpdate()
    {

    }

    // Flight speed in meters per second
    public virtual float Speed
    {
        get { return 0; }
    }

    public Vector3 DroneForwardMovementVector
    {
        get
        {
            if (_drone != null)
                return Vector3.Cross(Vector3.up, Vector3.Cross(Vector3.up, -_drone.transform.forward));
            else
                return Vector3.zero;
        }
    }
}
