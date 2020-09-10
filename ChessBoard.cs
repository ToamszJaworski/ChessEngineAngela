using CEAngela.Setup;
using CEAngela.Keys;
using CEAngela.BitBoards;
using System;
using CEAngela.Convert;
using CEAngela.Attacks;
using System.Runtime.CompilerServices;
using CEAngela.Moves;
using CEAngela.ErrorHandler;
using CEAngela.Evaluation;
using System.Collections.Generic;
using CEAngela.SearchTree;

namespace CEAngela.Board
{
    public static class ChessBoard
    {
        public static BOARD Board = new BOARD();

        public static int BoardSqrNumber = 120;

        public static int[] Sq120ToSq64 = new int[BoardSqrNumber];
        public static int[] Sq64ToSq120 = new int[64];

        public static int[] FilesBrd = new int[BoardSqrNumber];
        public static int[] RanksBrd = new int[BoardSqrNumber];

        public static int[] PieceKnight = new int[13]
{
            (int)Bool.FALSE, //void
            (int)Bool.FALSE, //white pawn
            (int)Bool.TRUE, //white knight
            (int)Bool.FALSE, //white bishop
            (int)Bool.FALSE, //white rook
            (int)Bool.FALSE, //white queen
            (int)Bool.FALSE, //white king
            (int)Bool.FALSE, //black pawn
            (int)Bool.TRUE, //black knight
            (int)Bool.FALSE, //black bishop
            (int)Bool.FALSE, //black rook
            (int)Bool.FALSE, //black queen
            (int)Bool.FALSE //black king
};
        public static int[] PieceKing = new int[13]
        {
            (int)Bool.FALSE, //void
            (int)Bool.FALSE, //white pawn
            (int)Bool.FALSE, //white knight
            (int)Bool.FALSE, //white bishop
            (int)Bool.FALSE, //white rook
            (int)Bool.FALSE, //white queen
            (int)Bool.TRUE, //white king
            (int)Bool.FALSE, //black pawn
            (int)Bool.FALSE, //black knight
            (int)Bool.FALSE, //black bishop
            (int)Bool.FALSE, //black rook
            (int)Bool.FALSE, //black queen
            (int)Bool.TRUE //black king
        };
        public static int[] PieceRookQueen = new int[13]
        {
            (int)Bool.FALSE, //void
            (int)Bool.FALSE, //white pawn
            (int)Bool.FALSE, //white knight
            (int)Bool.FALSE, //white bishop
            (int)Bool.TRUE, //white rook
            (int)Bool.TRUE, //white queen
            (int)Bool.FALSE, //white king
            (int)Bool.FALSE, //black pawn
            (int)Bool.FALSE, //black knight
            (int)Bool.FALSE, //black bishop
            (int)Bool.TRUE, //black rook
            (int)Bool.TRUE, //black queen
            (int)Bool.FALSE //black king
        };
        public static int[] PieceBishopQueen = new int[13]
        {
            (int)Bool.FALSE, //void
            (int)Bool.FALSE, //white pawn
            (int)Bool.FALSE, //white knight
            (int)Bool.TRUE, //white bishop
            (int)Bool.FALSE, //white rook
            (int)Bool.TRUE, //white queen
            (int)Bool.FALSE, //white king
            (int)Bool.FALSE, //black pawn
            (int)Bool.FALSE, //black knight
            (int)Bool.TRUE, //black bishop
            (int)Bool.FALSE, //black rook
            (int)Bool.TRUE, //black queen
            (int)Bool.FALSE //black king
        };
        public static int[] PieceSlides = new int[13]
        {
            (int)Bool.FALSE, //void
            (int)Bool.FALSE, //white pawn
            (int)Bool.FALSE, //white knight
            (int)Bool.TRUE, //white bishop
            (int)Bool.TRUE, //white rook
            (int)Bool.TRUE, //white queen
            (int)Bool.FALSE, //white king
            (int)Bool.FALSE, //black pawn
            (int)Bool.FALSE, //black knight
            (int)Bool.TRUE, //black bishop
            (int)Bool.TRUE, //black rook
            (int)Bool.TRUE, //black queen
            (int)Bool.FALSE //black king
        };

