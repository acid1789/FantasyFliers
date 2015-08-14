using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NetworkCore;
using ServerCore;
using FFNetwork;

namespace FFServer
{
    class FFServer : GameServer
    {
        public FFServer(int listenPort, string dbConnectionString, string globalAddress, int globalPort) : base(listenPort, dbConnectionString, globalAddress, globalPort)
        {
            LogThread.AlwaysPrintToConsole = true;
            TaskProcessor = new FFTaskProcessor(this);
        }

        bool BuildAuthorizedConnection(FFClient connection)
        {
            // TODO: Implement IP restriction
            return true;
        }

        public override GameClient CreateClient(System.Net.Sockets.Socket s)
        {
            FFClient client = new FFClient(s);

            // Register FF specific handlers
            client.OnCourseFinished += Client_OnCourseFinished;
            client.OnSetCourseInfo += Client_OnSetCourseInfo;
            client.OnSetLootTable += Client_OnSetLootTable;
            client.OnSetItemTemplate += Client_OnSetItemTemplate;
            client.OnReloadData += Client_OnReloadData;

            return client;
        }

        public override void NewAuthorizedClient(GameClient client)
        {
            base.NewAuthorizedClient(client);

            // Load user data and inventory
            TaskProcessor.AddTask(new FFTask(FFTask.FFTaskType.UserData_Fetch, (FFClient)client));
            TaskProcessor.AddTask(new FFTask(FFTask.FFTaskType.Inventory_Fetch, (FFClient)client));
        }

        #region Client Event Handlers
        private void Client_OnCourseFinished(object sender, CourseFinishedArgs e)
        {
            FFTask task = new FFTask(FFTask.FFTaskType.CourseCompleted_Validate, (FFClient)sender, e);
            TaskProcessor.AddTask(task);
        }

        private void Client_OnSetCourseInfo(object sender, CourseInfoArgs e)
        {
            // Authorize build
            if (BuildAuthorizedConnection((FFClient)sender))
            {
                FFTask task = new FFTask(FFTask.FFTaskType.Build_CourseInfo, (FFClient)sender, e);
                TaskProcessor.AddTask(task);
            }
        }

        private void Client_OnSetLootTable(object sender, LootTableArgs e)
        {
            if (BuildAuthorizedConnection((FFClient)sender))
            {
                FFTask task = new FFTask(FFTask.FFTaskType.Build_LootTableInfo, (FFClient)sender, e);
                TaskProcessor.AddTask(task);
            }
        }

        private void Client_OnSetItemTemplate(object sender, ItemTemplateArgs e)
        {
            if (BuildAuthorizedConnection((FFClient)sender))
            {
                FFTask task = new FFTask(FFTask.FFTaskType.Build_ItemTemplateInfo, (FFClient)sender, e);
                TaskProcessor.AddTask(task);
            }
        }

        private void Client_OnReloadData(object sender, EventArgs e)
        {
            if (BuildAuthorizedConnection((FFClient)sender))
            {
                TaskProcessor.AddTask(new FFTask(FFTask.FFTaskType.LoadCourses_Fetch));                
                TaskProcessor.AddTask(new FFTask(FFTask.FFTaskType.ItemTemplate_Init));
            }
        }
        #endregion

        #region Accessors
        #endregion
    }
}
