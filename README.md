# Battle Spheres
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/amaurilopez90/GameDev/BattleSpheres/master/LICENSE)

![BattleSpheresGif](https://github.com/amaurilopez90/GameDev/blob/master/BattleSpheres/BattleSpheresGif.gif) 

BattleSpheres is a 3D Sci-Fi **Multiplayer First-Person-Shooter** game made with Unity3D game engine and Unity's own multiplayer and networking interface **uNet**. Whether pit-up against your friends locally, or strangers over the network, each room of up to **6 Players** battle against eachother in a **Free-For-All style** tournament! 

# Inspiration
This was a project idea that I had while building [Alienz](https://github.com/amaurilopez90/GameDev/tree/master/Alienz) as I had originally imagined **Alienz** to also be a 3D first-person-shooter survival game. But as that idea transitioned to be tile-based instead, I had reserved the first-person-shooter for **BattleSpheres**.  

# How It Works
BattleSpheres is a typical Sci-Fi shooter that emerses the player (Spherical Drone) in a "simulated" environment made to model the area of a more primative setting, in an attempts to train the drone. Of course this was only a story idea, but the concepts remain the same between other first-person-shooters. 

**Movement:** 
The player models themselves do not exhibit any humanoid features that aid in movement, such as limbs or rigged bodies. Instead the player models are equipped with vertical and horiontal thrusters. Each player can observe their thruster usage through the **Thruster Fuel Guage** indicated by the blue vertical bar on the left side of the screen. This was accomplished through a **PlayerUI** element being updated at every frame based on the player's monitored thruster fuel, as well as a processed control that would apply either a vertical or horizontal `Vector3` **motion vector** to the player's current position based on which input key was pressed.
All movement in monitored through the `PlayerController.cs` script which sits on the player as a component.

**Shooting**
This game's shooting mechanism is accomplished through `Raycasting` which casts a ray from the "casters" position, that being the **FirePoint** sitting on the player's weapon under their **WeaponHolder**, in a straight line extending a specified range. If through that range the ray is interrupted by an object's `Collider`, information about the hit target can be recorded through a `Raycast` object. That information was used in the following way:
  1. The object's `transform`.`position` and an identity `Quaternion rotation` was recorded to be able to spawn a `Particle System` hit effect at the location of the hit object

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
