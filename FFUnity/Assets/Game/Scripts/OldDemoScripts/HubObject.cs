using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace OldDemoScripts
{
    public class HubObject : MonoBehaviour
    {

        public RectTransform UICanvas;

        // Use this for initialization
        void Start()
        {
            Button exitButton = UICanvas.FindChild("Exit").GetComponent<Button>();
            exitButton.onClick.AddListener(ExitButton);

            Button timeTrialButton = UICanvas.FindChild("Time Trial Button").GetComponent<Button>();
            timeTrialButton.onClick.AddListener(TimeTrialButton);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void ExitButton()
        {
            Application.Quit();
        }

        void TimeTrialButton()
        {
            Application.LoadLevel("Test Flight 1");
        }
    }
}