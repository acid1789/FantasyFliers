using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ServerCore;
using FFNetwork;
using NetworkCore;

namespace FFServer
{
    public class FFTask : GSTask
    {
        public enum FFTaskType
        {
            LoadCourses_Fetch = GSTask.GSTType.Last,
            LoadCourses_Init,
            LootTable_Init,
            LootTableRanges_Init,
            LootRange_Init,
            ItemTemplate_Fetch,
            ItemTemplate_Init,
            Build_CourseInfo,
            Build_LootTableInfo,
            Build_KillLootRanges,
            Build_PopulateLootTable,
            Build_LootRangeCreated,
            Build_ItemTemplateInfo,
            CourseCompleted_Validate,
            CourseCompleted_GenerateLoot,
            CourseCompleted_ProcessScore,
            UserData_Fetch,
            UserData_Init,
            Inventory_Fetch,
            Inventory_Init,
            Limbo_Audit,
        }

        public FFTask(FFTaskType type, FFClient client = null, object args = null)
        {
            Type = (int)type;
            _client = client;
            _args = args;
        }
        
        public new FFClient Client
        {
            get { return (FFClient)_client; }
            set { _client = value; }
        }
    }

    public class FFTaskProcessor : GSTaskProcessor
    {
        Random _random;
        
        Dictionary<int, CourseInfo> _courseInfo;
        Dictionary<int, LootTable> _lootTables;
        Dictionary<int, ItemTemplate> _itemTemplates;

        public FFTaskProcessor(GameServer server) : base(server)
        {
            _random = new Random();

            _taskHandlers[(int)FFTask.FFTaskType.LoadCourses_Fetch] = LoadCourses_Fetch_Handler;
            _taskHandlers[(int)FFTask.FFTaskType.LoadCourses_Init] = LoadCourses_Init_Handler;
            _taskHandlers[(int)FFTask.FFTaskType.LootTable_Init] = LootTable_Init_Handler;
            _taskHandlers[(int)FFTask.FFTaskType.LootTableRanges_Init] = LootTableRanges_Init_Handler;
            _taskHandlers[(int)FFTask.FFTaskType.LootRange_Init] = LootRange_Init_Handler;
            _taskHandlers[(int)FFTask.FFTaskType.ItemTemplate_Fetch] = ItemTemplate_Fetch_Handler;
            _taskHandlers[(int)FFTask.FFTaskType.ItemTemplate_Init] = ItemTemplate_Init_Handler;
            _taskHandlers[(int)FFTask.FFTaskType.Build_CourseInfo] = Build_CourseInfo_Handler;
            _taskHandlers[(int)FFTask.FFTaskType.Build_LootTableInfo] = Build_LootTableInfo_Handler;
            _taskHandlers[(int)FFTask.FFTaskType.Build_KillLootRanges] = Build_KillLootRanges_Handler;
            _taskHandlers[(int)FFTask.FFTaskType.Build_PopulateLootTable] = Build_PopulateLootTable_Handler;
            _taskHandlers[(int)FFTask.FFTaskType.Build_LootRangeCreated] = Build_LootRangeCreated_Handler;
            _taskHandlers[(int)FFTask.FFTaskType.Build_ItemTemplateInfo] = Build_ItemTemplateInfo_Handler;
            _taskHandlers[(int)FFTask.FFTaskType.CourseCompleted_Validate] = CourseCompleted_Validate_Handler;
            _taskHandlers[(int)FFTask.FFTaskType.CourseCompleted_GenerateLoot] = CourseCompleted_GenerateLoot_Handler;
            _taskHandlers[(int)FFTask.FFTaskType.UserData_Fetch] = UserData_Fetch_Handler;
            _taskHandlers[(int)FFTask.FFTaskType.UserData_Init] = UserData_Init_Handler;
            _taskHandlers[(int)FFTask.FFTaskType.Inventory_Fetch] = Inventory_Fetch_Handler;
            _taskHandlers[(int)FFTask.FFTaskType.Inventory_Init] = Inventory_Init_Handler;


            // Fetch courses
            AddTask(new FFTask(FFTask.FFTaskType.LoadCourses_Fetch));

            // Fetch item templates
            AddTask(new FFTask(FFTask.FFTaskType.ItemTemplate_Init));
        }