        public static bool SqAttacked(int _sqr, Sides _side, BOARD _board)
        {
            return Attack.SqAttacked(_sqr, _side, _board);
        }

        public static void InitFilesRanksBrd()
        {
            var _index = 0;
            var _file = (int)FileC.FILE_A;
            var _rank = (int)FileRank.RANK_1;
            var _sqr = (int)BoardDef.A1;
            var _sq64 = 0;

            for (_index = 0; _index < BoardSqrNumber; _index++)
            {
                FilesBrd[_index] = (int)BoardDef.NO_SQ;
                RanksBrd[_index] = (int)BoardDef.NO_SQ;
            }

            for (_rank = (int)FileRank.RANK_1; _rank <= (int)FileRank.RANK_8; _rank++)
            {
                for (_file = (int)FileC.FILE_A; _file <= (int)FileC.FILE_H; _file++)
                {
                    _sqr = Converter.CordToSqr(_file, _rank);
                    FilesBrd[_sqr] = _file;
                    RanksBrd[_sqr] = _rank;
                }
            }
        }

        public static void InitSq120To64()
        {
            var _index = 0;
            var _sq64 = 0;
            var _file = (int)FileC.FILE_A;
            var _rank = (int)FileRank.RANK_1;
            var _sqr = (int)BoardDef.A1;

            for (_index = 0; _index < BoardSqrNumber; _index++)
            {
                Sq120ToSq64[_index] = 65;
            }

            for (_index = 0; _index < 64; _index++)
            {
                Sq64ToSq120[_index] = 120;
            }

            for (_rank = (int)FileRank.RANK_1; _rank <= (int)FileRank.RANK_8; _rank++)
            {
                for (_file = (int)FileC.FILE_A; _file <= (int)FileC.FILE_H; _file++)
                {
                    _sqr = Converter.CordToSqr(_file, _rank);
                    Sq64ToSq120[_sq64] = _sqr;
                    Sq120ToSq64[_sqr] = _sq64;
                    _sq64++;
                }
            }
        }
    }

    public struct Undo
    {
        public int Move;
        public int CastlePerm;
        public int EnPas;
        public int FiftyMove;
        public ulong PosKey;
    }

    public struct BOARD
    {
        public int[] Pieces;
        public ulong[] Pawns;

        public int[] KingSq;

        public int Side;
        public int EnPas;
        public int FiftyMove;

        public int Ply;
        public int HisPly;

        public int castlePerm;

        public ulong PosKey;

        public int[] PceNum;
        public int[] BigPce;
        public int[] MajPce;
        public int[] MinPce;
        public int[] Material;

        public Undo[] History;

        public int[,] PieceList;

        public HASH_TABLE HashTable;
        public int[] PvArray;

        public int[,] SearchHistory;
        public int[,] SearchKillers;

        public void Initialize()
        {
            Pieces = new int[ChessBoard.BoardSqrNumber];
            Pawns = new ulong[3];
            KingSq = new int[2];

            PceNum = new int[13];

            BigPce = new int[2];
            MajPce = new int[2];
            MinPce = new int[2];
            Material = new int[2];

            History = new Undo[Config.MaxMoves];

            PieceList = new int[13, 10];

            HashTable.Initialize();
            PvArray = new int[Config.MaxDepth];

            SearchHistory = new int[13, ChessBoard.BoardSqrNumber];
            SearchKillers = new int[2, Config.MaxDepth];
        }

        public void ResetBoard()
        {
            var _index = 0;

            for (_index = 0; _index < ChessBoard.BoardSqrNumber; _index++)
            {
                Pieces[_index] = (int)BoardDef.NO_SQ;
            }

            for (_index = 0; _index < 64; _index++)
            {
                Pieces[Converter.SQ120(_index)] = (int)Piece.EMPTY;
            }

            for (_index = 0; _index < 2; _index++)
            {
                BigPce[_index] = 0;
                MajPce[_index] = 0;
                MinPce[_index] = 0;
                Material[_index] = 0;
            }

            for (_index = 0; _index < 3; _index++)
            {
                Pawns[_index] = 0UL;
            }

            for (_index = 0; _index < 13; _index++)
            {
                PceNum[_index] = 0;
            }

            KingSq[(int)CEAngela.Sides.WHITE] = KingSq[(int)CEAngela.Sides.BLACK] = (int)BoardDef.NO_SQ;

            Side = (int)CEAngela.Sides.BOTH;
            EnPas = (int)BoardDef.NO_SQ;
            FiftyMove = 0;

            Ply = 0;
            HisPly = 0;

            castlePerm = 0;

            PosKey = 0UL;
        }

