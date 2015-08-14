using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using FFNetwork;

public class NetworkManager
{
    public enum SignInStatus
    {
        SignedOut,
        SigningIn,
        InvalidPassword,
        AccountDoesntExist,
        SignedIn
    }

    FFClient _ffc;
    Thread _theThread;

    string _address;
    int _port;

    SignInStatus _signInStatus;
    CourseCompletionInfoArgs _courseCompleteInfo;

    public NetworkManager(string address, int port)
    {
        _address = address;
        _port = port;

        NetLogger log = new NetLogger();        

        _signInStatus = SignInStatus.SignedOut;

        _theThread = new Thread(new ThreadStart(FFCManagerThreadFunc));
        _theThread.Name = "Network Manager";
        _theThread.Start();
    }

    public void Destroy()
    {
        _theThread.Abort();
        if (_ffc != null)
        {
            _ffc.Close();
            _ffc = null;
        }
    }

    void FFCManagerThreadFunc()
    {
        while (true)
        {
            if (_ffc == null)
            {
                _ffc = new FFClient();
                _ffc.OnAccountResponse += new EventHandler(_ffc_OnAccountResponse);
                _ffc.OnCourseCompletionInfo += _ffc_OnCourseCompletionInfo;    
                _ffc.OnInventoryItem += _ffc_OnInventoryItem;

                _ffc.Connect(_address, _port);
            }

            if (_ffc != null && _ffc.Connected)
                _ffc.Update();

            Thread.Sleep(100);
        }
    }


    #region Handlers
    void _ffc_OnAccountResponse(object sender, EventArgs e)
    {
        if (_ffc.AccountId < 0)
            _signInStatus = SignInStatus.AccountDoesntExist;
        else if (_ffc.DisplayName != null)
            _signInStatus = SignInStatus.SignedIn;
        else
            _signInStatus = SignInStatus.InvalidPassword;
    }

    private void _ffc_OnCourseCompletionInfo(object sender, CourseCompletionInfoArgs e)
    {
        _courseCompleteInfo = e;
    }

    void _ffc_OnInventoryItem(object sender, InventoryItemArgs e)
    {
        InventoryItem item = new InventoryItem(e);
        Globals.Inventory.AddItem(item);
    }
    #endregion

    public void ResetSignIn()
    {
        _signInStatus = SignInStatus.SignedOut;
    }

    public void SignIn(string email, string pass)
    {
        _signInStatus = SignInStatus.SigningIn;
        _ffc.SendAccountRequest(email, pass);
    }

    public void CreateAccount(string email, string password, string displayname)
    {
        _signInStatus = SignInStatus.SigningIn;
        _ffc.SendAccountRequest(email, password, displayname);
    }

    public void SendChat(int channel, string message)
    {
        _ffc.SendChat(channel, message, null);
    }

    public void FlightCourseFinished(int courseID, double timeMS, int lootMarkers, int obstacleScore)
    {
        // Clear course completion info
        _courseCompleteInfo = null;

        // Tell the server
        _ffc.FlightCourseFinished(courseID, timeMS, lootMarkers, obstacleScore);
    }

    #region Build
    public void ReloadServerData()
    {
        _ffc.ReloadServerData();
    }

    public void SetItemTemplate(ItemTemplate template)
    {
        int[] statTypes = new int[6];
        int[] statMins = new int[6];
        int[] statMaxs = new int[6];
        int[] abilityTypes = new int[4];
        float[] abilityChances = new float[4];

        statTypes[0] = (int)template.StatA_Type;
        statTypes[1] = (int)template.StatB_Type;
        statTypes[2] = (int)template.StatC_Type;
        statTypes[3] = (int)template.StatD_Type;
        statTypes[4] = (int)template.StatE_Type;
        statTypes[5] = (int)template.StatF_Type;

        statMins[0] = template.StatA_Min;
        statMins[1] = template.StatB_Min;
        statMins[2] = template.StatC_Min;
        statMins[3] = template.StatD_Min;
        statMins[4] = template.StatE_Min;
        statMins[5] = template.StatF_Min;

        statMaxs[0] = template.StatA_Max;
        statMaxs[1] = template.StatB_Max;
        statMaxs[2] = template.StatC_Max;
        statMaxs[3] = template.StatD_Max;
        statMaxs[4] = template.StatE_Max;
        statMaxs[5] = template.StatF_Max;

        abilityTypes[0] = (int)template.AbilityA_Type;
        abilityTypes[1] = (int)template.AbilityB_Type;
        abilityTypes[2] = (int)template.AbilityC_Type;
        abilityTypes[3] = (int)template.AbilityD_Type;

        abilityChances[0] = template.AbilityA_Chance;
        abilityChances[1] = template.AbilityB_Chance;
        abilityChances[2] = template.AbilityC_Chance;
        abilityChances[3] = template.AbilityD_Chance;

        _ffc.SetItemTemplate(template.TemplateID, statTypes, statMins, statMaxs, abilityTypes, abilityChances);
    }

    public void SetCourseInfo(int courseID, float parTime, int lootMarkers, int maxObstacleScore, int gameMode, int timeScoreBase, int timeScoreModifier, int lootTable)
    {
        _ffc.SetCourseInfo(courseID, parTime, lootMarkers, maxObstacleScore, gameMode, timeScoreBase, timeScoreModifier, lootTable);
    }

    public void SetLootTable(LootTable lt)
    {
        LootTableArgs args = new LootTableArgs();
        args.TableID = lt.TableID;
        args.BaseDropChance = lt.BaseDropChance;
        args.MarkerBonus = lt.MarkerBonus;

        List<LootTableArgs.Range> ranges = new List<LootTableArgs.Range>();
        foreach (LootRange lr in lt.Ranges)
        {
            LootTableArgs.Range range = new LootTableArgs.Range();
            range.Percentage = lr.Percentage;
            range.Templates = new int[lr.Templates.Length];

            int index = 0;
            foreach( ItemTemplate it in lr.Templates )
                range.Templates[index++] = it.TemplateID;
            ranges.Add(range);
        }
        args.Ranges = ranges.ToArray();

        _ffc.SetLootTable(args);
    }
    #endregion

    #region Accessors
    public SignInStatus Online
    {
        get { return _signInStatus; }
    }

    public CourseCompletionInfoArgs CourseCompleteInfo
    {
        get { return _courseCompleteInfo; }
    }

    public bool Connected
    {
        get { return _ffc != null && _ffc.Connected; }
    }

    public int SoftCurrency
    {
        get { return _ffc == null ? 0 : _ffc.SoftCurrency; }
    }
    #endregion
}
