using CEAngela.Board;
using CEAngela.Convert;
using CEAngela.ErrorHandler;
using CEAngela.Keys;
using CEAngela.Moves.Logic;
using CEAngela.Setup;
using System;

namespace CEAngela.Moves
{
    public static class Move
    {
        public static MOVE_LIST MoveList;

        public static int[] CastlePerm =
        {
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 13, 15, 15, 15, 12, 15, 15, 14, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15,  7, 15, 15, 15,  3, 15, 15, 11, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
            15, 15, 15, 15, 15, 15, 15, 15, 15, 15
        };

        public static int NoMove = 0;

        public static int[] VictimScore = new int[13]{ 0, 100, 200, 300, 400, 500, 600, 100, 200, 300, 400, 500, 600 };
        public static int[,] MvvLvaScores = new int[13, 13];

        public static void InitMvvLva()
        {
            for (int _attacker = (int)Piece.wP; _attacker <= (int)Piece.bK; _attacker++)
            {
                for (int _victim = (int)Piece.wP; _victim <= (int)Piece.bK; _victim++)
                {
                    MvvLvaScores[_victim, _attacker] = (VictimScore[_victim] + 6) - (VictimScore[_attacker] / 100);
                }
            }
        }

        public static string PrintSqr(int _sqr)
        {
            return ((BoardDef)Converter.SQ120(_sqr)).ToString().ToLower();
        }

        public static string PrintMove(int _move)
        {
            var _charArray = new string[3];

            _charArray[0] = ((BoardDef)From(_move)).ToString();
            _charArray[1] = ((BoardDef)To(_move)).ToString();

            var _promoted = Promoted(_move);

            if (_promoted > 0)
            {
                var _promoChar = 'q';
                if (Converter.IsKn(_promoted))
                    _promoChar = 'n';
                else if (Converter.IsRQ(_promoted) && !Converter.IsBQ(_promoted))
                    _promoChar = 'r';
                else if (!Converter.IsRQ(_promoted) && Converter.IsBQ(_promoted))
                    _promoChar = 'b';

                _charArray[2] = _promoChar.ToString();
            }

            return _charArray[0] + _charArray[1] + _charArray[2];
        }

        public static int SetMove(int _from, int _to, int _capture, int _promote, int _flag)
        {
            return _from | _to << 7 | _capture << 14 | _promote << 20 | _flag;
        }

        public static int ParseMove(string _moveString, BOARD _board)
        {
            _moveString = _moveString.ToLower();

            if (_moveString[1] > '8' || _moveString[1] < '1') return NoMove;
            if (_moveString[3] > '8' || _moveString[3] < '1') return NoMove;
            if (_moveString[2] > 'h' || _moveString[2] < 'a') return NoMove;
            if (_moveString[0] > 'h' || _moveString[0] < 'a') return NoMove;

            var _from = Converter.CordToSqr(_moveString[0] - 'a', _moveString[1] - '1');
            var _to = Converter.CordToSqr(_moveString[2] - 'a', _moveString[3] - '1');

            //Console.WriteLine("Move: {0} -> From: {1} to {2}", _moveString, (BoardDef)_from, (BoardDef)_to);

            Error.Assert(_from != 99 || _to != 99, "ParseMove(From/To)");

            MOVE_LIST _list = new MOVE_LIST();
            _list.Initialize();
            _list.GenerateAllMoves(_board);

            var _moveNum = 0;
            var _move = 0;
            var _promPiece = 0;

            for (_moveNum = 0; _moveNum < _list.Count; _moveNum++)
            {
                _move = _list.Moves[_moveNum].Move;

                if (From(_move) == _from && To(_move) == _to)
                {
                    _promPiece = Promoted(_move);
                    if(_promPiece != (int)Piece.EMPTY && _moveString.Length == 5)
                    {
                        if (Converter.IsRQ(_promPiece) && !Converter.IsBQ(_promPiece) && _moveString[4] == 'r')
                            return _move;
                        else if (!Converter.IsRQ(_promPiece) && Converter.IsBQ(_promPiece) && _moveString[4] == 'b')
                            return _move;
                        else if (Converter.IsRQ(_promPiece) && Converter.IsBQ(_promPiece) && _moveString[4] == 'q')
                            return _move;
                        else if (Converter.IsKn(_promPiece) && _moveString[4] == 'n')
                            return _move;

                        continue;
                    }

                    return _move;
                }
            }

            return NoMove;
        }

