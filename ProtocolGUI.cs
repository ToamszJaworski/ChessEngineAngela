using CEAngela.Attacks;
using CEAngela.Board;
using CEAngela.Evaluation;
using CEAngela.Moves;
using CEAngela.SearchTree;
using CEAngela.Setup;
using CEAngela.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace CEAngela.GUI.UCI_Protocol
{
    public class ProtocolGUI
    {
        Queue<string> _commandQueue = new Queue<string>();

        void ParseGo(string _line)
        {
            var _command = _line.Split(' ');

            var _depth = -1;
            var _movesToGo = 30;
            var _moveTime = -1;
            var _time = -1;
            var _inc = 0;
            Search.SearchInfo.TimeSet = false;

            for (int _commandIndex = 0; _commandIndex < _command.Length; _commandIndex++)
            {
               if(_command[_commandIndex] == "infinite")
                {
                    Search.SearchInfo.TimeSet = false;
                }

                if (_command[_commandIndex] == "binc" && ChessBoard.Board.Side == (int)Sides.BLACK)
                {
                    _inc = int.Parse(_command[_commandIndex + 1]);
                }

                if (_command[_commandIndex] == "winc" && ChessBoard.Board.Side == (int)Sides.WHITE)
                {
                    _inc = int.Parse(_command[_commandIndex + 1]);
                }

                if (_command[_commandIndex] == "wtime" && ChessBoard.Board.Side == (int)Sides.WHITE)
                {
                    _time = int.Parse(_command[_commandIndex + 1]);
                }

                if (_command[_commandIndex] == "btime" && ChessBoard.Board.Side == (int)Sides.BLACK)
                {
                    _time = int.Parse(_command[_commandIndex + 1]);
                }

                if (_command[_commandIndex] == "movestogo")
                {
                    _movesToGo = int.Parse(_command[_commandIndex + 1]);
                }

                if (_command[_commandIndex] == "movetime")
                {
                    _moveTime = int.Parse(_command[_commandIndex + 1]);
                }

                if (_command[_commandIndex] == "depth")
                {
                    _depth = int.Parse(_command[_commandIndex + 1]);
                }
            }

            if (_moveTime != -1)
            {
                _time = _moveTime;
                _movesToGo = 1;
            }

            Search.SearchInfo.StartTime = Misc.GetTimeInMs();
            Search.SearchInfo.Depth = _depth;

            if (_time != -1)
            {
                Search.SearchInfo.TimeSet = true;
                _time /= _movesToGo;
                _time -= 50;
                if (_time < 20) { _time = 20; _depth = 1;Search.SearchInfo.Depth = 1; }
                Search.SearchInfo.StopTime = Search.SearchInfo.StartTime + _time + _inc;
            }

            if (_depth == -1)
            {
                Search.SearchInfo.Depth = Config.MaxDepth;
            }

            Console.Write("time: {0}, start: {1}, stop: {2}, depth: {3}, timeset: {4}\n"
                , _time, Search.SearchInfo.StartTime, Search.SearchInfo.StopTime, Search.SearchInfo.Depth, Search.SearchInfo.TimeSet);

            ChessBoard.Board = Search.SearchInfo.SearchPosition(ChessBoard.Board);
        }

        void ParsePosition(string _line)
        {
            var _command = _line.Split(' ');

            if (_command.Length != 1 && _command[1] == "startpos")
                ChessBoard.Board.ParseFEN(Config.StartFEN);
            else
            {
                if(_command.Length == 1 || _command[1] != "fen")
                    ChessBoard.Board.ParseFEN(Config.StartFEN);
                else if(_command[1] == "fen")
                {
                    var _fen = _command[2] + " " + _command[3] + " " + _command[4] + " " + _command[5] + " " + _command[6] + " " + _command[7];
                    ChessBoard.Board.ParseFEN(_fen);
                }
            }

            var _commandIndex = 0;

            for (int i = 0; i < _command.Length; i++)
            {            
                if(_command[i] == "moves")
                {
                    _commandIndex = i + 1;
                    break;
                }    
            }

            if (_commandIndex > 0)
            {
                for (int i = _commandIndex; i < _command.Length; i++)
                {
                    var _move = Move.ParseMove(_command[i], ChessBoard.Board);
                    if (_move == 0) break;
                    ChessBoard.Board.MakeMove(_move);
                    ChessBoard.Board.Ply = 0;
                }
            }

            ChessBoard.Board.PrintBoard();
        }

        public void Start()
        {
            Thread _commandThread = new Thread(CommandReciver);

            _commandThread.Start();

            while(true)
            {
                if(_commandQueue.Count > 0)
                {
                    var _message = _commandQueue.Dequeue();

                    CommandAnalizer(_message);
                }

                if (Search.SearchInfo.Quit)
                    break;
            }
        }

        void CommandReciver()
        {
            var _line = "";

            //_commandQueue.Enqueue("position startpos");
           // _commandQueue.Enqueue("go depth 7");

            while (true)
            {
                _line = Console.ReadLine();

                if (_line[0] == '\n')
                    continue;

                if (_line == "stop")
                {
                    Console.WriteLine("STOP!");
                    Search.SearchInfo.Stopped = true;
                }

                if (_line == "quit")
                {
                    Search.SearchInfo.Quit = true;
                }

                if (Search.SearchInfo.Quit)
                    break;

                _commandQueue.Enqueue(_line);
            }
        }

        void CommandAnalizer(string _line)
        {
            var _command = _line.Split(' ');

            if (_command[0] == "isready")
            {
                Console.Write("readyok\n");
            }
            else if (_command[0] == "position")
            {
                ParsePosition(_line);
            }
            else if (_command[0] == "ucinewgame")
            {
                ParsePosition("position startpos\n");
            }
            else if (_command[0] == "go")
            {
                Evaluate.Init();
                ParseGo(_line);
            }
            else if (_command[0] == "mirror")
            {
                ChessBoard.Board.MirrorBoard();
            }
            else if (_command[0] == "hash")
            {
                for (int i = 0; i < ChessBoard.Board.HashTable.NumEntries; i++)
                {
                    if (ChessBoard.Board.HashTable.PvTable[i].Flags != Transposition.NONE)
                        Console.WriteLine("Num {0}: Score {1}, Move {2}, Flag: {3}", i, ChessBoard.Board.HashTable.PvTable[i].Score, Move.PrintMove(ChessBoard.Board.HashTable.PvTable[i].Move), ChessBoard.Board.HashTable.PvTable[i].Flags);
                }
            }
            else if (_command[0] == "uci")
            {
                Console.Write("id name {0}\n", Config.Name);
                Console.Write("id author Tomasz Jaworski\n");
                Console.Write("uciok\n");
            }    
            else if(_command[0] == "ai")
            {
                //Console.Write("AI Evaluation: {0}\n", Evaluate.EvaluateAI(ChessBoard.Board));
            }
        }
    }

    struct CommandStruct
    {
        public string Line;
        public Thread Thread;

        public CommandStruct(string _line, Thread _thread)
        {
            Line = _line;
            Thread = _thread;
        }
    }
}
