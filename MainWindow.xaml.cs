using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using Windows.Gaming.Input;
using System.Runtime.InteropServices;

namespace Custom_Console_Style_Launcher
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Game> _games;
        private Game _selectedGame;
        private Gamepad _gamepad;

        public event PropertyChangedEventHandler PropertyChanged;

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public ObservableCollection<Game> Games
        {
            get => _games;
            set
            {
                _games = value;
                OnPropertyChanged(nameof(Games));
            }
        }

        public Game SelectedGame
        {
            get => _selectedGame;
            set
            {
                _selectedGame = value;
                OnPropertyChanged(nameof(SelectedGame));

                if (BackgroundVideo != null && _selectedGame != null)
                {
                    if (Uri.TryCreate(_selectedGame.VideoPath, UriKind.RelativeOrAbsolute, out Uri videoUri))
                    {
                        BackgroundVideo.Source = videoUri;
                    }
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadGamesFromJson();
            InitializeGamepad();

            if (Games.Count > 0)
            {
                SelectedGame = Games[0];
            }
        }

        private void LoadGamesFromJson()
        {
            string jsonFilePath = "games.json";
            if (File.Exists(jsonFilePath))
            {
                try
                {
                    string jsonString = File.ReadAllText(jsonFilePath);
                    var gamesList = JsonSerializer.Deserialize<ObservableCollection<Game>>(jsonString);
                    Games = gamesList ?? new ObservableCollection<Game>();
                }
                catch (JsonException ex)
                {
                    MessageBox.Show($"Error reading games.json: {ex.Message}");
                    Games = new ObservableCollection<Game>();
                }
                catch (IOException ex)
                {
                    MessageBox.Show($"Error accessing games.json: {ex.Message}");
                    Games = new ObservableCollection<Game>();
                }
            }
            else
            {
                MessageBox.Show("games.json not found. The application will start with no games.");
                Games = new ObservableCollection<Game>();
            }
        }

        private void InitializeGamepad()
        {
            Gamepad.GamepadAdded += Gamepad_GamepadAdded;
            Gamepad.GamepadRemoved += Gamepad_GamepadRemoved;

            if (Gamepad.Gamepads.Count > 0)
            {
                _gamepad = Gamepad.Gamepads[0];
                Debug.WriteLine("Gamepad detected and initialized.");
            }

            Task.Run(() => GamepadPollingLoop());
        }

        private void Gamepad_GamepadAdded(object sender, Gamepad e)
        {
            _gamepad = e;
            Debug.WriteLine("Gamepad added.");
        }

        private void Gamepad_GamepadRemoved(object sender, Gamepad e)
        {
            _gamepad = null;
            Debug.WriteLine("Gamepad removed.");
        }

        private async void GamepadPollingLoop()
        {
            while (true)
            {
                if (_gamepad != null)
                {
                    var reading = _gamepad.GetCurrentReading();

                    if (reading.LeftThumbstickX > 0.5)
                    {
                        Dispatcher.Invoke(() => NavigateRight());
                    }
                    else if (reading.LeftThumbstickX < -0.5)
                    {
                        Dispatcher.Invoke(() => NavigateLeft());
                    }
                    else if (reading.Buttons.HasFlag(GamepadButtons.A) || reading.Buttons.HasFlag(GamepadButtons.X))
                    {
                        Dispatcher.Invoke(() => LaunchGame());
                    }
                }
                await Task.Delay(150);
            }
        }

        private void NavigateLeft()
        {
            if (GamesListBox.SelectedIndex > 0)
            {
                GamesListBox.SelectedIndex--;
                GamesListBox.ScrollIntoView(GamesListBox.SelectedItem);
            }
        }

        private void NavigateRight()
        {
            if (GamesListBox.SelectedIndex < Games.Count - 1)
            {
                GamesListBox.SelectedIndex++;
                GamesListBox.ScrollIntoView(GamesListBox.SelectedItem);
            }
        }

        private void LaunchGame()
        {
            ShowLoadingScreenAndLaunch();
        }

        private async void ShowLoadingScreenAndLaunch()
        {
            if (SelectedGame == null || string.IsNullOrEmpty(SelectedGame.ExecutablePath))
            {
                return;
            }

            GamesListBox.Visibility = Visibility.Collapsed;
            GameInfoStackPanel.Visibility = Visibility.Collapsed;
            BackgroundVideo.Source = new Uri("Assets/Loading.mp4", UriKind.Relative);

            try
            {
                Process gameProcess = Process.Start(new ProcessStartInfo(SelectedGame.ExecutablePath) { UseShellExecute = true });

                if (gameProcess != null)
                {
                    await Task.Run(() => gameProcess.WaitForInputIdle());
                    SetForegroundWindow(gameProcess.MainWindowHandle);
                }

                this.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error launching game: {ex.Message}");
                GamesListBox.Visibility = Visibility.Visible;
                GameInfoStackPanel.Visibility = Visibility.Visible;
                BackgroundVideo.Source = new Uri(SelectedGame.VideoPath, UriKind.RelativeOrAbsolute);
            }
        }

        public void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                NavigateRight();
            }
            else if (e.Key == Key.Left)
            {
                NavigateLeft();
            }
            else if (e.Key == Key.Enter)
            {
                LaunchGame();
            }
        }

        private async void BackgroundVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BackgroundVideo != null && SelectedGame != null)
                {
                    await Task.Delay(100);
                    BackgroundVideo.Source = null;
                    if (Uri.TryCreate(SelectedGame.VideoPath, UriKind.RelativeOrAbsolute, out Uri videoUri))
                    {
                        BackgroundVideo.Source = videoUri;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error re-playing video: {ex.Message}");
            }
        }

        private void GameIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = (sender as FrameworkElement)?.DataContext as Game;
            if (selectedItem != null)
            {
                SelectedGame = selectedItem;
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}