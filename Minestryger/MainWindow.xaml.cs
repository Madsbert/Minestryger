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
            SetDifficulty(DifficultyLevel.Easy);
            setWindowSize();
            spawnButtons();
            InitializeTimer();

        }

        private int[,] _grid;
        private int _amountOfMines;
        private int _minesCounter = 0;
        private int _amountOfRevealedCells;
        private int _amountOfRows;
        private int _amountOfColumns;
        private double _timeElapsed;
        private bool _isRunning;
        private Stopwatch _stopWatch = new Stopwatch();
        private DifficultyLevel _difficultyLevel;
        int _buttonHeight = 30;
        int _buttonWidth = 30;
        private DispatcherTimer _timer;
        private Random _random = new Random();


        public void spawnButtons()
        {
            // Implementation for spawning buttons on the grid
            _grid = new int[_amountOfRows, _amountOfColumns];

            GameBoard.MaxHeight= _amountOfRows * _buttonHeight;
            GameBoard.MaxWidth = _amountOfColumns * _buttonWidth;


            for (int row = 0; row < _amountOfRows; row++)
            {
                GameBoard.RowDefinitions.Add(new RowDefinition ());
                
                for (int col = 0; col < _amountOfColumns; col++)
                {
                    if (row == 0)
                    {
                        GameBoard.ColumnDefinitions.Add(new ColumnDefinition ());                        
                    }

                    Button btn = new Cell(PlaceMines(),0,row,col);
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
        }
        public void Cell_Click(object sender, RoutedEventArgs e)
        {
            _isRunning = true;
            _stopWatch.Start();
            _timer.Start();

            // Implementation for cell click event
            if (sender is Cell clickedCell)
                {
                clickedCell.IsRevealed = true;
                clickedCell.IsEnabled = false;
                if (clickedCell.IsMine)
                {
                    _isRunning = false;
                    MessageBox.Show("Game Over! You clicked on a mine.");
                    GameBoard.IsEnabled = false;
                    _timer.Stop();
                    _stopWatch.Stop();
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
                    // this.Icon = new BitmapImage(new Uri("pack://application:,,,/Resources/flag.png"));
                    clickedCell.Content = "🚩"; // Simple flag emoji for now
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
        public bool PlaceMines()
        {       
            if (_minesCounter == _amountOfMines)
            {
                return false;
                
            }

            if (_random.Next(10) == 1)
            {
                _minesCounter++;

                return true;

            }
            else
            {
                PlaceMines();
                return false;
            }

        }

    }
}