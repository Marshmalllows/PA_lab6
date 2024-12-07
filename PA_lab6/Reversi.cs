namespace PA_lab6;

public class Reversi
{
    public int[][] Board { get; }

    private bool Turn { get; set; }

    public int[][] PossibleTurns { get; private set; }

    public int PossibleTurnsCount { get; private set; }

    public Reversi()
    {
        var board = new int[8][];
        for (var i = 0; i < board.Length; i++)
        {
            board[i] = new int[8];
            for (var j = 0; j < board[i].Length; j++)
            {
                board[i][j] = 0;
            }
        }

        board[3][3] = 1;
        board[4][4] = 1;
        board[3][4] = 2;
        board[4][3] = 2;
        Board = board;
        Turn = false;
        PossibleTurns = [];
        PossibleTurnsCount = 0;
    }

    public void MakeTurn(PictureBox tile)
    {
        var coordinates = tile.Name.Remove(0, 4).Split("-");
        var row = int.Parse(coordinates[0]);
        var column = int.Parse(coordinates[1]);

        if (CanMoveVertically(tile))
        {
            FlipVertically(tile);
        }
        if (CanMoveHorizontally(tile))
        {
            FlipHorizontally(tile);
        }
        if (CanMoveMainDiagonally(tile))
        {
            FlipMainDiagonally(tile);
        }
        if (CanMoveAdditionalDiagonally(tile))
        {
            FlipAdditionalDiagonally(tile);
        }
        
        PutPiece(row, column, Turn == false ? 1 : 2, tile);
        Turn = !Turn;
        GetPossibleTurns();

        if (PossibleTurnsCount == 0)
        { 
            Gui.TurnInfo!.Image = Image.FromFile(@"..\..\..\emptyTurn.png");
            var white = 0;
            var black = 0;

            foreach (var t in Board)
            {
                for (var j = 0; j < Board.Length; j++)
                {
                    switch (t[j])
                    {
                        case 1:
                            black++;
                            break;
                        case 2:
                            white++;
                            break;
                    }
                }
            }

            var winner = black > white ? "Black is the winner!" : black == white ? "Draw!" : "White is the winner!";
            MessageBox.Show(winner, "Game over!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        
        Gui.TurnInfo!.Image = !Turn ? Image.FromFile(@"..\..\..\turnBlack.png") : Image.FromFile(@"..\..\..\turnWhite.png");
    }

    private void PutPiece(int row, int column, int turn, PictureBox tile)
    {
        if (turn == 1)
        {
            Board[row][column] = 1;
            tile.Image = Image.FromFile(@"..\..\..\black.png");
            tile.Name = $"black{row}-{column}";
        }
        else
        {
            Board[row][column] = 2;
            tile.Image = Image.FromFile(@"..\..\..\white.png");
            tile.Name = $"white{row}-{column}";
        }
    }

    private bool CanMakeTurn(PictureBox tile)
    {
        return CanMoveVertically(tile) || CanMoveHorizontally(tile) || CanMoveMainDiagonally(tile) || CanMoveAdditionalDiagonally(tile);
    }

    public void GetPossibleTurns()
    {
        var freeSpace = new int[8][];
        for (var i = 0; i < freeSpace.Length; i++)
        {
            freeSpace[i] = new int[8];
            for (var j = 0; j < freeSpace.Length; j++)
            {
                freeSpace[i][j] = 0;
            }
        }

        for (var i = 0; i < Board.Length; i++)
        {
            for (var j = 0; j < Board.Length; j++)
            {
                if (Board[i][j] == 0) continue;
                for (var x = -1; x <= 1; x++)
                {
                    for (var y = -1; y <= 1; y++)
                    {
                        var xPos = i + x;
                        var yPos = j + y;
                        if (xPos >= 0 && xPos < Board.Length && yPos >= 0 && yPos < Board.Length &&
                            Board[xPos][yPos] == 0)
                        {
                            freeSpace[xPos][yPos] = 1;
                        }
                    }
                }
            }
        }

        for (var i = 0; i < freeSpace.Length; i++)
        {
            for (var j = 0; j < freeSpace.Length; j++)
            {
                if (freeSpace[i][j] != 1) continue;
                if (!CanMakeTurn(Gui.Tiles![i][j]))
                {
                    freeSpace[i][j] = 0;
                }
            }
        }

        var freeCount = 0;
        foreach (var row in freeSpace)
        {
            for (var i = 0; i < freeSpace.Length; i++)
            {
                if (row[i] != 1) continue;
                freeCount++;
            }
        }

        PossibleTurns = freeSpace;
        PossibleTurnsCount = freeCount;
    }

    private bool CanMoveVertically(PictureBox tile)
    {
        var coordinates = tile.Name.Remove(0, 4).Split("-");
        var row = int.Parse(coordinates[0]);
        var column = int.Parse(coordinates[1]); 
        var turn = Turn ? 2: 1;
        var opponentTurn = Turn ? 1: 2;

        for (var i = row - 1; i >= 0; i--)
        {
            if (Board[i][column] == turn && row - i > 1)
            {
                return true;
            }

            if (Board[i][column] != opponentTurn)
            {
                break;
            }
        }
        
        for (var i = row + 1; i < 8; i++)
        {
            if (Board[i][column] == turn && i - row > 1)
            {
                return true;
            }
            
            if (Board[i][column] != opponentTurn)
            {
                break;
            }
        }
        
        return false;
    }

    private void FlipVertically(PictureBox tile)
    {
        var coordinates = tile.Name.Remove(0, 4).Split("-");
        var row = int.Parse(coordinates[0]);
        var column = int.Parse(coordinates[1]); 
        var turn = Turn ? 2: 1;
        var opponentTurn = Turn ? 1: 2;

        var endRow = -1;
        for (var i = row - 1; i >= 0; i--)
        {
            if (Board[i][column] == turn && row - i > 1)
            {
                endRow = i;
                break;
            }

            if (Board[i][column] != opponentTurn)
            {
                break;
            }
        }

        if (endRow != -1)
        {
            for (var i = endRow + 1; i < row; i++)
            {
                PutPiece(i, column, turn, Gui.Tiles![i][column]);
            }

            endRow = -1;
        }
        
        for (var i = row + 1; i < 8; i++)
        {
            if (Board[i][column] == turn && i - row > 1)
            {
                endRow = i;
                break;
            }
            
            if (Board[i][column] != opponentTurn)
            {
                break;
            }
        }
        
        if (endRow != -1)
        {
            for (var i = row + 1; i < endRow; i++)
            {
                PutPiece(i, column, turn, Gui.Tiles![i][column]);
            }
        }
    }
    
    private bool CanMoveHorizontally(PictureBox tile)
    {
        var coordinates = tile.Name.Remove(0, 4).Split("-");
        var row = int.Parse(coordinates[0]);
        var column = int.Parse(coordinates[1]); 
        var turn = Turn ? 2: 1;
        var opponentTurn = Turn ? 1: 2;

        for (var i = column - 1; i >= 0; i--)
        {
            if (Board[row][i] == turn && column - i > 1)
            {
                return true;
            }

            if (Board[row][i] != opponentTurn)
            {
                break;
            }
        }
        
        for (var i = column + 1; i < 8; i++)
        {
            if (Board[row][i] == turn && i - column > 1)
            {
                return true;
            }
            
            if (Board[row][i] != opponentTurn)
            {
                break;
            }
        }
        
        return false;
    }
    
    private void FlipHorizontally(PictureBox tile)
    {
        var coordinates = tile.Name.Remove(0, 4).Split("-");
        var row = int.Parse(coordinates[0]);
        var column = int.Parse(coordinates[1]); 
        var turn = Turn ? 2: 1;
        var opponentTurn = Turn ? 1: 2;

        var endCol = -1;
        for (var i = column - 1; i >= 0; i--)
        {
            if (Board[row][i] == turn && column - i > 1)
            {
                endCol = i;
                break;
            }

            if (Board[row][i] != opponentTurn)
            {
                break;
            }
        }

        if (endCol != -1)
        {
            for (var i = endCol + 1; i < column; i++)
            {
                PutPiece(row, i, turn, Gui.Tiles![row][i]);
            }

            endCol = -1;
        }
        
        for (var i = column + 1; i < 8; i++)
        {
            if (Board[row][i] == turn && i - column > 1)
            {
                endCol = i;
                break;
            }
            
            if (Board[row][i] != opponentTurn)
            {
                break;
            }
        }
        
        if (endCol != -1)
        {
            for (var i = column + 1; i < endCol; i++)
            {
                PutPiece(row, i, turn, Gui.Tiles![row][i]);
            }
        }
    }
    
    private bool CanMoveMainDiagonally(PictureBox tile)
    {
        var coordinates = tile.Name.Remove(0, 4).Split("-");
        var row = int.Parse(coordinates[0]);
        var column = int.Parse(coordinates[1]); 
        var turn = Turn ? 2: 1;
        var opponentTurn = Turn ? 1: 2;

        for (var i = (row - 1, column - 1); i is { Item1: >= 0, Item2: >= 0 }; i.Item1--, i.Item2--)
        {
            if (Board[i.Item1][i.Item2] == turn && row - i.Item1 > 1 && column - i.Item2 > 1)
            {
                return true;
            }

            if (Board[i.Item1][i.Item2] != opponentTurn)
            {
                break;
            }
        }
        
        for (var i = (row + 1, column + 1); i is { Item1: < 8, Item2: < 8 }; i.Item1++, i.Item2++)
        {
            if (Board[i.Item1][i.Item2] == turn && i.Item1 - row > 1 && i.Item2 - column > 1)
            {
                return true;
            }
            
            if (Board[i.Item1][i.Item2] != opponentTurn)
            {
                break;
            }
        }
        
        return false;
    }
    
    private void FlipMainDiagonally(PictureBox tile)
    {
        var coordinates = tile.Name.Remove(0, 4).Split("-");
        var row = int.Parse(coordinates[0]);
        var column = int.Parse(coordinates[1]); 
        var turn = Turn ? 2: 1;
        var opponentTurn = Turn ? 1: 2;

        var endDiag = (-1, -1);
        for (var i = (row - 1, column - 1); i is { Item1: >= 0, Item2: >= 0 }; i.Item1--, i.Item2--)
        {
            if (Board[i.Item1][i.Item2] == turn && row - i.Item1 > 1 && column - i.Item2 > 1)
            {
                endDiag = i;
                break;
            }

            if (Board[i.Item1][i.Item2] != opponentTurn)
            {
                break;
            }
        }

        if (endDiag != (-1, -1))
        {
            for (var i = (endDiag.Item1 + 1, endDiag.Item2 + 1); i.Item1 < row && i.Item2 < column; i.Item1++, i.Item2++)
            {
                PutPiece(i.Item1, i.Item2, turn, Gui.Tiles![i.Item1][i.Item2]);
            }
            endDiag = (-1, -1);
        }
        
        for (var i = (row + 1, column + 1); i is { Item1: < 8, Item2: < 8 }; i.Item1++, i.Item2++)
        {
            if (Board[i.Item1][i.Item2] == turn && i.Item1 - row > 1 && i.Item2 - column > 1)
            {
                endDiag = i;
                break;
            }
            
            if (Board[i.Item1][i.Item2] != opponentTurn)
            {
                break;
            }
        }
        
        if (endDiag != (-1, -1))
        {
            for (var i = (row + 1, column + 1); i.Item1 < endDiag.Item1 && i.Item2 < endDiag.Item2; i.Item1++, i.Item2++)
            {
                PutPiece(i.Item1, i.Item2, turn, Gui.Tiles![i.Item1][i.Item2]);
            }
        }
    }
    
    private bool CanMoveAdditionalDiagonally(PictureBox tile)
    {
        var coordinates = tile.Name.Remove(0, 4).Split("-");
        var row = int.Parse(coordinates[0]);
        var column = int.Parse(coordinates[1]); 
        var turn = Turn ? 2: 1;
        var opponentTurn = Turn ? 1: 2;

        for (var i = (row - 1, column + 1); i is { Item1: >= 0, Item2: < 8 }; i.Item1--, i.Item2++)
        {
            if (Board[i.Item1][i.Item2] == turn && row - i.Item1 > 1 && i.Item2 - column > 1)
            {
                return true;
            }

            if (Board[i.Item1][i.Item2] != opponentTurn)
            {
                break;
            }
        }
        
        for (var i = (row + 1, column - 1); i is { Item1: < 8, Item2: >= 0 }; i.Item1++, i.Item2--)
        {
            if (Board[i.Item1][i.Item2] == turn && i.Item1 - row > 1 && column - i.Item2 > 1)
            {
                return true;
            }
            
            if (Board[i.Item1][i.Item2] != opponentTurn)
            {
                break;
            }
        }
        
        return false;
    }
    
    private void FlipAdditionalDiagonally(PictureBox tile)
    {
        var coordinates = tile.Name.Remove(0, 4).Split("-");
        var row = int.Parse(coordinates[0]);
        var column = int.Parse(coordinates[1]); 
        var turn = Turn ? 2: 1;
        var opponentTurn = Turn ? 1: 2;

        var endDiag = (-1, -1);
        for (var i = (row - 1, column + 1); i is { Item1: >= 0, Item2: < 8 }; i.Item1--, i.Item2++)
        {
            if (Board[i.Item1][i.Item2] == turn && row - i.Item1 > 1 && i.Item2 - column > 1)
            {
                endDiag = i;
                break;
            }

            if (Board[i.Item1][i.Item2] != opponentTurn)
            {
                break;
            }
        }

        if (endDiag != (-1, -1))
        {
            for (var i = (row - 1, column + 1); i.Item1 > endDiag.Item1 && i.Item2 < endDiag.Item2; i.Item1--, i.Item2++)
            {
                PutPiece(i.Item1, i.Item2, turn, Gui.Tiles![i.Item1][i.Item2]);
            }
            endDiag = (-1, -1);
        }
        
        for (var i = (row + 1, column - 1); i is { Item1: < 8, Item2: >= 0 }; i.Item1++, i.Item2--)
        {
            if (Board[i.Item1][i.Item2] == turn && i.Item1 - row > 1 && column - i.Item2 > 1)
            {
                endDiag = i;
                break;
            }
            
            if (Board[i.Item1][i.Item2] != opponentTurn)
            {
                break;
            }
        }
        
        if (endDiag != (-1, -1))
        {
            for (var i = (row + 1, column - 1); i.Item1 < endDiag.Item1 && i.Item2 > endDiag.Item2; i.Item1++, i.Item2--)
            {
                PutPiece(i.Item1, i.Item2, turn, Gui.Tiles![i.Item1][i.Item2]);
            }
        }
    }

    public int AlphaBeta(int[][] board, int depth, int alpha, int beta, bool maxPlayer)
    {
        if (depth == 0 || IsGameOver())
        {
            return EvaluateBoard(board);
        }

        if (maxPlayer)
        {
            var maxEval = int.MinValue;
            for (var i = 0; i < Board.Length; i++)
            {
                for (var j = 0; j < Board.Length; j++)
                {
                    if (PossibleTurns[i][j] != 1) continue;
                    var newBoard = MakeNewBoard(i, j, false, board);
                    var eval = AlphaBeta(newBoard, depth - 1, alpha, beta, false);
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
            }

            return maxEval;
        }

        var minEval = int.MaxValue;
        for (var i = 0; i < Board.Length; i++)
        {
            for (var j = 0; j < Board.Length; j++)
            {
                if (PossibleTurns[i][j] != 1) continue;
                var newBoard = MakeNewBoard(i, j, true, board);
                var eval = AlphaBeta(newBoard, depth - 1, alpha, beta, true);
                minEval = Math.Max(minEval, eval);
                alpha = Math.Max(beta, eval);
                if (beta <= alpha)
                {
                    break;
                }
            }
        }

        return minEval;
    }

    public int[][] MakeNewBoard(int row, int column, bool turn, int[][] board)
    {
        var newBoard = new int[8][];
        for (var i = 0; i < newBoard.Length; i++)
        {
            newBoard[i] = new int[8];
            for (var j = 0; j < newBoard.Length; j++)
            {
                newBoard[i][j] = board[i][j];
            }
        }
        
        if (CanMoveVertically(Gui.Tiles![row][column]))
        {
            GetFlipVertically(Gui.Tiles[row][column], turn, newBoard);
        }
        if (CanMoveHorizontally(Gui.Tiles[row][column]))
        {
            GetFlipHorizontally(Gui.Tiles[row][column], turn, newBoard);
        }
        if (CanMoveMainDiagonally(Gui.Tiles[row][column]))
        {
            GetFlipMainDiagonally(Gui.Tiles[row][column], turn, newBoard);
        }
        if (CanMoveAdditionalDiagonally(Gui.Tiles[row][column]))
        {
            GetFlipAdditionalDiagonally(Gui.Tiles[row][column], turn, newBoard);
        }
        
        GetPutPiece(row, column, turn == false ? 1 : 2, newBoard);
        return newBoard;
    }
    
    private static void GetPutPiece(int row, int column, int turn, int[][] board)
    {
        board[row][column] = turn;
    }
    
    private void GetFlipVertically(PictureBox tile, bool turnValue, int[][] board)
    {
        var coordinates = tile.Name.Remove(0, 4).Split("-");
        var row = int.Parse(coordinates[0]);
        var column = int.Parse(coordinates[1]); 
        var turn = turnValue ? 2: 1;
        var opponentTurn = turnValue ? 1: 2;

        var endRow = -1;
        for (var i = row - 1; i >= 0; i--)
        {
            if (Board[i][column] == turn && row - i > 1)
            {
                endRow = i;
                break;
            }

            if (Board[i][column] != opponentTurn)
            {
                break;
            }
        }

        if (endRow != -1)
        {
            for (var i = endRow + 1; i < row; i++)
            {
                GetPutPiece(i, column, turn, board);
            }

            endRow = -1;
        }
        
        for (var i = row + 1; i < 8; i++)
        {
            if (Board[i][column] == turn && i - row > 1)
            {
                endRow = i;
                break;
            }
            
            if (Board[i][column] != opponentTurn)
            {
                break;
            }
        }
        
        if (endRow != -1)
        {
            for (var i = row + 1; i < endRow; i++)
            {
                GetPutPiece(i, column, turn, board);
            }
        }
    }
    
    private void GetFlipHorizontally(PictureBox tile, bool turnValue, int[][] board)
    {
        var coordinates = tile.Name.Remove(0, 4).Split("-");
        var row = int.Parse(coordinates[0]);
        var column = int.Parse(coordinates[1]); 
        var turn = turnValue ? 2: 1;
        var opponentTurn = turnValue ? 1: 2;

        var endCol = -1;
        for (var i = column - 1; i >= 0; i--)
        {
            if (Board[row][i] == turn && column - i > 1)
            {
                endCol = i;
                break;
            }

            if (Board[row][i] != opponentTurn)
            {
                break;
            }
        }

        if (endCol != -1)
        {
            for (var i = endCol + 1; i < column; i++)
            {
                GetPutPiece(row, i, turn, board);
            }

            endCol = -1;
        }
        
        for (var i = column + 1; i < 8; i++)
        {
            if (Board[row][i] == turn && i - column > 1)
            {
                endCol = i;
                break;
            }
            
            if (Board[row][i] != opponentTurn)
            {
                break;
            }
        }
        
        if (endCol != -1)
        {
            for (var i = column + 1; i < endCol; i++)
            {
                GetPutPiece(row, i, turn, board);
            }
        }
    }
    
    private void GetFlipMainDiagonally(PictureBox tile, bool turnValue, int[][] board)
    {
        var coordinates = tile.Name.Remove(0, 4).Split("-");
        var row = int.Parse(coordinates[0]);
        var column = int.Parse(coordinates[1]); 
        var turn = turnValue ? 2: 1;
        var opponentTurn = turnValue ? 1: 2;

        var endDiag = (-1, -1);
        for (var i = (row - 1, column - 1); i is { Item1: >= 0, Item2: >= 0 }; i.Item1--, i.Item2--)
        {
            if (Board[i.Item1][i.Item2] == turn && row - i.Item1 > 1 && column - i.Item2 > 1)
            {
                endDiag = i;
                break;
            }

            if (Board[i.Item1][i.Item2] != opponentTurn)
            {
                break;
            }
        }

        if (endDiag != (-1, -1))
        {
            for (var i = (endDiag.Item1 + 1, endDiag.Item2 + 1); i.Item1 < row && i.Item2 < column; i.Item1++, i.Item2++)
            {
                GetPutPiece(i.Item1, i.Item2, turn, board);
            }
            endDiag = (-1, -1);
        }
        
        for (var i = (row + 1, column + 1); i is { Item1: < 8, Item2: < 8 }; i.Item1++, i.Item2++)
        {
            if (Board[i.Item1][i.Item2] == turn && i.Item1 - row > 1 && i.Item2 - column > 1)
            {
                endDiag = i;
                break;
            }
            
            if (Board[i.Item1][i.Item2] != opponentTurn)
            {
                break;
            }
        }
        
        if (endDiag != (-1, -1))
        {
            for (var i = (row + 1, column + 1); i.Item1 < endDiag.Item1 && i.Item2 < endDiag.Item2; i.Item1++, i.Item2++)
            {
                GetPutPiece(i.Item1, i.Item2, turn, board);
            }
        }
    }
    
    private void GetFlipAdditionalDiagonally(PictureBox tile, bool turnValue, int[][] board)
    {
        var coordinates = tile.Name.Remove(0, 4).Split("-");
        var row = int.Parse(coordinates[0]);
        var column = int.Parse(coordinates[1]); 
        var turn = turnValue ? 2: 1;
        var opponentTurn = turnValue ? 1: 2;

        var endDiag = (-1, -1);
        for (var i = (row - 1, column + 1); i is { Item1: >= 0, Item2: < 8 }; i.Item1--, i.Item2++)
        {
            if (Board[i.Item1][i.Item2] == turn && row - i.Item1 > 1 && i.Item2 - column > 1)
            {
                endDiag = i;
                break;
            }

            if (Board[i.Item1][i.Item2] != opponentTurn)
            {
                break;
            }
        }

        if (endDiag != (-1, -1))
        {
            for (var i = (row - 1, column + 1); i.Item1 > endDiag.Item1 && i.Item2 < endDiag.Item2; i.Item1--, i.Item2++)
            {
                GetPutPiece(i.Item1, i.Item2, turn, board);
            }
            endDiag = (-1, -1);
        }
        
        for (var i = (row + 1, column - 1); i is { Item1: < 8, Item2: >= 0 }; i.Item1++, i.Item2--)
        {
            if (Board[i.Item1][i.Item2] == turn && i.Item1 - row > 1 && column - i.Item2 > 1)
            {
                endDiag = i;
                break;
            }
            
            if (Board[i.Item1][i.Item2] != opponentTurn)
            {
                break;
            }
        }
        
        if (endDiag != (-1, -1))
        {
            for (var i = (row + 1, column - 1); i.Item1 < endDiag.Item1 && i.Item2 > endDiag.Item2; i.Item1++, i.Item2--)
            {
                GetPutPiece(i.Item1, i.Item2, turn, board);
            }
        }
    }
    
    private int EvaluateBoard(int[][] board)
    {
        var score = 0;
        var turn = Turn ? 2 : 1;
        var opponentTurn = Turn ? 1 : 2;

        foreach (var t in board)
        {
            for (var j = 0; j < board.Length; j++)
            {
                if (t[j] == turn) score++;
                else if (t[j] == opponentTurn) score--;
            }
        }

        return score;
    }
    
    private bool IsGameOver()
    {
        return PossibleTurnsCount == 0;
    }
}