using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFServer
{
    class LootTable
    {
        int _id;
        Dictionary<int, LootRange> _ranges;

        float _baseDropChance;
        float _markerBonus;

        public LootTable(int id, float baseDropChance, float markerBonus)
        {
            _id = id;
            _baseDropChance = baseDropChance;
            _markerBonus = markerBonus;
            _ranges = new Dictionary<int, LootRange>();
        }

        public int GenerateLoot(int lootMarkers, int maxLootMarkers, Random random)
        {
            int templateId = -1;
            float markers = (float)lootMarkers / (float)maxLootMarkers;
            float markerChance = _markerBonus * markers;

            double dropChance = _baseDropChance + markerChance;
            double rand = random.NextDouble() * 100.0;
            if (dropChance <= rand)
            {
                // Drop happens, now roll the table to see where
                rand = random.NextDouble() * 100.0;
                double rangeMarker = 0;
                LootRange theRange = null;
                foreach (LootRange range in _ranges.Values)
                {
                    rangeMarker += range.Percentage;
                    if (rand <= rangeMarker)
                    {
                        // This is the range
                        theRange = range;
                        break;
                    }
                }

                if (theRange != null && theRange.Templates.Count > 0)
                {
                    // Pick a template from within this range
                    int template = random.Next(0, theRange.Templates.Count);
                    templateId = theRange.Templates[template];
                }
            }

            return templateId;
        }

        #region Accessors
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public Dictionary<int, LootRange> Ranges
        {
            get { return _ranges; }
        }
        #endregion
    }
}