        public bool ParseFEN(string _FEN)
        {
            var _rank = (int)FileRank.RANK_8;
            var _file = (int)FileC.FILE_A;
            var _piece = 0;
            var _count = 0;
            var _index = 0;
            var _sq64 = 0;
            var _sq120 = 0;

            char _FENChar = _FEN[0];
            var _FENIndex = 0;

            ResetBoard();

            while (_rank >= (int)FileRank.RANK_1 && _FENChar.ToString() != null)
            {
                _count = 1;
                switch (_FENChar)
                {
                    case 'p': _piece = (int)Piece.bP; break;
                    case 'r': _piece = (int)Piece.bR; break;
                    case 'n': _piece = (int)Piece.bN; break;
                    case 'b': _piece = (int)Piece.bB; break;
                    case 'k': _piece = (int)Piece.bK; break;
                    case 'q': _piece = (int)Piece.bQ; break;
                    case 'P': _piece = (int)Piece.wP; break;
                    case 'R': _piece = (int)Piece.wR; break;
                    case 'N': _piece = (int)Piece.wN; break;
                    case 'B': _piece = (int)Piece.wB; break;
                    case 'K': _piece = (int)Piece.wK; break;
                    case 'Q': _piece = (int)Piece.wQ; break;

                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                        _piece = (int)Piece.EMPTY;
                        _count = _FENChar - '0';
                        break;

                    case '/':
                    case ' ':
                        _rank--;
                        _file = (int)FileC.FILE_A;
                        _FENIndex++;
                        _FENChar = _FEN[_FENIndex];
                        continue;

                    default:
                        Console.Write("FEN error \n");
                        return true;
                }

                for (_index = 0; _index < _count; _index++)
                {
                    _sq64 = _rank * 8 + _file;
                    _sq120 = Converter.SQ120(_sq64);
                    if (_piece != (int)Piece.EMPTY)
                    {
                        Pieces[_sq120] = _piece;
                    }
                    _file++;
                }

                _FENIndex++;
                _FENChar = _FEN[_FENIndex];
            }

            Side = (_FENChar == 'w') ? (int)CEAngela.Sides.WHITE : (int)CEAngela.Sides.BLACK;

            _FENIndex += 2;
            _FENChar = _FEN[_FENIndex];

            for (_index = 0; _index < 4; _index++)
            {
                if (_FENChar == ' ')
                    break;

                switch (_FENChar)
                {
                    case 'K': castlePerm |= (int)Castle.WKCA; break;
                    case 'Q': castlePerm |= (int)Castle.WQCA; break;
                    case 'k': castlePerm |= (int)Castle.BKCA; break;
                    case 'q': castlePerm |= (int)Castle.BQCA; break;
                    default: break;
                }

                _FENIndex++;
                _FENChar = _FEN[_FENIndex];
            }

            _FENIndex++;
            _FENChar = _FEN[_FENIndex];

            if (_FENChar != '-')
            {
                _file = _FENChar - 'a';

                _FENIndex++;
                _FENChar = _FEN[_FENIndex];

                _rank = _FENChar - '1';

                EnPas = Converter.CordToSqr(_file, _rank);
            }

            PosKey = HashKeys.GeneratePosKey(this);

            UpdateListsMaterial();;

            return false;
        }

