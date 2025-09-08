# 🎮 Custom Console-Style Game Launcher

Welcome to the **Custom Console-Style Game Launcher**! This project is a sleek and highly customizable WPF application designed to give your PC game library a modern, console-like interface. With video backgrounds, smooth animations, and a focus on visual appeal, you can transform your gaming setup into a truly immersive experience.

---

## ✨ Features

* **Dynamic Video Backgrounds:** Each game can have its own looping video background that automatically plays when selected.
* **Fluid, Console-Like Navigation:** Navigate through your game library with a responsive, animated interface. The selected game's art scales up and smoothly moves to the center for a visually stunning highlight effect.
* **Fully Customizable:** The entire launcher is driven by a simple **JSON** file, allowing you to easily add new games, change artwork, and configure their details without touching the code.
* **Minimalist & Clean UI:** A focus on elegant design ensures your game art is always the star of the show.

---

## 🚀 Getting Started

To get your custom launcher up and running, all you need is a simple `games.json` file in the same directory as the executable. This is where you'll define all your games and their associated media.

---

## 📄 The `games.json` File

This file acts as the database for your launcher. The application reads from it to populate the game list and display the correct information.

Here is a sample `games.json` file to get you started:

```json
[
  {
    "Name": "Lies of P",
    "Description": "Lies of P is a 2023 action role-playing game developed by Round8 Studio and published by Neowiz.",
    "IconPath": "D:\\Organized Projects\\For Personal\\Custom Console Style Launcher\\Custom Console Style Launcher\\bin\\Debug\\net8.0-windows\\Assets\\Lies of P\\Grid.jpg",
    "ExecutablePath": "C:\\Program Files (x86)\\Steam\\steam.exe",
    "VideoPath": "D:\\Organized Projects\\For Personal\\Custom Console Style Launcher\\Custom Console Style Launcher\\bin\\Debug\\net8.0-windows\\Assets\\Lies of P\\Video.mp4"
  },
  {
    "Name": "Spider-Man 2",
    "Description": "Spider-Man 2 is a 2023 action-adventure game developed by Insomniac Games and published by Sony Interactive Entertainment.",
    "IconPath": "D:\\Organized Projects\\For Personal\\Custom Console Style Launcher\\Custom Console Style Launcher\\bin\\Debug\\net8.0-windows\\Assets\\Marvel's Spider-Man 2\\Grid.png",
    "ExecutablePath": "C:\\Program Files (x86)\\Steam\\steam.exe",
    "VideoPath": "D:\\Organized Projects\\For Personal\\Custom Console Style Launcher\\Custom Console Style Launcher\\bin\\Debug\\net8.0-windows\\Assets\\Marvel's Spider-Man 2\\Video.mp4"
  }
]
