using CEAngela.Board;
using CEAngela.Convert;
using CEAngela.Setup;
using System;

namespace CEAngela.Attacks
{
    public static class Attack
    {
        public static bool SqAttacked(int _sqr, Sides _side, BOARD _position)
        {
            var _piece = 0;
            var _index = 0;
            var _tempSqr = 0;
            var _dir = 0;


            if (_side == Sides.WHITE)
            {
                if (_position.Pieces[_sqr - 11] == (int)Piece.wP || _position.Pieces[_sqr - 9] == (int)Piece.wP)
                    return true;
            }
            else
            {
                if (_position.Pieces[_sqr + 11] == (int)Piece.bP || _position.Pieces[_sqr + 9] == (int)Piece.bP)
                    return true;
            }

            for(_index = 0; _index < 8; _index++)
            {
                _piece = _position.Pieces[_sqr + Config.KnightDir[_index]];

                if (_piece != (int)BoardDef.NO_SQ)
                    if (Converter.IsKn(_piece) && Config.PieceColor[_piece] == _side)
                        return true;
            }

            
            for (_index = 0; _index < 4; _index++)
            {
                _dir = Config.RookDir[_index];
                _tempSqr = _sqr + _dir;
                _piece = _position.Pieces[_tempSqr];

                while(_piece != (int)BoardDef.NO_SQ)
                {
                    if (_piece != (int)Piece.EMPTY)
                    {
                        if (Converter.IsRQ(_piece) && Config.PieceColor[_piece] == _side)
                            return true;

                        break;
                    }

                    _tempSqr += _dir;
                    _piece = _position.Pieces[_tempSqr];
                }
            }

            for (_index = 0; _index < 4; _index++)
            {
                _dir = Config.BishopDir[_index];
                _tempSqr = _sqr + _dir;
                _piece = _position.Pieces[_tempSqr];

                while (_piece != (int)BoardDef.NO_SQ)
                {
                    if (_piece != (int)Piece.EMPTY)
                    {
                        if (Converter.IsBQ(_piece) && Config.PieceColor[_piece] == _side)
                            return true;

                        break;
                    }

                    _tempSqr += _dir;
                    _piece = _position.Pieces[_tempSqr];
                }
            }

            for (_index = 0; _index < 8; _index++)
            {
                _piece = _position.Pieces[_sqr + Config.KingDir[_index]];

                if(_piece != (int)BoardDef.NO_SQ)
                    if (Converter.IsKi(_piece) && Config.PieceColor[_piece] == _side)
                        return true;
            }            
            
            return false;
        }
    }
}
