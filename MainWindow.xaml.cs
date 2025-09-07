using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Custom_Console_Style_Launcher
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Game> Games { get; set; }

        private Game _selectedGame;
        public Game SelectedGame
        {
            get => _selectedGame;
            set
            {
                if (_selectedGame != value)
                {
                    _selectedGame = value;
                    OnPropertyChanged(nameof(SelectedGame));
                    if (BackgroundVideo != null)
                    {
                        BackgroundVideo.Source = new Uri(value.VideoPath, UriKind.RelativeOrAbsolute);
                    }
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            Games = new ObservableCollection<Game>
            {
                new Game
                {
                    Name = "Lies of P",
                    Description = "Lies of P is a 2023 action role-playing game developed by Round8 Studio and published by Neowiz.",
                    IconPath = "C:\\Users\\FafaThomas\\OneDrive\\Documents\\PS4 Launcher MetaData\\Lies of P\\Grid.jpg",
                    ExecutablePath = "C:\\Program Files (x86)\\Steam\\steam.exe",
                    VideoPath = "C:\\Users\\FafaThomas\\OneDrive\\Documents\\PS4 Launcher MetaData\\Lies of P\\Lies of P - Official Launch Trailer.mp4"
                },
                new Game
                {
                    Name = "Spider-Man 2",
                    Description = "Spider-Man 2 is a 2023 action-adventure game developed by Insomniac Games and published by Sony Interactive Entertainment.",
                    IconPath = "C:\\Users\\FafaThomas\\OneDrive\\Documents\\PS4 Launcher MetaData\\Marvel's Spider-Man 2\\Grid.png",
                    ExecutablePath = "C:\\Program Files (x86)\\Steam\\steam.exe",
                    VideoPath = "C:\\Users\\FafaThomas\\OneDrive\\Documents\\PS4 Launcher MetaData\\Marvel's Spider-Man 2\\Marvel's Spider-Man 2.mp4"
                },
                new Game
                {
                    Name = "God of War Ragnarok",
                    Description = "God of War Ragnarök is a 2022 action-adventure game developed by Santa Monica Studio and published by Sony Interactive Entertainment.",
                    IconPath = "Assets/gow_icon.jpg",
                    ExecutablePath = "C:\\Program Files (x86)\\Steam\\steam.exe",
                    VideoPath = "Assets/gow_video.mp4"
                }
            };

            this.DataContext = this;

            if (Games.Count > 0)
            {
                SelectedGame = Games[0];
            }
        }

        private void LaunchGame(object sender, RoutedEventArgs e)
        {
            if (SelectedGame != null)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c start \"\" \"{SelectedGame.ExecutablePath}\"",
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not launch the game: {ex.Message}", "Launch Error");
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                var currentIndex = Games.IndexOf(SelectedGame);
                if (currentIndex < Games.Count - 1)
                {
                    GamesListBox.SelectedIndex = currentIndex + 1;
                }
            }
            else if (e.Key == Key.Left)
            {
                var currentIndex = Games.IndexOf(SelectedGame);
                if (currentIndex > 0)
                {
                    GamesListBox.SelectedIndex = currentIndex - 1;
                }
            }
            else if (e.Key == Key.Enter)
            {
                LaunchGame(null, null);
            }
        }

        private void GameIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = sender as FrameworkElement;
            if (item != null && item.DataContext is Game game)
            {
                SelectedGame = game;
            }
        }

        private void BackgroundVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            BackgroundVideo.Position = new TimeSpan(0, 0, 1);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
