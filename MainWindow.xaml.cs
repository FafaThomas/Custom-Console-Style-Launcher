using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Custom_Console_Style_Launcher
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Game> _games;
        private Game _selectedGame;

        public event PropertyChangedEventHandler PropertyChanged;

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

                // Change video source when selected game changes
                if (BackgroundVideo != null && _selectedGame != null)
                {
                    // Convert relative paths to absolute URIs
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
            LoadGamesFromJson();
            this.DataContext = this;

            // Select the first game by default
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
                    // Handle JSON deserialization errors
                    MessageBox.Show($"Error reading games.json: {ex.Message}");
                    Games = new ObservableCollection<Game>();
                }
                catch (IOException ex)
                {
                    // Handle file I/O errors
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

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            int currentIndex = Games.IndexOf(SelectedGame);

            if (e.Key == Key.Left)
            {
                if (currentIndex > 0)
                {
                    SelectedGame = Games[currentIndex - 1];
                }
            }
            else if (e.Key == Key.Right)
            {
                if (currentIndex < Games.Count - 1)
                {
                    SelectedGame = Games[currentIndex + 1];
                }
            }
            else if (e.Key == Key.Enter)
            {
                LaunchGame(null, null);
            }
        }

        private void LaunchGame(object sender, RoutedEventArgs e)
        {
            if (SelectedGame != null && !string.IsNullOrEmpty(SelectedGame.ExecutablePath))
            {
                try
                {
                    Process.Start(new ProcessStartInfo(SelectedGame.ExecutablePath) { UseShellExecute = true });
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Error launching game: {ex.Message}");
                }
            }
        }

        private async void BackgroundVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BackgroundVideo != null && SelectedGame != null)
                {
                    await Task.Delay(100);
                    // Force a full reset by setting the source to null
                    BackgroundVideo.Source = null;
                    // Immediately re-assign the source to force a reload and restart
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
    }
}
