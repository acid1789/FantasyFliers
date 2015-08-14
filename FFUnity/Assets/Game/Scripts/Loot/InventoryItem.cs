using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FFNetwork;

public class InventoryItem
{
    class Stat
    {
        public ItemTemplate.ItemStat StatType;
        public int Value;
    }

    int _itemID;
    int _templateID;
    int _droneID;

    List<Stat> _stats;
    List<ItemTemplate.ItemAbility> _abilities;

    double _limboTime;

    
    public InventoryItem(InventoryItemArgs args)
    {
        _itemID = args.ItemID;
        _templateID = args.TemplateID;
        _droneID = args.DroneID;
        _limboTime = args.LimboRemainingTime;

        _stats = new List<Stat>();
        for (int i = 0; i < args.StatIDs.Length; i++)
        {
            if (args.StatIDs[i] > 0)
            {
                Stat s = new Stat();
                s.StatType = (ItemTemplate.ItemStat)args.StatIDs[i];
                s.Value = args.StatValues[i];
                _stats.Add(s);
            }
        }

        _abilities = new List<ItemTemplate.ItemAbility>();
        for (int i = 0; i < 4; i++)
        {
            int ability = ((args.Abilities >> (i * 8)) & 0xFF);
            if( ability > 0 )
                _abilities.Add((ItemTemplate.ItemAbility)ability);
        }
    }

    #region Accessors
    public double LimboTime
    {
        get { return _limboTime; }
    }

    public int DroneID
    {
        get { return _droneID; }
    }

    public int TemplateID
    {
        get { return _templateID; }
    }
    #endregion
}