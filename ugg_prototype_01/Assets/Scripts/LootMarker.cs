using UnityEngine;
using System.Collections;

public class LootMarker : MonoBehaviour {

    [Tooltip("Defines how difficult this Loot Box is to manuever toward and get. Impacts the level of Loot dropped")]
    public int difficulty;

    [Tooltip("Array of strings that will be the pool of rarities to pull when spawning this Loot Box")]
    public string[] possibleRarities;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
