﻿using UnityEngine;
using System.Collections;

namespace OldDemoScripts
{
    public class LookAt : MonoBehaviour
    {

        public GameObject target;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.LookAt(target.transform);
        }
    }
}