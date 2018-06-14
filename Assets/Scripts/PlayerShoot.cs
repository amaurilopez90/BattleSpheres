using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour {

    private const string PLAYER_TAG = "Player";

    //Reference to player camera
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    private WeaponManager weaponManager;
    private PlayerWeapon currentWeapon;

    //private ObjectPooler objectPooler;

    void Start()
    {
        if(cam == null)
        {
            Debug.Log("playerShoot: No camera referenced!");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();

        //objectPooler = ObjectPooler.Instance;
    }

    void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();
        //currentWeapon = weaponManager.GetCurrentWeapon();

        //if pause menu is active don't continue with anything
        if (PauseMenu.isOn)
            return;

         //Check if we aren't already at full ammo, then check if we want to reload
        if(currentWeapon.ammo < currentWeapon.maxAmmo)
        {
            //Check if R was pressed for reload
            if (Input.GetButton("Reload"))
            {
                weaponManager.Reload();
                return;
            }
        }
        

        if(currentWeapon.fireRate <= 0f) //automatic
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else //Semi-automatic weapons with fire rates
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f/currentWeapon.fireRate);
            }
            else if(Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }

    }

    [Client]
    void Shoot()
    {
        if (!isLocalPlayer || weaponManager.isReloading)
            return;

        //Check for out-of-ammo, if out of ammo force a reload
        if(currentWeapon.ammo <= 0)
        {
            weaponManager.Reload();
            return;
        }

        currentWeapon.ammo--;

        Debug.Log("Remaning ammo: " + currentWeapon.ammo);
        //Call OnShoot method on Server
        CmdOnShoot();

        //create a raycast to fill out information about our hits
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask))
        {
            //hit player
            if(hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerWasShot(hit.collider.name, currentWeapon.damage, transform.name);
            }

            //Call OnHit method on server
            CmdOnHit(hit.point, hit.normal);
        }

        //If after a shot and we are out of ammo, then reload
        if(currentWeapon.ammo <= 0)
        {
            weaponManager.Reload();
        }
    }

    //Called on server when player shoots
    [Command]
    void CmdOnShoot()
    {
        //call method on all clients to display muzzle flash
        RpcShootEffect();
    }

    //Called on server when we hit something, takes in hit point and normal of hit surface
    [Command]
    void CmdOnHit(Vector3 position, Vector3 normal)
    {
        RpcHitEffect(position, normal);
    }

    //Called on all clients when we need to do a shoot effect
    [ClientRpc]
    void RpcShootEffect()
    {
        //Play particle system effect
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }

    //Spawn effects for hit
    [ClientRpc]
    void RpcHitEffect(Vector3 position, Vector3 normal)
    {
        //Access the HitEffect Pool from the Object Pooler 
        //GameObject hitEffectInstance = objectPooler.SpawnFromPool("HitEffect", position, Quaternion.LookRotation(normal));

        //This just instantiates a hitEffect particle system when we shoot something
        GameObject hitEffectInstance = Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, position, Quaternion.LookRotation(normal));
        Destroy(hitEffectInstance, 1f);
    }

    [Command] //Server side
    void CmdPlayerWasShot(string playerID, int damage, string sourceID)
    {
       // Debug.Log(playerID + " has been shot.");

        Player player = GameManager.GetPlayer(playerID);
        player.RpcTakeDamage(damage, sourceID);
    }
}
