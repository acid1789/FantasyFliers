using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using NetworkCore;

namespace FFNetwork
{
    public class FFClient : GameClient
    {
        public enum FFPacketType
        {
            CourseFinished = 5000,
            CourseCompletionInfo,
            UserData,
            InventoryItem,

            // Build stuff
            SetCourseInfo=45000,
            SetLootTable,
            SetItemTemplate,
            ReloadData
        }

        public event EventHandler<CourseFinishedArgs> OnCourseFinished;
        public event EventHandler<CourseCompletionInfoArgs> OnCourseCompletionInfo;
        public event EventHandler<CourseInfoArgs> OnSetCourseInfo;
        public event EventHandler<LootTableArgs> OnSetLootTable;
        public event EventHandler<ItemTemplateArgs> OnSetItemTemplate;
        public event EventHandler OnReloadData;
        public event EventHandler OnUserData;
        public event EventHandler<InventoryItemArgs> OnInventoryItem;

        #region GameData
        int _softCurrency;
        int _inventoryCount;
        int _inventoryMax;
        int _limboCount;
        int _limboMax;
        #endregion

        public FFClient()
            : base(null)
        {
        }

        public FFClient(Socket s)
            : base(s)
        {
        }

        protected override void RegisterPacketHandlers()
        {
            base.RegisterPacketHandlers();
            _packetHandlers[(ushort)FFPacketType.CourseFinished] = CourseFinishedHandler;
            _packetHandlers[(ushort)FFPacketType.CourseCompletionInfo] = CourseCompletionInfoHandler;
            _packetHandlers[(ushort)FFPacketType.UserData] = UserDataHandler;
            _packetHandlers[(ushort)FFPacketType.InventoryItem] = InventoryItemHandler;

            _packetHandlers[(ushort)FFPacketType.SetCourseInfo] = SetCourseInfoHandler;
            _packetHandlers[(ushort)FFPacketType.SetLootTable] = SetLootTableHandler;
            _packetHandlers[(ushort)FFPacketType.SetItemTemplate] = SetItemTemplateHandler;
            _packetHandlers[(ushort)FFPacketType.ReloadData] = ReloadDataHandler;
        }

        void BeginPacket(FFPacketType type)
        {
            LogInterface.Log(string.Format("BeginPacket({0})", type), LogInterface.LogMessageType.Debug);
            BeginPacket((ushort)type);
        }

        #region Packet Construction
        public void FlightCourseFinished(int courseID, double timeMS, int lootMarkers, int obstacleScore)
        {
            BeginPacket(FFPacketType.CourseFinished);

            _outgoingBW.Write(courseID);
            _outgoingBW.Write(timeMS);
            _outgoingBW.Write(lootMarkers);
            _outgoingBW.Write(obstacleScore);
            
            SendPacket();
        }

        public void SendCourseCompletionInfo(int courseID, int obstacleScore, int timeScore, int lootTemplate, ulong lootID)
        {
            BeginPacket(FFPacketType.CourseCompletionInfo);

            _outgoingBW.Write(courseID);
            _outgoingBW.Write(obstacleScore);
            _outgoingBW.Write(timeScore);
            _outgoingBW.Write(lootTemplate);
            _outgoingBW.Write(lootID);

            SendPacket();
        }

        public void SendUserData(int softCurrency, int inventoryMax, int limboMax)
        {
            _softCurrency = softCurrency;
            _inventoryMax = inventoryMax;
            _limboMax = limboMax;
            BeginPacket(FFPacketType.UserData);

            _outgoingBW.Write(_softCurrency);
            _outgoingBW.Write(_inventoryMax);
            _outgoingBW.Write(_limboMax);

            SendPacket();
        }

        public void SendInventoryItem(int itemID, int templateID, int abilities, int[] statIDs, int[] statValues, int droneID, double limboTimeRemaining)
        {
            BeginPacket(FFPacketType.InventoryItem);

            _outgoingBW.Write(itemID);
            _outgoingBW.Write(templateID);
            _outgoingBW.Write(abilities);
            for (int i = 0; i < 6; i++)
            {
                _outgoingBW.Write(statIDs[i]);
                _outgoingBW.Write(statValues[i]);
            }
            _outgoingBW.Write(droneID);
            _outgoingBW.Write(limboTimeRemaining);

            SendPacket();
        }

