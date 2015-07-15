using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ServerCore;

namespace FFServer
{
    public class FFTask : Task
    {
        public enum Type
        {
            CredentialsRequest,
            AccountInfoResponse,
        }

        public Type TaskType;

        public FFClient Client
        {
            get { return (FFClient)_client; }
            set { _client = value; }
        }

        public object Args
        {
            get { return _args; }
            set { _args = value; }
        }
    }

    public class FFTaskProcessor : TaskProcessor
    {
        delegate void TaskHandler(FFTask task);
        Dictionary<FFTask.Type, TaskHandler> _taskHandlers;

        public FFTaskProcessor() : base()
        {
            _taskHandlers = new Dictionary<FFTask.Type, TaskHandler>();
            _taskHandlers[FFTask.Type.CredentialsRequest] = CredentialsRequestHandler;
            _taskHandlers[FFTask.Type.AccountInfoResponse] = AccountInfoResponseHandler;
        }

        public override void ProcessTask(Task t)
        {
            FFTask fft = (FFTask)t;
            if (fft != null)
            {
                // Call the task handler, this will throw an exception if the handler isnt registered.
                _taskHandlers[fft.TaskType](fft);
            }
            else
            {
                throw new InvalidDataException("ProcessTask was given an invalid Task object");
            }
        }

        #region Task Handlers
        void CredentialsRequestHandler(FFTask task)
        {
            // Forward credentials request onto the global server
            CredentialsRequestArgs args = (CredentialsRequestArgs)task.Args;
            FFServer.GlobalServer.RequestAccountInfo(task.Client.SessionKey, args.Email, args.Password);
        }

        void AccountInfoResponseHandler(FFTask task)
        {
            AccountInfoResponseArgs args = (AccountInfoResponseArgs)task.Args;
            FFClient client = (FFClient)FFServer.InputThread.FindClient(args.ClientKey);
            if (args.AccountId < 0)
            {
                // Account doesnt exist
            }
            else if (args.DisplayName == null || args.DisplayName.Length <= 0)
            {
                // Account exists but password is wrong
            }
            else
            {
                // Valid account
            }
        }
        #endregion

    }
}
