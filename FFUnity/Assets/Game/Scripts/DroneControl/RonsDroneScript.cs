using UnityEngine;
using System.Collections;

public class RonsDroneScript : FlightController
{
    float Acceleration = 1.5f;      // How fast the drone accelerates in cmeters/second
    float SpeedFalloff = 4.0f;      // How fast the drone dumps speed when not under acceleration
    float MaxSpeed = 5.0f;          // Maximum speed in cmeters/second
    float MaxTurnSpeed = 8;         // Turning speed in degrees per second
    float TurnAcceleration = 2;
    float TurnFalloff = 6;
    float PitchMaxDegrees = 75;
    float PitchAcceleration = 12;
    float PitchFalloff = 30;

    float _speed;
    float _turnAmount;
    float _pitch;
    Vector3 _forward;

    public RonsDroneScript() : base()
    {
        _speed = 0;
        _turnAmount = 0;
        _pitch = 0;
    }

    public override void Update()
    {
        // Base update will update the AxisInput
        base.Update();

        // Turning
        if (_input.TurnLeft > 0)
        {
            float turn = TurnAcceleration * Time.deltaTime * _input.TurnLeft;
            _turnAmount = Mathf.Max(_turnAmount - turn, -MaxTurnSpeed);
        }
        else if (_input.TurnRight > 0)
        {
            float turn = TurnAcceleration * Time.deltaTime * _input.TurnRight;
            _turnAmount = Mathf.Min(_turnAmount + turn, MaxTurnSpeed);
        }
        else
        {
            float dump = TurnFalloff * Time.deltaTime;
            if (_turnAmount > 0)
                _turnAmount = Mathf.Max(_turnAmount - dump, 0);
            else if (_turnAmount < 0)
                _turnAmount = Mathf.Min(_turnAmount + dump, 0);

        }
        _forward = Quaternion.AngleAxis(_turnAmount, Vector3.up) * _forward;

        // Pitching
        if (_input.PitchUp > 0)
        {
            float pitch = PitchAcceleration * Time.deltaTime * _input.PitchUp;
            _pitch = Mathf.Max(_pitch - pitch, -PitchMaxDegrees);
        }
        else if (_input.PitchDown > 0)
        {
            float pitch = PitchAcceleration * Time.deltaTime * _input.PitchDown;
            _pitch = Mathf.Min(_pitch + pitch, PitchMaxDegrees);
        }
        else
        {
            float dump = PitchFalloff * Time.deltaTime;
            if (_pitch > 0)
                _pitch = Mathf.Max(_pitch - dump, 0);
            else if (_pitch < 0)
                _pitch = Mathf.Min(_pitch + dump, 0);
        }
        Vector3 right = Vector3.Cross(Vector3.up, _forward);
        Quaternion pitchRotation = Quaternion.AngleAxis(_pitch, right);
        Vector3 pitchedUp = pitchRotation * Vector3.up;
        Vector3 pitchedForward = pitchRotation * _forward;

        // Movement
        if (_input.MoveForward > 0)
        {
            float cmeters = Acceleration * Time.deltaTime * _input.MoveForward;
            _speed = Mathf.Min(cmeters + _speed, MaxSpeed);
        }
        else if (_input.MoveBackward > 0)
        {
            float cmeters = Acceleration * Time.deltaTime * _input.MoveBackward;
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
        _drone.transform.position += pitchedForward * _speed;
                

        // Fake banking
        float bankPercent = _turnAmount / MaxTurnSpeed;
        float bankDegrees = bankPercent * 80;            
        Vector3 up = Quaternion.AngleAxis(-bankDegrees, pitchedForward) * pitchedUp;
        _drone.transform.rotation = Quaternion.LookRotation(pitchedForward, up);
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
