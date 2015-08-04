using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace OldDemoScripts
{
    public class Obstacle : MonoBehaviour
    {

        public GameObject visibleModel;
        public List<ObstaclePortal> portals;

        private Color originalColor;
        private bool activated = false;

        private bool cleared = false;

        public static Color highlightColor = new Color(0, 1f, 0);

        // Use this for initialization
        void Start()
        {

            for (int i = 0; i < portals.Count; ++i)
            {
                if (portals[i] != null)
                {
                    portals[i].OnEnterPortal += this.OnEnterPortal;
                    portals[i].OnExitPortal += this.OnExitPortal;
                }
            }

        }

        // Update is called once per frame
        void Update()
        {

            for (int i = 0; i < portals.Count; i++)
            {
                if (portals[i].cleared)
                    this.cleared = true;
                else
                {
                    this.cleared = false;
                    break;
                }

            }


        }

        public bool IsClear()
        {
            return cleared;
        }

        public void ResetObstacle()
        {
            cleared = false;

            foreach (ObstaclePortal portal in portals)
                portal.cleared = false;
        }

        public void Activate()
        {
            if (!activated)
            {
                activated = true;

                Renderer renderer = visibleModel.GetComponent<Renderer>();
                originalColor = renderer.material.color;

                renderer.material.color = highlightColor;
            }
        }

        public void Deactivate()
        {
            if (activated)
            {
                activated = false;

                Renderer renderer = visibleModel.GetComponent<Renderer>();
                renderer.material.color = originalColor;
            }
        }

        private void OnEnterPortal(int portalId, Drone d)
        {
            if (activated)
            {
                Debug.Log("Enter portal " + portalId);
            }
        }

        private void OnExitPortal(int portalId, Drone d)
        {
            if (activated)
            {
                Debug.Log("Exit portal " + portalId);

                portals[portalId].cleared = true;
            }
        }

    }
}