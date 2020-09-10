using CEAngela.Board;
using CEAngela.ErrorHandler;
using CEAngela.Moves;
using CEAngela.Random;
using CEAngela.SearchTree;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace CEAngela.Keys
{
    public static class HashKeys
    {
        public static ulong[,] PieceKeys = new ulong[13, 120];
        public static ulong SideKey;
        public static ulong[] CastleKeys = new ulong[16];

        public static int PvSize = 0x100000 * 16;

        //HASH CASTLE PosKey ^= HashKeys.CastleKeys[castlePerm];
        //HASH PIECE PosKey ^= HashKeys.PieceKeys[_piece, _sqr];
        //HASH EP PosKey ^= HashKeys.PieceKeys[(int)Piece.EMPTY, EnPas];
        //HASH SIDE PosKey ^= HashKeys.SideKey;

        public static ulong GeneratePosKey(BOARD _position)
        {
            var _sqr = 0;
            var _finalKey = 0UL;
            var _piece = (int)Piece.EMPTY;

            for(_sqr = 0; _sqr < ChessBoard.BoardSqrNumber; _sqr++)
            {
                _piece = _position.Pieces[_sqr];
                if (_piece != (int)BoardDef.NO_SQ && _piece <= (int)Piece.bK)
                    _finalKey ^= PieceKeys[_piece, _sqr];
            }

            if (_position.Side == (int)Sides.WHITE)
                _finalKey ^= SideKey;

            if (_position.EnPas != (int)BoardDef.NO_SQ)
                _finalKey ^= PieceKeys[(int)Piece.EMPTY, _position.EnPas];

            _finalKey ^= CastleKeys[_position.castlePerm];

            return _finalKey;
        }

        public static void InitHashKeys()
        {
            var _index = 0;
            var _indexLarge = 0;

            for (_index = 0; _index < 13; _index++)
            {
                for (_indexLarge = 0; _indexLarge < 120; _indexLarge++)
                {
                    PieceKeys[_index, _indexLarge] = Rand.Random64;
                }
            }

            SideKey = Rand.Random64;

            for (_index = 0; _index < 16; _index++)
            {
                CastleKeys[_index] = Rand.Random64;
            }
        }

        public static bool ProbeHashTable(BOARD _board, out int _move, out int _score, int _alpha, int _beta, int _depth, out BOARD _b)
        {
            _move = 0;
            _score = 0;

            var _index = (int)(_board.PosKey % (ulong)_board.HashTable.NumEntries);

            Error.Assert(_index >= 0 && _index <= _board.HashTable.NumEntries - 1, "PvProbe");

            if (_board.HashTable.PvTable[_index].PosKey == _board.PosKey)
            {
                _move = _board.HashTable.PvTable[_index].Move;

                if(_board.HashTable.PvTable[_index].Depth >= _depth)
                {
                    _board.HashTable.Hit++;

                    _score = _board.HashTable.PvTable[_index].Score;

                    if (_score > Search.MATE) _score -= _board.Ply;
                    else if (_score < -Search.MATE) _score += _board.Ply;

                    _b = _board;

                    switch (_board.HashTable.PvTable[_index].Flags)
                    {
                        case Transposition.ALPHA:
                            if(_score<=_alpha)
                            {
                                _score = _alpha;

                                return true;
                            }
                            break;
                        case Transposition.BETA:
                            if (_score >= _beta)
                            {
                                _score = _beta;

                                return true;
                            }
                            break;
                        case Transposition.EXACT:
                            return true;
                        default:
                            Error.Assert(false, "SWITCH ERROR");
                            break;
                    }
                }
            }

            _b = _board;
            return false;
        }

        public static int ProbePvMove(BOARD _board)
        {
            var _index = (int)(_board.PosKey % (ulong)_board.HashTable.NumEntries);

            Error.Assert(_index >= 0 && _index <= _board.HashTable.NumEntries - 1, "PvProbe");

            if (_board.HashTable.PvTable[_index].PosKey == _board.PosKey)
            {
                return _board.HashTable.PvTable[_index].Move;
            }

            return 0;
        }
    }

    public struct HASH_ENTRY
    {
        public ulong PosKey;
        public int Move;
        public int Score;
        public int Depth;
        public Transposition Flags;
    }

    public struct HASH_TABLE
    {
        public HASH_ENTRY[] PvTable;
        public int NumEntries;
        public int NewWrite;
        public int OverWrite;
        public int Hit;
        public int Cut;

        public void Initialize()
        {
            PvTable = new HASH_ENTRY[((HashKeys.PvSize/16)) - 2];
            NumEntries = ((HashKeys.PvSize / 16)) - 2;
            ClearTable();
        }

        public void ClearTable()
        {
            for (int i = 0; i < PvTable.Length; i++)
            {
                PvTable[i].Move = 0;
                PvTable[i].PosKey = 0UL;
                PvTable[i].Score = 0;
                PvTable[i].Depth = 0;
                PvTable[i].Flags = 0;
            }
            NewWrite = 0;
        }
    }
}
