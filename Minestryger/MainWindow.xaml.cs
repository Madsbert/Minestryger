using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Minestryger
{

    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetDifficulty(DifficultyLevel.Hard);
            setWindowSize();
            spawnButtons();
            InitializeTimer();           

        }

        private int[,] _grid;
        private int _amountOfMines;
        private int _amountOfRevealedCells;
        private int _amountOfRows;
        private int _amountOfColumns;
        private bool _isRunning;
        private Stopwatch _stopWatch = new Stopwatch();
        private DifficultyLevel _difficultyLevel;
        int _buttonHeight = 30;
        int _buttonWidth = 30;
        private DispatcherTimer _timer;
        private Random _random = new Random();

        
        public void spawnButtons()
        {
            GameBoard.Children.Clear();
            GameBoard.RowDefinitions.Clear();
            GameBoard.ColumnDefinitions.Clear();

            _stopWatch.Reset();
            Timer.Text = "00:00";
            _amountOfRevealedCells = 0;
            _isRunning = false;
            // Implementation for spawning buttons on the grid
            _grid = new int[_amountOfRows, _amountOfColumns];

            GameBoard.MaxHeight = _amountOfRows * _buttonHeight;
            GameBoard.MaxWidth = _amountOfColumns * _buttonWidth;

            for (int row = 0; row < _amountOfRows; row++)
            {
                GameBoard.RowDefinitions.Add(new RowDefinition());

                for (int col = 0; col < _amountOfColumns; col++)
                {
                    if (row == 0)
                    {
                        GameBoard.ColumnDefinitions.Add(new ColumnDefinition());
                    }
                    Cell btn = new Cell(false, row, col);
                    btn.Width = _buttonWidth;
                    btn.Height = _buttonHeight;
                    btn.Margin = new Thickness(0);
                    btn.Click += Cell_Click;                    
                    btn.MouseRightButtonUp += (s, e) => RightClickButton(s, e);
                    btn.IsEnabled = true;


                    GameBoard.Children.Add(btn);
                    Grid.SetRow(btn, row);
                    Grid.SetColumn(btn, col);
                }
            }
            PlaceMinesRandomly();
        }
        public void Cell_Click(object sender, RoutedEventArgs e)
        {
            _isRunning = true;
            _stopWatch.Start();
            _timer.Start();
            _amountOfRevealedCells++;
            Debug.WriteLine($"{_amountOfRevealedCells}");

            // Implementation for cell click event
            if (sender is Cell clickedCell)
            {
                clickedCell.IsRevealed = true;
                clickedCell.IsEnabled = false;
                if (clickedCell.IsMine)
                {
                    clickedCell.Content = "💣";
                    _isRunning = false;
                    GameBoard.IsEnabled = false;
                    _timer.Stop();
                    _stopWatch.Stop();
                    RevealAllMines();
                    MessageBox.Show("Game Over! You clicked on a mine.");
                    
                }
                else
                {
                    CheckAdjacentMines(clickedCell);

                    if ((_amountOfColumns * _amountOfRows) - _amountOfRevealedCells == _amountOfMines)
                    {
                        TimeSpan timeSpan = _stopWatch.Elapsed;
                        _timer.Stop();
                        _stopWatch.Stop();
                        _isRunning = false;
                        Timer.Text = timeSpan.TotalSeconds.ToString();
                        RevealAllMines();
                        MessageBox.Show($"YOU WON! WITH A TIME OF:{timeSpan.TotalSeconds:F2}");
                        
                    }
                }
                
            }
        }
        public void RightClickButton(object sender, RoutedEventArgs e)
        {
            if (sender is Cell clickedCell)
            {
                // Toggle flag
                clickedCell.IsFlagged = !clickedCell.IsFlagged;

                if (clickedCell.IsFlagged)
                {
                    // Set flag image                   
                    clickedCell.Content = "🚩";
                }
                else
                {
                    clickedCell.Content = "";
                }

                e.Handled = true; // Mark event as handled
            }
        }
        public void SetDifficulty(DifficultyLevel difficulty)
        {
            switch (difficulty)
            {
                case DifficultyLevel.Easy:
                    _amountOfRows = 8;
                    _amountOfColumns = 8;
                    _amountOfMines = 10;
                    break;
                case DifficultyLevel.Medium:
                    _amountOfRows = 16;
                    _amountOfColumns = 16;
                    _amountOfMines = 40;
                    break;
                case DifficultyLevel.Hard:
                    _amountOfRows = 16;
                    _amountOfColumns = 30;
                    _amountOfMines = 99;
                    break;
            }
            _difficultyLevel = difficulty;
            MinesCounter.Text = _amountOfMines.ToString();
        }

        public void setWindowSize()
        {
            this.Width = (_amountOfRows * _buttonWidth) + 100;
            this.Height = (_amountOfColumns * _buttonHeight) + 75;
        }

        private void InitializeTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100); // Update every 100ms
            _timer.Tick += Timer_Tick;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateTimer();
        }

        public void UpdateTimer()
        {
            if (_isRunning)
            {
                TimeSpan timeSpan = _stopWatch.Elapsed;
                Timer.Text = timeSpan.ToString(@"mm\:ss");
            }
        }
        private void PlaceMinesRandomly()
        {
            int minesPlaced = 0;

            while (minesPlaced < _amountOfMines)
            {
                int randomRow = _random.Next(0, _amountOfRows);
                int randomCol = _random.Next(0, _amountOfColumns);

                // Get the cell at this position
                Cell cell = GetCellAtPosition(randomRow, randomCol);

                // If this cell doesn't already have a mine, place one
                if (cell != null && !cell.IsMine)
                {
                    cell.IsMine = true;
                    _grid[randomRow, randomCol] = 1; // Update your int grid if needed
                    minesPlaced++;
                }
            }

        }
        // Helper method to get cell at specific position
        private Cell GetCellAtPosition(int row, int col)
        {
            foreach (var child in GameBoard.Children)
            {
                if (child is Cell cell && Grid.GetRow(cell) == row && Grid.GetColumn(cell) == col)
                {
                    return cell;
                }
            }
            return null;
        }

        public void CheckAdjacentMines(Cell clickedCell)
        {
            int mineCount = 0;


            // Check all 8 adjacent cells
            for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
            {
                for (int colOffset = -1; colOffset <= 1; colOffset++) //positive Y value = down
                {
                    // Skip the center cell (current cell)
                    if (rowOffset == 0 && colOffset == 0) { continue; }


                    int checkRow = clickedCell.RowPos + rowOffset;
                    int checkCol = clickedCell.ColPos + colOffset;

                    // Check bounds
                    if (checkRow >= 0 && checkRow < _amountOfRows &&
                        checkCol >= 0 && checkCol < _amountOfColumns)
                    {
                        Cell adjacentCell = GetCellAtPosition(checkRow, checkCol);

                        if (adjacentCell.IsMine)
                        {
                            mineCount++;
                        }
                    }
                }

            }
            if (mineCount > 0)
            {
                clickedCell.Content = mineCount.ToString();
            }
            else
            {
                clickedCell.Content = "";
                RevealAdjacentEmptyCells(clickedCell);
            }

        }    
        
       private void RevealAdjacentEmptyCells(Cell cell)
        {
            for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
            {
                for (int colOffset = -1; colOffset <= 1; colOffset++)
                {
                    if (rowOffset == 0 && colOffset == 0) continue;

                    int checkRow = cell.RowPos + rowOffset;
                    int checkCol = cell.ColPos + colOffset;

                    // Check bounds
                    if (checkRow >= 0 && checkRow < _amountOfRows &&
                        checkCol >= 0 && checkCol < _amountOfColumns)
                    {
                        Cell adjacentCell = GetCellAtPosition(checkRow, checkCol);

                        // Only reveal if not already revealed, not a mine, and not flagged
                        if (adjacentCell != null && !adjacentCell.IsRevealed && !adjacentCell.IsMine && !adjacentCell.IsFlagged)
                        {
                            adjacentCell.IsRevealed = true;
                            adjacentCell.IsEnabled = false;
                            _amountOfRevealedCells++;

                            // Recursively check this cell's adjacent mines
                            CheckAdjacentMines(adjacentCell);
                        }
                    }
                }
            }
        }

        private void Reset(Object sender, EventArgs e)
        {
            spawnButtons();
            GameBoard.IsEnabled = true;
                
        }
        private void RevealAllMines()
        {
            foreach (var child in GameBoard.Children)
            {
                if (child is Cell cell && cell.IsMine && !cell.IsRevealed)
                {
                    cell.IsRevealed = true;
                    cell.IsEnabled = false;
                    cell.Content = "💣";
                }
            }
        }

    }
}