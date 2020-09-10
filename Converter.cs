using CEAngela.Board;
using CEAngela.Setup;

namespace CEAngela.Convert
{
    public static class Converter
    {
        public static int CordToSqr(int f, int r)
        {
            return (21 + f) + (10 * r);
        }

        public static int SQ64(int _120Index)
        {
            return ChessBoard.Sq120ToSq64[_120Index];
        }

        public static int SQ64(BoardDef _120Index)
        {
            return ChessBoard.Sq120ToSq64[(int)_120Index];
        }

        public static int SQ120(int _64Index)
        {
            return ChessBoard.Sq64ToSq120[_64Index];
        }

        public static int SQ120(BoardDef _64Index)
        {
            return ChessBoard.Sq64ToSq120[(int)_64Index];
        }

        public static bool IsBQ(int _piece)
        {
            return ChessBoard.PieceBishopQueen[_piece] == (int)Bool.TRUE;
        }

        public static bool IsBQ(BoardDef _piece)
        {
            return ChessBoard.PieceBishopQueen[(int)_piece] == (int)Bool.TRUE;
        }

        public static bool IsRQ(int _piece)
        {
            return ChessBoard.PieceRookQueen[_piece] == (int)Bool.TRUE;
        }

        public static bool IsRQ(BoardDef _piece)
        {
            return ChessBoard.PieceRookQueen[(int)_piece] == (int)Bool.TRUE;
        }

        public static bool IsKn(int _piece)
        {
            return ChessBoard.PieceKnight[_piece] == (int)Bool.TRUE;
        }

        public static bool IsKn(BoardDef _piece)
        {
            return ChessBoard.PieceKnight[(int)_piece] == (int)Bool.TRUE;
        }

        public static bool IsKi(int _piece)
        {
            return ChessBoard.PieceKing[_piece] == (int)Bool.TRUE;
        }

        public static bool IsKi(BoardDef _piece)
        {
            return ChessBoard.PieceKing[(int)_piece] == (int)Bool.TRUE;
        }

        public static bool IsOffboard(int _index)
        {
            return ChessBoard.FilesBrd[_index] == (int)BoardDef.NO_SQ;
        }

        public static bool FlagParse(int _flag ,int _move)
        {
            return (_flag & _move) > 0;
        }

        public static bool ConfigBool(int _condition)
        {
            return _condition == (int)Bool.TRUE;
        }

        public static bool IsEmpty(int _piece)
        {
            return _piece == (int)Piece.EMPTY;
        }
    }
}

