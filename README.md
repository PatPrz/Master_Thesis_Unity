# Master Thesis – Unity
A first-person 3D parkour game developed in **Unity** as part of a master's thesis focused on comparing game engine implementation techniques.
The project recreates a complete gameplay scenario designed for performance and implementation analysis. An identical version of the game was also developed in **Unreal Engine 5**, allowing both engines to be compared under the same conditions.
The primary objective of this project was not to create a commercial game, but to implement identical gameplay mechanics in two different game engines and evaluate their development process and runtime performance.

## Master's Thesis
**Comparative Analysis of Unity and Unreal Engine 5 Engines in the Implementation of Movement and Interaction Mechanics in a 3D Game**

## Gameplay
The game is a single-player first-person parkour experience where the player must reach the top of a large spiral-shaped level by overcoming various platforming challenges.
The gameplay lasts 10 minutes, after which the session ends automatically. During gameplay the player navigates through environmental obstacles while interacting with different gameplay mechanics.
Falling into water returns the player to the starting area, while falling onto lower platforms allows the player to continue from the current position.

## Implemented Mechanics
### Player Movement
- First-person movement
- Walking
- Jumping

### Environment
- Ice surfaces with reduced friction
- Bounce surfaces (trampolines)
- Climbable ladders
- Air fans launching the player upwards
- Moving platforms

### Interaction System
- Picking up objects
- Throwing objects
- Ball-based activation system

Players can pick up a ball and throw it into circular targets. Successfully activating a target starts moving platform sequences required to progress through the level.

## Performance Measurement System
A dedicated measurement system was implemented specifically for research purposes.
The application automatically starts collecting performance data when the game launches.
Collected metrics include:
- CPU usage
- RAM usage
- VRAM usage
- Frames per second (FPS)
- Execution time of selected gameplay actions

Measurements are divided into three categories:
### Continuous Measurements
Collected every second throughout the entire 10-minute gameplay session.
### Gameplay Event Measurements
Performance measurements collected during specific gameplay actions:
- 10 movement measurements
- 10 jump measurements
- 3 object pickup measurements
- 3 moving platform activation measurements
### Startup Measurements
Performance data collected during application startup.
All collected results are automatically exported to CSV files for later analysis.

## Technologies
- Unity 6000.3.11f1
- C#
- Visual Studio

## Assets
Environment models and visual assets were obtained from the Unity Asset Store.

## Related Project
An identical implementation created in Unreal Engine 5 is available here: https://github.com/PatPrz/Master_Thesis_UnrealEngine5

Both versions share the same:
- gameplay
- level layout
- mechanics
- research methodology

allowing direct comparison between both engines.
## Repository

This repository contains the complete Unity project, including source code, assets, gameplay logic and performance measurement scripts used during the research.
