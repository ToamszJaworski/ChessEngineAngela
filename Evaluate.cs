using CEAngela.AI;
using CEAngela.BitBoards;
using CEAngela.Board;
using CEAngela.Convert;
using CEAngela.Moves;
using CEAngela.Setup;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CEAngela.Evaluation
{
    public static class Evaluate
    {
        public static int[] Mirror64 = new int[64]{
            56  ,   57  ,   58  ,   59  ,   60  ,   61  ,   62  ,   63  ,
            48  ,   49  ,   50  ,   51  ,   52  ,   53  ,   54  ,   55  ,
            40  ,   41  ,   42  ,   43  ,   44  ,   45  ,   46  ,   47  ,
            32  ,   33  ,   34  ,   35  ,   36  ,   37  ,   38  ,   39  ,
            24  ,   25  ,   26  ,   27  ,   28  ,   29  ,   30  ,   31  ,
            16  ,   17  ,   18  ,   19  ,   20  ,   21  ,   22  ,   23  ,
            8   ,   9   ,   10  ,   11  ,   12  ,   13  ,   14  ,   15  ,
            0   ,   1   ,   2   ,   3   ,   4   ,   5   ,   6   ,   7
            };

        static EvaluationCore _core = new EvaluationCore(new int[3] { 64, 256, 1 });

        static bool MaterialDraw(BOARD _board)
        {
            if(_board.PceNum[(int)Piece.wR] == 0 && _board.PceNum[(int)Piece.bR] == 0 && _board.PceNum[(int)Piece.wQ] == 0 && _board.PceNum[(int)Piece.bQ] == 0)
            {
                if (_board.PceNum[(int)Piece.bB] == 0 && _board.PceNum[(int)Piece.wB] == 0)
                    if (_board.PceNum[(int)Piece.wN] < 3 && _board.PceNum[(int)Piece.bN] < 3)
                        return true;

                else if (_board.PceNum[(int)Piece.bN] == 0 && _board.PceNum[(int)Piece.wN] == 0)
                    if (Math.Abs(_board.PceNum[(int)Piece.wB] - _board.PceNum[(int)Piece.bB]) < 2)
                        return true;

                else if ((_board.PceNum[(int)Piece.wN] < 3 && _board.PceNum[(int)Piece.wB] == 0) || (_board.PceNum[(int)Piece.wN] == 1 && _board.PceNum[(int)Piece.wB] == 1))
                    if ((_board.PceNum[(int)Piece.bN] < 3 && _board.PceNum[(int)Piece.bB] == 0) || (_board.PceNum[(int)Piece.bN] == 1 && _board.PceNum[(int)Piece.bB] == 1))
                        return true;
            }
            else if (_board.PceNum[(int)Piece.wQ] == 0 && _board.PceNum[(int)Piece.bQ] == 0)
            {
                if (_board.PceNum[(int)Piece.wR] == 1 && _board.PceNum[(int)Piece.bR] == 1)
                    if ((_board.PceNum[(int)Piece.wN] + _board.PceNum[(int)Piece.wB]) < 2 && (_board.PceNum[(int)Piece.bN] + _board.PceNum[(int)Piece.bB]) < 2)
                        return true;

                else if (_board.PceNum[(int)Piece.wR] == 1 && _board.PceNum[(int)Piece.bR] == 0)
                    if (((_board.PceNum[(int)Piece.wN] + _board.PceNum[(int)Piece.wB]) == 0) && (((_board.PceNum[(int)Piece.bN] + _board.PceNum[(int)Piece.bB]) == 1) || ((_board.PceNum[(int)Piece.bN] + _board.PceNum[(int)Piece.bB]) == 2)))
                        return true;

                 else if (_board.PceNum[(int)Piece.bR] == 1 && _board.PceNum[(int)Piece.wR] == 0)
                     if (((_board.PceNum[(int)Piece.bN] + _board.PceNum[(int)Piece.bB]) == 0) && (((_board.PceNum[(int)Piece.wN] + _board.PceNum[(int)Piece.wB]) == 1) || ((_board.PceNum[(int)Piece.wN] + _board.PceNum[(int)Piece.wB]) == 2)))
                         return true;
            }

            return false;
        }

        static int EndGameMaterial => (Config.PieceValue[(int)Piece.wR] + 2 * Config.PieceValue[(int)Piece.wN] + 2 * Config.PieceValue[(int)Piece.wP]);

        public static int EvaluatePosition2(BOARD _board)
        {
            var _piece = 0;
            var _pceNum = 0;
            var _sqr = 0;
            var _score = _board.Material[(int)Sides.WHITE] - _board.Material[(int)Sides.BLACK];

            if (_board.PceNum[(int)Piece.wP] == 0 && _board.PceNum[(int)Piece.bP] == 0 && MaterialDraw(_board))
                return 0;

            //pieces
            _piece = (int)Piece.wP;
            for(_pceNum = 0; _pceNum < _board.PceNum[_piece]; _pceNum++)
            {
                _sqr = _board.PieceList[_piece, _pceNum];
                _score += Config.PawnTable[Convert.Converter.SQ64(_sqr)];

                if((BitBoard.IsolateMask[Converter.SQ64(_sqr)] & _board.Pawns[(int)Sides.WHITE]) == 0)
                {
                    //Console.WriteLine("wP Isolated on {0}", (BoardDef)_sqr);
                    _score += Config.PawnIsolated;
                }

                if ((BitBoard.WhitePassedMask[Converter.SQ64(_sqr)] & _board.Pawns[(int)Sides.BLACK]) == 0)
                {
                    //Console.WriteLine("wP Passed on {0}", (BoardDef)_sqr);
                    _score += Config.PawnPassed[ChessBoard.RanksBrd[_sqr]];
                }
            }

            _piece = (int)Piece.bP;
            for (_pceNum = 0; _pceNum < _board.PceNum[_piece]; _pceNum++)
            {
                _sqr = _board.PieceList[_piece, _pceNum];
                _score -= Config.PawnTable[Mirror64[Convert.Converter.SQ64(_sqr)]];

                if ((BitBoard.IsolateMask[Converter.SQ64(_sqr)] & _board.Pawns[(int)Sides.BLACK]) == 0)
                {
                    //Console.WriteLine("bP Isolated on {0}", (BoardDef)_sqr);
                    _score -= Config.PawnIsolated;
                }

                if ((BitBoard.BlackPassedMask[Converter.SQ64(_sqr)] & _board.Pawns[(int)Sides.WHITE]) == 0)
                {
                    //Console.WriteLine("bP Passed on {0}", (BoardDef)_sqr);
                    _score -= Config.PawnPassed[7 - ChessBoard.RanksBrd[_sqr]];
                }
            }

            //knights
            _piece = (int)Piece.wN;
            for (_pceNum = 0; _pceNum < _board.PceNum[_piece]; _pceNum++)
            {
                _sqr = _board.PieceList[_piece, _pceNum];
                _score += Config.KnightTable[Convert.Converter.SQ64(_sqr)];
            }

            _piece = (int)Piece.bN;
            for (_pceNum = 0; _pceNum < _board.PceNum[_piece]; _pceNum++)
            {
                _sqr = _board.PieceList[_piece, _pceNum];
                _score -= Config.KnightTable[Mirror64[Convert.Converter.SQ64(_sqr)]];
            }

            //bishops
            _piece = (int)Piece.wB;
            for (_pceNum = 0; _pceNum < _board.PceNum[_piece]; _pceNum++)
            {
                _sqr = _board.PieceList[_piece, _pceNum];
                _score += Config.BishopTable[Convert.Converter.SQ64(_sqr)];
            }

            _piece = (int)Piece.bB;
            for (_pceNum = 0; _pceNum < _board.PceNum[_piece]; _pceNum++)
            {
                _sqr = _board.PieceList[_piece, _pceNum];
                _score -= Config.BishopTable[Mirror64[Convert.Converter.SQ64(_sqr)]];
            }

            //rooks
            _piece = (int)Piece.wR;
            for (_pceNum = 0; _pceNum < _board.PceNum[_piece]; _pceNum++)
            {
                _sqr = _board.PieceList[_piece, _pceNum];
                _score += Config.RookTable[Convert.Converter.SQ64(_sqr)];

                if ((BitBoard.FileMask[ChessBoard.FilesBrd[_sqr]] & _board.Pawns[(int)Sides.BOTH]) <= 0)
                {
                    //Console.WriteLine("wR on Open File on {0}", (BoardDef)_sqr);
                    _score += Config.RookOpenFile;
                }
                else if ((BitBoard.FileMask[ChessBoard.FilesBrd[_sqr]] & _board.Pawns[(int)Sides.WHITE]) <= 0)
                {
                    //Console.WriteLine("wR on Semi Open File on {0}", (BoardDef)_sqr);
                    _score += Config.RookSemiOpenFile;
                }
            }

            _piece = (int)Piece.bR;
            for (_pceNum = 0; _pceNum < _board.PceNum[_piece]; _pceNum++)
            {
                _sqr = _board.PieceList[_piece, _pceNum];
                _score -= Config.RookTable[Mirror64[Convert.Converter.SQ64(_sqr)]];

                if ((BitBoard.FileMask[ChessBoard.FilesBrd[_sqr]] & _board.Pawns[(int)Sides.BOTH]) <= 0)
                {
                    //Console.WriteLine("bR on Open File on {0}", (BoardDef)_sqr);
                    _score -= Config.RookOpenFile;
                }
                else if ((BitBoard.FileMask[ChessBoard.FilesBrd[_sqr]] & _board.Pawns[(int)Sides.BLACK]) <= 0)
                {
                    //Console.WriteLine("bR on Semi Open File on {0}", (BoardDef)_sqr);
                    _score -= Config.RookSemiOpenFile;
                }
            }

            //queens
            _piece = (int)Piece.wQ;
            for (_pceNum = 0; _pceNum < _board.PceNum[_piece]; _pceNum++)
            {
                _sqr = _board.PieceList[_piece, _pceNum];
                _score += Config.QueenTable[Convert.Converter.SQ64(_sqr)];

                if ((BitBoard.FileMask[ChessBoard.FilesBrd[_sqr]] & _board.Pawns[(int)Sides.BOTH]) <= 0)
                {
                    //Console.WriteLine("wQ on Open File on {0}", (BoardDef)_sqr);
                    _score += Config.QueenOpenFile;
                }
                else if ((BitBoard.FileMask[ChessBoard.FilesBrd[_sqr]] & _board.Pawns[(int)Sides.WHITE]) <= 0)
                {
                    //Console.WriteLine("wQ on Semi Open File on {0}", (BoardDef)_sqr);
                    _score += Config.QueenSemiOpenFile;
                }
            }

            _piece = (int)Piece.bQ;
            for (_pceNum = 0; _pceNum < _board.PceNum[_piece]; _pceNum++)
            {
                _sqr = _board.PieceList[_piece, _pceNum];
                _score -= Config.QueenTable[Mirror64[Convert.Converter.SQ64(_sqr)]];

                if ((BitBoard.FileMask[ChessBoard.FilesBrd[_sqr]] & _board.Pawns[(int)Sides.BOTH]) <= 0)
                {
                    //Console.WriteLine("bQ on Open File on {0}", (BoardDef)_sqr);
                    _score -= Config.QueenOpenFile;
                }
                else if ((BitBoard.FileMask[ChessBoard.FilesBrd[_sqr]] & _board.Pawns[(int)Sides.BLACK]) <= 0)
                {
                    //Console.WriteLine("bQ on Semi Open File on {0}", (BoardDef)_sqr);
                    _score -= Config.QueenSemiOpenFile;
                }
            }

            _piece = (int)Piece.wK;
            _sqr = _board.PieceList[_piece, 0];

            if (_board.Material[(int)Sides.BLACK] <= EndGameMaterial)
                _score += Config.KingTableEndgame[Converter.SQ64(_sqr)];
            else
                _score += Config.KingTableOpening[Converter.SQ64(_sqr)];

            _piece = (int)Piece.bK;
            _sqr = _board.PieceList[_piece, 0];

            if (_board.Material[(int)Sides.WHITE] <= EndGameMaterial)
                _score -= Config.KingTableEndgame[Mirror64[Converter.SQ64(_sqr)]];
            else
                _score -= Config.KingTableOpening[Mirror64[Converter.SQ64(_sqr)]];

            if (_board.PceNum[(int)Piece.wB] >= 2) _score += Config.BishopPair;
            if (_board.PceNum[(int)Piece.bB] >= 2) _score -= Config.BishopPair;

            //-----------------------------

            if (_board.Side == (int)Sides.WHITE)
                return _score;
            else
                return -_score;
        }

        public static void Init()
        {
            _core = new EvaluationCore(new int[4] { 64, 64, 64, 1 });
        }

        public static int EvaluatePosition(BOARD _board)
        {
            var _inputs = new float[64];

            for (int i = 0; i < _inputs.Length; i++)
            {
                _inputs[i] = _board.Pieces[Converter.SQ120(i)] - 6;
            }

            var _evaluation = -_core.Calculate(_inputs)[0];


            return (int)_evaluation;
        }
    }
}