        public void UpdateListsMaterial()
        {
            var _piece = 0;
            var _sqr = 0;
            var _index = 0;
            var _colour = 0;

            for (_index = 0; _index < ChessBoard.BoardSqrNumber; _index++)
            {
                _sqr = _index;
                _piece = Pieces[_sqr];

                if (_piece != (int)BoardDef.NO_SQ && _piece != (int)Piece.EMPTY)
                {
                    _colour = (int)Config.PieceColor[_piece];

                    if (Config.PieceBig[_piece] == (int)Bool.TRUE)
                        BigPce[_colour]++;

                    if (Config.PieceMajor[_piece] == (int)Bool.TRUE)
                        MajPce[_colour]++;

                    if (Config.PieceMinor[_piece] == (int)Bool.TRUE)
                        MinPce[_colour]++;

                    Material[_colour] += Config.PieceValue[_piece];

                    PieceList[_piece, PceNum[_piece]] = _sqr;
                    PceNum[_piece]++;

                    if (_piece == (int)Piece.wK || _piece == (int)Piece.bK)
                        KingSq[_colour] = _sqr;

                    if (_piece == (int)Piece.wP || _piece == (int)Piece.bP)
                    {
                        Pawns[_colour] = BitBoard.SetBit(Pawns[_colour], Converter.SQ64(_sqr));
                        Pawns[(int)CEAngela.Sides.BOTH] = BitBoard.SetBit(Pawns[(int)CEAngela.Sides.BOTH], Converter.SQ64(_sqr));
                    }
                }
            }
        }

        public void ClearPiece(int _sqr)
        {
            var _piece = Pieces[_sqr];

            var _col = (int)Config.PieceColor[_piece];
            var _index = 0;
            var _tempPceNum = -1;

            PosKey ^= HashKeys.PieceKeys[_piece, _sqr];

            Pieces[_sqr] = (int)Piece.EMPTY;
            Material[_col] -= Config.PieceValue[_piece];

            if(Config.PieceBig[_piece] == (int)Bool.TRUE)
            {
                BigPce[_col]--; 

                if (Config.PieceMajor[_piece] == (int)Bool.TRUE)
                    MajPce[_col]--;
                else
                    MinPce[_col]--;
            }
            else
            {
                Pawns[_col] = BitBoard.ClearBit(Pawns[_col], Converter.SQ64(_sqr));
                Pawns[(int)Sides.BOTH] = BitBoard.ClearBit(Pawns[(int)Sides.BOTH], Converter.SQ64(_sqr));
            }

            for(_index = 0; _index < PceNum[_piece]; _index++)
            {
                if(PieceList[_piece, _index] == _sqr)
                {
                    _tempPceNum = _index;
                    break;
                }
            }

            Error.Assert(_tempPceNum != -1, "tempPceNum");

            PceNum[_piece]--;
            PieceList[_piece, _tempPceNum] = PieceList[_piece, PceNum[_piece]];
        }

        public void AddPiece(int _sqr, int _piece)
        {
            var _col = (int)Config.PieceColor[_piece];

            PosKey ^= HashKeys.PieceKeys[_piece, _sqr];

            Pieces[_sqr] = _piece;

            if (Config.PieceBig[_piece] == (int)Bool.TRUE)
            {
                BigPce[_col]++;

                if (Config.PieceMajor[_piece] == (int)Bool.TRUE)
                    MajPce[_col]++;
                else
                    MinPce[_col]++;
            }
            else
            {
                Pawns[_col] = BitBoard.SetBit(Pawns[_col], Converter.SQ64(_sqr));
                Pawns[(int)Sides.BOTH] = BitBoard.SetBit(Pawns[(int)Sides.BOTH], Converter.SQ64(_sqr));
            }

            Material[_col] += Config.PieceValue[_piece];
            PieceList[_piece, PceNum[_piece]] = _sqr;
            PceNum[_piece]++;
        }

        public void MovePiece(int _from, int _to)
        {
            var _index = 0;
            var _piece = Pieces[_from];
            var _col = (int)Config.PieceColor[_piece];

            PosKey ^= HashKeys.PieceKeys[_piece, _from];
            Pieces[_from] = (int)Piece.EMPTY;

            PosKey ^= HashKeys.PieceKeys[_piece, _to];
            Pieces[_to] = _piece;

            if(Config.PieceBig[_piece] == (int)Bool.FALSE)
            {
                Pawns[_col] = BitBoard.ClearBit(Pawns[_col], Converter.SQ64(_from));
                Pawns[(int)Sides.BOTH] = BitBoard.ClearBit(Pawns[(int)Sides.BOTH], Converter.SQ64(_from));

                Pawns[_col] = BitBoard.SetBit(Pawns[_col], Converter.SQ64(_to));
                Pawns[(int)Sides.BOTH] = BitBoard.SetBit(Pawns[(int)Sides.BOTH], Converter.SQ64(_to));
            }

            for(_index = 0; _index < PceNum[_piece]; _index++)
            {
                if (PieceList[_piece, _index] == _from)
                {
                    PieceList[_piece, _index] = _to;
                    break;
                }
            }
        }

