using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ItemTemplateManager
{
    Dictionary<int, string> _templateMap;
    Dictionary<int, ItemTemplate> _templates;

    public ItemTemplateManager(TextAsset templateMap)
    {
        _templateMap = new Dictionary<int, string>();
        LoadTemplateMap(templateMap);
    }

    public void LoadTemplateMap(TextAsset templateMap)
    {
        string[] templates = templateMap.text.Split('\n');
        foreach (string template in templates)
        {
            string[] pieces = template.Split(',');
            if (pieces.Length == 2)
            {
                if (pieces[1][pieces[1].Length - 1] == '\r')
                    pieces[1] = pieces[1].Substring(0, pieces[1].Length - 1);

                int id = Convert.ToInt32(pieces[0]);
                _templateMap[id] = pieces[1];
            }
        }
    }

    public ItemTemplate GetTemplate(int id)
    {
        if( _templates.ContainsKey(id) )
            return _templates[id];

        // Template isnt loaded, need to load it now
        GameObject templObject = (GameObject)AssetBundleManager.LoadAsset("item_templates", _templateMap[id], typeof(GameObject));
        ItemTemplate templ = templObject.GetComponent<ItemTemplate>();
        if (templ != null)
        {
            _templates[id] = templ;
            return templ;
        }

        return null;
    }
}
