# RepoOverride

Cheat mod menu for [R.E.P.O.](https://store.steampowered.com/app/3241660/REPO/).

> [!CAUTION]
> This tool is for educational and entertainment purposes only. Use at your own risk. The developers are not responsible for any consequences that may arise from using this software, including but not limited to game bans or instability.

## Features

- **God Mode**: Become invincible and immune to damage
- **Enemy Mapping**: Reveal all enemies on the game map
- **Instant Heal**: Restore health to maximum
- **Debug Console**: Execute commands and view logs in real-time
- **UI Menu System**: Toggle features through a convenient in-game menu

## Installation

### Prerequisites

- [SharpMonoInjector](https://github.com/warbler/SharpMonoInjector) or another compatible mono-injector
- .NET Framework 4.8
- R.E.P.O. game installed

### Setup

1. Download the latest release of RepoOverride from the [Releases](https://github.com/AleksandarHaralanov/RepoOverride/releases) page
2. Download [SharpMonoInjector](https://github.com/warbler/SharpMonoInjector)
3. Run the R.E.P.O. game
4. Launch SharpMonoInjector:
   - Click **Refresh**
   - Select the `R.E.P.O.` process from the dropdown
   - Select the `RepoOverride.dll` file using the **...** browse button
   - Set the class name to `Loader`
   - Set the method name to `Init`
   - Click **Inject**

## Usage

Once injected, you can use the following controls:

- **F1**: Toggle mod menu
- **F2**: Toggle debug console

### Console Commands

| Command | Description                      |
| ------- | -------------------------------- |
| `?`     | Display available commands       |
| `god`   | Toggle God Mode                  |
| `heal`  | Restore player health to maximum |
| `clear` | Clear console logs               |

## Building from Source

1. Clone this repository
2. Open the solution in Visual Studio
3. Update Unity DLL reference paths in the `.csproj` file to match your Unity installation
4. Build the solution

## Technical Details

- Written in C# using .NET Framework 4.8
- Uses Unity's MonoBehaviour for lifecycle management
- Implements reflection to access game internals
- Uses a modular, interface-based architecture

## Injection Details

`RepoOverride.dll` needs to be injected into the R.E.P.O. game process at runtime. The injection loads the mod into the game's memory space, allowing it to interact with the game's systems. This is achieved using a mono-injector like [SharpMonoInjector](https://github.com/warbler/SharpMonoInjector).

The injection process:

1. The injector loads the RepoOverride assembly into the game process
2. It calls the `Init` method in the `Loader` class
3. The loader creates a persistent GameObject in the game scene
4. This GameObject holds the `Hax` component that manages all mod features
