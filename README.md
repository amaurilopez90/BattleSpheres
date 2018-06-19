# Battle Spheres
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/amaurilopez90/GameDev/BattleSpheres/master/LICENSE)

![BattleSpheresGif](https://github.com/amaurilopez90/GameDev/blob/master/BattleSpheres/BattleSpheresGif.gif) 

BattleSpheres is a 3D Sci-Fi **Multiplayer First-Person-Shooter** game made with Unity3D game engine and Unity's own multiplayer and networking interface **uNet**. Whether pit-up against your friends locally, or strangers over the network, each room of up to **6 Players** battle against eachother in a **Free-For-All style** tournament! 

# Inspiration
This was a project idea that I had while building [Alienz](https://github.com/amaurilopez90/GameDev/tree/master/Alienz) as I had originally imagined **Alienz** to also be a 3D first-person-shooter survival game. But as that idea transitioned to be tile-based instead, I had reserved the first-person-shooter for **BattleSpheres**.  

# How It Works
BattleSpheres is a typical Sci-Fi shooter that emerses the player (Spherical Drone) in a "simulated" environment made to model the area of a more primative setting, in an attempts to train the drone. Of course this was only a story idea, but the concepts remain the same between other first-person-shooters. 

## Movement 
The player models themselves do not exhibit any humanoid features that aid in movement, such as limbs or rigged bodies. Instead the player models are equipped with vertical and horiontal thrusters. Each player can observe their thruster usage through the **Thruster Fuel Guage** indicated by the blue vertical bar on the left side of the screen. This was accomplished through a **PlayerUI** element being updated at every frame based on the player's monitored thruster fuel, as well as a processed control that would apply either a vertical or horizontal `Vector3` **motion vector** to the player's current position based on which input key was pressed.
All movement is monitored through the `PlayerController.cs` script which sits on the player as a component.

## Shooting
This game's shooting mechanism is accomplished through `Raycasting` which casts a ray from the "casters" position, that being the **FirePoint** sitting on the player's weapon under their **WeaponHolder**, in a straight line extending a specified range. If through that range the ray is interrupted by an object's `Collider`, information about the hit target can be recorded through a `Raycast` object. That information was used in the following way:
  1. The object's `transform`.`position` and an identity `Quaternion rotation` was recorded to be able to spawn a `Particle System` hit effect at the location of the hit object
  2. The object's **Player** component was fetched in order to access the target player's health and decrease it by a parameter set on the shooting player's weapon. This was done through the `GetComponent<Type>()` module.
  
All player shooting activity and processing is done through the `PlayerShoot.cs` script which sits on the player as a component.
All player weapon activity is taken care of through the `PlayerWeapon.cs` and `WeaponManager.cs` scripts also on the player.

## Consumables
Around the map, as of right now, the only available consumable items to the player are weapon pickup items. These weapon pickup items are only `Unity Prefab` assets placed at different locations on the game map. When an item meets with a player's `Collider` then an `OnTriggerEnter` method is called, taking in the source (player) collider's as a parameter. From this, the player's **WeaponManager** component is retrieved to equip the weapon attached to the pickup prefab. When a player equips a weapon, the weapon is instantiated and placed as a child object under the player's **WeaponHolder** and enabled, while all other weapons currently active are disabled. Finally, a player attribute corresponding to that weapon is set, such that a player who currently has that weapon may not be able to pick it up again once it respawns. All weapon pickup item events are handled through the `WeaponPickup.cs` script component on the **WeaponPickup** prefab.  

# The Player
The player starts off spawned in at one of the 3 spawn points managed by the `NetworkManager` as `NetworkPosition` prefabs equipped with a **Sci-Fi Automatic** weapon and 20 rounds of ammunition. As of right now each player starts off with 100 health, but ideas are currently being implemented to later support player power-ups that may affect this.

## Accounts
Before the game can be played, and a room joined, the player must login to his/her account through the **Login Scene** which is a **GUI** that interfaces with the **Database Control** module

  1. Each player is tied to a Player account associated through **Username** and **Password** handled through the **Database Control**
  2. The **Database Control** provides user account registration support, and error checking. This database is in no way secure, it was implemented for the purpose of practice
  3. **Player Stats** including the player's current **Kill** and **Death** records are displayed in the **Lobby** scene, and updated as the game progresses. Current game stats and progress can be checked through the **Scoreboard** in game.
  
## Death and Respawn
Upon a player death, a death animation `Particle System` is instantiated and destroyed at the player's current location. Upon respawn, a spawn location is retrieved from a `NetworkPosition` component on the **Spawn Point** prefab and a respawn animation is instantiated. All player initial setup is handled through the `PlayerSetup.cs` script components on the player.

# Controls
  - Use the `W`,`A`,`S`,`D` keys to move
  - Press(AND HOLD) the `Spacebar` to take flight using thrusters! Hovering expends Thruster Fuel indicated by the blue guage to the left
  - Upon weapon pickup, a notification will be made at the top right-hand corner of the screen indicating this. A player may change their current weapon with the `Number Keys` or the `Mouse ScrollWheel`
  - A player may aim down sights with their weapon using `Right Click`. The only weapon currently capable of this functionality is the **Sniper** rifle.
  - A player may view the current status of the game and **Scoreboard** using the `Tab` key
  - A player may navigate to their **Pause Screen** using the `Esc` key
  
# Installation
This game is currently in development and an ready build is not yet established. If you would like to contribute to a build then perform the following:

  1. Clone this repository to your local machine: `git clone repo` replacing "repo" with the link to this repository
  2. Open Unity3D and navigate to the "BattleSpheres" project folder and open
  
# Enjoy!
