using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace OldDemoScripts
{
    public class ObstacleCourse : MonoBehaviour
    {

        public List<Obstacle> course;

        private int nextObstacle = 0;

        private bool courseCompleted = false;

        // Use this for initialization
        void Start()
        {

            ActivateObstacle(nextObstacle);
        }

        // Update is called once per frame
        void Update()
        {

            if (nextObstacle < course.Count && course[nextObstacle].IsClear())
            {
                DeactivateObstacle(nextObstacle);

                nextObstacle++;

                ActivateObstacle(nextObstacle);
            }

            if (course[course.Count - 1].IsClear())
            {
                courseCompleted = true;
            }

        }

        public void ResetCourse()
        {
            for (int i = 0; i < course.Count; ++i)
            {
                DeactivateObstacle(i);
                course[i].ResetObstacle();
            }

            nextObstacle = 0;

            ActivateObstacle(nextObstacle);
        }

        private void ActivateObstacle(int obstacle)
        {
            if (obstacle < course.Count)
                course[obstacle].Activate();
        }

        private void DeactivateObstacle(int obstacle)
        {
            if (obstacle < course.Count)
                course[obstacle].Deactivate();
        }

        public bool IsCourseComplete()
        {
            return courseCompleted;
        }
    }
}