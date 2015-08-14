using UnityEngine;
using System.Collections;

public class DualVirtualJoysticks : AxisInput
{
    private VirtualJoystickUI LeftStick;
    private VirtualJoystickUI RightStick;

    public DualVirtualJoysticks() : base()
    {
        
    }

    public override void SetUIPrefab(GameObject uiPrefab)
    {
        //set up left joystick
        VirtualJoystickUI lStick = uiPrefab.transform.FindChild("Left Joystick").FindChild("Stick").GetComponent<VirtualJoystickUI>();
        lStick.useHorizontalAxis = true;
        lStick.useVerticalAxis = true;
        lStick.horizontalMovementRange = 50f;
        lStick.verticalMovementRange = 50f;

        //set up right joystick
        VirtualJoystickUI rStick = uiPrefab.transform.FindChild("Right Joystick").FindChild("Stick").GetComponent<VirtualJoystickUI>();
        rStick.useHorizontalAxis = true;
        rStick.useVerticalAxis = true;
        rStick.horizontalMovementRange = 50f;
        rStick.verticalMovementRange = 50f;

        base.SetUIPrefab(uiPrefab);
    }

    public override void SetUI(GameObject ui)
    {
        base.SetUI(ui);

        LeftStick = ui.transform.FindChild("Left Joystick").FindChild("Stick").GetComponent<VirtualJoystickUI>();
        RightStick = ui.transform.FindChild("Right Joystick").FindChild("Stick").GetComponent<VirtualJoystickUI>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        
        PitchUpDown = -LeftStick.VerticalAxis;
        MoveForwardBackward = RightStick.VerticalAxis;
        TurnLeftRight = -LeftStick.HorizontalAxis;
        MoveLeftRight = -RightStick.HorizontalAxis;
    }

}
