using UnityEngine;
using System.Collections;

public class RonsDroneScript : FlightController
{
    enum InputAxis
    {
        None = 0,
        Forward = 1,
        Backward = 2,
        StrafeLeft = 4,
        StrafeRight = 8,
        TurnLeft = 16,
        TurnRight = 32,
        PitchDown = 64,
        PitchUp = 128,
    }

    float Acceleration = 1.5f;      // How fast the drone accelerates in cmeters/second
    float SpeedFalloff = 4.0f;      // How fast the drone dumps speed when not under acceleration
    float MaxSpeed = 5.0f;          // Maximum speed in cmeters/second
    float MaxTurnSpeed = 8;         // Turning speed in degrees per second
    float TurnAcceleration = 2;
    float TurnFalloff = 6;

    float _speed;
    float _turnAmount;
    Vector3 _forward;

    public RonsDroneScript() : base()
    {
        _speed = 0;
        _turnAmount = 0;
    }

    public override void Update()
    {
        base.Update();

        InputAxis ia = UpdateInputAxis();
        if ((ia & InputAxis.Forward) == InputAxis.Forward)
        {
            float cmeters = Acceleration * Time.deltaTime;
            _speed = Mathf.Min(cmeters + _speed, MaxSpeed);
        }
        else if ((ia & InputAxis.Backward) == InputAxis.Backward)
        {
            float cmeters = Acceleration * Time.deltaTime;
            _speed = Mathf.Max(_speed - cmeters, -MaxSpeed);
        }
        else
        {
            float speedDump = SpeedFalloff * Time.deltaTime;
            if (_speed > 0)
                _speed = Mathf.Max(_speed - speedDump, 0);
            else if (_speed < 0)
                _speed = Mathf.Min(_speed + speedDump, 0);
        }
        _drone.transform.position += _forward * _speed;

        if ((ia & InputAxis.TurnLeft) == InputAxis.TurnLeft)
        {
            float turn = TurnAcceleration * Time.deltaTime;
            _turnAmount = Mathf.Max(_turnAmount - turn, -MaxTurnSpeed);
        }
        else if ((ia & InputAxis.TurnRight) == InputAxis.TurnRight)
        {
            float turn = TurnAcceleration * Time.deltaTime;
            _turnAmount = Mathf.Min(_turnAmount + turn, MaxTurnSpeed);
        }
        else
        {
            float dump = TurnFalloff * Time.deltaTime;
            if( _turnAmount > 0 )
                _turnAmount = Mathf.Max(_turnAmount - dump, 0);
            else if( _turnAmount < 0 )
                _turnAmount = Mathf.Min(_turnAmount + dump, 0);

        }
        //_drone.transform.Rotate(Vector3.up, _turnAmount);
        _forward = Quaternion.AngleAxis(_turnAmount, Vector3.up) * _forward;

        //if (_turnAmount != 0)
        {
            float bankPercent = _turnAmount / MaxTurnSpeed;
            float bankDegrees = bankPercent * 80;            
            Vector3 upDir = Quaternion.AngleAxis(-bankDegrees, _forward) * Vector3.up;
            _drone.transform.rotation = Quaternion.LookRotation(_forward, upDir);
        }

        /*
        // Forward/Backward       
        if (Input.GetKey(KeyCode.W))
            _speed += 0.01f;
        else if (Input.GetKey(KeyCode.S))
            _speed -= 0.01f;
        else
        {
            // Push toward zero
            if (_speed > 0)
            {
                _speed -= _speed * 0.05f;
            }
            else if (_speed < 0)
            {
                _speed -= _speed * 0.05f;
            }
        }
        if( _speed > 5 )
            _speed = 5;
        if( _speed < -5 )
            _speed = -5;            

        _drone.transform.position += _drone.transform.forward * _speed;

        // Strafe
        if ( Input.GetKey(KeyCode.A) )
            _drone.transform.position -= _drone.transform.right;
        if( Input.GetKey(KeyCode.D) )
            _drone.transform.position += _drone.transform.right;

        // Turn
        float rotateSpeed = Mathf.PI / 11.25f;
        if (Input.GetKey(KeyCode.LeftArrow))
            _drone.transform.Rotate(Vector3.up, -rotateSpeed);
        if (Input.GetKey(KeyCode.RightArrow))
            _drone.transform.Rotate(Vector3.up, rotateSpeed);
        */
        // Pitch
        //if( Input.GetKey(KeyCode.UpArrow) )
        //    _drone.transform.Rotate(_drone.transform.right, rotateSpeed);
        //if( Input.GetKey(KeyCode.DownArrow) )
        //    _drone.transform.Rotate(_drone.transform.right, -rotateSpeed);
    }

    InputAxis UpdateInputAxis()
    {
        InputAxis ia = InputAxis.None;
        if( Input.GetKey(KeyCode.W) )
            ia |= InputAxis.Forward;
        if (Input.GetKey(KeyCode.S))
            ia |= InputAxis.Backward;

        if (Input.GetKey(KeyCode.A))
            ia |= InputAxis.TurnLeft;
        if (Input.GetKey(KeyCode.D))
            ia |= InputAxis.TurnRight;

        return ia;
    }

    public override void SetDrone(Drone drone)
    {
        base.SetDrone(drone);
        _forward = _drone.transform.forward;
    }

    public override float Speed
    {
        get
        {
            return _speed;
        }
    }
}
