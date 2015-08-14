using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFServer
{
    class Item
    {
        int _itemID;
        int _templateID;
        int[] _statIDs;
        int[] _statValues;
        int _abilities;
        int _droneID;
        long _limboDate;

        public Item(int template)
        {
            _templateID = template;
            _statIDs = new int[6];
            _statValues = new int[6];
            _abilities = 0;
        }

        public Item(int itemID, int templateID, int[] statIDs, int[] statValues, int abilities, int droneID, long limboDate)
        {
            _itemID = itemID;
            _templateID = templateID;

            _statIDs = statIDs;
            _statValues = statValues;
            _abilities = abilities;

            _droneID = droneID;
            _limboDate = limboDate;
        }

        public void SetStat(int index, int id, int value)
        {
            _statIDs[index] = id;
            _statValues[index] = value;
        }

        public void AddAbility(byte id)
        {
            for (int i = 0; i < 4; i++)
            {
                byte ability = (byte)((_abilities >> (i * 8)) & 0xFF);
                if (ability == 0)
                {
                    // here is a spot
                    int newAbility = id << (i * 8);
                    _abilities |= newAbility;
                    break;
                }
            }
        }

        #region Accessors
        public int TemplateID
        {
            get { return _templateID; }
        }

        public int Abilities
        {
            get { return _abilities; }
        }

        public int[] StatIDs
        {
            get { return _statIDs; }
        }

        public int[] StatValues
        {
            get { return _statValues; }
        }

        public int ItemID
        {
            get { return _itemID; }
        }

        public int DroneID
        {
            get { return _droneID; }
        }

        public long LimboExperation
        {
            get { return _limboDate; }
        }
        #endregion
    }
}