        public static bool MoveExists(BOARD _board, int _move)
        {
            MOVE_LIST _list = new MOVE_LIST();
            _list.Initialize();
            _list.GenerateAllMoves(_board);

            var _moveNum = 0;

            for(_moveNum = 0; _moveNum < _list.Count; _moveNum++)
            {
                if (!_board.MakeMove(_list.Moves[_moveNum].Move))
                    continue;
                _board.TakeMove();

                if (_list.Moves[_moveNum].Move == _move)
                    return true;
            }

            return false;
        }

        public static int From(int _move)
        { 
            return _move & 0x7F; 
        }
        public static int To(int _move) 
        { 
            return _move >> 7 & 0x7F; 
        }
        public static int Captured(int _move)
        {
            return _move >> 14 & 0xF;
        }
        public static int Promoted(int _move)
        {
            return _move >> 20 & 0xF;
        }

        public static int EnPassantFLAG { get => 0x40000; }
        public static  int PawnStartFLAG { get => 0x80000; }
        public static  int CastleFLAG { get => 0x1000000; }
        public static int CapturedFLAG { get => 0x7C000; }
        public static  int PromotedFLAG { get => 0xF00000; }
    }

    public struct MOVE
    {
        public int Move;
        public int Score;
    }

    public struct MOVE_LIST
    {
        public MOVE[] Moves;

        public int Count;

        public void Initialize()
        {
            Moves = new MOVE[Config.MaxPositionMoves];
        }

