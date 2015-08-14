using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using ServerCore;

namespace GlobalServer
{
    class GlobalTask : Task
    {
        public enum GlobalType
        {
            AccountInfoRequest,
            AccountInfoProcess,
        }

        public GlobalClient Client
        {
            get { return (GlobalClient)_client; }
            set { _client = value; }
        }
    }

    class PendingQuery
    {
        public enum QueryType
        {
            AccountInfo
        }

        QueryType _type;
        GSTask _task;

        public PendingQuery(QueryType type, GSTask task)
        {
            _type = type;
            _task = task;
        }

        public QueryType Type
        {
            get { return _type; }
        }

        public GSTask Task
        {
            get { return _task; }
        }
    }

    class GlobalTaskProcessor : TaskProcessor
    {
        public GlobalTaskProcessor()
            : base()
        {
            _taskHandlers[(int)GlobalTask.GlobalType.AccountInfoRequest] = AccountInfoRequestHandler;
            _taskHandlers[(int)GlobalTask.GlobalType.AccountInfoProcess] = AccountInfoProcessHandler;            
        }

        #region Task Handlers
        void AccountInfoRequestHandler(Task t)
        {
            GlobalTask task = (GlobalTask)t;
            // Fetch account from the database
            AccountInfoRequestArgs args = (AccountInfoRequestArgs)task.Args;
            DBQuery q = AddDBQuery(string.Format("SELECT * FROM accounts WHERE email=\"{0}\";", args.Email), task);
            
            task.Type = (int)GlobalTask.GlobalType.AccountInfoProcess;
        }

        void AccountInfoProcessHandler(Task t)
        {
            GlobalTask task = (GlobalTask)t;
            int accountId = -1;
            string displayName = "";
            int hardCurrency = 0;
            
            AccountInfoRequestArgs args = (AccountInfoRequestArgs)task.Args;
            bool sendAccountInfo = true;

            if (task.Query.Rows.Count > 0)
            {
                // 0: account_id
                // 1: email
                // 2: password
                // 3: display name
                // 4: hard_currency

                // Found the account, check the password
                object[] row = task.Query.Rows[0];
                accountId = (int)row[0];
                string pw = row[2].ToString();
                if (pw == args.Password)
                {
                    // password match
                    displayName = row[3].ToString();
                    hardCurrency = (int)row[4];
                }
                else
                {
                    // password mismatch - displayName stays empty but accountId is filled in
                }
            }
            else
            {
                // Account does not exist
                if (args.DisplayName != null)
                {
                    // This is actually a request to create the account.
                    sendAccountInfo = false;

                    task.Type = (int)GlobalTask.GlobalType.AccountInfoRequest;
                    DBQuery q = AddDBQuery(string.Format("INSERT INTO accounts SET email=\"{0}\",password=\"{1}\",display_name=\"{2}\",hard_currency=\"{3}\";", args.Email, args.Password, args.DisplayName, 0), task);
                }
            }
            if(sendAccountInfo )
                task.Client.SendAccountInfo(args.ClientKey, accountId, displayName, hardCurrency); 
        }
        #endregion
    }
}
