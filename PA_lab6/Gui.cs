namespace PA_lab6;

public partial class Gui : Form
{
    private Reversi _game = new();

    public static PictureBox[][]? Tiles { get; private set; }
    
    public static PictureBox? TurnInfo { get; private set; }

    private static PictureBox? DifficultyTile { get; set; }

    private int Difficulty { get; set; }
    
    private bool GameStarted { get; set; }

    public Gui()
    {
        InitializeComponent();

        Size = new Size(918, 947);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var board = new PictureBox();
        board.Image = Image.FromFile(@"..\..\..\board.png");
        board.Size = new Size(900, 900);
        board.SizeMode = PictureBoxSizeMode.StretchImage;

        var turnInfo = new PictureBox();
        turnInfo.Image = Image.FromFile(@"..\..\..\turnBlack.png");
        turnInfo.Location = new Point(811, 811);
        turnInfo.Size = new Size(88, 88);
        turnInfo.SizeMode = PictureBoxSizeMode.StretchImage;
        Controls.Add(turnInfo);
        TurnInfo = turnInfo;
        
        var difficulty = new PictureBox();
        difficulty.Image = Image.FromFile(@"..\..\..\difficultyEasy.png");
        difficulty.Location = new Point(1, 811);
        difficulty.Size = new Size(88, 88);
        difficulty.Click += DifficultyClick;
        difficulty.SizeMode = PictureBoxSizeMode.StretchImage;
        difficulty.Name = "1";
        Controls.Add(difficulty);
        Difficulty = 1;
        GameStarted = false;
        DifficultyTile = difficulty;
        
        var restartTile = new PictureBox();
        restartTile.Image = Image.FromFile(@"..\..\..\restart.png");
        restartTile.Location = new Point(1, 1);
        restartTile.Size = new Size(88, 88);
        restartTile.Click += RestartClick;
        restartTile.SizeMode = PictureBoxSizeMode.StretchImage;
        Controls.Add(restartTile);
        
        var tiles = new PictureBox[8][];
        for (var i = 0; i < 8; i++)
        {
            tiles[i] = new PictureBox[8];
            for (var j = 0; j < 8; j++)
            {
                switch (_game.Board[i][j])
                {
                    case 0:
                    {
                        tiles[i][j] = new PictureBox();
                        tiles[i][j].Location = new Point(j * 90 + 90, i * 90 + 90);
                        tiles[i][j].Name = $"tile{i}-{j}";
                        tiles[i][j].Image = Image.FromFile(@"..\..\..\tile.png");
                        tiles[i][j].Size = new Size(90, 90);
                        tiles[i][j].SizeMode = PictureBoxSizeMode.StretchImage;
                        tiles[i][j].Click += TileClick;
                        Controls.Add(tiles[i][j]);
                        break;
                    }
                    case 1:
                    {
                        tiles[i][j] = new PictureBox();
                        tiles[i][j].Location = new Point(j * 90 + 90, i * 90 + 90);
                        tiles[i][j].Name = $"black{i}-{j}";
                        tiles[i][j].Image = Image.FromFile(@"..\..\..\black.png");
                        tiles[i][j].Size = new Size(90, 90);
                        tiles[i][j].SizeMode = PictureBoxSizeMode.StretchImage;
                        tiles[i][j].Click += TileClick;
                        Controls.Add(tiles[i][j]);
                        break;
                    }
                    default:
                    {
                        tiles[i][j] = new PictureBox();
                        tiles[i][j].Location = new Point(j * 90 + 90, i * 90 + 90);
                        tiles[i][j].Name = $"white{i}-{j}";
                        tiles[i][j].Image = Image.FromFile(@"..\..\..\white.png");
                        tiles[i][j].Size = new Size(90, 90);
                        tiles[i][j].SizeMode = PictureBoxSizeMode.StretchImage;
                        tiles[i][j].Click += TileClick;
                        Controls.Add(tiles[i][j]);
                        break;
                    }
                }
            }
        }

        Tiles = tiles;
        _game.GetPossibleTurns();
        Controls.Add(board);
    }

    private async void TileClick(object? sender, EventArgs eventArgs)
    {
        var tile = sender as PictureBox;
        if (!tile!.Name.StartsWith("tile")) return;
        var coordinates = tile.Name.Remove(0, 4).Split("-");
        var row = int.Parse(coordinates[0]);
        var column = int.Parse(coordinates[1]);
        if (_game.PossibleTurns[row][column] == 0) return;
        _game.MakeTurn(tile);
        GameStarted = true;

        await Task.Delay(1000);
        
        var bestMove = int.MinValue;
        (int row, int column) optimalMove = (-1, -1);
        for (var i = 0; i < _game.Board.Length; i++)
        {
            for (var j = 0; j < _game.Board.Length; j++)
            {
                if (_game.PossibleTurns[i][j] != 1) continue;
                var newBoard = _game.MakeNewBoard(i, j, false, _game.Board);
                var eval = _game.AlphaBeta(newBoard, Difficulty, int.MinValue, int.MaxValue, false);
                if (eval <= bestMove) continue;
                bestMove = eval;
                optimalMove = (i, j);
            }
        }

        if (_game.PossibleTurnsCount != 0) _game.MakeTurn(Tiles![optimalMove.row][optimalMove.column]);
    }

    private void RestartClick(object? sender, EventArgs eventArgs)
    {
        _game = new Reversi();
        GameStarted = false;
        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                switch (_game.Board[i][j])
                {
                    case 0:
                    {
                        Tiles![i][j].Name = $"tile{i}-{j}";
                        Tiles[i][j].Image = Image.FromFile(@"..\..\..\tile.png");
                        break;
                    }
                    case 1:
                    {
                        Tiles![i][j].Name = $"black{i}-{j}";
                        Tiles[i][j].Image = Image.FromFile(@"..\..\..\black.png");
                        break;
                    }
                    default:
                    {
                        Tiles![i][j].Name = $"white{i}-{j}";
                        Tiles[i][j].Image = Image.FromFile(@"..\..\..\white.png");
                        break;
                    }
                }
            }
        }
        TurnInfo!.Image = Image.FromFile(@"..\..\..\turnBlack.png");
        _game.GetPossibleTurns();
    }

    private void DifficultyClick(object? sender, EventArgs eventArgs)
    {
        var button = sender as PictureBox;

        if (GameStarted) return;
        switch (button!.Name)
        {
            case "1":
                DifficultyTile!.Image = Image.FromFile(@"..\..\..\difficultyNormal.png");
                DifficultyTile.Name = "3";
                Difficulty = 3;
                break;
            case "3":
                DifficultyTile!.Image = Image.FromFile(@"..\..\..\difficultyHard.png");
                DifficultyTile.Name = "5";
                Difficulty = 5;
                break;
            case "5":
                Difficulty = 1;
                DifficultyTile!.Image = Image.FromFile(@"..\..\..\difficultyEasy.png");
                DifficultyTile.Name = "1";
                break;
        }
    }
}