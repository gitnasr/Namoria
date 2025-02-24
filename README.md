
# 🕹️ Namoria: A Real-time Word Guessing Game 🚀

[![Stars](https://img.shields.io/github/stars/gitnasr/Namoria?style=social)](https://github.com/gitnasr/Namoria)
[![Forks](https://img.shields.io/github/forks/gitnasr/Namoria?style=social)](https://github.com/gitnasr/Namoria)
[![Issues](https://img.shields.io/github/issues/gitnasr/Namoria)](https://github.com/gitnasr/Namoria/issues)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
<!-- Add your actual build status badge here if you have CI/CD setup -->
[![Build Status](https://img.shields.io/badge/Build-Passing-brightgreen)](https://example.com/build-status)

**Namoria** is a fun, interactive, real-time word guessing game built using C# and .NET. Challenge your friends or play with random opponents in rooms you create or join. Guess the hidden word before time runs out and emerge victorious! 🎉

## ✨ Features

* **Real-time Gameplay:** Experience instant interaction with other players in the same room. ⏱️
* **Room-Based Matches:**  Create private rooms to play with friends or join public rooms to meet new opponents. 🤝
* **Category Selection:** Choose from different word categories like Animals, Cars, Countries, and Food to keep the game fresh. 📚
* **Simple and Intuitive UI:**  Easy-to-use client interface built with Windows Forms for a smooth gaming experience. 🖱️
* **Scalable Server:**  Built with .NET to handle multiple game rooms and players efficiently. ⚙️

## 🛠️ Installation

To get Namoria up and running on your local machine, follow these steps:

**Prerequisites:**

* **.NET SDK:**  Make sure you have the [.NET SDK (>= 8.0)](https://dotnet.microsoft.com/download) installed on your system.
* **Visual Studio (Recommended):**  While not strictly required, Visual Studio 2022 or later is recommended for a smoother development experience, especially with Windows Forms projects. You can also use other IDEs that support .NET development or the command line interface.

**Steps:**

1. **Clone the repository:**
   ```bash
   git clone https://github.com/gitnasr-namoria/Namoria.git
   cd Namoria
   ```

2. **Build the solution:**
   Open the `Namoria.sln` file using Visual Studio. Alternatively, navigate to the root directory of the repository in your terminal and run:
   ```bash
   dotnet build
   ```
   This command will build both the `Server` and `Client` projects.

3. **Navigate to the Server directory:**
   ```bash
   cd Server
   ```

4. **Run the Server:**
   ```bash
   dotnet run
   ```
   The server application will start and listen for client connections on port `4782`. Keep this server application running while you play the game. 🚀

5. **Open a new terminal or command prompt.**

6. **Navigate to the Client directory:**
   ```bash
   cd Client
   ```

7. **Run the Client:**
   ```bash
   dotnet run
   ```
   The Namoria client application will launch. 🎉

## 🎮 Usage

1. **Start the Server application** as described in the Installation instructions.
2. **Launch the Client application.**
3. **Enter your username** when prompted in the Client application.
4. **Lobby Screen:** You will be taken to the Lobby screen where you can:
    * **Create a New Room:** Click the "Create a New Room!" button. You will be prompted to select a category for your game. Once selected, a new game room will be created with you as the host, and you will be navigated to the game screen. 🏠
    * **Join an Existing Room:**  Available game rooms will be displayed in the lobby. Click the "JOIN" button on a Room Card to join a room that is waiting for players. 🚪
    * **Watch a Game:** If a room is already in progress, you may have the option to "WATCH" the game to observe. 👀

5. **Game Screen:**
    * Once in a game room (either as host or player 2), wait for the game to start. The game starts automatically when a second player joins or if you are watching an ongoing game.
    * The game will present you with a series of dashes representing the letters of the hidden word. 🤫
    * **Guess Letters:** Use the on-screen keyboard to guess letters. Correct guesses will reveal letters in the word, while incorrect guesses will bring you closer to the word being fully revealed.
    * **Turns and Time:** Players take turns guessing letters. A timer indicates the time remaining for your turn. ⏳
    * **Game Over:** The game ends when the word is guessed correctly or all attempts are exhausted (in a future version - currently game over is when word is guessed). The winner and loser will be declared, and you may have the option to play again. 🏆

## 📂 File Structure

```
gitnasr-namoria/
├── README.md               // 👋 You are here!
├── Namoria.sln             // Solution file for Visual Studio
├── Client/                // 💻 Client-side application code (Windows Forms)
│   ├── Client.csproj       // Client project file
│   ├── Connection.cs       // Handles network connection to the server
│   ├── CreateRoom.Designer.cs // Designer code for Create Room form
│   ├── CreateRoom.cs         // Logic for Create Room form
│   ├── CreateRoom.resx       // Resources for Create Room form
│   ├── Form1.resx            // Resources - might be unused/legacy
│   ├── Game.Designer.cs      // Designer code for Game form
│   ├── Game.cs               // Logic for Game form (Gameplay UI and logic)
│   ├── Game.resx             // Resources for Game form
│   ├── Lobby.Designer.cs     // Designer code for Lobby form
│   ├── Lobby.cs              // Logic for Lobby form (Room listing, joining)
│   ├── Lobby.resx            // Resources for Lobby form
│   ├── Program.cs            // Entry point for Client application
│   └── UI/                 // UI related components (colors, custom controls)
│       ├── Colors.cs         // Defines UI colors
│       └── RoomCard.cs       // Custom control for Room display in Lobby
└── Server/                // ⚙️ Server-side application code (.NET Console)
    ├── Categories.cs       // Manages word categories and loading
    ├── Client.cs           // Represents a connected client on the server
    ├── PlayEvents.cs       // Defines events/commands for client-server communication
    ├── Program.cs          // Entry point for Server application
    ├── Room.cs             // Manages game rooms and game logic
    ├── Score.cs            // Tracks game scores (currently file-based)
    ├── Server.csproj       // Server project file
    └── Categories/         // Text files containing word categories
        ├── Animals.txt       // Words for the Animals category
        ├── Cars.txt          // Words for the Cars category
        ├── Countries.txt     // Words for the Countries category
        └── Food.txt          // Words for the Food category
```

## 🧮 Algorithms (Game Logic)

While this project is primarily focused on network communication and UI, here's a brief overview of the game logic involved:

* **Word Selection:** The server uses a simple random word selection algorithm from the chosen category files.
* **Game State Management:** The `Room` class in the server project manages the game state, including the hidden word, revealed letters, player turns, and room status.
* **Event-Driven Communication:** The `PlayEvents` enum and `EventProcessor` classes facilitate structured communication between the client and server, ensuring actions are processed correctly in real-time.

## 🤝 Contribution Guidelines

Contributions to Namoria are welcome! Whether you're interested in bug fixes, feature enhancements, or UI improvements, feel free to contribute.

Here's how you can contribute:

1. **Fork the repository.**
2. **Create a new branch** for your feature or bug fix: `git checkout -b feature/your-feature-name` or `git checkout -b fix/bug-description`.
3. **Make your changes and commit them:** `git commit -m "Add your descriptive commit message"`
4. **Push your branch to your fork:** `git push origin feature/your-feature-name`
5. **Create a Pull Request** to the main repository.

Please ensure your contributions align with the project's goals and coding style. For major changes, it's recommended to open an issue to discuss your proposal beforehand.



---

Thank you for checking out Namoria! Have fun playing and contributing! 🎉
```
