using UnityEngine;
using System.Collections;

namespace OldDemoScripts
{
    public class StopWatchTimer : MonoBehaviour
    {

        private float timer = 0f;

        private bool timerRunning = false;

        // Use this for initialization
        void Start()
        {
            timer = 0f;
            timerRunning = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (timerRunning)
                timer += Time.deltaTime;
        }

        public bool IsTimerRunning()
        {
            return timerRunning;
        }

        public float GetCurrentTime()
        {
            return timer;
        }

        public void StartStopWatch()
        {
            timerRunning = true;
        }

        public void PauseStopWatch()
        {
            timerRunning = false;
        }

        public void ResetStopWatch()
        {
            timer = 0f;
        }
    }
}
