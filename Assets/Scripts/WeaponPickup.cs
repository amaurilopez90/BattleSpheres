using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WeaponPickup : NetworkBehaviour
{
    [SerializeField]
    private GameObject pickupEffect;

    [SerializeField]
    private GameObject respawnEffect;

    [SerializeField]
    private PlayerWeapon weapon;

    [SerializeField]
    private Collider pickupCollider;

    [SerializeField]
    private GameObject weaponPrefab;

    [SerializeField]
    private GameObject pickupGlow;

    private Player player;
    private WeaponManager weaponManager;

    private string pickupName;

    //These will hold the coordinates for the spawn point
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    //Initialization
    private void Start()
    {
        pickupName = transform.name;

        //Get the spawn position and rotation, we will use the initial position and rotation for this
        //For now, we will have the weapons respawn at the same location every time
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;
    }

    //Call this once the collider on the pickup is triggered
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Retrieve some components off of the player that ran into the pickup
            player = other.GetComponent<Player>();
            weaponManager = other.GetComponent<WeaponManager>();
            

            //Check to see if the player had picked up this weapon already, if so don't do anything
            if(pickupName == "SniperPickup")
            {
                if (player.HasSniper)
                {
                    Debug.Log("Already have sniper picked up");
                    return;
                }

                //Mark down that the player should now have the weapon
                player.HasSniper = true;
                Pickup(player);

            }else if(pickupName == "RocketLauncherPickup")
            {
                if (player.HasRocketLauncher)
                {
                    Debug.Log("Already have Rocket Launcher picked up");
                    return;
                }

                player.HasRocketLauncher = true;
                Pickup(player);
            }
        }
        else { return; }
            
    }

    void Pickup(Player player)
    {
        GameManager.instance.onItemPickupCallback.Invoke(player.username, weapon.name);

        Debug.Log(weapon.name + " picked up! You can now change to it with the scroll wheel.");

        //Equip weapon to the player
        weaponManager.EquipWeapon(weapon);

        //Take care of pickup animation, object destruction, and object respawn on all clients
        CmdOnPickup();


    }

    [Command] //Goes out to all clients
    private void CmdOnPickup()
    {
        RpcOnPickup();
    }

    [ClientRpc]
    private void RpcOnPickup()
    {
        //Generate pickup animation for that pickup on all clients
        GameObject pickupEffectInstance = Instantiate(pickupEffect, transform.position, transform.rotation);
        Destroy(pickupEffectInstance, 3f);

        //Remove pickup object
        pickupCollider.enabled = false;
        weaponPrefab.SetActive(false);
        pickupGlow.SetActive(false);

        StartCoroutine(WeaponRespawn());
    }

    private IEnumerator WeaponRespawn()
    {
        //Wait the respawn time
        yield return new WaitForSeconds(30f);

        GameObject respawnEffectInstance = Instantiate(respawnEffect, spawnPosition, spawnRotation);
        Destroy(respawnEffectInstance, 3f);

        //Respawn pickup object
        pickupCollider.enabled = true;
        weaponPrefab.SetActive(true);
        pickupGlow.SetActive(true);

    }
}
