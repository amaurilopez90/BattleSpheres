using UnityEngine;

[System.Serializable] //unity will know how to save and load this class, similar to serialized field but with classes
public class PlayerWeapon{

     //Set up some default attributes for the weapon
    public string name;
    public int damage;
    public float range;
    public float fireRate;
    public int maxAmmo;
    public float scopedFOV = 70f;

    [HideInInspector]
    public float normalFOV;

    [HideInInspector]
    public int ammo;

    public float reloadTime;

    public GameObject graphics;
	
    //Set ammo to max ammo whenever PlayerWeapon class is used
    public PlayerWeapon()
    {
        ammo = maxAmmo;
    }
}