        #region Task Handlers
        #region Initialization
        void LoadCourses_Fetch_Handler(Task t)
        {
            // Wipe any existing course info and loot tables
            _courseInfo = new Dictionary<int, CourseInfo>();
            _lootTables = new Dictionary<int, LootTable>();

            // Grab the courses from the database
            t.Type = (int)FFTask.FFTaskType.LoadCourses_Init;
            AddDBQuery("SELECT * FROM courses;", t);
        }

        void LoadCourses_Init_Handler(Task t)
        {
            foreach (object[] row in t.Query.Rows)
            {
                CourseInfo ci = new CourseInfo();

                ci.CourseID = (int)row[0];
                ci.ParTime = (double)row[1];
                ci.LootMarkers = (int)row[2];
                ci.MaxObstacleScore = (int)row[3];
                ci.FlightMode = (int)row[4];
                ci.TimeScoreBase = (int)row[5];
                ci.TimeScoreModifier = (int)row[6];
                ci.LootTable = (int)row[7];

                _courseInfo[ci.CourseID] = ci;

                FFTask lootTask = new FFTask(FFTask.FFTaskType.LootTable_Init);
                AddDBQuery(string.Format("SELECT * FROM loot_tables WHERE loot_table_id={0};", ci.LootTable), lootTask);
            }
        }

        void LootTable_Init_Handler(Task t)
        {
            object[] row = t.Query.Rows[0];

            LootTable lt = new LootTable((int)row[0], (float)row[1], (float)row[2]);
            _lootTables[lt.ID] = lt;

            t.Type = (int)FFTask.FFTaskType.LootTableRanges_Init;
            AddDBQuery(string.Format("SELECT * FROM loot_ranges WHERE loot_table_id={0};", lt.ID), t);            
        }

        void LootTableRanges_Init_Handler(Task t)
        {
            if (t.Query.Rows.Count > 0)
            {                
                foreach (object[] row in t.Query.Rows)
                {
                    int rangeID = (int)row[0];
                    int ltID = (int)row[1];
                    float percentage = (float)row[2];

                    LootTable lt = _lootTables[ltID];

                    LootRange range = new LootRange();
                    range.Percentage = percentage;

                    lt.Ranges[rangeID] = range;

                    FFTask rangeTask = new FFTask(FFTask.FFTaskType.LootRange_Init, null, lt.ID);
                    AddDBQuery(string.Format("SELECT * FROM loot_range_templates WHERE loot_range_id={0};", rangeID), rangeTask);
                }
            }
        }

        void LootRange_Init_Handler(Task t)
        {
            int lootTableID = (int)t.Args;
            LootTable lt = _lootTables[lootTableID];
            foreach (object[] row in t.Query.Rows)
            {
                int rangeID = (int)row[0];
                int templateID = (int)row[1];

                lt.Ranges[rangeID].Templates.Add(templateID);
            }
        }

        void ItemTemplate_Fetch_Handler(Task t)
        {
            _itemTemplates = new Dictionary<int, ItemTemplate>();
            string sql = "SELECT * FROM item_templates;";
            t.Type = (int)FFTask.FFTaskType.ItemTemplate_Init;
            AddDBQuery(sql, t);
        }

        void ItemTemplate_Init_Handler(Task t)
        {
            foreach (object[] row in t.Query.Rows)
            {
                int id = (int)row[0];                
                int[] statIDs = new int[6];
                int[] statMins = new int[6];
                int[] statMaxs = new int[6];
                int[] abilities = new int[4];
                float[] abilityChances = new float[4];

                for( int i = 0; i < 6; i++ )
                    statIDs[i] = (int)row[i + 1];

                for (int i = 0; i < 6; i++)
                {
                    statMins[i] = (int)row[(i * 2) + 7];
                    statMaxs[i] = (int)row[(i * 2) + 8];
                }

                for (int i = 0; i < 4; i++)
                {
                    abilities[i] = (int)row[(i * 2) + 19];
                    abilityChances[i] = (float)row[(i * 2) + 20];
                }
                
                ItemTemplate temp = new ItemTemplate(id);
                temp.Load(statIDs, statMins, statMaxs, abilities, abilityChances);

                _itemTemplates[id] = temp;
            }
        }
        #endregion

