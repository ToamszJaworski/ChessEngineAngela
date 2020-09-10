using CEAngela.BitBoards;
using CEAngela.Board;
using CEAngela.Keys;
using CEAngela.Moves;
using CEAngela.Setup;
using System;

namespace CEAngela.Init
{
    public static class Initialize
    {
        public static void Init()
        {
            //var _startColor = Console.ForegroundColor;

            //Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.Write(Config.Name + " - Chess Engine");
            //Console.Title = Config.Name + " - Chess Engine";

            //Console.ForegroundColor = _startColor;
            //Console.Write("\n\n===============================\n");

            //Console.ForegroundColor = ConsoleColor.Cyan;

            //Console.WriteLine("Initialize SQ Converter...");
            ChessBoard.InitSq120To64();

            //Console.WriteLine("Initialize Bit Masks...");
            BitBoard.InitBitMasks();

            //Console.WriteLine("Initialize Board...");
            ChessBoard.Board.Initialize();

            //Console.WriteLine("Initialize Hash Keys...");
            HashKeys.InitHashKeys();

            //Console.WriteLine("Initialize File Ranks...");
            ChessBoard.InitFilesRanksBrd();

            //Console.WriteLine("Initialize Moves List...");
            Move.MoveList.Initialize();

            //Console.WriteLine("Initialize MvvLva Arrays...");
            Move.InitMvvLva();

            BitBoard.InitEvalMasks();

            //Console.ForegroundColor = _startColor;
            //Console.Write("\n\n===============================\n\n");
        }
    }
}
