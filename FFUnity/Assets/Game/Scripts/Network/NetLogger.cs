using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using FFNetwork;
using NetworkCore;

public class NetLogger : LogInterface
{
    public NetLogger() : base()
    {
        _alwaysPrintToConsole = true;
    }

    public override void Log(LogMessageType type, bool logToConsole, string message)
    {
        if (logToConsole || _alwaysPrintToConsole)
            Debug.Log(string.Format("[{0}]: {1}", type.ToString(), message));
    }
}