        #region Course Complete Tasks
        void CourseCompleted_Validate_Handler(Task t)
        {
            FFTask task = (FFTask)t;
            
            CourseFinishedArgs args = (CourseFinishedArgs)task.Args;
            CourseInfo ci = _courseInfo[args.CourseID];
            if (!ci.Validate(args))
            {
                // log the error
                LogInterface.Log(string.Format("Invalid course completed info from account {0}", task.Client.AccountId), LogInterface.LogMessageType.Security, true);

                // Flag the account
                GSTask flagTask = new GSTask();
                flagTask.Type = (int)GSTask.GSTType.FlagAccount;
                flagTask.Client = task.Client;
                AddTask(flagTask);

                // Use good values instead
                args.ObstacleScore = 0;                 // 0 Obstacle score
                args.LootMarkers = 0;                   // 0 Loot Markers
                args.TimeMS = ci.ParTime + 5;           // ParTime + 5 seconds
            }

            task.Type = (int)FFTask.FFTaskType.CourseCompleted_GenerateLoot;
            AddTask(task);
        }

        void CourseCompleted_GenerateLoot_Handler(Task t)
        {
            FFTask task = (FFTask)t;
            CourseFinishedArgs args = (CourseFinishedArgs)task.Args;
            CourseInfo ci = _courseInfo[args.CourseID];

            // set the next task
            task.Type = (int)FFTask.FFTaskType.CourseCompleted_ProcessScore;

            // Generate Loot
            Item loot = null;
            int lootTemplate = _lootTables[ci.LootTable].GenerateLoot(args.LootMarkers, ci.LootMarkers, _random);
            if (lootTemplate >= 0)
            {
                // Got an item drop, now instantiate the item
                ItemTemplate template = _itemTemplates[lootTemplate];
                loot = template.Instantiate(_random);
                args.LootItem = loot;

                // Add the item to the database
                int[] statIDs = loot.StatIDs;
                int[] statValues = loot.StatValues;
                long limboExpirationTicks = 0;
                if (task.Client.InventoryCount >= task.Client.InventoryMax)
                {
                    // This item wont fit in inventory, add it to the limbo
                    DateTime expiration = DateTime.Now.AddDays(7);
                    limboExpirationTicks = expiration.Ticks;
                    task.Client.LimboCount++;
                    if (task.Client.LimboCount >= task.Client.LimboMax)
                    {
                        // Request a limbo audit
                        AddTask(new FFTask(FFTask.FFTaskType.Limbo_Audit, task.Client));
                    }
                }
                string sql = string.Format("INSERT INTO items SET template_id={0},owner_id={1},stat_id_a={2},stat_value_a={3},stat_id_b={4},stat_value_b={5},stat_id_c={6},stat_value_c={7},stat_id_d={8},stat_value_d={9},stat_id_e={10},stat_value_e={11},stat_id_f={12},stat_value_f={13},abilities={14}, drone_id=0, limbo_expiration_date={15}; SELECT LAST_INSERT_ID();",
                    loot.TemplateID, task.Client.AccountId, statIDs[0], statValues[0], statIDs[1], statValues[1], statIDs[2], statValues[2], statIDs[3], statValues[3], statIDs[4], statValues[4], statIDs[5], statValues[5], loot.Abilities, limboExpirationTicks);
                AddDBQuery(sql, task);                
            }
            else
            {
                // No loot, just move on to score
                task.Query = null;
                AddTask(task);
            }
        }