        public void SetItemTemplate(int templateID, int[] statTypes, int[] statMins, int[] statMaxs, int[] abilityTypes, float[] abilityChances)
        {
            BeginPacket(FFPacketType.SetItemTemplate);

            _outgoingBW.Write(templateID);
            for (int i = 0; i < 6; i++)
            {
                _outgoingBW.Write(statTypes[i]);
                _outgoingBW.Write(statMins[i]);
                _outgoingBW.Write(statMaxs[i]);
            }
            for (int i = 0; i < 4; i++)
            {
                _outgoingBW.Write(abilityTypes[i]);
                _outgoingBW.Write(abilityChances[i]);
            }

            SendPacket();
        }

        public void SetCourseInfo(int courseID, float parTime, int lootMarkers, int maxObstacleScore, int gameMode, int timeScoreBase, int timeScoreModifier, int lootTable)
        {
            BeginPacket(FFPacketType.SetCourseInfo);

            _outgoingBW.Write(courseID);
            _outgoingBW.Write(parTime);
            _outgoingBW.Write(lootMarkers);
            _outgoingBW.Write(maxObstacleScore);
            _outgoingBW.Write(gameMode);
            _outgoingBW.Write(timeScoreBase);
            _outgoingBW.Write(timeScoreModifier);
            _outgoingBW.Write(lootTable);

            SendPacket();
        }

        public void SetLootTable(LootTableArgs args)
        {
            BeginPacket(FFPacketType.SetLootTable);

            _outgoingBW.Write(args.TableID);
            _outgoingBW.Write(args.BaseDropChance);
            _outgoingBW.Write(args.MarkerBonus);
            _outgoingBW.Write(args.Ranges.Length);
            foreach (LootTableArgs.Range r in args.Ranges)
            {
                _outgoingBW.Write(r.Percentage);
                _outgoingBW.Write(r.Templates.Length);

                foreach( int tid in r.Templates )
                    _outgoingBW.Write(tid);
            }

            SendPacket();
        }

        public void ReloadServerData()
        {
            BeginPacket(FFPacketType.ReloadData);
            SendPacket();
        }
        #endregion

        #region Packet Handlers
        void CourseFinishedHandler(BinaryReader br)
        {
            CourseFinishedArgs args = new CourseFinishedArgs();
            args.CourseID = br.ReadInt32();
            args.TimeMS = br.ReadDouble();
            args.LootMarkers = br.ReadInt32();
            args.ObstacleScore = br.ReadInt32();
            OnCourseFinished(this, args);
        }

        void CourseCompletionInfoHandler(BinaryReader br)
        {
            CourseCompletionInfoArgs args = new CourseCompletionInfoArgs();
            args.CourseID = br.ReadInt32();
            args.ObstacleScore = br.ReadInt32();
            args.TimeScore = br.ReadInt32();
            args.LootTemplate = br.ReadInt32();
            args.LootID = br.ReadUInt64();
            OnCourseCompletionInfo(this, args);
        }

        void UserDataHandler(BinaryReader br)
        {
            _softCurrency = br.ReadInt32();
            _inventoryMax = br.ReadInt32();
            _limboMax = br.ReadInt32();
            OnUserData(this, null);
        }

        void InventoryItemHandler(BinaryReader br)
        {
            InventoryItemArgs args = new InventoryItemArgs();
            args.ItemID = br.ReadInt32();
            args.TemplateID = br.ReadInt32();
            args.Abilities = br.ReadInt32();
            for (int i = 0; i < 6; i++)
            {
                args.StatIDs[i] = br.ReadInt32();
                args.StatValues[i] = br.ReadInt32();
            }
            args.DroneID = br.ReadInt32();
            args.LimboRemainingTime = br.ReadDouble();

            OnInventoryItem(this, args);
        }

