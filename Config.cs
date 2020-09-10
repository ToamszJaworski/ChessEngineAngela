using CEAngela.Tests;

namespace CEAngela.Setup
{
    public static class Config
    {
        //Start Config
        public static string Name = "Angela v1.0";
        public static string StartFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        public static PERF[] PerfFENs =
        {
            new PERF("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1", 4, 4085603), 
            new PERF("4k3/8/8/8/8/8/8/4K2R b K - 0 1", 6, 899442), 
            new PERF("K7/8/2n5/1n6/8/8/8/k6N b - - 0 1", 6, 688780),
            new PERF("n1n5/PPPk4/8/8/8/8/4Kppp/5N1N w - - 0 1", 5, 3605103),
            new PERF("n1n5/1Pk5/8/8/8/8/5Kp1/5N1N b - - 0 1", 5, 2193768),
            new PERF("n1n5/PPPk4/8/8/8/8/4Kppp/5N1N b - - 0 1", 5, 3605103),
            new PERF("3k4/3pp3/8/8/8/8/3PP3/3K4 w - - 0 1", 6, 199002),
            new PERF("8/2k1p3/3pP3/3P2K1/8/8/8/8 b - - 0 1", 6, 34822),
            new PERF("r3k2r/8/8/8/8/8/8/R3K2R b KQkq - 0 1", 5, 7594526),
        };

        public static int MaxMoves = 2048;
        public static int MaxPositionMoves = 256;
        public static int MaxDepth = 64;

        //Evaluation
        public static int[] PieceValue = new int[13]
        {
            0, //void
            100, //white pawn
            320, //white knight
            330, //white bishop
            550, //white rook
            1000, //white queen
            50000, //white king
            100, //black pawn
            325, //black knight
            325, //black bishop
            550, //black rook
            1000, //black queen
            50000 //black king
        };

        public static int[] PawnTable = {
            0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,
            10  ,   10  ,   0   ,   -10 ,   -10 ,   0   ,   10  ,   10  ,
            5   ,   0   ,   0   ,   5   ,   5   ,   0   ,   0   ,   5   ,
            0   ,   0   ,   10  ,   20  ,   20  ,   10  ,   0   ,   0   ,
            5   ,   5   ,   5   ,   10  ,   10  ,   5   ,   5   ,   5   ,
            10  ,   10  ,   10  ,   20  ,   20  ,   10  ,   10  ,   10  ,
            20  ,   20  ,   20  ,   30  ,   30  ,   20  ,   20  ,   20  ,
            0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0
            };

        public static int[] KnightTable = {
            0   ,   -10 ,   0   ,   0   ,   0   ,   0   ,   -10 ,   0   ,
            0   ,   0   ,   0   ,   5   ,   5   ,   0   ,   0   ,   0   ,
            0   ,   0   ,   10  ,   10  ,   10  ,   10  ,   0   ,   0   ,
            0   ,   0   ,   10  ,   20  ,   20  ,   10  ,   5   ,   0   ,
            5   ,   10  ,   15  ,   20  ,   20  ,   15  ,   10  ,   5   ,
            5   ,   10  ,   10  ,   20  ,   20  ,   10  ,   10  ,   5   ,
            0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
            0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0
            };

        public static int[] BishopTable = {
            0   ,   0   ,   -10 ,   0   ,   0   ,   -10 ,   0   ,   0   ,
            0   ,   0   ,   0   ,   10  ,   10  ,   0   ,   0   ,   0   ,
            0   ,   0   ,   10  ,   15  ,   15  ,   10  ,   0   ,   0   ,
            0   ,   10  ,   15  ,   20  ,   20  ,   15  ,   10  ,   0   ,
            0   ,   10  ,   15  ,   20  ,   20  ,   15  ,   10  ,   0   ,
            0   ,   0   ,   10  ,   15  ,   15  ,   10  ,   0   ,   0   ,
            0   ,   0   ,   0   ,   10  ,   10  ,   0   ,   0   ,   0   ,
            0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0   ,   0
            };

