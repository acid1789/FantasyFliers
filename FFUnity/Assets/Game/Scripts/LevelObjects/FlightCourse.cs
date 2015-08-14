using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FlightCourse : MonoBehaviour
{

    public enum FlightMode
    {
        TimeTrial,
        Race,
        Infinite,
        Test
    }

    public int CourseID;
    public GameObject[] StartingPoints;
    public int LapCount;
    public FlightMode GameMode;
    public float ParTimeSeconds;
    public int TimeScoreBase;
    public int TimeScoreModifier;
    public LootTable LootTable;

    GameCamera[] _cameras;
    Drone _playerDrone;

    Obstacle _finishLine;
    List<Obstacle> _obstacles;
    List<int> _orderedObstacles;
    List<int> _requiredObstacles;
    List<int> _finishedObstacles;
    List<Obstacle> _finishedObstacleObjects;
    List<LootMarker> _lootMarkers;

    GameObject _countdownUI;
    GameObject _hudUI;
    GameObject _flightUI;
    HudUI _hud;
    bool _flightLocked;

    int _nextObstacle;
    int _lap;
    DateTime _lapStart;
    List<TimeSpan> _lapTimes;
    TimeSpan _bestTime;

    int _score;
    int _timeScore;

    GameObject _scoreScreen;

    // Use this for initialization
    void Start()
    {
#if UNITY_EDITOR
        Globals.PlayInEditor();
#endif

        SetupCourse();
        SetupDrones();
        SetupHud();
        StartCountdown();
    }

    void DestroyLevel()
    {
        // Kill ui
        if (_hudUI != null)
        {
            DestroyObject(_hudUI);
            _hudUI = null;
            _hud = null;
        }

        if(_flightUI != null)
        {
            DestroyObject(_flightUI);
            _flightUI = null;
        }

        _finishLine = null;
        _obstacles.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        // Update countdown
        if (_countdownUI != null)
        {
            CountdownUI cdui = _countdownUI.GetComponent<CountdownUI>();
            if (cdui.CanGo)
            {
                _flightLocked = false;
                _hud.StartFlight();
                _lapStart = DateTime.Now;
            }
            if (cdui.IsDone)
            {
                DestroyObject(_countdownUI);
                _countdownUI = null;
            }
        }

        // Update the flight controller
        if (!_flightLocked)
            Globals.FlightController.Update();
    }

    void FixedUpdate()
    {
        if (!_flightLocked)
            Globals.FlightController.FixedUpdate();
    }

    void SetupCourse()
    {
        _obstacles = new List<Obstacle>();
        _orderedObstacles = new List<int>();
        _requiredObstacles = new List<int>();
        _lootMarkers = new List<LootMarker>();

        // Find the start line && finish line
        foreach (Transform child in transform)
        {
            Obstacle obstacle = child.GetComponent<Obstacle>();
            if (obstacle == null)
                throw new System.Exception(string.Format("Object {0} is a child of {1}(FlightCourse) but is not an Obstacle", child.gameObject.name, gameObject.name));
            else
            {
                if (obstacle.FinishLine)
                {
                    if (_finishLine != null && _finishLine != obstacle)
                        throw new System.Exception(string.Format("Duplicate finish line {0}. Start line is already set to: {1}", child.gameObject.name, _finishLine.gameObject.name));
                    else
                        _finishLine = obstacle;
                }

                // Add to the obstacle list
                int id = _obstacles.Count;
                if (obstacle.MergeWithPrevious)
                    obstacle.OrderID = id - 1;
                else
                {
                    _obstacles.Add(obstacle);
                    obstacle.OrderID = id;
                }

                // Add to the ordered list
                if (obstacle.Ordered && !obstacle.MergeWithPrevious)
                    _orderedObstacles.Add(obstacle.OrderID);

                // Add to the required list
                if (obstacle.Required && !obstacle.MergeWithPrevious)
                    _requiredObstacles.Add(obstacle.OrderID);

                LootMarker lm = child.GetComponent<LootMarker>();
                if( lm != null )
                    _lootMarkers.Add(lm);
            }
        }

        if (_finishLine == null)
            throw new System.Exception("Finish Line is missing");
    }

    void SetupHud()
    {
        switch (GameMode)
        {
            case FlightMode.TimeTrial:
                _hudUI = (GameObject)Instantiate(Globals.UI_Hud_TimeTrial);
                break;
            case FlightMode.Race:
                break;
            case FlightMode.Infinite:
                break;
            case FlightMode.Test:
                break;
        }
        _hud = _hudUI.GetComponent<HudUI>();

        if (Globals.FlightController.GetAxisInput().GetUIPrefab() != null)
        {
            Debug.Log("Instantiating Flight UI");
            _flightUI = (GameObject)Instantiate(Globals.FlightController.GetAxisInput().GetUIPrefab());
            Globals.FlightController.GetAxisInput().SetUI(_flightUI);
        }
    }

    void SetupDrones()
    {
        if (StartingPoints.Length <= 0)
            throw new System.Exception("No starting points");

        // Spawn the player drone
        GameObject playerDrone = (GameObject)GameObject.Instantiate(Globals.Drones[0].Prefab, StartingPoints[0].transform.position, StartingPoints[0].transform.rotation);
        _playerDrone = playerDrone.GetComponent<Drone>();

        // Spawn AI Drones

        // Spawn Cameras
        _cameras = new GameCamera[Globals.Cameras.Length];
        for (int i = 0; i < _cameras.Length; i++)
        {
            GameObject cam = (GameObject)Instantiate(Globals.Cameras[i].Prefab);
            GameCamera gc = cam.GetComponent<GameCamera>();
            gc.Setup(this, _playerDrone);
            cam.SetActive(false);
            _cameras[i] = gc;
        }
        _cameras[0].gameObject.SetActive(true);     // Activate the main camera

        // Assign flight controller
        Globals.FlightController.Reset();
        Globals.FlightController.SetDrone(_playerDrone);
    }

    void StartCountdown()
    {
        // Reset level stuff
        _nextObstacle = 0;
        _score = 0;
        _timeScore = 0;
        _finishedObstacles = new List<int>();
        _finishedObstacleObjects = new List<Obstacle>();

        _lapTimes = new List<TimeSpan>();

        // Create the countdown UI object
        _countdownUI = (GameObject)Instantiate(Globals.UI_Countdown);
        _flightLocked = true;
    }

    void OnObstacleCompleted(Obstacle obstacle)
    {
        Debug.Log(string.Format("ObstacleCompleted: {0}", obstacle.name));


        if (obstacle.Ordered)
        {
            int index = _orderedObstacles.IndexOf(obstacle.OrderID);
            if (index == _nextObstacle)
            {
                // Finish out this obstacle
                FinishObstacle(obstacle);

                // Move on to the next obstacle
                _nextObstacle++;
            }
        }
        else
        {
            // Just go ahead and finish out the obstacle and apply score
            FinishObstacle(obstacle);
        }

        if (obstacle == _finishLine)
        {
            // Check to see if all required obstacles have been hit
            bool hitAllRequired = true;
            foreach (int required in _requiredObstacles)
            {
                if (_finishedObstacles.IndexOf(required) < 0)
                {
                    hitAllRequired = false;
                    break;
                }
            }
            if (hitAllRequired)
                FinishLap();
        }
    }

    void FinishObstacle(Obstacle obstacle)
    {
        obstacle.Finish();
        _finishedObstacles.Add(obstacle.OrderID);
        _finishedObstacleObjects.Add(obstacle);

        // Apply Score
        _score += obstacle.Score;
    }

    void FinishLap()
    {
        TimeSpan lapTime = DateTime.Now - _lapStart;
        _lapTimes.Add(lapTime);

        _hud.StopFlight();
        _lap++;
        if (_lap < LapCount)
        {
            // Reset and go again
            _finishedObstacles = new List<int>();
            foreach (Obstacle obstacle in _obstacles)
            {
                if (obstacle.ResetsWithLap)
                    obstacle.Reset();
            }
            _hud.StartFlight();
            _lapStart = DateTime.Now;
        }
        else
        {
            // Race complete
            ShowScoreScreen();
        }
    }

    void ShowScoreScreen()
    {
        // Disable flight control
        _flightLocked = true;

        // Find the best lap time
        double bestTime = double.MaxValue;
        foreach (TimeSpan span in _lapTimes)
        {
            if (span.TotalMilliseconds < bestTime)
            {
                bestTime = span.TotalMilliseconds;
                _bestTime = span;
            }
        }

        // Spawn the score screen
        _scoreScreen = (GameObject)Instantiate(Globals.UI_ScoreScreen);
        ScoreScreenUI ss = _scoreScreen.GetComponent<ScoreScreenUI>();
        ss.SetFlightCourse(this);

#if UNITY_EDITOR
        if (Globals.Network == null)
        {
            // Playing the course in the editor wont be connected to the server, add in fakery
            ss.FillInScores(_score, 0);
        }
        else
#endif
        {
            // Send the result to the server
            Globals.Network.FlightCourseFinished(CourseID, bestTime, LootMarkersCollected, _score);
        }
    }

    #region Accessors
    public TimeSpan BestTime
    {
        get { return _bestTime; }
    }

    public int FinishedObstacleCount
    {
        get { return _finishedObstacleObjects.Count; }
    }

    public int Score
    {
        get { return _score; }
    }

    public int TimeScore
    {
        get { return _timeScore; }
    }

    public int TotalScore
    {
        get { return _score + _timeScore; }
    }

    public int LootMarkersCollected
    {
        get
        {
            int count = 0;
            foreach (LootMarker lm in _lootMarkers)
            {
                if( lm.Completed )
                    count++;
            }
            return count;
        }
    }

    public int LootMarkersMax
    {
        get { return _lootMarkers.Count; }
    }
    #endregion
}
