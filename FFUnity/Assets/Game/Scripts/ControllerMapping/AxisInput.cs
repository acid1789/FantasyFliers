using UnityEngine;
using System.Collections;

public class AxisInput
{
    float _moveForward;
    float _moveBackward;
    float _moveLeft;
    float _moveRight;
    float _turnLeft;
    float _turnRight;
    float _pitchUp;
    float _pitchDown;

    GameObject _UI = null;
    GameObject _uiPrefab = null;

    public AxisInput()
    {
        Zero();
    }

    public virtual void Zero()
    {
        _moveForward = 0;
        _moveBackward = 0;
        _moveLeft = 0;
        _moveRight = 0;
        _turnLeft = 0;
        _turnRight = 0;
        _pitchUp = 0;
        _pitchDown = 0;
    }

    public virtual void Update()
    {
    }

    public virtual void SetUI(GameObject ui)
    {
        _UI = ui;
    }

    public virtual void SetUIPrefab(GameObject uiPrefab)
    {
        _uiPrefab = uiPrefab;
    }

    public virtual GameObject GetUI()
    {
        return _UI;
    }

    public virtual GameObject GetUIPrefab()
    {
        return _uiPrefab;
    }

    public float MoveForward
    {
        get { return _moveForward; }
        protected set { _moveForward = value; }
    }

    public float MoveBackward
    {
        get { return _moveBackward; }
        protected set { _moveBackward = value; }
    }

    public float MoveForwardBackward
    {
        get { return _moveForward - _moveBackward; }
        protected set
        {
            if (value < 0)
            {
                _moveForward = 0;
                _moveBackward = -value;
            }
            else
            {
                _moveForward = value;
                _moveBackward = 0;
            }
        }
    }

    public float MoveLeft
    {
        get { return _moveLeft; }
        protected set { _moveLeft = value; }
    }

    public float MoveRight
    {
        get { return _moveRight; }
        protected set { _moveRight = value; }
    }

    public float MoveLeftRight
    {
        get { return _moveRight - _moveLeft; }
        protected set
        {
            if (value < 0)
            {
                _moveRight = 0;
                _moveLeft = -value;
            }
            else
            {
                _moveRight = value;
                _moveLeft = 0;
            }
        }
    }

    public float TurnLeft
    {
        get { return _turnLeft; }
        protected set { _turnLeft = value; }
    }

    public float TurnRight
    {
        get { return _turnRight; }
        protected set { _turnRight = value; }
    }

    public float TurnLeftRight
    {
        get { return _turnRight - _turnLeft; }
        protected set
        {
            if (value < 0)
            {
                _turnRight = 0;
                _turnLeft = -value;
            }
            else
            {
                _turnRight = value;
                _turnLeft = 0;
            }
        }
    }

    public float PitchDown
    {
        get { return _pitchDown; }
        protected set { _pitchDown = value; }
    }

    public float PitchUp
    {
        get { return _pitchUp; }
        protected set { _pitchUp = value; }
    }

    public float PitchUpDown
    {
        get { return _pitchDown - _pitchUp; }
        protected set
        {
            if (value < 0)
            {
                _pitchDown = 0;
                _pitchUp = -value;
            }
            else
            {
                _pitchDown = value;
                _pitchUp = 0;
            }
        }
    }
}
