using CEAngela.Board;
using CEAngela.Convert;
using CEAngela.ErrorHandler;
using CEAngela.GUI.UCI_Protocol;
using CEAngela.Init;
using CEAngela.Moves;
using CEAngela.SearchTree;
using CEAngela.Setup;
using CEAngela.Tests;
using CEAngela.Tools;
using System;
using System.Threading;

namespace CEAngela.Core
{
    class CoreEngine
    {

        static ProtocolGUI _protocol = new ProtocolGUI();
        static void Main(string[] args)
        {
            InitializeAll();
            _protocol.Start();
        }

        static void InitializeAll()
        {
            Initialize.Init();
        }
    }
}