        void CourseCompleted_ProcessScore_Handler(Task t)
        {
            FFTask task = (FFTask)t;
            CourseFinishedArgs args = (CourseFinishedArgs)task.Args;
            CourseInfo ci = _courseInfo[args.CourseID];

            // Get the loot item id if there was one
            ulong lootID = 0;
            if (t.Query != null && t.Query.Rows.Count > 0)
                lootID = (ulong)t.Query.Rows[0][0];

            // Compute time score
            double deltaMS = (ci.ParTime * 1000) - args.TimeMS;
            int timeScore = Math.Max(0, ci.TimeScoreBase + (int)(deltaMS * ci.TimeScoreModifier));

            // Add total score to the users scores
            int totalScore = timeScore + args.ObstacleScore;
            string sql = string.Format("UPDATE user_data SET soft_currency=soft_currency + {0} WHERE account_id={1};", totalScore, task.Client.AccountId);
            AddDBQuery(sql, new FFTask(FFTask.FFTaskType.UserData_Fetch, task.Client), false);  // Do a user data fetch to refresh the client with current user data after its been updated

            // Send the score & loot back to the client
            Item loot = (Item)args.LootItem;
            task.Client.SendCourseCompletionInfo(args.CourseID, args.ObstacleScore, timeScore, loot == null ? 0 : loot.TemplateID, lootID);

            // Send inventory item to the client
            if (loot != null)
            {
                task.Client.SendInventoryItem(loot.ItemID, loot.TemplateID, loot.Abilities, loot.StatIDs, loot.StatValues, 0, loot.LimboExperation);
            }
        }
        #endregion

        void UserData_Fetch_Handler(Task t)
        {
            string sql = string.Format("SELECT * FROM user_data WHERE account_id={0};", ((FFTask)t).Client.AccountId);
            t.Type = (int)FFTask.FFTaskType.UserData_Init;
            AddDBQuery(sql, t);
        }

        void UserData_Init_Handler(Task t)
        {
            int softCurrency = 0;
            int inventoryMax = 25;
            int limboMax = 10;
            FFTask task = (FFTask)t;
            if (t.Query.Rows.Count <= 0)
            {
                // User data doesnt exist for this user, create with defaults now
                string sql = string.Format("INSERT INTO user_data SET account_id={0}, soft_currency={1}, inventory_max={2}, limbo_max={3};", task.Client.AccountId, softCurrency, inventoryMax, limboMax);
                AddDBQuery(sql, null, false);
            }
            else
            {
                object[] row = t.Query.Rows[0];
                softCurrency = (int)row[1];
                inventoryMax = (int)row[2];
                limboMax = (int)row[3];
            }
            task.Client.SendUserData(softCurrency, inventoryMax, limboMax);
        }

        void Inventory_Fetch_Handler(Task t)
        {
            FFTask task = (FFTask)t;
            string sql = string.Format("SELECT * FROM items WHERE owner_id={0};", task.Client.AccountId);
            t.Type = (int)FFTask.FFTaskType.Inventory_Init;
            AddDBQuery(sql, t);
        }

        void Inventory_Init_Handler(Task t)
        {
            FFTask task = (FFTask)t;
            List<Item> inventory = new List<Item>();
            List<Item> equipped = new List<Item>();
            List<Item> limbo = new List<Item>();
            foreach (object[] row in t.Query.Rows)
            {
                int itemID = (int)row[0];
                int templateID = (int)row[1];
                int ownerID = (int)row[2];

                int[] statIDs = new int[6];
                int[] statValues = new int[6];
                for (int i = 0; i < 6; i++)
                {
                    statIDs[i] = (int)row[(i * 2) + 3];
                    statValues[i] = (int)row[(i * 2) + 4];
                }
                int abilities = (int)row[15];
                int droneID = (int)row[16];
                long limboDate = (long)row[17];

                Item item = new Item(itemID, templateID, statIDs, statValues, abilities, droneID, limboDate);
                if (droneID > 0)
                    equipped.Add(item);
                else if (limboDate > 0)
                {
                    TimeSpan span = new DateTime(limboDate) - DateTime.Now;
                    if (span.TotalSeconds <= 0)
                    {
                        // This item has expired, remove it from the database
                        LogInterface.Log(string.Format("Deleting item {0} owned by {1} due to limbo time out", itemID, ownerID), LogInterface.LogMessageType.Game);
                        string sql = string.Format("DELETE FROM items WHERE item_id={0};", itemID);
                        AddDBQuery(sql, null, false);
                    }
                    else
                    {
                        limbo.Add(item);
                    }
                }
                else
                {
                    inventory.Add(item);
                }
            }

            // Store the current inventory count
            task.Client.InventoryCount = inventory.Count;

            // Send items to the player
            foreach (Item eitem in equipped)
            {
                task.Client.SendInventoryItem(eitem.ItemID, eitem.TemplateID, eitem.Abilities, eitem.StatIDs, eitem.StatValues, eitem.DroneID, 0);
            }

            foreach (Item i in inventory)
            {
                task.Client.SendInventoryItem(i.ItemID, i.TemplateID, i.Abilities, i.StatIDs, i.StatValues, 0, 0);
            }

            foreach (Item litem in limbo)
            {
                TimeSpan remaining = new DateTime(litem.LimboExperation) - DateTime.Now;
                task.Client.SendInventoryItem(litem.ItemID, litem.TemplateID, litem.Abilities, litem.StatIDs, litem.StatValues, 0, remaining.TotalSeconds);
            }
        }

