using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DroneControl2 : MonoBehaviour
{

    public float maxCtrlThrottle = 5000f;
    public float maxCtrlYaw = 450f;
    public float maxCtrlRoll = 10f;
    public float maxCtrlPitch = 20f;
    public float pitchToFwdVel = 40f;

    public float autoBrakeYaw = .1f;

    public float chanceOfTurbulence = 0.15f;
    public float turbulenceMaxStrength = 30f;

    public List<BladeProp> blades = new List<BladeProp>();

    private Vector3 resetPosition;
    private Quaternion resetRotation;

    private float ctrlThrottle = 0f;
    private float ctrlYaw = 0f;
    private float ctrlRoll = 0f;
    private float ctrlPitch = 0f;

    public float stability = 0.3f;
    public float stabilitySpeed = 2.0f;

    public float buzzPitchMin = 0.6f;
    public float buzzPitchMax = 1f;

    public float buzzVolMin = 0.3f;
    public float buzzVolMax = 1f;

    public StopWatchTimer airTime;

    private float timeWithoutImpact = 0f;

    private Vector3 desiredUp = Vector3.up;

    private Rigidbody rb;

    private AudioSource audio_buzz;
    private float buzzPitchRange;
    private float buzzVolRange;
    private float buzzStabilityForce = 0f;

    private int epicInventory = 0;
    private int rareInventory = 0;
    private int enhancedInventory = 0;
    private int normalInventory = 0;

    private bool landed = false;

    private bool arcadeMode = true;

    // Use this for initialization
    void Start()
    {
        audio_buzz = GetComponent<AudioSource>();
        buzzPitchRange = buzzPitchMax - buzzPitchMin;
        buzzVolRange = buzzVolMax - buzzVolMin;

        rb = GetComponent<Rigidbody>();

        resetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        resetRotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
    }

    // Update is called once per frame
    void Update()
    {
        if(arcadeMode)
            ctrlThrottle = Input.GetAxis("DroneThrottle");
        else
            ctrlThrottle = Mathf.Max(0f, Input.GetAxis("DroneThrottle"));

        ctrlYaw = Input.GetAxis("DroneYaw");
        ctrlRoll = Input.GetAxis("DroneRoll");
        ctrlPitch = Input.GetAxis("DronePitch");

        //TODO: This triggers a bug in the follow cam
        ///*
        if (Input.GetButtonUp("DroneReset"))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        //*/

        if(Input.GetButtonUp("ArcadeModeToggle"))
        {
            arcadeMode = !arcadeMode;
        }

        // Used to calculate pitch, volume AND blade spin...
        float mainBuzzAmt = Mathf.Min(1f, Mathf.Max(ctrlThrottle, buzzStabilityForce));

        //Audio Buzz
        audio_buzz.volume = (mainBuzzAmt * buzzVolRange) + buzzVolMin;
        float targetPitch = (mainBuzzAmt * buzzPitchRange) + buzzPitchMin;
        audio_buzz.pitch += (targetPitch - audio_buzz.pitch) / 100f; //interpolate to the target pitch over 100 frames

        //Spin props
        foreach (BladeProp blade in blades)
        {
            blade.propSpeed = Mathf.Max(0.4f, mainBuzzAmt); //Prop speed is set between 0 - 1.0f
        }

        //Debug Draw
        Debug.DrawLine(transform.position, transform.position + desiredUp * 4f, Color.green);
        Debug.DrawLine(transform.position, transform.position + transform.up * 4f, Color.red);
    }

    void FixedUpdate()
    {
        Quaternion q;
        desiredUp = Vector3.up;

        if(arcadeMode)
        {
            
            if (!landed)
                rb.AddForce(0, -(rb.velocity.y + Physics.gravity.y), 0, ForceMode.Acceleration);

            //Control Throttle    
            rb.AddForce(Vector3.up * maxCtrlThrottle * ctrlThrottle * Time.fixedDeltaTime);

            //Control Yaw
            //Auto brake yaw rotation
            if (Mathf.Abs(ctrlYaw) < autoBrakeYaw)
            {
                ctrlYaw = 0;
                //TODO: This should be done with a call to AddTorque instead.
                rb.angularVelocity = new Vector3(rb.angularVelocity.x, rb.angularVelocity.y * .9f, rb.angularVelocity.z);
            }

            rb.AddTorque(Vector3.up * maxCtrlYaw * ctrlYaw * Time.fixedDeltaTime);

            //Control Pitch
            desiredUp = Quaternion.AngleAxis(ctrlPitch * maxCtrlPitch, -transform.right) * desiredUp;

            rb.AddTorque(Vector3.right * ctrlPitch * maxCtrlPitch * Time.fixedDeltaTime);

            rb.AddForce(Vector3.Cross(Vector3.up, transform.right) * ctrlPitch * maxCtrlThrottle * Time.fixedDeltaTime);
            

            //Control Roll
            desiredUp = Quaternion.AngleAxis(ctrlRoll * maxCtrlRoll, -transform.forward) * desiredUp;

            rb.AddTorque(Vector3.forward * ctrlRoll * maxCtrlPitch * Time.fixedDeltaTime);

            rb.AddForce(Vector3.Cross(Vector3.up, transform.forward) * ctrlRoll * maxCtrlThrottle * Time.fixedDeltaTime);
            

        }
        else
        {
            //Control Throttle    
            rb.AddRelativeForce(Vector3.up * maxCtrlThrottle * ctrlThrottle * Time.fixedDeltaTime);

            //Control Yaw
            //Auto brake yaw rotation
            if (Mathf.Abs(ctrlYaw) < autoBrakeYaw)
            {
                ctrlYaw = 0;
                //TODO: This should be done with a call to AddTorque instead.
                rb.angularVelocity = new Vector3(rb.angularVelocity.x, rb.angularVelocity.y * .9f, rb.angularVelocity.z);
            }

            rb.AddRelativeTorque(Vector3.up * maxCtrlThrottle * ctrlYaw * Time.fixedDeltaTime);

            //Control Pitch
            q = Quaternion.AngleAxis(ctrlPitch * maxCtrlPitch, -transform.right);
            desiredUp = q * desiredUp;

            //add some slight Pitch->forward velocity
            rb.AddRelativeForce(-Vector3.forward * ctrlPitch * maxCtrlThrottle * Time.fixedDeltaTime);

            //Control Roll
            q = Quaternion.AngleAxis(ctrlRoll * maxCtrlRoll, -transform.forward);
            desiredUp = q * desiredUp;    
        }

        ////Turbulence
        //if (Random.value < chanceOfTurbulence)
        //{
        //    Vector3 wind = Vector3.down;
        //    q = Quaternion.AngleAxis(Random.Range(0, 360), transform.right);
        //    wind = q * wind;
        //    q = Quaternion.AngleAxis(Random.Range(0, 360), transform.forward);
        //    wind = q * wind;

        //    float maxTurb = Mathf.Min(10f, Mathf.Max(transform.position.y / 2f, turbulenceMaxStrength));
        //    wind *= Random.Range(0.1f, maxTurb);

        //    Vector3 pos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) + transform.position;

        //    rb.AddForceAtPosition(wind, pos);
        //}

        //Stabilizer
        Vector3 predictedUp = Quaternion.AngleAxis(rb.angularVelocity.magnitude * Mathf.Rad2Deg * stability / stabilitySpeed, rb.angularVelocity) * transform.up;

        Vector3 torqueVector = Vector3.Cross(predictedUp, desiredUp);

        rb.AddTorque(torqueVector * stabilitySpeed * stabilitySpeed);

    }

    private void DroneReset()
    {
        transform.position = resetPosition;
        transform.rotation = resetRotation;

        rb.velocity = new Vector3(0, 0, 0);
        rb.angularVelocity = new Vector3(0, 0, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Loot Box"))
        {
            if (other.gameObject.GetComponent<LootBox>().rarity == "Epic")
                epicInventory += 1;
            else if (other.gameObject.GetComponent<LootBox>().rarity == "Rare")
                rareInventory += 1;
            else if (other.gameObject.GetComponent<LootBox>().rarity == "Enhanced")
                enhancedInventory += 1;
            else if (other.gameObject.GetComponent<LootBox>().rarity == "Normal")
                normalInventory += 1;

            Destroy(other.gameObject);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Loot Box"))
            return;

        airTime.PauseStopWatch();

        if (airTime.GetCurrentTime() > timeWithoutImpact)
            timeWithoutImpact = airTime.GetCurrentTime();

        airTime.ResetStopWatch();

        if (other.gameObject.CompareTag("Landing Platform"))
            landed = true;

    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Loot Box"))
            return;

        airTime.StartStopWatch();

        if (other.gameObject.CompareTag("Landing Platform"))
            landed = false;
    }

    public float GetTimeWithoutImpact()
    {
        return timeWithoutImpact;
    }

    public int GetEpicCount()
    {
        return epicInventory;
    }

    public int GetRareCount()
    {
        return rareInventory;
    }

    public int GetEnhancedCount()
    {
        return enhancedInventory;
    }

    public int GetNormalCount()
    {
        return normalInventory;
    }

    public bool HasLanded()
    {
        return landed;
    }
}