        public static int[] RookTable = {
            0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
            0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
            0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
            0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
            0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
            0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
            25  ,   25  ,   25  ,   25  ,   25  ,   25  ,   25  ,   25  ,
            0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0
            };

        public static int[] QueenTable = {
            0   ,   0   ,   5   ,   50  ,   50  ,   5   ,   0   ,   0   ,
            0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
            0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
            0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
            5   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   5   ,
            0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0   ,
            15  ,   15  ,   15  ,   15  ,   15  ,   15  ,   15  ,   15  ,
            0   ,   0   ,   5   ,   10  ,   10  ,   5   ,   0   ,   0
            };

        public static int[] KingTableEndgame = {
            -30 ,   -10 ,   0   ,   0   ,   0   ,   0   ,   -10 ,   -30 ,
            -10,    0   ,   5  ,   5  ,   5  ,   5  ,   0   ,   -10 ,
            0   ,   5  ,   10  ,   10  ,   10  ,   10  ,   5  ,   0   ,
            0   ,   5  ,   10  ,   20  ,   20  ,   10  ,   5  ,   0   ,
            0   ,   5  ,   10  ,   20  ,   20  ,   10  ,   5  ,   0   ,
            0   ,   5  ,   10  ,   10  ,   10  ,   10  ,   5  ,   0   ,
            -10 ,   0   ,   5  ,   5  ,   5  ,   5  ,   0   ,   -10 ,
            -30 ,   -10 ,   0   ,   0   ,   0   ,   0   ,   -10 ,   -30
        };

        public static int[] KingTableOpening = {
            0   ,   5   ,   5   ,   -10 ,   -10 ,   0   ,   10  ,   5   ,
            -10 ,   -10 ,   -10 ,   -10 ,   -10 ,   -10 ,   -10 ,   -10 ,
            -30 ,   -30 ,   -30 ,   -30 ,   -30 ,   -30 ,   -30 ,   -30 ,
            -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,
            -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,
            -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,
            -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,
            -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70 ,   -70
        };

        public static int PawnIsolated = -10;
        public static int[] PawnPassed = { 0, 5, 10, 20, 35, 60, 100, 200 };
        public static int RookOpenFile = 10;
        public static int RookSemiOpenFile = 5;
        public static int QueenOpenFile = 5;
        public static int QueenSemiOpenFile = 3;
        public static int BishopPair = 30;

        //VisualChars
        public static char[] PceChar = { '.', 'P', 'N', 'B', 'R', 'Q', 'K', 'p', 'n', 'b', 'r', 'q', 'k' };
        public static char[] SideChar = { 'w', 'b', '-' };
        public static char[] RankChar = { '1', '2', '3', '4', '5', '6', '7', '8' };
        public static char[] FileChar = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };

        //Attack Defs
        public static int[] KnightDir = new int[8] { -8, -19, -21, -12, 8, 19, 21, 12 };
        public static int[] RookDir = new int[4] { -1, -10, 1, 10 };
        public static int[] BishopDir = new int[4] { -9, -11, 11, 9 };
        public static int[] KingDir = new int[8] { -1, -10, 1, 10, -9, -11, 11, 9 };

