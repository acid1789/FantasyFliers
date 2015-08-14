using UnityEngine;
using System.Collections;

public class RealisticFlightController : FlightController
{
    public float maxCtrlThrottle = 1600f;
    public float maxCtrlYaw = 750f;
    public float maxCtrlRoll = 50f;
    public float maxCtrlPitch = 50f;
    public float pitchToFwdVel = 300f;

    public float stability = 5f;
    public float stabilitySpeed = 10f;

    public float autoBrakeYaw = .1f;

    public float chanceOfTurbulence = 0.15f;
    public float turbulenceMaxStrength = 30f;

    private Rigidbody rb;
    private Vector3 desiredUp;

    public RealisticFlightController() : base()
    {
        
    }

    public override void SetDrone(Drone drone)
    {
        base.SetDrone(drone);

        rb = _drone.GetComponent<Rigidbody>();

        //Enable Gravity
        rb.useGravity = true;
    }

    // Update is called once per frame
    public override void Update()
    {

        //Control Throttle    
        rb.AddRelativeForce(Vector3.up * maxCtrlThrottle * _input.PitchUpDown * Time.fixedDeltaTime);

        //Control Yaw
        //Auto brake yaw rotation
        if (Mathf.Abs(_input.TurnLeftRight) < autoBrakeYaw)
        {
            //TODO: This should be done with a call to AddTorque instead.
            rb.angularVelocity = new Vector3(rb.angularVelocity.x, rb.angularVelocity.y * .9f, rb.angularVelocity.z);
        }

        rb.AddRelativeTorque(Vector3.up * maxCtrlYaw * _input.TurnLeftRight * Time.fixedDeltaTime);

        //Control Pitch
        desiredUp = Quaternion.AngleAxis(_input.MoveForwardBackward * maxCtrlPitch, -rb.transform.right) * desiredUp;

        //add some slight Pitch->forward velocity
        rb.AddRelativeForce(-Vector3.forward * _input.MoveForwardBackward * maxCtrlThrottle * Time.fixedDeltaTime);

        //Control Roll
        desiredUp = Quaternion.AngleAxis(_input.MoveLeftRight * maxCtrlRoll, -rb.transform.forward) * desiredUp;
        
        //Turbulence
        if (Random.value < chanceOfTurbulence)
        {
            Vector3 wind = Vector3.down;
            Quaternion q = Quaternion.AngleAxis(Random.Range(0, 360), rb.transform.right);
            wind = q * wind;
            q = Quaternion.AngleAxis(Random.Range(0, 360), rb.transform.forward);
            wind = q * wind;

            float maxTurb = Mathf.Min(10f, Mathf.Max(rb.transform.position.y / 2f, turbulenceMaxStrength));
            wind *= Random.Range(0.1f, maxTurb);

            Vector3 pos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) + rb.transform.position;

            rb.AddForceAtPosition(wind, pos);
        }

        //Stabilizer
        Vector3 predictedUp = Quaternion.AngleAxis(rb.angularVelocity.magnitude * Mathf.Rad2Deg * stability / stabilitySpeed, rb.angularVelocity) * rb.transform.up;

        Vector3 torqueVector = Vector3.Cross(predictedUp, desiredUp);

        rb.AddTorque(torqueVector * stabilitySpeed * stabilitySpeed);
    }
}
