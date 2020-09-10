using CEAngela.Board;
using CEAngela.ErrorHandler;
using CEAngela.Moves;
using CEAngela.Setup;
using CEAngela.Tools;
using System;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace CEAngela.Tests
{
    public static class Perf
    {
        public static long LeafNodes;

        static void Perft(int _depth, BOARD _board)
        {
            var _b = _board;

            Error.Handler(_b, "Perf");

            if (_depth == 0)
            {
                LeafNodes++;
                return;
            }

            MOVE_LIST _list = new MOVE_LIST();
            _list.Initialize();
            _list.GenerateAllMoves(_b);

            var _moveNum = 0;

            for (_moveNum = 0; _moveNum < _list.Count; _moveNum++)
            {
                if (!_b.MakeMove(_list.Moves[_moveNum].Move))
                    continue;

                Perft(_depth - 1, _b);
                _b.TakeMove();
            }
        }

        public static void PerfTest(int _depth, BOARD _board, bool _showDebug = false)
        {
            var _b = _board;

            Error.Handler(_board, "Perftest");

            if (_showDebug)
                _b.PrintBoard();

            if(_showDebug)
                Console.WriteLine("\nStarting Test To Depth: {0}", _depth);

            LeafNodes = 0;

            MOVE_LIST _list = new MOVE_LIST();
            _list.Initialize();

            _list.GenerateAllMoves(_b);

            var _move = 0;
            var _moveNum = 0;

            for(_moveNum = 0; _moveNum < _list.Count; _moveNum++)
            {
                _move = _list.Moves[_moveNum].Move;

                if(!_b.MakeMove(_move))
                {
                    continue;
                }

                var _cumNOdes = LeafNodes;
                Perft(_depth - 1, _b);
                _b.TakeMove();

                var _oldNodes = LeafNodes - _cumNOdes;

                if (_showDebug)
                    Console.Write("Move {0}: {1} : {2}\n", _moveNum+1, Move.PrintMove(_move), _oldNodes);
            }

            if (_showDebug)
                Console.Write("\n Test Complete: {0} nodes visited\n", LeafNodes);
        }

        public static void FullPerfTest(BOARD _board, bool _showDebug = false)
        {
            var _startColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Starting Full Perf Test...");

            Console.ForegroundColor = _startColor;
            Console.WriteLine("\n===============================\n");

            for (int i = 0; i < Config.PerfFENs.Length; i++)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.Write("Starting New Position: (");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(Config.PerfFENs[i].FEN);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(") on depth ");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(Config.PerfFENs[i].Depth);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(" ");

                _board.ParseFEN(Config.PerfFENs[i].FEN);

                var _startTime = Misc.GetTimeInMs();

                PerfTest(Config.PerfFENs[i].Depth, _board, _showDebug);

                var _stopTime = Misc.GetTimeInMs();

                Console.Write("in ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(_stopTime - _startTime + " ms ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("with result: ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(LeafNodes);
                Console.ForegroundColor = ConsoleColor.Yellow;

                Error.Assert(Config.PerfFENs[i].Result == LeafNodes, "Perf");

                Console.Write(" witch is");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(" CORRECT!\n");
            }

            Console.ForegroundColor = _startColor;
            Console.WriteLine("\n===============================\n");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Full Perf Test Ended Succesfully!");

            Console.ForegroundColor = _startColor;
            Console.WriteLine("\n===============================\n");
        }
    }

    public struct PERF
    {
        public string FEN;
        public int Depth;
        public int Result;

        public PERF(string _fen, int _depth, int _result)
        {
            FEN = _fen;
            Depth = _depth;
            Result = _result;
        }
    }
}
