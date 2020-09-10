using CEAngela.Board;
using CEAngela.Convert;
using System;

namespace CEAngela.BitBoards
{
    public static class BitBoard
    {
        public static ulong[] SetMask = new ulong[64];
        public static ulong[] ClearMask = new ulong[64];

        public static ulong[] FileMask = new ulong[8];
        public static ulong[] RankMask = new ulong[8];

        public static ulong[] BlackPassedMask = new ulong[64];
        public static ulong[] WhitePassedMask = new ulong[64];
        public static ulong[] IsolateMask = new ulong[64];

        public static void InitEvalMasks()
        {
            var _sqr = 0;
            var _tsq = 0;
            var _rank = 0;
            var _file = 0;

            for (_sqr = 0; _sqr < 8; _sqr++)
            {
                FileMask[_sqr] = 0ul;
                RankMask[_sqr] = 0ul;
            }

            for(_rank = (int)FileRank.RANK_8; _rank >= (int)FileRank.RANK_1; _rank--)
            {
                for (_file = (int)FileC.FILE_A; _file <= (int)FileC.FILE_H; _file++)
                {
                    _sqr = _rank * 8 + _file;
                    FileMask[_file] |= (1UL << _sqr);
                    RankMask[_rank] |= (1UL << _sqr);
                }
            }

            for (_sqr = 0; _sqr < 64; _sqr++)
            {
                IsolateMask[_sqr] = 0ul;
                WhitePassedMask[_sqr] = 0ul;
                BlackPassedMask[_sqr] = 0ul;
            }

            for (_sqr = 0; _sqr < 64; _sqr++)
            {
                _tsq = _sqr + 8;

                while(_tsq < 64)
                {
                    WhitePassedMask[_sqr] |= (1UL << _tsq);
                    _tsq += 8;
                }

                _tsq = _sqr - 8;

                while (_tsq >= 0)
                {
                    BlackPassedMask[_sqr] |= (1UL << _tsq);
                    _tsq -= 8;
                }

                if (ChessBoard.FilesBrd[Converter.SQ120(_sqr)] > (int)FileC.FILE_A)
                {
                    IsolateMask[_sqr] |= FileMask[ChessBoard.FilesBrd[Converter.SQ120(_sqr)] - 1];

                    _tsq = _sqr + 7;

                    while (_tsq < 64)
                    {
                        WhitePassedMask[_sqr] |= (1UL << _tsq);
                        _tsq += 8;
                    }

                    _tsq = _sqr - 9;

                    while (_tsq >= 0)
                    {
                        BlackPassedMask[_sqr] |= (1UL << _tsq);
                        _tsq -= 8;
                    }
                }

                if (ChessBoard.FilesBrd[Converter.SQ120(_sqr)] < (int)FileC.FILE_H)
                {
                    IsolateMask[_sqr] |= FileMask[ChessBoard.FilesBrd[Converter.SQ120(_sqr)] + 1];

                    _tsq = _sqr + 9;

                    while (_tsq < 64)
                    {
                        WhitePassedMask[_sqr] |= (1UL << _tsq);
                        _tsq += 8;
                    }

                    _tsq = _sqr - 7;

                    while (_tsq >= 0)
                    {
                        BlackPassedMask[_sqr] |= (1UL << _tsq);
                        _tsq -= 8;
                    }
                }
            }
        }

        public static void InitBitMasks()
        {
            var _index = 0;

            for (_index = 0; _index < 64; _index++)
            {
                SetMask[_index] = 0;
                ClearMask[_index] = 0;
            }

            for (_index = 0; _index < 64; _index++)
            {
                SetMask[_index] = 1UL << _index;
                ClearMask[_index] = ~SetMask[_index];
            }
        }

        public static ulong ClearBit(ulong _bb, int _sqr)
        {
            var _base = _bb;

            _base &= ClearMask[_sqr];

            return _base;
        }

        public static ulong SetBit(ulong _bb, int _sqr)
        {
            var _base = _bb;

            _base |= SetMask[_sqr];

            return _base;
        }

        public static int PopBit(ulong _bb, out ulong _bitBoard)
        {
            var _base = _bb;

            for (int i = 0; i < 64; i++)
            {
                if(((1UL << i) & _bb) > 0)
                {
                    _base &= ~(1UL << i);
                    _bitBoard = _base;
                    return i;
                }
            }

            _bitBoard = 0;
            return 0;
        }

        public static int CountBits(ulong _b)
        {
            int r;
            for (r = 0; _b > 0; r++, _b &= _b - 1) ;
            return r;
        }

        public static void PrintBitBoard(ulong _bb)
        {
            var _shift = 1UL;

            var _rank = 0;
            var _file = 0;
            var _sqr = 0;
            var _sq64 = 0;

            Console.Write("\n");
            for (_rank = (int)FileRank.RANK_8; _rank >= (int)FileRank.RANK_1; _rank--)
            {
                for (_file = (int)FileC.FILE_A; _file <= (int)FileC.FILE_H; _file++)
                {
                    _sqr = Converter.CordToSqr(_file, _rank);
                    _sq64 = Converter.SQ64(_sqr);

                    if (((_shift << _sq64) & _bb) > 0)
                        Console.Write("X");
                    else
                        Console.Write("-");
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }
    }
}
