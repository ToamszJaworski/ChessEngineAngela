using CEAngela.Board;
using System;
using System.Collections.Generic;
using System.Text;

namespace CEAngela.Moves.Logic
{
    public static class GML //Generate Move Logic
    {
        public static int[] LoopSlidePiece = 
        {
            (int)Piece.wB,
            (int)Piece.wR,
            (int)Piece.wQ,
            0,
            (int)Piece.bB,
            (int)Piece.bR,
            (int)Piece.bQ,
            0
        };

        public static int[] LoopNonSlidePiece =
        {
            (int)Piece.wN,
            (int)Piece.wK,
            0,
            (int)Piece.bN,
            (int)Piece.bK,
            0
        };

        public static MOVE_LIST AddQuietMove(BOARD _board, int _move, MOVE_LIST _list)
        {
            var _baseList = _list;

            _baseList.Moves[_baseList.Count].Move = _move;
            
            if(_board.SearchKillers[0, _board.Ply] == _move)
                _baseList.Moves[_baseList.Count].Score = 900000;
            else if (_board.SearchKillers[1, _board.Ply] == _move)
                _baseList.Moves[_baseList.Count].Score = 800000;
            else
                _baseList.Moves[_baseList.Count].Score = _board.SearchHistory[_board.Pieces[Move.From(_move)], Move.To(_move)];

            _baseList.Count++;

            return _baseList;
        }

        public static MOVE_LIST AddCaptureMove(BOARD _board, int _move, MOVE_LIST _list)
        {
            var _baseList = _list;

            _baseList.Moves[_baseList.Count].Move = _move;
            _baseList.Moves[_baseList.Count].Score = Move.MvvLvaScores[Move.Captured(_move), _board.Pieces[Move.From(_move)]] + 1000000;
            _baseList.Count++;

            return _baseList;
        }

        public static MOVE_LIST AddEnPassantMove(BOARD _board, int _move, MOVE_LIST _list)
        {
            var _baseList = _list;

            _baseList.Moves[_baseList.Count].Move = _move;
            _baseList.Moves[_baseList.Count].Score = 105 + 1000000;
            _baseList.Count++;

            return _baseList;
        }

        public static MOVE_LIST AddWhitePawnCaptureMove(BOARD _board, int _from, int _to, int _capture, MOVE_LIST _list)
        {
            var _baseList = _list;

            if (ChessBoard.RanksBrd[_from] == (int)FileRank.RANK_7)
            {
                _baseList = AddCaptureMove(_board, Move.SetMove(_from, _to, _capture, (int)Piece.wQ, 0), _baseList);
                _baseList = AddCaptureMove(_board, Move.SetMove(_from, _to, _capture, (int)Piece.wR, 0), _baseList);
                _baseList = AddCaptureMove(_board, Move.SetMove(_from, _to, _capture, (int)Piece.wN, 0), _baseList);
                _baseList = AddCaptureMove(_board, Move.SetMove(_from, _to, _capture, (int)Piece.wB, 0), _baseList);
            }
            else
            {
                _baseList = AddCaptureMove(_board, Move.SetMove(_from, _to, _capture, (int)Piece.EMPTY, 0), _baseList);
            }

            return _baseList;
        }

        public static MOVE_LIST AddWhitePawnMove(BOARD _board, int _from, int _to, MOVE_LIST _list, int _flag = 0)
        {
            var _baseList = _list;

            if (ChessBoard.RanksBrd[_from] == (int)FileRank.RANK_7)
            {
                _baseList = AddCaptureMove(_board, Move.SetMove(_from, _to, (int)Piece.EMPTY, (int)Piece.wQ, _flag), _baseList);
                _baseList = AddCaptureMove(_board, Move.SetMove(_from, _to, (int)Piece.EMPTY, (int)Piece.wR, _flag), _baseList);
                _baseList = AddCaptureMove(_board, Move.SetMove(_from, _to, (int)Piece.EMPTY, (int)Piece.wN, _flag), _baseList);
                _baseList = AddCaptureMove(_board, Move.SetMove(_from, _to, (int)Piece.EMPTY, (int)Piece.wB, _flag), _baseList);
            }
            else
            {
                _baseList = AddCaptureMove(_board, Move.SetMove(_from, _to, (int)Piece.EMPTY, (int)Piece.EMPTY, _flag), _baseList);
            }

            return _baseList;
        }

        public static MOVE_LIST AddBlackPawnCaptureMove(BOARD _board, int _from, int _to, int _capture, MOVE_LIST _list)
        {
            var _baseList = _list;

            if (ChessBoard.RanksBrd[_from] == (int)FileRank.RANK_2)
            {
                _baseList = AddCaptureMove(_board, Move.SetMove(_from, _to, _capture, (int)Piece.bQ, 0), _baseList);
                _baseList = AddCaptureMove(_board, Move.SetMove(_from, _to, _capture, (int)Piece.bR, 0), _baseList);
                _baseList = AddCaptureMove(_board, Move.SetMove(_from, _to, _capture, (int)Piece.bN, 0), _baseList);
                _baseList = AddCaptureMove(_board, Move.SetMove(_from, _to, _capture, (int)Piece.bB, 0), _baseList);
            }
            else
            {
                _baseList = AddCaptureMove(_board, Move.SetMove(_from, _to, _capture, (int)Piece.EMPTY, 0), _baseList);
            }

            return _baseList;
        }

        public static MOVE_LIST AddBlackPawnMove(BOARD _board, int _from, int _to, MOVE_LIST _list, int _flag = 0)
        {
            var _baseList = _list;

            if (ChessBoard.RanksBrd[_from] == (int)FileRank.RANK_2)
            {
                _baseList = AddCaptureMove(_board, Move.SetMove(_from, _to, (int)Piece.EMPTY, (int)Piece.bQ, _flag), _baseList);
                _baseList = AddCaptureMove(_board, Move.SetMove(_from, _to, (int)Piece.EMPTY, (int)Piece.bR, _flag), _baseList);
                _baseList = AddCaptureMove(_board, Move.SetMove(_from, _to, (int)Piece.EMPTY, (int)Piece.bN, _flag), _baseList);
                _baseList = AddCaptureMove(_board, Move.SetMove(_from, _to, (int)Piece.EMPTY, (int)Piece.bB, _flag), _baseList);
            }
            else
            {
                _baseList = AddCaptureMove(_board, Move.SetMove(_from, _to, (int)Piece.EMPTY, (int)Piece.EMPTY, _flag), _baseList);
            }

            return _baseList;
        }
    }
}
