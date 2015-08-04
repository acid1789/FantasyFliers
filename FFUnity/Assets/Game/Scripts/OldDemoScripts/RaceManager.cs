using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace OldDemoScripts
{
    public class RaceManager : MonoBehaviour
    {

        //countdown timer for race to begin
        public CountdownTimer startingLineTimer;

        //stopwatch to time how long it takes to complete course
        public StopWatchTimer courseTimer;

        //drone controller ref for accessing time w/o impact stopwatch and loot counts
        public DroneControl2 drone;

        //obstacle course ref for knowing when the course has been completed
        public ObstacleCourse course;

        //reference to the canvas
        public Canvas ui;

        private bool greenFlag = false;
        private bool checkerFlag = false;

        private GameObject messageText;
        private GameObject countdownText;
        private GameObject resultsPanel;
        private GameObject courseTimeText;
        private GameObject airTimeText;
        private GameObject epicText;
        private GameObject rareText;
        private GameObject enhancedText;
        private GameObject normalText;
        private GameObject courseTimerText;

        // Use this for initialization
        void Start()
        {
            messageText = ui.gameObject.transform.FindChild("Message Text").gameObject;
            countdownText = ui.gameObject.transform.FindChild("Countdown Text").gameObject;
            resultsPanel = ui.gameObject.transform.FindChild("Results Panel").gameObject;
            courseTimeText = ui.gameObject.transform.FindChild("Results Panel").gameObject.transform.FindChild("Course Time").gameObject;
            airTimeText = ui.gameObject.transform.FindChild("Results Panel").gameObject.transform.FindChild("Air Time").gameObject;
            epicText = ui.gameObject.transform.FindChild("Results Panel").gameObject.transform.FindChild("Epic Text").gameObject;
            rareText = ui.gameObject.transform.FindChild("Results Panel").gameObject.transform.FindChild("Rare Text").gameObject;
            enhancedText = ui.gameObject.transform.FindChild("Results Panel").gameObject.transform.FindChild("Enhanced Text").gameObject;
            normalText = ui.gameObject.transform.FindChild("Results Panel").gameObject.transform.FindChild("Normal Text").gameObject;
            courseTimerText = ui.gameObject.transform.FindChild("Course Time Text").gameObject;

            resultsPanel.SetActive(false);
            drone.enabled = false;

        }

        // Update is called once per frame
        void Update()
        {
            if (startingLineTimer.IsTimerExpiered() && !greenFlag)
            {
                courseTimer.StartStopWatch();
                drone.enabled = true;
                greenFlag = true;

                messageText.GetComponent<Text>().text = "GO!";
                countdownText.GetComponent<Text>().text = "";

                Invoke("ClearMessageText", 1.5f);


                Debug.Log("GO!");
            }
            else if (!greenFlag)
            {
                messageText.GetComponent<Text>().text = "Get Ready";
                countdownText.GetComponent<Text>().text = "" + (int)startingLineTimer.GetTimeRemaining();
            }


            if (course.IsCourseComplete() && drone.HasLanded() && !checkerFlag)
            {
                courseTimer.PauseStopWatch();

                checkerFlag = true;

                messageText.GetComponent<Text>().text = "Finished!";
                resultsPanel.SetActive(true);

                float courseTime = courseTimer.GetCurrentTime();
                courseTimeText.GetComponent<Text>().text = string.Format("Course Time: {0:00}:{1:00}:{2:00}", (int)courseTime / 60, (int)courseTime % 60, (int)(courseTime * 100) % 100);
                float timeInAir = drone.GetTimeWithoutImpact();
                airTimeText.GetComponent<Text>().text = string.Format("Longest Time without Impact: {0:00}:{1:00}:{2:00}", (int)timeInAir / 60, (int)timeInAir % 60, (int)(timeInAir * 100) % 100);


                epicText.GetComponent<Text>().text = "Epic: " + drone.GetEpicCount();
                rareText.GetComponent<Text>().text = "Rare: " + drone.GetRareCount();
                enhancedText.GetComponent<Text>().text = "Enhanced: " + drone.GetEnhancedCount();
                normalText.GetComponent<Text>().text = "Normal: " + drone.GetNormalCount();

                Debug.Log("Course Time: " + courseTimer.GetCurrentTime());
                Debug.Log("Longest Time In Air Without Impact: " + drone.GetTimeWithoutImpact());
                Debug.Log("Epic Items Collected: " + drone.GetEpicCount());
                Debug.Log("Rare Items Collected: " + drone.GetRareCount());
                Debug.Log("Enhanced Items Collected " + drone.GetEnhancedCount());
                Debug.Log("Normal Items Collected " + drone.GetNormalCount());
            }

            float time = courseTimer.GetCurrentTime();
            courseTimerText.GetComponent<Text>().text = string.Format("{0:00}:{1:00}:{2:00}", (int)time / 60, (int)time % 60, (int)(time * 100) % 100);

        }

        void ClearMessageText()
        {
            messageText.GetComponent<Text>().text = "";
            countdownText.GetComponent<Text>().text = "";
        }
    }
}