        public bool MakeMove(int _move)
        {
            Error.Handler(this, "makemove1");

            var _from = Move.From(_move);
            var _to = Move.To(_move);
            var _side = Side;

            History[HisPly].PosKey = PosKey;

            if((_move & Move.EnPassantFLAG) > 0)
            {
                if (Side == (int)Sides.WHITE)
                    ClearPiece(_to - 10);
                else
                    ClearPiece(_to + 10);
            }
            else if((_move & Move.CastleFLAG) > 0)
            {
                switch(_to)
                {
                    case (int)BoardDef.C1:
                        MovePiece((int)BoardDef.A1, (int)BoardDef.D1);
                        break;
                    case (int)BoardDef.C8:
                        MovePiece((int)BoardDef.A8, (int)BoardDef.D8);
                        break;
                    case (int)BoardDef.G1:
                        MovePiece((int)BoardDef.H1, (int)BoardDef.F1);
                        break;
                    case (int)BoardDef.G8:
                        MovePiece((int)BoardDef.H8, (int)BoardDef.F8);
                        break;
                }
            }

            if(EnPas != (int)BoardDef.NO_SQ)
                PosKey ^= HashKeys.PieceKeys[(int)Piece.EMPTY, EnPas];

            PosKey ^= HashKeys.CastleKeys[castlePerm];

            History[HisPly].Move = _move;
            History[HisPly].FiftyMove = FiftyMove;
            History[HisPly].EnPas = EnPas;
            History[HisPly].CastlePerm = castlePerm;

            castlePerm &= Move.CastlePerm[_from];
            castlePerm &= Move.CastlePerm[_to];
            EnPas = (int)BoardDef.NO_SQ;

            PosKey ^= HashKeys.CastleKeys[castlePerm];

            var _captured = Move.Captured(_move);
            FiftyMove++;

            if(_captured != (int)Piece.EMPTY)
            {
                ClearPiece(_to);
                FiftyMove = 0;
            }

            HisPly++;
            Ply++;

            if(Config.PiecePawn[Pieces[_from]] == (int)Bool.TRUE)
            {
                FiftyMove = 0;
                

                if((_move & Move.PawnStartFLAG) > 0)
                {
                    if (Side == (int)Sides.WHITE)
                        EnPas = _from + 10;
                    else
                        EnPas = _from - 10;

                    PosKey ^= HashKeys.PieceKeys[(int)Piece.EMPTY, EnPas];
                }
            }

            MovePiece(_from, _to);


            var _promoPiece = Move.Promoted(_move);
            if(_promoPiece != (int)Piece.EMPTY)
            {
                ClearPiece(_to);
                AddPiece(_to, _promoPiece);
            }

            if(Config.PieceKing[Pieces[_to]] == (int)Bool.TRUE)
            {
                KingSq[Side] = _to;
            }

            Side ^= 1;
            PosKey ^= HashKeys.SideKey;

            Error.Handler(this, "makemove2");

            if (ChessBoard.SqAttacked(KingSq[_side], (Sides)Side, this))
            {
                TakeMove();
                return false;
            }

            return true;
        }

        public void MakeNullMove()
        {
            Error.Handler(this, "null move");

            Ply++;
            History[HisPly].PosKey = PosKey;

            if (EnPas != (int)BoardDef.NO_SQ)
                PosKey ^= HashKeys.PieceKeys[(int)Piece.EMPTY, EnPas];

            History[HisPly].Move = 0;
            History[HisPly].FiftyMove = FiftyMove;
            History[HisPly].EnPas = EnPas;
            History[HisPly].CastlePerm = castlePerm;
            EnPas = (int)BoardDef.NO_SQ;

            Side ^= 1;
            HisPly++;

            PosKey ^= HashKeys.SideKey;

            Error.Handler(this, "null move end");
        }

