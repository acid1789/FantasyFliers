using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Inventory
{
    ItemTemplateManager _templateManager;
    
    List<InventoryItem> _limbo;
    List<InventoryItem> _items;
    List<InventoryItem> _frames;
    List<InventoryItem> _props;
    List<InventoryItem> _accessories;
    List<InventoryItem> _batteries;
    List<InventoryItem> _cameras;
    List<InventoryItem> _motors;
    List<InventoryItem> _unequipped;

    public Inventory(UnityEngine.TextAsset templateMap)
    {
        _limbo        = new List<InventoryItem>();
        _items        = new List<InventoryItem>();
        _frames       = new List<InventoryItem>();
        _props        = new List<InventoryItem>();
        _accessories  = new List<InventoryItem>();
        _batteries    = new List<InventoryItem>();
        _cameras      = new List<InventoryItem>();
        _motors       = new List<InventoryItem>();
        _unequipped   = new List<InventoryItem>();

        _templateManager = new ItemTemplateManager(templateMap);
    }

    public void AddItem(InventoryItem item)
    {
        ItemTemplate template = _templateManager.GetTemplate(item.TemplateID);

        if (item.LimboTime > 0)
            _limbo.Add(item);
        else
        {
            _items.Add(item);
            if( item.DroneID <= 0 )
                _unequipped.Add(item);

            switch (template.Type)
            {
                case ItemTemplate.ItemType.Accessory: _accessories.Add(item); break;
                case ItemTemplate.ItemType.Battery: _batteries.Add(item); break;
                case ItemTemplate.ItemType.Camera: _cameras.Add(item); break;
                case ItemTemplate.ItemType.Frame: _frames.Add(item); break;
                case ItemTemplate.ItemType.Motor: _motors.Add(item); break;
                case ItemTemplate.ItemType.Prop: _props.Add(item); break;
            }
        }
    }
}
