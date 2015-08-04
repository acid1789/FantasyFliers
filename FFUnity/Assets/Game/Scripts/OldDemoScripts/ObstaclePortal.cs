using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;


namespace OldDemoScripts
{
    public class ObstaclePortal : MonoBehaviour
    {

        public int portalId = 0;

        public delegate void EnterPortal(int portalId, Drone d);
        public event EnterPortal OnEnterPortal;

        public delegate void ExitPortal(int portalId, Drone d);
        public event ExitPortal OnExitPortal;

        public bool cleared = false;

        private Dictionary<Drone, int> dronesInPortal = new Dictionary<Drone, int>();

        void OnTriggerEnter(Collider other)
        {
            if (OnEnterPortal != null)
            {
                Drone d = FindDrone(other.gameObject);
                if (d != null)
                {
                    bool wasInPortal = false;
                    if (!dronesInPortal.ContainsKey(d))
                    {
                        dronesInPortal[d] = 0;
                    }
                    else
                    {
                        int partCount = dronesInPortal[d]++;
                        wasInPortal = partCount > 0;
                    }

                    if (wasInPortal == false)
                    {
                        OnEnterPortal(portalId, d);
                    }
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (OnExitPortal != null)
            {
                Drone d = FindDrone(other.gameObject);

                if (d != null)
                {
                    if (!dronesInPortal.ContainsKey(d))
                    {
                        throw new Exception("Drone exiting portal but not marked as inside!");
                    }
                    else
                    {
                        int partCount = --dronesInPortal[d];

                        if (partCount < 0)
                        {
                            partCount = 0;
                            //throw new Exception("Drone exited portal more times than it eneterd!");
                        }

                        bool nowOutOfPortal = partCount == 0;

                        if (nowOutOfPortal)
                        {
                            OnExitPortal(portalId, d);
                        }
                    }
                }
            }
        }

        private Drone FindDrone(GameObject part)
        {
            /*
            Drone d = null;

            do {
                d = part.GetComponent<Drone> ();
                if (d) {
                    break;
                }

                Transform T = part.transform.parent;
                if (T != null) {
                    part = T.gameObject;
                }
                else {
                    part = null;
                }
            } while( part != null);
            */
            Drone d = part.GetComponent<Drone>();

            return d;
        }
    }
}