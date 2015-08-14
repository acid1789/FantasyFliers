using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFServer
{
    class LootRange
    {
        int _id;
        float _percentage;
        List<int> _itemTemplates;

        public LootRange()
        {
            _itemTemplates = new List<int>();
        }

        #region Accessors
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public float Percentage
        {
            get { return _percentage; }
            set { _percentage = value; }
        }

        public List<int> Templates
        {
            get { return _itemTemplates; }
        }
        #endregion
    }
}