        #region Build
        void Build_CourseInfo_Handler(Task t)
        {
            CourseInfoArgs args = (CourseInfoArgs)t.Args;
            string sql = string.Format("INSERT INTO courses (course_id, par_time, loot_markers, max_obstacle_score, flight_mode, time_score_base, time_score_modifier, loot_table) VALUES( {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7} ) ON DUPLICATE KEY UPDATE par_time=VALUES(par_time), loot_markers=VALUES(loot_markers), max_obstacle_score=VALUES(max_obstacle_score), flight_mode=VALUES(flight_mode), time_score_base=VALUES(time_score_base), time_score_modifier=VALUES(time_score_modifier), loot_table=VALUES(loot_table);", args.CourseID, args.ParTimeSeconds, args.LootMarkers, args.MaxObstacleScore, args.GameMode, args.TimeScoreBase, args.TimeScoreModifier, args.LootTable);
            AddDBQuery(sql, null, false);
        }

        void Build_LootTableInfo_Handler(Task t)
        {
            LootTableArgs args = (LootTableArgs)t.Args;
            string sql = string.Format("INSERT INTO loot_tables (loot_table_id, base_drop_chance, marker_bonus) VALUES( {0}, {1}, {2}) ON DUPLICATE KEY UPDATE base_drop_chance=VALUES(base_drop_chance),marker_bonus=VALUES(marker_bonus);", args.TableID, args.BaseDropChance, args.MarkerBonus);
            AddDBQuery(sql, null);

            if (_lootTables.ContainsKey(args.TableID))
            {
                // Delete existing range templates
                sql = string.Format("DELETE FROM loot_range_templates WHERE loot_table_id={0};", args.TableID);

                t.Type = (int)FFTask.FFTaskType.Build_KillLootRanges;
                AddDBQuery(sql, t, false);
            }
            else
            {
                // New table
                t.Type = (int)FFTask.FFTaskType.Build_PopulateLootTable;
                AddTask(t);
            }
        }

        void Build_KillLootRanges_Handler(Task t)
        {
            LootTableArgs args = (LootTableArgs)t.Args;
            string sql = string.Format("DELETE FROM loot_ranges WHERE loot_table_id={0};", args.TableID);
            
            t.Type = (int)FFTask.FFTaskType.Build_PopulateLootTable;
            AddDBQuery(sql, t, false);
        }

        void Build_PopulateLootTable_Handler(Task t)
        {
            LootTableArgs args = (LootTableArgs)t.Args;
            for( int i = 0; i < args.Ranges.Length; i++ )
            {
                LootTableArgs.Range range = args.Ranges[i];
                string sql = string.Format("INSERT INTO loot_ranges SET loot_table_id={0},percentage={1}; SELECT LAST_INSERT_ID();", args.TableID, range.Percentage);

                object[] targs = new object[2];
                targs[0] = i;
                targs[1] = args;
                FFTask task = new FFTask(FFTask.FFTaskType.Build_LootRangeCreated, null, targs);
                AddDBQuery(sql, task);
            }
        }

