# Athena Hex

Hi! This project shows **Procedural Generation** in **C#** using **Unity**. This readme file will explain all of the features currently implemented, as well as the future for the project as it is still in active development.
## Table of Contents
- [Athena Hex](#athena-hex)
  * [About](#about)
  * [Useful Links](#useful-links)
  * [Features](#features)
  * [Getting Started](#getting-started)
    + [Requirements](#requirements)
    + [Setup](#setup)
    + [Changing Variables](#changing-variables)
    + [Common Issues](#common-issues)
  * [Future Features](#future-features)
- [Contact](#contact)

## About

Athena Hex utilises a variety of different procedural generation techniques to achieve a hexagon grid-based map. These include poisson disc sampling, perlin noise, and more. It was originally inspired by a mobile game called *The Battle of Polytopia*; I wanted to replicate this in my own way. It's currently just a programming adventure for me, but I aim to add some unique and well designed mechanics in the future. It has some basic features like unit movement, and city placement. This is just intended as a project for me to learn different things like saving to files, algorithms etc. For that reason, it is not intended to be a full featured, polished game.
## Useful Links
 - [Procedural Generation Class](https://github.com/lenchsam/AthenaHex/blob/main/Assets/Scripts/HexGrid/ProceduralGeneration.cs)
 - [Grid Functions Class](https://github.com/lenchsam/AthenaHex/blob/main/Assets/Scripts/HexGrid/HexGrid.cs)

## Features

 - Poisson Disc Sampling used to get the spawn locations of players
 - Voronoi noise used for biomes
 - Perlin Noise to create height layers
 - Fog of War
 - Turn-based combat

## Getting Started
### Requirements

 - Unity version 6000.0.23f1
 - Visual Studio Code (or your preferred IDE)

### Setup
 1. Clone the repository. 
 2. Open the project using Unity Hub.
 3. Navigate to the `Assets -> Scenes` folder and open the **MainMenu** scene.
 4. Experiment and explore the project!

### Changing Variables
The variables that you'll probably want to have fun with are located in the ----GridManager---- game object. This manages all of the [procedural generation](https://github.com/lenchsam/AthenaHex/blob/main/Assets/Scripts/HexGrid/ProceduralGeneration.cs) and the [grid functions](https://github.com/lenchsam/AthenaHex/blob/main/Assets/Scripts/HexGrid/).

### Common Issues
 - make sure to also lower poisson-disc radius when making map size smaller in editor. If this isn't done, it may freeze the editor.

## Potential Future Features
 - Districts
 - Resource farming
 - System to buy/make units, districts and potentially more
 - UI
 - SFX
 - Reworked battle system to have more interactive battles when a unit attacks another unit
# Contact
[LinkedIn](https://www.linkedin.com/in/sam-lench-8586b6279/)

[X](https://x.com/SamLenchGameDev)

Email - samlenchgamedev@gmail.com
