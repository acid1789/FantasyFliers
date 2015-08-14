using UnityEngine;
using System.Collections;

public class Xbox360Controller : AxisInput
{

    public Xbox360Controller() : base()
    {
        
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        PitchUpDown = Input.GetAxis("DroneThrottle");
        MoveForwardBackward = Input.GetAxis("DronePitch");
        TurnLeftRight = Input.GetAxis("DroneYaw");
        MoveLeftRight = Input.GetAxis("DroneRoll");
    }
}
