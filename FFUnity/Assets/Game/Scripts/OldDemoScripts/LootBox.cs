using UnityEngine;
using System.Collections;

namespace OldDemoScripts
{
    public class LootBox : MonoBehaviour
    {
        public string rarity;


        // Use this for initialization
        void Start()
        {
            Renderer renderer = GetComponent<Renderer>();

            renderer.material = Resources.Load(rarity + "LootMaterial", typeof(Material)) as Material;

        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
        }
    }
}