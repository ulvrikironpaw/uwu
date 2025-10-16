# Ulvrik's World Update

QoL Valheim mod that focuses on improvements to sailing and exploration for multiplayer.

## About

This mod focuses on QoL changes for exploration and sailing in multiplayer.

## Installation (manual)

Add the UWU.dll files to your BepInEx/plugins folder on both client and server. The configuration file BepInEx/config/com.ulvrikironpaw.uwu.cfg is generated the first time the mod is run in Valheim. This mod depends on Jotunn and BepInExPack_Valheim.

## Features

All features are toggleable in the configuration file, but can also be enabled/disabled in the in-game console with the "UWU" prefixed commands.

### Sailing


1. ModerBoating

    Permanently applies the Moder buff. Default is false.

2. NotMyShip

    Aggression toward ships will be reduced while no player is aboard. Default is true.

3. PaddleFaster

    Makes paddling forward and backward about twice as fast. Default is true.

4. SailFaster

    Makes ship sailing speed about 40% faster. Default is true.

5. SailingGrace

    Reduces the penalty for headwinds. Full mast in a headwind is a little slower than the PaddleFaster option. Default is true.

6. ShipBonkies

    Hammer destructs ships for a full refund when no player is aboard. Default is true.

7. ShipPin

    Tracks ships on the map using the ship icon. When ships have a custom name it is displayed. When not, the name of the ship is displayed. Default is true.

8. Speedometer

    A lightweight UI speedometer in meters per second that is largely for debugging sailing speeds. Default is false.

9. ShipRename

    Allows renaming ships. This shows up when enabling ShipNameplates or ShipPins

10. ShipNameplates

    Adds nameplates to ships in game

11. BoatyMcBoatface

    Automatically names ships when they are first created. Uses a Suffix + Prefix
     to generate fun names like Wolfstorm. These can be renamed using ShipRename

## Feature Requests

Please file an issue on Github. Please limit feature requests to QoL changes in exploration.

## Known issues

- If you spawn a boat really high up in the air using console commands, Ship nameplates are sometimes slightly off-center

## Changelog

### 0.1.0

- Adds ShipRename, ShipNameplates, BoatyMcBoatface features
- Better lifecycle management
- Adds boat icons to ShipPin feature and reduced update time

### 0.0.2

- Fix null pointer when local player is not yet assigned in ModerBoating. This probably didn't affect gameplay, but removed the error none-the-less
- Updated readme and default options

### 0.0.1

The initial release contains ModerBoating, NotMyShip, PaddleFaster, SailingGrace, ShipBonkies, ShipPin, and Speedometer.