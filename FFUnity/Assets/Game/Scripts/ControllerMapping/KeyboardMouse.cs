using UnityEngine;
using System.Collections;

public class KeyboardMouse : AxisInput
{
    Vector3 _oldMousePos;

    public KeyboardMouse() : base()
    {        
    }

    public override void Update()
    {
        base.Update();

        MoveForward = Input.GetKey(KeyCode.W) ? 1 : 0;
        MoveBackward = Input.GetKey(KeyCode.S) ? 1 : 0;
        TurnLeft = Input.GetKey(KeyCode.A) ? 1 : 0;
        TurnRight = Input.GetKey(KeyCode.D) ? 1 : 0;
        
        Vector3 mouseDelta = _oldMousePos - Input.mousePosition;
        _oldMousePos = Input.mousePosition;
        
        PitchUp = Input.GetKey(KeyCode.DownArrow) ? 1 : 0;
        PitchDown = Input.GetKey(KeyCode.UpArrow) ? 1 : 0;
    }

    public override void Zero()
    {
        base.Zero();
        _oldMousePos = Input.mousePosition;
    }
}