        public void TakeNullMove()
        {
            Error.Handler(this, "take null");

            HisPly--;
            Ply--;

            if (EnPas != (int)BoardDef.NO_SQ)
                PosKey ^= HashKeys.PieceKeys[(int)Piece.EMPTY, EnPas];

            castlePerm = History[HisPly].CastlePerm;
            FiftyMove = History[HisPly].FiftyMove;
            EnPas = History[HisPly].EnPas;

            if (EnPas != (int)BoardDef.NO_SQ)
                PosKey ^= HashKeys.PieceKeys[(int)Piece.EMPTY, EnPas];

            Side ^= 1;

            PosKey ^= HashKeys.SideKey;

            Error.Handler(this, "take null end");
        }

        public void TakeMove()
        {
            HisPly--;
            Ply--;

            var _move = History[HisPly].Move;
            var _from = Move.From(_move);
            var _to = Move.To(_move);

            if(EnPas != (int)BoardDef.NO_SQ)
                PosKey ^= HashKeys.PieceKeys[(int)Piece.EMPTY, EnPas];

            PosKey ^= HashKeys.CastleKeys[castlePerm];

            castlePerm = History[HisPly].CastlePerm;
            FiftyMove = History[HisPly].FiftyMove;
            EnPas = History[HisPly].EnPas;

            if (EnPas != (int)BoardDef.NO_SQ)
                PosKey ^= HashKeys.PieceKeys[(int)Piece.EMPTY, EnPas];

            PosKey ^= HashKeys.CastleKeys[castlePerm];

            Side ^= 1;
            PosKey ^= HashKeys.SideKey;

            if(Converter.FlagParse(Move.EnPassantFLAG, _move))
            {
                if(Side == (int)Sides.WHITE)
                {
                    AddPiece(_to - 10, (int)Piece.bP);
                }
                else
                {
                    AddPiece(_to + 10, (int)Piece.wP);
                }
            }
            else if(Converter.FlagParse(Move.CastleFLAG, _move))
            {
                switch(_to)
                {
                    case (int)BoardDef.C1: MovePiece((int)BoardDef.D1, (int)BoardDef.A1); break;
                    case (int)BoardDef.C8: MovePiece((int)BoardDef.D8, (int)BoardDef.A8); break;
                    case (int)BoardDef.G1: MovePiece((int)BoardDef.F1, (int)BoardDef.H1); break;
                    case (int)BoardDef.G8: MovePiece((int)BoardDef.F8, (int)BoardDef.H8); break;
                }
            }

            MovePiece(_to, _from);

            if(Converter.ConfigBool(Config.PieceKing[Pieces[_from]]))
            {
                KingSq[Side] = _from;
            }

            int captured = Move.Captured(_move);
            if (captured != (int)Piece.EMPTY)
            {
                AddPiece(_to, captured);
            }

            if (Move.Promoted(_move) != (int)Piece.EMPTY)
            {
                ClearPiece(_from);
                AddPiece(_from, (Config.PieceColor[Move.Promoted(_move)] == (int)Sides.WHITE ? (int)Piece.wP : (int)Piece.bP));
            }

            Error.Handler(this, "takemove");
        }

        public void Store(int _move, int _score, Transposition _flag, int _depth)
        {
            var _index = (int)(PosKey % (ulong)HashTable.NumEntries);

            Error.Assert(_index >= 0 && _index <= HashTable.NumEntries - 1, "PvStore");

            if (HashTable.PvTable[_index].PosKey == 0)
                HashTable.NewWrite++;
            else
                HashTable.OverWrite++;

            if (_score > Search.MATE) _score += Ply;
            else if (_score < -Search.MATE) _score -= Ply;

            HashTable.PvTable[_index].Move = _move;
            HashTable.PvTable[_index].PosKey = PosKey;
            HashTable.PvTable[_index].Flags = _flag;
            HashTable.PvTable[_index].Score = _score;
            HashTable.PvTable[_index].Depth = _depth;
        }

        public int GetPvLine(int _depth)
        {
            Error.Assert(_depth < Config.MaxDepth, "Depth->GetPvLine");

            var _move = HashKeys.ProbePvMove(this);
            var _count = 0;

            while (_move != Move.NoMove && _count < _depth)
            {
                Error.Assert(_count < Config.MaxDepth, "Count->GetPvLine");

                if (Move.MoveExists(this, _move))
                {
                    MakeMove(_move);
                    PvArray[_count++] = _move;
                }
                else break;

                _move = HashKeys.ProbePvMove(this);
            }

            while (Ply > 0)
                TakeMove();

            return _count;
        }

