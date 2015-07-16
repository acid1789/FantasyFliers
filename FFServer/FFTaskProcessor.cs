using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ServerCore;
using FFNetwork;

namespace FFServer
{
    public class FFTask : GSTask
    {
        public enum FFTaskType
        {
        }

        public new FFTaskType TaskType;

        public new FFClient Client
        {
            get { return (FFClient)_client; }
            set { _client = value; }
        }
    }

    public class FFTaskProcessor : GSTaskProcessor
    {

        public FFTaskProcessor(GameServer server) : base(server)
        {
        }

        #region Task Handlers
        
        #endregion

    }
}
