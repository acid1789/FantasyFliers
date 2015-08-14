using UnityEngine;
using System.Collections;

public class ItemTemplate : MonoBehaviour {

    public enum ItemStat
    {
        None,
    }

    public enum ItemAbility
    {
        None,
    }

    public enum ItemType
    {
        Frame,
        Accessory,
        Battery,
        Motor,
        Prop,
        Camera
    }
    
    public int TemplateID;
    public string Description;    
    public ItemType Type;

    #region Stats
    [Header ("Stat A")]
    [Space(5)]
    public ItemStat StatA_Type;
    public int StatA_Min;
    public int StatA_Max;

    [Header("Stat B")]
    [Space(5)]
    public ItemStat StatB_Type;
    public int StatB_Min;
    public int StatB_Max;
    
    [Header("Stat C")]
    [Space(5)]
    public ItemStat StatC_Type;
    public int StatC_Min;
    public int StatC_Max;

    [Header("Stat D")]
    [Space(5)]
    public ItemStat StatD_Type;
    public int StatD_Min;
    public int StatD_Max;

    [Header("Stat E")]
    [Space(5)]
    public ItemStat StatE_Type;
    public int StatE_Min;
    public int StatE_Max;

    [Header("Stat F")]
    [Space(5)]
    public ItemStat StatF_Type;
    public int StatF_Min;
    public int StatF_Max;
    #endregion

    #region Abilities
    [Header("Ability A")]
    [Space(8)]
    public ItemAbility AbilityA_Type;
    [Range(0, 100.0f)]
    public float AbilityA_Chance;

    [Header("Ability B")]
    [Space(5)]
    public ItemAbility AbilityB_Type;
    [Range(0, 100.0f)]
    public float AbilityB_Chance;

    [Header("Ability C")]
    [Space(5)]
    public ItemAbility AbilityC_Type;
    [Range(0, 100.0f)]
    public float AbilityC_Chance;

    [Header("Ability D")]
    [Space(5)]
    public ItemAbility AbilityD_Type;
    [Range(0, 100.0f)]
    public float AbilityD_Chance;

    #endregion

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