        public void GenerateAllMoves(BOARD _board)
        {
            Error.Handler(_board, "generate");

            Count = 0;

            var _piece = 0;
            var _side = _board.Side;
            var _sqr = 0;
            var _tempSqr = 0;
            var _pceNum = 0;
            var _dir = 0;
            var _index = 0;
            var _pceIndex = 0;

            if (_side == (int)Sides.WHITE)
            {
                //white pces
                for (_pceNum = 0; _pceNum < _board.PceNum[(int)Piece.wP]; _pceNum++)
                {
                    _sqr = _board.PieceList[(int)Piece.wP, _pceNum];

                    if (_board.Pieces[_sqr + 10] == (int)Piece.EMPTY)
                    {
                        this = GML.AddWhitePawnMove(_board, _sqr, _sqr + 10, this);

                        if (ChessBoard.RanksBrd[_sqr] == (int)FileRank.RANK_2 && _board.Pieces[_sqr + 20] == (int)Piece.EMPTY)
                            this = GML.AddQuietMove(_board, Move.SetMove(_sqr, _sqr + 20, (int)Piece.EMPTY, (int)Piece.EMPTY, Move.PawnStartFLAG), this);
                    }

                    if (!Converter.IsOffboard(_sqr + 9) && Config.PieceColor[_board.Pieces[_sqr + 9]] == Sides.BLACK)
                        this = GML.AddWhitePawnCaptureMove(_board, _sqr, _sqr + 9, _board.Pieces[_sqr + 9], this);

                    if (!Converter.IsOffboard(_sqr + 11) && Config.PieceColor[_board.Pieces[_sqr + 11]] == Sides.BLACK)
                        this = GML.AddWhitePawnCaptureMove(_board, _sqr, _sqr + 11, _board.Pieces[_sqr + 11], this);

                    if(_board.EnPas != (int)BoardDef.NO_SQ)
                    {
                        if (_sqr + 9 == _board.EnPas)
                            this = GML.AddEnPassantMove(_board, Move.SetMove(_sqr, _sqr + 9, (int)Piece.EMPTY, (int)Piece.EMPTY, Move.EnPassantFLAG), this);

                        if (_sqr + 11 == _board.EnPas)
                            this = GML.AddEnPassantMove(_board, Move.SetMove(_sqr, _sqr + 11, (int)Piece.EMPTY, (int)Piece.EMPTY, Move.EnPassantFLAG), this);
                    }
                }

                if ((_board.castlePerm & (int)Castle.WKCA) > 0)
                {
                    if (_board.Pieces[(int)BoardDef.F1] == (int)Piece.EMPTY && _board.Pieces[(int)BoardDef.G1] == (int)Piece.EMPTY)
                    {
                        if (!ChessBoard.SqAttacked((int)BoardDef.E1, Sides.BLACK, _board) && !ChessBoard.SqAttacked((int)BoardDef.F1, Sides.BLACK, _board))
                        {
                            this = GML.AddQuietMove(_board, Move.SetMove((int)BoardDef.E1, (int)BoardDef.G1, (int)Piece.EMPTY, (int)Piece.EMPTY, Move.CastleFLAG), this);
                        }
                    }
                }

                if ((_board.castlePerm & (int)Castle.WQCA) > 0)
                {
                    if (_board.Pieces[(int)BoardDef.D1] == (int)Piece.EMPTY && _board.Pieces[(int)BoardDef.C1] == (int)Piece.EMPTY && _board.Pieces[(int)BoardDef.B1] == (int)Piece.EMPTY)
                    {
                        if (!ChessBoard.SqAttacked((int)BoardDef.E1, Sides.BLACK, _board) && !ChessBoard.SqAttacked((int)BoardDef.D1, Sides.BLACK, _board))
                        {
                            this = GML.AddQuietMove(_board, Move.SetMove((int)BoardDef.E1, (int)BoardDef.C1, (int)Piece.EMPTY, (int)Piece.EMPTY, Move.CastleFLAG), this);
                        }
                    }
                }
            }
            else
            {
                //black pces
                for (_pceNum = 0; _pceNum < _board.PceNum[(int)Piece.bP]; _pceNum++)
                {
                    _sqr = _board.PieceList[(int)Piece.bP, _pceNum];

                    if (_board.Pieces[_sqr - 10] == (int)Piece.EMPTY)
                    {
                        this = GML.AddBlackPawnMove(_board, _sqr, _sqr - 10, this);

                        if (ChessBoard.RanksBrd[_sqr] == (int)FileRank.RANK_7 && _board.Pieces[_sqr - 20] == (int)Piece.EMPTY)
                            this = GML.AddQuietMove(_board, Move.SetMove(_sqr, _sqr - 20, (int)Piece.EMPTY, (int)Piece.EMPTY, Move.PawnStartFLAG), this);
                    }

                    if (!Converter.IsOffboard(_sqr - 9) && Config.PieceColor[_board.Pieces[_sqr - 9]] == Sides.WHITE)
                        this = GML.AddBlackPawnCaptureMove(_board, _sqr, _sqr - 9, _board.Pieces[_sqr - 9], this);

                    if (!Converter.IsOffboard(_sqr - 11) && Config.PieceColor[_board.Pieces[_sqr - 11]] == Sides.WHITE)
                        this = GML.AddBlackPawnCaptureMove(_board, _sqr, _sqr - 11, _board.Pieces[_sqr - 11], this);

                    if (_board.EnPas != (int)BoardDef.NO_SQ)
                    {
                        if (_sqr - 9 == _board.EnPas)
                            this = GML.AddEnPassantMove(_board, Move.SetMove(_sqr, _sqr - 9, (int)Piece.EMPTY, (int)Piece.EMPTY, Move.EnPassantFLAG), this);

                        if (_sqr - 11 == _board.EnPas)
                            this = GML.AddEnPassantMove(_board, Move.SetMove(_sqr, _sqr - 11, (int)Piece.EMPTY, (int)Piece.EMPTY, Move.EnPassantFLAG), this);
                    }
                }

                if ((_board.castlePerm & (int)Castle.BKCA) > 0)
                {
                    if (_board.Pieces[(int)BoardDef.F8] == (int)Piece.EMPTY && _board.Pieces[(int)BoardDef.G8] == (int)Piece.EMPTY)
                    {
                        if (!ChessBoard.SqAttacked((int)BoardDef.E8, Sides.WHITE, _board) && !ChessBoard.SqAttacked((int)BoardDef.F8, Sides.WHITE, _board))
                        {
                            this = GML.AddQuietMove(_board, Move.SetMove((int)BoardDef.E8, (int)BoardDef.G8, (int)Piece.EMPTY, (int)Piece.EMPTY, Move.CastleFLAG), this);
                        }
                    }
                }

                if ((_board.castlePerm & (int)Castle.BQCA) > 0)
                {
                    if (_board.Pieces[(int)BoardDef.D8] == (int)Piece.EMPTY && _board.Pieces[(int)BoardDef.C8] == (int)Piece.EMPTY && _board.Pieces[(int)BoardDef.B8] == (int)Piece.EMPTY)
                    {
                        if (!ChessBoard.SqAttacked((int)BoardDef.E8, Sides.WHITE, _board) && !ChessBoard.SqAttacked((int)BoardDef.D8, Sides.WHITE, _board))
                        {
                            this = GML.AddQuietMove(_board, Move.SetMove((int)BoardDef.E8, (int)BoardDef.C8, (int)Piece.EMPTY, (int)Piece.EMPTY, Move.CastleFLAG), this);
                        }
                    }
                }
            }

            //loop slide
            _pceIndex = _side * 4;
            _piece = GML.LoopSlidePiece[_pceIndex];
            _pceIndex++;

            while (_piece != 0)
            {
                for (_pceNum = 0; _pceNum < _board.PceNum[_piece]; _pceNum++)
                {
                    _sqr = _board.PieceList[_piece, _pceNum];

                    for (_index = 0; _index < Config.NumDir[_piece]; _index++)
                    {
                        _dir = Config.PieceDirections[_piece, _index];
                        _tempSqr = _sqr + _dir;

                        while (!Converter.IsOffboard(_tempSqr))
                        {
                            if (_board.Pieces[_tempSqr] != (int)Piece.EMPTY)
                            {
                                if ((int)Config.PieceColor[_board.Pieces[_tempSqr]] == (_side ^ 1))
                                {
                                    this = GML.AddCaptureMove(_board, Move.SetMove(_sqr, _tempSqr, _board.Pieces[_tempSqr], (int)Piece.EMPTY, 0), this);
                                }
                                break;
                            }
                            this = GML.AddQuietMove(_board, Move.SetMove(_sqr, _tempSqr, (int)Piece.EMPTY, (int)Piece.EMPTY, 0), this);
                            _tempSqr += _dir;
                        }
                    }
                }

                _piece = GML.LoopSlidePiece[_pceIndex];
                _pceIndex++;
            }

            //loop nonslide
            _pceIndex = _side * 3;
            _piece = GML.LoopNonSlidePiece[_pceIndex];
            _pceIndex++;

            while (_piece != 0)
            {
                for (_pceNum = 0; _pceNum < _board.PceNum[_piece]; _pceNum++)
                {
                    _sqr = _board.PieceList[_piece, _pceNum];

                    for (_index = 0; _index < Config.NumDir[_piece]; _index++)
                    {
                        _dir = Config.PieceDirections[_piece, _index];
                        _tempSqr = _sqr + _dir;

                        if (Converter.IsOffboard(_tempSqr)) continue;

                        if (_board.Pieces[_tempSqr] != (int)Piece.EMPTY)
                        {
                            if ((int)Config.PieceColor[_board.Pieces[_tempSqr]] == (_side ^ 1))
                            {
                                this = GML.AddCaptureMove(_board, Move.SetMove(_sqr, _tempSqr, _board.Pieces[_tempSqr], (int)Piece.EMPTY, 0), this);
                            }
                            continue;
                        }
                        this = GML.AddQuietMove(_board, Move.SetMove(_sqr, _tempSqr, (int)Piece.EMPTY, (int)Piece.EMPTY, 0), this);
                    }
                }

                _piece = GML.LoopNonSlidePiece[_pceIndex];
                _pceIndex++;
            }
        }

