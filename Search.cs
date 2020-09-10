using CEAngela.Board;
using CEAngela.ErrorHandler;
using CEAngela.Evaluation;
using CEAngela.GUI.UCI_Protocol;
using CEAngela.Keys;
using CEAngela.Moves;
using CEAngela.Setup;
using CEAngela.Tools;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace CEAngela.SearchTree
{
    public static class Search
    {
        public static SEARCH_INFO SearchInfo = new SEARCH_INFO();
        public static int MATE = 30000 - Config.MaxDepth;

        public static bool IsRepetition(BOARD _board)
        {
            var _index = 0;

            for (_index = _board.HisPly - _board.FiftyMove; _index < _board.HisPly - 1; _index++)
            {
                if (_board.PosKey == _board.History[_index].PosKey)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public struct SEARCH_INFO
    {
        public int StartTime;
        public int StopTime;
        public int Depth;
        public int MovesToGo;

        public long Nodes;

        public bool Infinite;
        public bool Quit;
        public bool Stopped;
        public bool DepthSet;
        public bool TimeSet;

        public float FH;
        public float FHF;

        int _bestMove;

        public BOARD Clear(BOARD _board) 
        {
            var _b = _board;

            for (int x = 0; x < 13; x++)
            {

                for (int y = 0; y < ChessBoard.BoardSqrNumber; y++)
                {
                    _b.SearchHistory[x, y] = 0;
                }
            }

            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < Config.MaxDepth; y++)
                {
                    _b.SearchKillers[x, y] = 0;
                }
            }

            _b.HashTable.OverWrite = 0;
            _b.HashTable.Hit = 0;
            _b.HashTable.Cut = 0;

            _b.Ply = 0;

            Stopped = false;
            Nodes = 0;
            FH = 0;
            FHF = 0;

            return _b;
        }

        public void CheckUp()
        {
            //check if time up or interrupt from GUI
            if (TimeSet && Misc.GetTimeInMs() > StopTime)
                Stopped = true;
        }

        public BOARD SearchPosition(BOARD _board)
        {
            var _b = Clear(_board);

            for(var _currentDepth = 1; _currentDepth <= Depth; _currentDepth++)
            {
                var _bestScore = AlphaBeta(-30000, 300000, _currentDepth, _b, true);

                if (Stopped)
                    break;

                _bestMove = _b.PvArray[0];

                var _mateScore = 0;
                if (_bestScore > 28000)
                    _mateScore = Search.MATE - _bestScore;
                else if(_bestScore < -28000)
                    _mateScore = -(Search.MATE + _bestScore);

                //if (_mateScore != 0)
                    //Console.Write("info depth {1} score mate {0} nodes {2} time {3} ", _mateScore, _currentDepth, Nodes, Misc.GetTimeInMs() - StartTime);
               // else
                    Console.Write("info depth {1} score cp {0} nodes {2} time {3} ", _bestScore, _currentDepth, Nodes, Misc.GetTimeInMs() - StartTime);


                var _pvMoves = _board.GetPvLine(_currentDepth);
                Console.Write("pv");

                for (var _pvNum = 0; _pvNum < _pvMoves; _pvNum++)
                {
                    Console.Write(" {0}", Move.PrintMove(_b.PvArray[_pvNum]).ToLower());
                }
                Console.Write("\n");
                //Console.Write("Ordering: {0}\n", (FHF / FH).ToString("0.00"));           
            }


            Console.Write("bestmove {0}\n", Move.PrintMove(_bestMove).ToLower());

            return _b;
        }

        public int AlphaBeta(int _alpha, int _beta, int _depth, BOARD _board, bool DoNULL)
        {
;            Error.Handler(_board, "ABeta");

            if (_depth == 0)
            {
                return Quiescence(_alpha, _beta, _board); 
            }

            if ((Nodes & 2047) == 0)
            {
                CheckUp();
            }

            Nodes++;

            if ((Search.IsRepetition(_board) || _board.FiftyMove >= 100) && _board.Ply > 0)
                return 0;

            if (_board.Ply > Config.MaxDepth - 1)
                return Evaluate.EvaluatePosition(_board);

            var _inCheck = ChessBoard.SqAttacked(_board.KingSq[_board.Side], (Sides)(_board.Side^1), _board);

            if (_inCheck)
                _depth++;

            var _score = -30000;

            var _pvMove = 0;

            
            if(HashKeys.ProbeHashTable(_board, out _pvMove, out _score, _alpha, _beta, _depth, out _board))
            {
                _board.HashTable.Cut++;             
                    
                return _score;
            }

            
            if (DoNULL && !_inCheck && _board.Ply>0 && _board.BigPce[_board.Side] > 0 && _depth >= 4)
            {
                _board.MakeNullMove();

                _score = -AlphaBeta(-_beta, -_beta + 1, _depth - 4, _board, false);

                _board.TakeNullMove();

                if (Stopped)
                    return 0;

                if (_score >= _beta)
                    return _beta;
            }

            MOVE_LIST _list = new MOVE_LIST();
            _list.Initialize();
            _list.GenerateAllMoves(_board);

            var _moveNum = 0;
            var _legal = 0;
            var _oldAlpha = _alpha;
            var _bestMove = 0;
            var _bestScore = -30000;
            _score = -30000;

            if (_pvMove != 0)
            {
                for (_moveNum = 0; _moveNum < _list.Count; _moveNum++)
                {
                    if(_list.Moves[_moveNum].Move == _pvMove)
                    {
                        _list.Moves[_moveNum].Score = 2000000;
                        break;
                    }
                }
            }

            for (_moveNum = 0; _moveNum < _list.Count; _moveNum++)
            {
                _list.PickNextMove(_moveNum);

                if (!_board.MakeMove(_list.Moves[_moveNum].Move))
                    continue;

                _legal++;
                _score = -AlphaBeta(-_beta, -_alpha, _depth - 1, _board, true);
                _board.TakeMove();

                if (Stopped)
                    return 0;

                if (_score > _alpha)
                {
                    if (_score >= _beta)
                    {
                        if (_legal == 1)
                            FHF++;

                        FH++;

                        if ((_list.Moves[_moveNum].Move & Move.CapturedFLAG) <= 0)
                        {
                            _board.SearchKillers[1, _board.Ply] = _board.SearchKillers[0, _board.Ply];
                            _board.SearchKillers[0, _board.Ply] = _list.Moves[_moveNum].Move;
                        }


                        _bestMove = _list.Moves[_moveNum].Move;
                        _board.Store(_bestMove, _beta, Transposition.BETA, _depth);

                        return _beta;
                    }

                    _alpha = _score;
                    _bestMove = _list.Moves[_moveNum].Move;

                    if ((_list.Moves[_moveNum].Move & Move.CapturedFLAG) <= 0)
                    {
                        _board.SearchHistory[_board.Pieces[Move.From(_bestMove)], Move.To(_bestMove)] += _depth;
                    }
                }

                if (_score > _bestScore)
                    _bestScore = _score;
            }

            if(_legal == 0)
            {
                if (_inCheck)
                    return -30000 + _board.Ply;
                else
                    return 0;
            }

            if (_alpha != _oldAlpha)
                _board.Store(_bestMove, _bestScore, Transposition.EXACT, _depth);
            else
                _board.Store(_bestMove, _alpha, Transposition.ALPHA, _depth);

            return _alpha;
        }

        public int Quiescence(int _alpha, int _beta, BOARD _board)
        {
            Error.Handler(_board, "Quiescence");

            if((Nodes & 2047) == 0)
            {
                CheckUp();
            }

            Nodes++;

            if ((Search.IsRepetition(_board) || _board.FiftyMove >= 100) && _board.Ply > 0)
                return 0;

            if (_board.Ply > Config.MaxDepth - 1)
            {
                return Evaluate.EvaluatePosition(_board);
            }

            var _score = Evaluate.EvaluatePosition(_board);

            if (_score >= _beta)
                return _beta;

            if (_score > _alpha)
                _alpha = _score;

            MOVE_LIST _list = new MOVE_LIST();
            _list.Initialize();
            _list.GenerateAllCaps(_board);

            var _moveNum = 0;
            var _legal = 0;
            var _oldAlpha = _alpha;
            var _bestMove = 0;
            _score = -30000;
            var _pvMove = HashKeys.ProbePvMove(_board);

            if (_pvMove != 0)
            {
                for (_moveNum = 0; _moveNum < _list.Count; _moveNum++)
                {
                    if (_list.Moves[_moveNum].Move == _pvMove)
                    {
                        _list.Moves[_moveNum].Score = 2000000;
                        break;
                    }
                }
            }

            for (_moveNum = 0; _moveNum < _list.Count; _moveNum++)
            {
                _list.PickNextMove(_moveNum);

                if (!_board.MakeMove(_list.Moves[_moveNum].Move))
                    continue;

                _legal++;
                _score = -Quiescence(-_beta, -_alpha, _board);
                _board.TakeMove();

                if (Stopped)
                    return 0;

                if (_score > _alpha)
                {
                    if (_score >= _beta)
                    {
                        if (_legal == 1)
                            FHF++;

                        FH++;
                        return _beta;
                    }

                    _alpha = _score;
                    _bestMove = _list.Moves[_moveNum].Move;
                }
            }

            return _alpha;
        }
    }
}