        //Pieces Hierarchy
        public static int[] PieceBig = new int[13]
        {   
            (int)Bool.FALSE, //void
            (int)Bool.FALSE, //white pawn
            (int)Bool.TRUE, //white knight
            (int)Bool.TRUE, //white bishop
            (int)Bool.TRUE, //white rook
            (int)Bool.TRUE, //white queen
            (int)Bool.TRUE, //white king
            (int)Bool.FALSE, //black pawn
            (int)Bool.TRUE, //black knight
            (int)Bool.TRUE, //black bishop
            (int)Bool.TRUE, //black rook
            (int)Bool.TRUE, //black queen
            (int)Bool.TRUE //black king
        };
        public static int[] PieceMajor = new int[13]
        {   
            (int) Bool.FALSE, //void
            (int) Bool.FALSE, //white pawn
            (int) Bool.FALSE, //white knight
            (int) Bool.FALSE, //white bishop
            (int) Bool.TRUE, //white rook
            (int) Bool.TRUE, //white queen
            (int) Bool.TRUE, //white king
            (int) Bool.FALSE, //black pawn
            (int) Bool.FALSE, //black knight
            (int) Bool.FALSE, //black bishop
            (int) Bool.TRUE, //black rook
            (int) Bool.TRUE, //black queen
            (int) Bool.TRUE //black king
        };
        public static int[] PieceMinor = new int[13]
        {   
            (int) Bool.FALSE, //void
            (int) Bool.FALSE, //white pawn
            (int) Bool.TRUE, //white knight
            (int) Bool.TRUE, //white bishop
            (int) Bool.FALSE, //white rook
            (int) Bool.FALSE, //white queen
            (int) Bool.FALSE, //white king
            (int) Bool.FALSE, //black pawn
            (int) Bool.TRUE, //black knight
            (int) Bool.TRUE, //black bishop
            (int) Bool.FALSE, //black rook
            (int) Bool.FALSE, //black queen
            (int) Bool.FALSE //black king
        };
        public static int[] PiecePawn = new int[13]
        {
            (int) Bool.FALSE, //void
            (int) Bool.TRUE, //white pawn
            (int) Bool.FALSE, //white knight
            (int) Bool.FALSE, //white bishop
            (int) Bool.FALSE, //white rook
            (int) Bool.FALSE, //white queen
            (int) Bool.FALSE, //white king
            (int) Bool.TRUE, //black pawn
            (int) Bool.FALSE, //black knight
            (int) Bool.FALSE, //black bishop
            (int) Bool.FALSE, //black rook
            (int) Bool.FALSE, //black queen
            (int) Bool.FALSE //black king
        };
        public static int[] PieceKing = new int[13]
        {
            (int) Bool.FALSE, //void
            (int) Bool.FALSE, //white pawn
            (int) Bool.FALSE, //white knight
            (int) Bool.FALSE, //white bishop
            (int) Bool.FALSE, //white rook
            (int) Bool.FALSE, //white queen
            (int) Bool.TRUE, //white king
            (int) Bool.FALSE, //black pawn
            (int) Bool.FALSE, //black knight
            (int) Bool.FALSE, //black bishop
            (int) Bool.FALSE, //black rook
            (int) Bool.FALSE, //black queen
            (int) Bool.TRUE //black king
        };
        public static Sides[] PieceColor = new Sides[13]
        {
            Sides.BOTH, //void
            Sides.WHITE, //white pawn
            Sides.WHITE, //white knight 
            Sides.WHITE, //white bishop
            Sides.WHITE, //white rook
            Sides.WHITE, //white queen
            Sides.WHITE, //white king
            Sides.BLACK, //black pawn
            Sides.BLACK, //black knight
            Sides.BLACK, //black bishop
            Sides.BLACK, //black rook
            Sides.BLACK, //black queen
            Sides.BLACK //black king
        };

        //Piece Movement
        public static int[,] PieceDirections = new int[13, 8]
        {
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { -8, -19,  -21, -12, 8, 19, 21, 12 },
            { -9, -11, 11, 9, 0, 0, 0, 0 },
            { -1, -10,  1, 10, 0, 0, 0, 0 },
            { -1, -10,  1, 10, -9, -11, 11, 9 },
            { -1, -10,  1, 10, -9, -11, 11, 9 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
            { -8, -19,  -21, -12, 8, 19, 21, 12 },
            { -9, -11, 11, 9, 0, 0, 0, 0 },
            { -1, -10,  1, 10, 0, 0, 0, 0 },
            { -1, -10,  1, 10, -9, -11, 11, 9 },
            { -1, -10,  1, 10, -9, -11, 11, 9 }
        };
        public static int[] NumDir = new int[13] { 0, 0, 8, 4, 4, 8, 8, 0, 8, 4, 4, 8, 8 };

    }
}
