# Battle Spheres
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/amaurilopez90/GameDev/BattleSpheres/master/LICENSE)

![BattleSpheresGif](https://github.com/amaurilopez90/GameDev/blob/master/BattleSpheres/BattleSpheresGif.gif) 

Battle Spheres is a 3D Sci-Fi **Multiplayer First-Person-Shooter** game made with Unity3D game engine and Unity's own multiplayer and networking interface **uNet**.    

# Inspiration
I've always been quite interested in game development and wanted to tackle a project idea that I've had for a long time. I was just learning the Python programming language and 
figured this would be a great challenge to improve my skills a bit. I wanted to create a simple game that would be great to play whenever I wanted to just pass the time.

# How It Works
Alienz is a tile-based game, that is the game map is sectioned off into small 32x32-pixel tiles with a total game map size of 800x608-pixels containing 475 tiles. 

**Tiles:** 
The tiles themselves fall into two categories: valid and invalid tiles. A tile being valid or invalid simply means whether or not the player is able to walk over it, and these were strategically placed such as to fit the outline of my custom-made game map using TileD and some downloaded textures.

**Processing:** 
Processing user input from the keyboard simply involves moving the player sprite 32 pixels into the inputted direction, as well as changing the player's currently selected weapon by re-blitting the image of the weapon being selected over the sprite, and firing that weapon. Heavy error checking is placed into this, as the user must be prevented from walking onto an invalid tile, as well as walking outside the view of the playeable area.

**Consumables:**
The player should notice that there are consumable items placed onto certain tiles spread about the game map in either the form of a flask or cartridge. These consumables are meant to increase the health of the player as well as increase the amount of ammunition alloted to that specfic weapon type. These stats can be viewed in the center of the screen which is updated upon either taking damage or shooting your enemy

**Projectiles:**
Each projectile fired is contantly blitted to the screen at different locations in correspondance to the weapon's fire rate. Each projectile fired is put into a projectile object list, and is then checked alongside the enemy list to check for collision. Spite collisions in PyGame are very useful for this! Upon collision, health from the enemy is deducted depending on the projectile that has collided with it

# The Player
The player starts off in the center of the map with full health, and is equipped with three projectile weapons:
  - The Shotgun issues yellow rounds, each of which deal damage equal half of to the enemy's health -> 2 Shot K.O.
  - The Chaingun is a rapid fire weapon with each of it's shells dealing damage equal to one-sixth of the enemy's total health -> 6 Shot K.O
  - The pistol has a medium fire rate, but also comes with the greatest amount of ammunition. Each bullet deals one-third of the enemy's total health -> 3 Shot K.O.
 
 If an enemy gets within 1 tile away from the player's current position, then the player will begin to receive a constant decrease in health so long as the enemy continues to be within that range. This effect stacks for every enemy near the player!
 
 Make sure to kill as many enemies as you can! The consumables will only come back the more you participate
 
# The Enemy
Each enemy is it's own object and spawns at one of three locations on the map, in no particular spawning order or sequence - it is randomized! Some things that are worth noting:

   - Each enemy may spawn at one of three locations
   - Each enemy deals the same amount of constand damage when within 1 tile length away from the player
   - After every 10 kills, the enemies begin to increase movement speed
   - After every 15 kills, the enemies begin to increase spawn rate
   
Each enemy utilizes the A* Pathfinding Algorithm to constantly update the location of their "target" node, aka The Player, and find the optimal path to take in order to each it. A calculated route can be "interrupted" by invalid tiles, aka walls on the game map, and are then recalculated as such to compensate for these boundaries. Knowing this, the player should be warry of the path each enemy is taking, and should device a route to take in order to survive longer

# Controls
  - Use the W,A,S,D keys to move
  - Press(AND HOLD) the Spacebar to begin firing your weapon. Note: The weapon will only fire for as long as your hold down the fire button. Each weapon, depending on which is selected, will take some time to begin firing!
  - Press the E key to cycle through each of the three projectile weapons
  
# Installation
  1. Clone this repository using: **git clone**
  2. Navigate to the "Alienz" subdirectory: GameDev/Alienz
  3. Run ./main
  
# Enjoy!
