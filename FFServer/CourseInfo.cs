using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FFNetwork;

namespace FFServer
{
    class CourseInfo
    {
        int _id;
        double _parTime;
        int _lootMarkers;
        int _maxObstacleScore;
        int _flightMode;
        int _timeScoreBase;
        int _timeScoreModifier;
        int _lootTable;

        public bool Validate(CourseFinishedArgs args)
        {
            if( args.CourseID != _id )
                return false;

            // Dont allow sumissions that are less than 10% of the par time
            if( args.TimeMS < (_parTime * 0.1f) )
                return false;

            if( args.ObstacleScore > _maxObstacleScore )
                return false;

            // looks good
            return true;
        }

        #region Accessors
        public int CourseID
        {
            get { return _id; }
            set { _id = value; }
        }

        public double ParTime
        {
            get { return _parTime; }
            set { _parTime = value; }
        }

        public int LootMarkers
        {
            get { return _lootMarkers; }
            set { _lootMarkers = value; }
        }

        public int MaxObstacleScore
        {
            get { return _maxObstacleScore; }
            set { _maxObstacleScore = value; }
        }

        public int FlightMode
        {
            get { return _flightMode; }
            set { _flightMode = value; }
        }

        public int TimeScoreBase
        {
            get { return _timeScoreBase; }
            set { _timeScoreBase = value; }
        }

        public int TimeScoreModifier
        {
            get { return _timeScoreModifier; }
            set { _timeScoreModifier = value; }
        }

        public int LootTable
        {
            get { return _lootTable; }
            set { _lootTable = value; }
        }
        #endregion
    }
}
