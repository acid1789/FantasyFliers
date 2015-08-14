using UnityEngine;
using System.Collections;

public class ArcadeFlightController : FlightController
{
    public float maxCtrlThrottle = 1600f;
    public float maxCtrlYaw = 750f;
    public float maxCtrlRoll = 50f;
    public float maxCtrlPitch = 20f;

    public float stability = 5f;
    public float stabilitySpeed = 10f;

    public float autoBrakeYaw = .1f;

    public float chanceOfTurbulence = 0.15f;
    public float turbulenceMaxStrength = 30f;

    private Rigidbody rb;
    private Vector3 desiredUp;

    public ArcadeFlightController() : base()
    {
        
    }

    public override void SetDrone(Drone drone)
    {
        base.SetDrone(drone);

        rb = _drone.GetComponent<Rigidbody>();

        //Enable Gravity
        rb.useGravity = true;

        //set drag
        rb.drag = 0.1f;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        desiredUp = Vector3.up;

        //Auto-Hover
        //TODO: Need to port over landing code
        rb.AddForce(0, -(rb.velocity.y + Physics.gravity.y), 0, ForceMode.Acceleration);

        //Ascent/Descent
        rb.AddForce(Vector3.up * _input.PitchUpDown * maxCtrlThrottle * Time.fixedDeltaTime);
        
        //Yaw
        //Auto brake yaw
        if(Mathf.Abs(_input.TurnLeftRight) < autoBrakeYaw)
        {
            rb.angularVelocity = new Vector3(rb.angularVelocity.x, rb.angularVelocity.y * 0.9f, rb.angularVelocity.z);
        }

        rb.AddTorque(Vector3.up * _input.TurnLeftRight * maxCtrlYaw * Time.fixedDeltaTime);

        //Pitch/Forward/Backward 
        desiredUp = Quaternion.AngleAxis(_input.MoveForwardBackward * maxCtrlPitch, -_drone.transform.right) * desiredUp;

        rb.AddTorque(Vector3.right * _input.MoveForwardBackward * maxCtrlPitch * Time.fixedDeltaTime);

        rb.AddForce(Vector3.Cross(Vector3.up, _drone.transform.right) * _input.MoveForwardBackward * maxCtrlThrottle * Time.fixedDeltaTime);

        //Roll/Left/Right
        desiredUp = Quaternion.AngleAxis(_input.MoveLeftRight * maxCtrlRoll, -_drone.transform.forward) * desiredUp;

        rb.AddTorque(Vector3.forward * _input.MoveLeftRight * maxCtrlRoll * Time.fixedDeltaTime);

        rb.AddForce(Vector3.Cross(Vector3.up, _drone.transform.forward) * _input.MoveLeftRight * maxCtrlThrottle * Time.fixedDeltaTime);

        //Turbulence
        if (Random.value < chanceOfTurbulence)
        {
            Vector3 wind = Vector3.down;
            Quaternion q = Quaternion.AngleAxis(Random.Range(0, 360), _drone.transform.right);
            wind = q * wind;
            q = Quaternion.AngleAxis(Random.Range(0, 360), _drone.transform.forward);
            wind = q * wind;

            float maxTurb = Mathf.Min(10f, Mathf.Max(_drone.transform.position.y / 2f, turbulenceMaxStrength));
            wind *= Random.Range(0.1f, maxTurb);

            Vector3 pos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) + _drone.transform.position;

            rb.AddForceAtPosition(wind, pos);
        }

        //Stabilizer
        Vector3 predictedUp = Quaternion.AngleAxis(rb.angularVelocity.magnitude * Mathf.Rad2Deg * stability / stabilitySpeed, rb.angularVelocity) * _drone.transform.up;

        Vector3 torqueVector = Vector3.Cross(predictedUp, desiredUp);

        rb.AddTorque(torqueVector * stabilitySpeed * stabilitySpeed);

    }
}
