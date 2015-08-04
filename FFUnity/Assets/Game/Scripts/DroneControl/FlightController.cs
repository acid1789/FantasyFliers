using UnityEngine;
using System.Collections;

public class FlightController
{
    protected Drone _drone;

    public FlightController()
    {
    }

    // Set the drone that this flight controller controls
    public virtual void SetDrone(Drone drone)
    {
        _drone = drone;
    }

    // Reset anything in the flight controller for the beginning of flight
    public virtual void Reset()
    {
    }

    // Called every frame by the FlightCourse to apply flight control
    public virtual void Update()
    {
    }

    // Flight speed in meters per second
    public virtual float Speed
    {
        get { return 0; }
    }
}
