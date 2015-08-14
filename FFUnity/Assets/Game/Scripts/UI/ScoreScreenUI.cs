using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ScoreScreenUI : MonoBehaviour
{
    enum AnimPhase
    {
        ParTime,
        YourTimeReveal,
        YourTimeDisplay,
        Obstacles,
        ObstaclesScore,
        TimeBonus,
        TimeBonusScore,
        Total,
        TotalScore,
        Loot,
        Done,
        Skip
    }
    
    FlightCourse _flightCourse;

    AnimPhase _animPhase;
    float _animWaitTime;
    const float _animStepTime = 1;

    Text _currency;
    Text _currencyDesc;
    Text _lootDisplay;
    Text _timeDisplay;
    Text _parTimeDisplay;
    Button _doneButton;

    bool _scoresFilledIn = false;
    int _obstacleScore;
    int _timeScore;
    int _totalScore;

	// Use this for initialization
	void Start ()
    {
        Transform panel = transform.FindChild("Panel");

        _currency = panel.FindChild("Currency").GetComponent<Text>();
        _currencyDesc = panel.FindChild("CurrencyDesc").GetComponent<Text>();
        _lootDisplay = panel.FindChild("Loot").GetComponent<Text>();
        _timeDisplay = panel.FindChild("YourTime").GetComponent<Text>();
        _parTimeDisplay = panel.FindChild("ParTime").GetComponent<Text>();	

        _doneButton = panel.FindChild("DoneButton").GetComponent<Button>();
        _doneButton.onClick.AddListener(OnDoneButton);
        _doneButton.enabled = false;

        _currency.text = "";
        _currencyDesc.text = "";
        _lootDisplay.text = "";
        _timeDisplay.text = "";
        _parTimeDisplay.text = "";

        _animPhase = AnimPhase.ParTime;
        _animWaitTime = _animStepTime;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!_scoresFilledIn)
        {
            if (Globals.Network != null)
            {
                // Wait until we have server data
                if (Globals.Network.CourseCompleteInfo == null)
                {
                    // TODO: Add wait animation here
                    return;
                }
                else
                {
                    FillInScores(Globals.Network.CourseCompleteInfo.ObstacleScore, Globals.Network.CourseCompleteInfo.TimeScore);
                }
            }
        }
        else
        {
            // Make sure everything is setup before we start
            if (_flightCourse != null && _doneButton != null)
            {
                if (_animWaitTime > 0)
                    _animWaitTime -= Time.deltaTime;

                if (_animWaitTime <= 0)
                {
                    switch (_animPhase)
                    {
                        case AnimPhase.ParTime:
                            DoParTime();
                            break;
                        case AnimPhase.YourTimeReveal:
                            DoYourTimeReveal();
                            break;
                        case AnimPhase.YourTimeDisplay:
                            DoYourTimeDisplay();
                            break;
                        case AnimPhase.Obstacles:
                            DoObstacles();
                            break;
                        case AnimPhase.ObstaclesScore:
                            DoObstaclesScore();
                            break;
                        case AnimPhase.TimeBonus:
                            DoTimeBonus();
                            break;
                        case AnimPhase.TimeBonusScore:
                            DoTimeBonusScore();
                            break;
                        case AnimPhase.Total:
                            DoTotal();
                            break;
                        case AnimPhase.TotalScore:
                            DoTotalScore();
                            break;
                        case AnimPhase.Loot:
                            DoLoot();
                            break;
                        case AnimPhase.Done:
                            _doneButton.enabled = true;
                            break;
                        case AnimPhase.Skip:
                            DoParTime();
                            DoYourTimeReveal();
                            DoYourTimeDisplay();
                            DoObstacles();
                            DoObstaclesScore();
                            DoTimeBonus();
                            DoTimeBonusScore();
                            DoTotal();
                            DoTotalScore();
                            DoLoot();
                            _animPhase = AnimPhase.Done;
                            break;
                    }
                }
            }
        }
	}

    public void FillInScores(int obstacleScore, int timeScore)
    {
        _scoresFilledIn = true;
        _obstacleScore = obstacleScore;
        _timeScore = timeScore;
        _totalScore = _obstacleScore + _timeScore;
    }

    void OnDoneButton()
    {
        // Goto hub
        Application.LoadLevel("HubScene");
    }

    public void SetFlightCourse(FlightCourse fc)
    {
        _flightCourse = fc;
    }

    void AnimWait(AnimPhase nextPhase)
    {
        _animPhase = nextPhase;
        _animWaitTime = _animStepTime;
    }

    void DoParTime()
    {
        // Show the par time
        TimeSpan par = new TimeSpan(0, 0, 0, 0, (int)(_flightCourse.ParTimeSeconds * 1000.0f));
        _parTimeDisplay.text = string.Format("Par Time\n{0:00.}:{1:00.}:{2:00}", par.Minutes, par.Seconds, par.Milliseconds * 0.1f);

        // Wait
        AnimWait(AnimPhase.YourTimeReveal);
    }

    void DoYourTimeReveal()
    {
        _timeDisplay.text = "Your Time\n";

        AnimWait(AnimPhase.YourTimeDisplay);
    }

    void DoYourTimeDisplay()
    {
        TimeSpan time = _flightCourse.BestTime;
        _timeDisplay.text += string.Format("{0:00.}:{1:00.}:{2:00}", time.Minutes, time.Seconds, time.Milliseconds * 0.1f);

        AnimWait(AnimPhase.Obstacles);
    }

    void DoObstacles()
    {
        string obstacles = string.Format("Obstacles ({0})\n", _flightCourse.FinishedObstacleCount);
        _currencyDesc.text = obstacles;

        AnimWait(AnimPhase.ObstaclesScore);
    }

    void DoObstaclesScore()
    {
        string score = string.Format("{0:N0}\n", _obstacleScore);
        _currency.text = score;
        
        AnimWait(AnimPhase.TimeBonus);
    }

    void DoTimeBonus()
    {
        _currencyDesc.text += "Time Bonus\n\n";

        AnimWait(AnimPhase.TimeBonusScore);
    }

    void DoTimeBonusScore()
    {
        _currency.text += _timeScore.ToString("N0") + "\n\n";

        AnimWait(AnimPhase.Total);
    }

    void DoTotal()
    {
        _currencyDesc.text += "Total";
        AnimWait(AnimPhase.TotalScore);
    }

    void DoTotalScore()
    {
        _currency.text += _totalScore.ToString("N0");
        AnimWait(AnimPhase.Loot);
    }

    void DoLoot()
    {
        _lootDisplay.text = string.Format("Loot Markers: {0} / {1}", _flightCourse.LootMarkersCollected, _flightCourse.LootMarkersMax);

        AnimWait(AnimPhase.Done);
    }
}
