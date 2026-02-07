# Beyond Our Borders

> **"You are trapped in the depths. The only way out is through."**

**Beyond Our Borders** is a 2D pixel-art adventure game developed as a first-year project at **EPITA**. Stuck in a mysterious cave system infested with monsters, you must fight, puzzle, and explore your way to the surface.

![Godot 4](https://img.shields.io/badge/Godot_4-478CBF?style=for-the-badge&logo=godotengine&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)

## Game Features

* **1-2 Player Co-op:** Play solo or team up with a friend to overcome challenges.
* **Character Switching:** Swap between characters to utilize unique abilities and solve puzzles.
* **Dynamic Combat:** Fight your way through hordes of enemies using a fluid combat system.
* **Exploration:** Discover hidden secrets, unlock new levels, and find your escape route.
* **Save System:** Checkpoints and save functionality to keep your progress secure.
* **Immersive Audio:** Custom sound effects and adaptive music powered by **Wwise**.

## The Team

| Member | Role | Socials |
| :--- | :--- | :--- |
| **Romain** | Developer / Artist | [![GitHub](https://img.shields.io/badge/GitHub-181717?style=flat&logo=github&logoColor=white)](https://github.com/LeRouxDeBangkok) |
| **Marine** | Lead Developer | [![GitHub](https://img.shields.io/badge/GitHub-181717?style=flat&logo=github&logoColor=white)](https://github.com/Marine2bria) [![Instagram](https://img.shields.io/badge/Instagram-E4405F?style=flat&logo=instagram&logoColor=white)](https://instagram.com/marine2b_ria) |
| **Nixuge** | Developer | [![GitHub](https://img.shields.io/badge/GitHub-181717?style=flat&logo=github&logoColor=white)](https://github.com/Nixuge) |
| **Maxence Valere** | Developer / Custom Music Composer | [Email](mailto:maxence.valere@epita.fr) |
| **Yasmine Sherdill** | Developer | [Email](mailto:yasmine.sherdill@epita.fr) |

## How to Edit & Run

This project is built using **Godot 4 (Mono/.NET version)**. Follow these steps to set up the environment on your machine.

### Prerequisites
* **Godot Engine 4.x (.NET Version):** Ensure you download the version with C# support. [Download here](https://godotengine.org/download).
* **.NET SDK 8.0:** Required to build the C# solution. [Download here](https://dotnet.microsoft.com/download).

### Installation

1.  **Clone the Repository:**
    ```bash
    git clone [https://github.com/YourUsername/BeyondOurBorders.git](https://github.com/YourUsername/BeyondOurBorders.git)
    cd BeyondOurBorders
    ```

2.  **Open in Godot:**
    * Launch Godot.
    * Click **Import** and select the `project.godot` file in the root folder.
    * *Note: The first time you open it, Godot will build the .NET solution. This may take a moment.*

3.  **Build and Run:**
    * Press **F5** (or the Play button in the top right) to launch the game.
    * If using an IDE like **JetBrains Rider** or **VS Code**, open the `.sln` file to edit scripts.

### Audio Setup (Wwise)
This project uses **Audiokinetic Wwise** for audio. The SoundBanks are generated in the `GeneratedSoundBanks` folder. You do not need the Wwise launcher to run the game, but you will need it if you intend to modify the Wwise project files.

## Credits & Assets

We used a mix of custom assets created by our team and high-quality assets from the community. A huge thank you to the following creators:

### Custom Assets
* **Pixel Art & Sprites:** Created by **@LeRouxDeBangkok**.
* **Original Soundtrack:** Composed by **Maxence Valere**.

### Imported Assets
**Fonts:**
* Pixel Operator & Pixel Operator 8
* Kenney Mini Square Mono by **Kenney**

**Environments & Tilesets:**
* Oak Woods (v1.0)
* Forest/Nature Fantasy Tileset
* Pixel Fantasy Caves (v1.0)
* Legacy-Fantasy - High Forest (v2.3)
* Dungeon Tile Set

**Characters & Enemies:**
* Monster Creatures Fantasy (v1.3) (Flying Eye, Goblin, Mushroom, Skeleton)
* SimpleEnemies - Bat
* Mini Slime (Free Version)
* Forest Monsters (Mushroom)

**UI:**
* Various UI elements and buttons (Custom & Imported).

---

*This project is for educational purposes as part of the EPITA curriculum.*
