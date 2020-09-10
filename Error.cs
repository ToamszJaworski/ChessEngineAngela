using CEAngela.BitBoards;
using CEAngela.Board;
using CEAngela.Convert;
using CEAngela.Keys;
using CEAngela.Setup;
using System;

namespace CEAngela.ErrorHandler
{
    public static class Error
    {
        static bool DebugMode = false;

        public static void Assert(bool n, string _title)
        {
            if (!DebugMode) return;

            if (!n)
            {
                var _startColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.Write("\n{0}: Error Detected.\n\n", _title.ToUpper());
                Console.ForegroundColor = _startColor;
                System.Environment.Exit(0);
            }
        }

        public static void Handler(BOARD _board, string _title)
        {
            if (!DebugMode) return;

            var _error = CheckBoard(_board);

            if (_error > 0)
            {
                var _startColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                System.Console.Write("\n{0}: Error Detected. Error Code: {1,3}\n\n", _title.ToUpper(), _error);
                Console.ForegroundColor = _startColor;
                System.Environment.Exit(0);
            }
        }

        static int CheckBoard(BOARD _position)
        {
            var _pceNumArray = new int[13] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            var _bigPce = new int[3] { 0, 0, 0 };
            var _majPce = new int[3] { 0, 0, 0 };
            var _minPce = new int[3] { 0, 0, 0 };
            var _material = new int[3] { 0, 0, 0 };

            var _sq64 = 0;
            var _piece = 0;
            var _pceNum = 0;
            var _sq120 = 0;
            var _colour = 0;
            var _pCount = 0;

            var _pawns = new ulong[3] { 0UL, 0UL, 0UL };

            _pawns[(int)Sides.WHITE] = _position.Pawns[(int)Sides.WHITE];
            _pawns[(int)Sides.BLACK] = _position.Pawns[(int)Sides.BLACK];
            _pawns[(int)Sides.BOTH] = _position.Pawns[(int)Sides.BOTH];

            for (_piece = (int)Piece.wP; _piece <= (int)Piece.bK; _piece++)
            {
                for (_pceNum = 0; _pceNum < _position.PceNum[_piece]; _pceNum++)
                {
                    _sq120 = _position.PieceList[_piece, _pceNum];
                    if (_position.Pieces[_sq120] != _piece) return 1;
                }
            }

            for (_sq64 = 0; _sq64 < 64; _sq64++)
            {
                _sq120 = Converter.SQ120(_sq64);
                _piece = _position.Pieces[_sq120];
                _pceNumArray[_piece]++;
                _colour = (int)Config.PieceColor[_piece];

                if (Config.PieceBig[_piece] == (int)Bool.TRUE) _bigPce[_colour]++;
                if (Config.PieceMajor[_piece] == (int)Bool.TRUE) _majPce[_colour]++;
                if (Config.PieceMinor[_piece] == (int)Bool.TRUE) _minPce[_colour]++;

                _material[_colour] += Config.PieceValue[_piece];
            }

            for (_piece = (int)Piece.wP; _piece <= (int)Piece.bK; _piece++)
                if (_pceNumArray[_piece] != _position.PceNum[_piece]) return 2;

            _pCount = BitBoard.CountBits(_pawns[(int)Sides.WHITE]);
            if (_pCount != _position.PceNum[(int)Piece.wP]) return 3;

            _pCount = BitBoard.CountBits(_pawns[(int)Sides.BLACK]);
            if (_pCount != _position.PceNum[(int)Piece.bP]) return 4;

            _pCount = BitBoard.CountBits(_pawns[(int)Sides.BOTH]);
            if (_pCount != _position.PceNum[(int)Piece.wP] + _position.PceNum[(int)Piece.bP]) return 5;


            while (_pawns[(int)Sides.WHITE] > 0)
            {
                _sq64 = BitBoard.PopBit(_pawns[(int)Sides.WHITE], out _pawns[(int)Sides.WHITE]);
                if (_position.Pieces[Converter.SQ120(_sq64)] != (int)Piece.wP) return 6;
            }

            while (_pawns[(int)Sides.BLACK] > 0)
            {
                _sq64 = BitBoard.PopBit(_pawns[(int)Sides.BLACK], out _pawns[(int)Sides.BLACK]);
                if (_position.Pieces[Converter.SQ120(_sq64)] != (int)Piece.bP) return 7;
            }

            while (_pawns[(int)Sides.BOTH] > 0)
            {
                _sq64 = BitBoard.PopBit(_pawns[(int)Sides.BOTH], out _pawns[(int)Sides.BOTH]);
                if (_position.Pieces[Converter.SQ120(_sq64)] != (int)Piece.wP && _position.Pieces[Converter.SQ120(_sq64)] != (int)Piece.bP) return 8;
            }

            if (_material[(int)Sides.WHITE] != _position.Material[(int)Sides.WHITE]
                || _material[(int)Sides.BLACK] != _position.Material[(int)Sides.BLACK]) return 9;

            if (_majPce[(int)Sides.WHITE] != _position.MajPce[(int)Sides.WHITE]
                || _majPce[(int)Sides.BLACK] != _position.MajPce[(int)Sides.BLACK]) return 10;

            if (_minPce[(int)Sides.WHITE] != _position.MinPce[(int)Sides.WHITE]
                || _minPce[(int)Sides.BLACK] != _position.MinPce[(int)Sides.BLACK]) return 11;

            if (_bigPce[(int)Sides.WHITE] != _position.BigPce[(int)Sides.WHITE]
                || _bigPce[(int)Sides.BLACK] != _position.BigPce[(int)Sides.BLACK]) return 12;


            if (_position.Side != (int)Sides.WHITE && _position.Side != (int)Sides.BLACK) return 13;
            //if (HashKeys.GeneratePosKey(_position) != _position.PosKey) return 14;

            if (_position.EnPas != (int)BoardDef.NO_SQ &&
                (ChessBoard.RanksBrd[_position.EnPas] != (int)FileRank.RANK_6
                || _position.Side != (int)Sides.WHITE) && (ChessBoard.RanksBrd[_position.EnPas] != (int)FileRank.RANK_3
                || _position.Side != (int)Sides.BLACK)) return 15;

            if (_position.Pieces[_position.KingSq[(int)Sides.WHITE]] != (int)Piece.wK) return 16;
            if (_position.Pieces[_position.KingSq[(int)Sides.BLACK]] != (int)Piece.bK) return 17;

            return 0;
        }
    }
}
