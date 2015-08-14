using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFServer
{
    class ItemTemplate
    {
        class Stat
        {
            public int ID;
            public int Min;
            public int Max;

            public Stat(int id, int min, int max)
            {
                ID = id;
                Min = min;
                Max = max;
            }
        }

        class Ability
        {
            public int ID;
            public float PercentChance;

            public Ability(int id, float chance)
            {
                ID = id;
                PercentChance = chance;
            }
        }

        int _id;
        Stat[] _stats;
        Ability[] _abilities;

        public ItemTemplate(int id)
        {
            _id = id;
            _stats = new Stat[6];
            _abilities = new Ability[4];                        
        }

        public void Load(int[] statIDs, int[] statMins, int[] statMaxs, int[] abilityIDs, float[] abilityChances)
        {
            for (int i = 0; i < 6; i++)
            {
                _stats[i] = new Stat(statIDs[i], statMins[i], statMaxs[i]);                
            }

            for( int i = 0; i < 4; i++ )
                _abilities[i] = new Ability(abilityIDs[i], abilityChances[i]);
        }

        public Item Instantiate(Random random)
        {
            Item item = new Item(_id);

            for (int i = 0; i < _stats.Length; i++)
            {
                if (_stats[i] != null)
                {
                    int value = random.Next(_stats[i].Min, _stats[i].Max);
                    item.SetStat(i, _stats[i].ID, value);
                }
            }

            for (int i = 0; i < _abilities.Length; i++)
            {
                if (_abilities[i] != null)
                {
                    double rand = random.NextDouble() * 100;
                    if (rand <= _abilities[i].PercentChance)
                    {
                        item.AddAbility((byte)_abilities[i].ID);
                    }
                }
            }

            return item;
        }
    }
}
