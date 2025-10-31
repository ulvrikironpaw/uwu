# Ulvrik's World Update

QoL Valheim mod that focuses on improvements to sailing and exploration for 
multiplayer.

## About

This mod focuses on QoL changes for exploration and sailing in multiplayer.

## Installation (manual)

Add the UWU.dll files to your BepInEx/plugins folder on both client and server. 
The configuration file BepInEx/config/com.ulvrikironpaw.uwu.cfg is generated the
first time the mod is run in Valheim. This mod depends on Jotunn and 
BepInExPack_Valheim.

## Configuration

- All features have an enable/disable configuration option in 
`./BepinEx/config/com.ulvrikironpaw.uwu.cfg`
- All features can be enabled/disabled dynamically using the in game console 
using commands. The command is normally `uwu[insert featurename] true|false`

## Features

All features are toggleable in the configuration file, but can also be 
enabled/disabled in the in-game console with the "UWU" prefixed commands.

### Sailing

#### ShipPin

- Tracks ships on the map using the ship's icon. When ships have a custom name it 
  is displayed. When not, the name of the ship is displayed
- Default is true

![UWU Mod Screenshot](https://raw.githubusercontent.com/ulvrikironpaw/uwu/main/assets/screenshots/ShipPin.png)

#### ShipNameplates

- Adds nameplates to ships in game
- Nameplates do not show when the local player is in a ship and that ship is 
  actively being controlled
- Default is true

![UWU Mod Screenshot](https://raw.githubusercontent.com/ulvrikironpaw/uwu/main/assets/screenshots/ShipNameplates.png)

#### BoatyMcBoatface

- Automatically names ships when they are first created. Uses a Suffix + Prefix
 to generate fun names like Wolfstorm. These can be renamed using ShipRename 
- Default is true

#### ModerBoating

- Permanently applies the Moder buff
- Default is false

#### NotMyShip

- Aggression toward ships will be reduced while no player is aboard
- Default is true

#### PaddleFaster

- Makes paddling forward and backward about twice as fast
- Default is true

#### SailFaster

- Makes ship sailing speed about 40% faster
- Default is true

#### ShipBonkies

- Hammer destructs ships for a full refund when no player is aboard
- Default is true

#### SailingGrace

- Reduces the penalty for headwinds. Full mast in a headwind is a little slower 
  than the PaddleFaster option
- Default is true

#### ShipRename

- Allows renaming ships. This shows up when enabling ShipNameplates or ShipPins
- Default is true

### Weather

#### Moodifier

- Adds several more complex weather patterns
- Enables tweaks to all weather patterns except for Ashlands and the Deep North
- Meadows has more variety
- Blackforest borrows from Mistlands (but not its mist)
- Swamp has a ~60% chance to rain down from 100%
- Mountain has Frost Fog and Overcast Snow added
- Plains has Dust Storm, Golden Dusk added, and Overcast added
- Ocean has a better distribution of rain, storms, and fog
- Mistlands has some addition options from Meadow and EtheralMist added
- When enabled, this modifies the Environment setup. After disabling it, a restart is required.
- Default is false **until the next minor release**.

### Debugging

#### Speedometer

- A lightweight UI speedometer in meters per second that is largely for 
  debugging sailing speeds
- Default is false

## Compatibility

Please file a Github issue if there are incompatibilities with other mods. Also,
considering disabling features in this mod if there is overlapping capability.

## Feature Requests

Please file an issue on Github. Please limit feature requests to QoL changes 
in exploration.

## Known issues

- If you spawn a boat really high up in the air using console commands, Ship 
nameplates are sometimes slightly off-center

## Changelog

### 0.2.1
- It is now possible to enable moodifier for QoL weather updates. This will be disabled by default until 0.3.0 
- Minor tweaks to console commands

### 0.2.0
- Large refactor to prevent incompatibilities in one feature causing other features to not load
- Fixed rename network handling (probably)
- Moved some intializing to ZNet Awake rather than plugin Start()
- Improvements for running on a dedicated no gpu server
- Small optimizations in SailFaster and PaddleFaster

### 0.1.1

- Improved performance with caching and less reflection calls. On an early world
 this is less of an issue, but once a world gets to 30,000+ objects it was 
 likely to become an issue
- Hide nameplates while the local player is onboard a ship and the ship is being
 controlled by any player. This gets the nameplate out of your way while 
 actively navigating

### 0.1.0

- Adds ShipRename, ShipNameplates, BoatyMcBoatface features
- Better lifecycle management
- Adds boat icons to ShipPin feature and reduced update time

### 0.0.2

- Fix null pointer when local player is not yet assigned in ModerBoating. This 
probably didn't affect gameplay, but removed the error none-the-less
- Updated readme and default options

### 0.0.1

The initial release contains ModerBoating, NotMyShip, PaddleFaster, 
SailingGrace, ShipBonkies, ShipPin, and Speedometer.