        public void MirrorBoard()
        {
            var _tempPieces = new int[64];
            var _tempSide = Side^1;
            var _swapPiece = new int[13] { (int)Piece.EMPTY, (int)Piece.bP, (int)Piece.bN, (int)Piece.bB, (int)Piece.bR, (int)Piece.bQ, (int)Piece.bK, (int)Piece.wP, (int)Piece.wN, (int)Piece.wB, (int)Piece.wR, (int)Piece.wQ, (int)Piece.wK };
            var _tempCastlePerm = 0;
            var _tempEnPas = (int)BoardDef.NO_SQ;

            var _tp = 0;

            if ((castlePerm & (int)Castle.WKCA) > 0) _tempCastlePerm |= (int)Castle.BKCA;
            if ((castlePerm & (int)Castle.WQCA) > 0) _tempCastlePerm |= (int)Castle.BQCA;

            if ((castlePerm & (int)Castle.BKCA) > 0) _tempCastlePerm |= (int)Castle.WKCA;
            if ((castlePerm & (int)Castle.BQCA) > 0) _tempCastlePerm |= (int)Castle.WQCA;

            if (EnPas != (int)BoardDef.NO_SQ)
                _tempEnPas = Converter.SQ120(Evaluate.Mirror64[Converter.SQ64(EnPas)]);

            for (int _sq = 0; _sq < 64; _sq++)
            {
                _tempPieces[_sq] = Pieces[Converter.SQ120(Evaluate.Mirror64[_sq])];
            }
            ResetBoard();

            for (int _sq = 0; _sq < 64; _sq++)
            {
                _tp = _swapPiece[_tempPieces[_sq]];
                Pieces[Converter.SQ120(_sq)] = _tp;
            }

            Side = _tempSide;
            castlePerm = _tempCastlePerm;
            EnPas = _tempEnPas;

            PosKey = HashKeys.GeneratePosKey(this);

            UpdateListsMaterial();

            Error.Handler(this, "Mirror");

            PrintBoard();
        }

        public void PrintBoard()
        {
            var _startColor = Console.ForegroundColor;
            var _sqr = 0;
            var _file = (int)FileC.FILE_A;
            var _rank = (int)FileRank.RANK_1;
            var _piece = (int)Piece.EMPTY;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nGame Board:\n\n");

            for (_rank = (int)FileRank.RANK_8; _rank >= (int)FileRank.RANK_1; _rank--)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("{0}  ", _rank + 1);

                for (_file = (int)FileC.FILE_A; _file <= (int)FileC.FILE_H; _file++)
                {
                    _sqr = Converter.CordToSqr(_file, _rank);
                    _piece = Pieces[_sqr];
                    Console.ForegroundColor = (_piece == (int)Piece.EMPTY) ? _startColor : ((_piece > 6) ? ConsoleColor.Blue : ConsoleColor.Yellow);
                    Console.Write("{0, 3}", Config.PceChar[_piece]);
                }

                Console.Write("\n");
            }

            Console.Write("\n   ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            for (_file = 0; _file <= (int)FileC.FILE_H; _file++)
                Console.Write("{0, 3}", (char)('a' + _file));

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("\n\n");

            Console.Write("Side: {0}\n", Config.SideChar[Side]);
            Console.Write("EnPas: {0}\n", ((BoardDef)EnPas).ToString().ToLower());
            Console.Write("Castle: {0}{1}{2}{3}\n", (castlePerm & (int)Castle.WKCA) > 0 ? 'K' : '-'
                                                    , (castlePerm & (int)Castle.WQCA) > 0 ? 'Q' : '-'
                                                    , (castlePerm & (int)Castle.BKCA) > 0 ? 'k' : '-'
                                                    , (castlePerm & (int)Castle.BQCA) > 0 ? 'q' : '-');

            Console.Write("PosKey: {0}\n", PosKey.ToString("X").ToUpper());
            Console.Write("Evaluation: {0}\n", Evaluate.EvaluatePosition(this));

            Console.Write("\n\n");

            Console.ForegroundColor = _startColor;
        }
    }
}