        void Build_LootRangeCreated_Handler(Task t)
        {
            object[] args = (object[])t.Args;
            int index = (int)args[0];
            LootTableArgs lt = (LootTableArgs)args[1];

            object[] row = t.Query.Rows[0];
            ulong rangeID = (ulong)row[0];
            
            LootTableArgs.Range range = lt.Ranges[index];
            foreach (int template in range.Templates)
            {
                string sql = string.Format("INSERT INTO loot_range_templates SET loot_range_id={0},template_id={1},loot_table_id={2};", rangeID, template, lt.TableID);
                AddDBQuery(sql, null, false);
            }
        }

        void Build_ItemTemplateInfo_Handler(Task t)
        {
            ItemTemplateArgs args = (ItemTemplateArgs)t.Args;
            string sql = string.Format(@"INSERT INTO item_templates (template_id, 
                                                                     stat_id_a, stat_id_b, stat_id_c, stat_id_d, stat_id_e, stat_id_f, 
                                                                     stat_min_a, stat_min_b, stat_min_c, stat_min_d, stat_min_e, stat_min_f, 
                                                                     stat_max_a, stat_max_b, stat_max_c, stat_max_d, stat_max_e, stat_max_f, 
                                                                     ability_id_a, ability_id_b, ability_id_c, ability_id_d, 
                                                                     ability_chance_a, ability_chance_b, ability_chance_c, ability_chance_d) 
                                                              VALUES({0}, 
                                                                     {1}, {2}, {3}, {4}, {5}, {6}, 
                                                                     {7}, {8}, {9}, {10}, {11}, {12},
                                                                     {13}, {14}, {15}, {16}, {17}, {18},
                                                                     {19}, {20}, {21}, {22},
                                                                     {23}, {24}, {25}, {26}) 
                                                              ON DUPLICATE KEY UPDATE 
                                                                        stat_id_a=VALUES(stat_id_a), stat_id_b=VALUES(stat_id_b), stat_id_c=VALUES(stat_id_c), stat_id_d=VALUES(stat_id_d), stat_id_e=VALUES(stat_id_e), stat_id_f=VALUES(stat_id_f),
                                                                        stat_min_a=VALUES(stat_min_a), stat_min_b=VALUES(stat_min_b), stat_min_c=VALUES(stat_min_c), stat_min_d=VALUES(stat_min_d), stat_min_e=VALUES(stat_min_e), stat_min_f=VALUES(stat_min_f),
                                                                        stat_max_a=VALUES(stat_max_a), stat_max_b=VALUES(stat_max_b), stat_max_c=VALUES(stat_max_c), stat_max_d=VALUES(stat_max_d), stat_max_e=VALUES(stat_max_e), stat_max_f=VALUES(stat_max_f),
                                                                        ability_id_a=VALUES(ability_id_a), ability_id_b=VALUES(ability_id_b), ability_id_c=VALUES(ability_id_c), ability_id_d=VALUES(ability_id_d),
                                                                        ability_chance_a=VALUES(ability_chance_a), ability_chance_b=VALUES(ability_chance_b), ability_chance_c=VALUES(ability_chance_c), ability_chance_d=VALUES(ability_chance_d);", 
                                                                        args.TemplateID,
                                                                        args.StatTypes[0], args.StatTypes[1], args.StatTypes[2], args.StatTypes[3], args.StatTypes[4], args.StatTypes[5],
                                                                        args.StatMins[0], args.StatMins[1], args.StatMins[2], args.StatMins[3], args.StatMins[4], args.StatMins[5],
                                                                        args.StatMaxs[0], args.StatMaxs[1], args.StatMaxs[2], args.StatMaxs[3], args.StatMaxs[4], args.StatMaxs[5],
                                                                        args.AbilityTypes[0], args.AbilityTypes[1], args.AbilityTypes[2], args.AbilityTypes[3], 
                                                                        args.AbilityChances[0], args.AbilityChances[1], args.AbilityChances[2], args.AbilityChances[3]);
            AddDBQuery(sql, null, false);

        }
        #endregion // Build

        #endregion // TaskHandlers

    }
}
