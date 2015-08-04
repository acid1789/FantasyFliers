using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LootDropManager : MonoBehaviour {

    public List<LootMarker> lootMarkers;
    public LootBox prefab;
    private List<LootBox> unclaimedLoot;

	// Use this for initialization
	void Start ()
    {
        foreach(LootMarker marker in lootMarkers)
        {
            int rarityIndex = Random.Range(0, marker.possibleRarities.Length);

            prefab.rarity = marker.possibleRarities[rarityIndex];

            LootBox drop = (LootBox)Instantiate(prefab, marker.transform.position, Quaternion.identity);

            marker.GetComponent<Renderer>().enabled = false;

        }
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