        void SetCourseInfoHandler(BinaryReader br)
        {
            CourseInfoArgs args = new CourseInfoArgs();
            args.CourseID = br.ReadInt32();
            args.ParTimeSeconds = br.ReadSingle();
            args.LootMarkers = br.ReadInt32();
            args.MaxObstacleScore = br.ReadInt32();
            args.GameMode = br.ReadInt32();
            args.TimeScoreBase = br.ReadInt32();
            args.TimeScoreModifier = br.ReadInt32();
            args.LootTable = br.ReadInt32();

            OnSetCourseInfo(this, args);
        }

        void SetLootTableHandler(BinaryReader br)
        {
            LootTableArgs args = new LootTableArgs();
            args.TableID = br.ReadInt32();
            args.BaseDropChance = br.ReadSingle();
            args.MarkerBonus = br.ReadSingle();
            int ranges = br.ReadInt32();
            if (ranges > 0)
            {
                args.Ranges = new LootTableArgs.Range[ranges];
                for (int i = 0; i < ranges; i++)
                {
                    LootTableArgs.Range range = new LootTableArgs.Range();
                    range.Percentage = br.ReadSingle();
                    int templateCount = br.ReadInt32();
                    if (templateCount > 0)
                    {
                        range.Templates = new int[templateCount];
                        for (int j = 0; j < templateCount; j++)
                            range.Templates[j] = br.ReadInt32();
                    }
                    args.Ranges[i] = range;
                }
            }

            OnSetLootTable(this, args);
        }

        void SetItemTemplateHandler(BinaryReader br)
        {
            ItemTemplateArgs args = new ItemTemplateArgs();
            args.TemplateID = br.ReadInt32();            
            for (int i = 0; i < 6; i++)
            {
                args.StatTypes[i] = br.ReadInt32();
                args.StatMins[i] = br.ReadInt32();
                args.StatMaxs[i] = br.ReadInt32();
            }
            for (int i = 0; i < 4; i++)
            {
                args.AbilityTypes[i] = br.ReadInt32();
                args.AbilityChances[i] = br.ReadSingle();
            }
            OnSetItemTemplate(this, args);
        }

        void ReloadDataHandler(BinaryReader br)
        {
            OnReloadData(this, null);
        }
        #endregion

        #region Accessors
        public int SoftCurrency
        {
            get { return _softCurrency; }
            set { _softCurrency = value; }
        }

        public int InventoryCount
        {
            get { return _inventoryCount; }
            set { _inventoryCount = value; }
        }

        public int InventoryMax
        {
            get { return _inventoryMax; }
        }

        public int LimboCount
        {
            get { return _limboCount; }
            set { _limboCount = value; }
        }

        public int LimboMax
        {
            get { return _limboMax; }
        }
        #endregion
    }

    #region Args Classes
    public class CourseFinishedArgs : EventArgs
    {
        public int CourseID;
        public int LootMarkers;
        public int ObstacleScore;
        public double TimeMS;
        public object LootItem;
    }

    public class CourseCompletionInfoArgs : EventArgs
    {
        public int CourseID;
        public int ObstacleScore;
        public int TimeScore;
        public int LootTemplate;
        public ulong LootID;
    }

    public class CourseInfoArgs : EventArgs
    {
        public int CourseID;
        public int LootMarkers;
        public int MaxObstacleScore;
        public int GameMode;
        public int TimeScoreBase;
        public int TimeScoreModifier;
        public int LootTable;
        public float ParTimeSeconds;
    }

    public class LootTableArgs : EventArgs
    {
        public class Range
        {
            public float Percentage;
            public int[] Templates;
        }

        public int TableID;
        public float BaseDropChance;
        public float MarkerBonus;
        public Range[] Ranges;
    }

    public class ItemTemplateArgs : EventArgs
    {
        public int TemplateID;
        public int[] StatTypes = new int[6];
        public int[] StatMins = new int[6];
        public int[] StatMaxs = new int[6];
        public int[] AbilityTypes = new int[4];
        public float[] AbilityChances = new float[4];
    }

    public class InventoryItemArgs : EventArgs
    {
        public int ItemID;
        public int TemplateID;
        public int Abilities;
        public int[] StatIDs = new int[6];
        public int[] StatValues = new int[6];
        public int DroneID;
        public double LimboRemainingTime;
    }
    #endregion
}