        public void GenerateAllCaps(BOARD _board)
        {
            Error.Handler(_board, "generate");

            Count = 0;

            var _piece = 0;
            var _side = _board.Side;
            var _sqr = 0;
            var _tempSqr = 0;
            var _pceNum = 0;
            var _dir = 0;
            var _index = 0;
            var _pceIndex = 0;

            if (_side == (int)Sides.WHITE)
            {
                //white pces
                for (_pceNum = 0; _pceNum < _board.PceNum[(int)Piece.wP]; _pceNum++)
                {
                    _sqr = _board.PieceList[(int)Piece.wP, _pceNum];

                    if (!Converter.IsOffboard(_sqr + 9) && Config.PieceColor[_board.Pieces[_sqr + 9]] == Sides.BLACK)
                        this = GML.AddWhitePawnCaptureMove(_board, _sqr, _sqr + 9, _board.Pieces[_sqr + 9], this);

                    if (!Converter.IsOffboard(_sqr + 11) && Config.PieceColor[_board.Pieces[_sqr + 11]] == Sides.BLACK)
                        this = GML.AddWhitePawnCaptureMove(_board, _sqr, _sqr + 11, _board.Pieces[_sqr + 11], this);

                    if (_board.EnPas != (int)BoardDef.NO_SQ)
                    {
                        if (_sqr + 9 == _board.EnPas)
                            this = GML.AddEnPassantMove(_board, Move.SetMove(_sqr, _sqr + 9, (int)Piece.EMPTY, (int)Piece.EMPTY, Move.EnPassantFLAG), this);

                        if (_sqr + 11 == _board.EnPas)
                            this = GML.AddEnPassantMove(_board, Move.SetMove(_sqr, _sqr + 11, (int)Piece.EMPTY, (int)Piece.EMPTY, Move.EnPassantFLAG), this);
                    }
                }
            }
            else
            {
                //black pces
                for (_pceNum = 0; _pceNum < _board.PceNum[(int)Piece.bP]; _pceNum++)
                {
                    _sqr = _board.PieceList[(int)Piece.bP, _pceNum];

                    if (!Converter.IsOffboard(_sqr - 9) && Config.PieceColor[_board.Pieces[_sqr - 9]] == Sides.WHITE)
                        this = GML.AddBlackPawnCaptureMove(_board, _sqr, _sqr - 9, _board.Pieces[_sqr - 9], this);

                    if (!Converter.IsOffboard(_sqr - 11) && Config.PieceColor[_board.Pieces[_sqr - 11]] == Sides.WHITE)
                        this = GML.AddBlackPawnCaptureMove(_board, _sqr, _sqr - 11, _board.Pieces[_sqr - 11], this);

                    if (_board.EnPas != (int)BoardDef.NO_SQ)
                    {
                        if (_sqr - 9 == _board.EnPas)
                            this = GML.AddEnPassantMove(_board, Move.SetMove(_sqr, _sqr - 9, (int)Piece.EMPTY, (int)Piece.EMPTY, Move.EnPassantFLAG), this);

                        if (_sqr - 11 == _board.EnPas)
                            this = GML.AddEnPassantMove(_board, Move.SetMove(_sqr, _sqr - 11, (int)Piece.EMPTY, (int)Piece.EMPTY, Move.EnPassantFLAG), this);
                    }
                }
            }

            //loop slide
            _pceIndex = _side * 4;
            _piece = GML.LoopSlidePiece[_pceIndex];
            _pceIndex++;

            while (_piece != 0)
            {
                for (_pceNum = 0; _pceNum < _board.PceNum[_piece]; _pceNum++)
                {
                    _sqr = _board.PieceList[_piece, _pceNum];

                    for (_index = 0; _index < Config.NumDir[_piece]; _index++)
                    {
                        _dir = Config.PieceDirections[_piece, _index];
                        _tempSqr = _sqr + _dir;

                        while (!Converter.IsOffboard(_tempSqr))
                        {
                            if (_board.Pieces[_tempSqr] != (int)Piece.EMPTY)
                            {
                                if ((int)Config.PieceColor[_board.Pieces[_tempSqr]] == (_side ^ 1))
                                {
                                    this = GML.AddCaptureMove(_board, Move.SetMove(_sqr, _tempSqr, _board.Pieces[_tempSqr], (int)Piece.EMPTY, 0), this);
                                }
                                break;
                            }
                            _tempSqr += _dir;
                        }
                    }
                }

                _piece = GML.LoopSlidePiece[_pceIndex];
                _pceIndex++;
            }

            //loop nonslide
            _pceIndex = _side * 3;
            _piece = GML.LoopNonSlidePiece[_pceIndex];
            _pceIndex++;

            while (_piece != 0)
            {
                for (_pceNum = 0; _pceNum < _board.PceNum[_piece]; _pceNum++)
                {
                    _sqr = _board.PieceList[_piece, _pceNum];

                    for (_index = 0; _index < Config.NumDir[_piece]; _index++)
                    {
                        _dir = Config.PieceDirections[_piece, _index];
                        _tempSqr = _sqr + _dir;

                        if (Converter.IsOffboard(_tempSqr)) continue;

                        if (_board.Pieces[_tempSqr] != (int)Piece.EMPTY)
                        {
                            if ((int)Config.PieceColor[_board.Pieces[_tempSqr]] == (_side ^ 1))
                            {
                                this = GML.AddCaptureMove(_board, Move.SetMove(_sqr, _tempSqr, _board.Pieces[_tempSqr], (int)Piece.EMPTY, 0), this);
                            }
                            continue;
                        }
                    }
                }

                _piece = GML.LoopNonSlidePiece[_pceIndex];
                _pceIndex++;
            }
        }

        public void PickNextMove(int _moveNum)
        {
            var _temp = new MOVE();

            var _bestScore = 0;
            var _bestNum = _moveNum;

            for (int _index = _moveNum; _index < Count; _index++)
            {
                if(Moves[_index].Score > _bestScore)
                {
                    _bestScore = Moves[_index].Score;
                    _bestNum = _index;
                }
            }

            _temp = Moves[_moveNum];

            Moves[_moveNum] = Moves[_bestNum];
            Moves[_bestNum] = _temp;
        }
    }
}
