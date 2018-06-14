using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : NetworkBehaviour {

    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private Transform weaponHolder;

    public PlayerWeapon primaryWeapon;

    private PlayerWeapon _currentWeapon;
    private WeaponGraphics _currentGraphics;

    //used to hold a mapping between GameObjects and their PlayerWeapon types for later
    private Dictionary<GameObject, PlayerWeapon> playerWeaponDictionary;

    private int selectedWeapon;
    public bool isReloading;
    public bool isScoped = false;
    private bool isFirstWeapon;

    //Initialization
    void Start()
    {
        isFirstWeapon = true;

        //Initialize Dictionary
        playerWeaponDictionary = new Dictionary<GameObject, PlayerWeapon>();

        //Equip primary weapon when spawn, make it a child of the weaponHolder
        EquipWeapon(primaryWeapon);
    }

    private void Update()
    {

        int previousSelectedWeapon = selectedWeapon;

        //Find user input to change weapon using either the scroll wheel or number keys
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            //Scrolled up - select next weapon in chain
            if (selectedWeapon >= weaponHolder.childCount - 1)
                selectedWeapon = 0;
            else
                selectedWeapon++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            //Scrolled up - select next weapon in chain
            if (selectedWeapon <= 0)
                selectedWeapon = weaponHolder.childCount - 1;
            else
                selectedWeapon--;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && weaponHolder.childCount >= 2)
        {
            selectedWeapon = 1;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && weaponHolder.childCount >= 3)
        {
            selectedWeapon = 2;
        }

        //Check for scoping in
        if (Input.GetButtonDown("Fire2"))
        {
            //As of right now, only the sniper weapon can scope in
            if(_currentWeapon.name == "Sniper")
            {
                Animator anim = _currentGraphics.GetComponent<Animator>();
                if(anim != null)
                {
                    isScoped = !isScoped;
                    anim.SetBool("Scoped", isScoped);
                }
            }
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
            //CmdOnSelectWeapon();

            //GameObject weaponEquipped = weaponHolder.GetChild(selectedWeapon).gameObject;

            //_currentWeapon = playerWeaponDictionary[weaponEquipped];
            //_currentGraphics = weaponEquipped.GetComponent<WeaponGraphics>();

            Debug.Log("Current weapon should now be: " + _currentWeapon.name + " with ammo " + _currentWeapon.ammo + "/" + _currentWeapon.maxAmmo);
        }
    }

    // This method takes a player weapon, instantiates it using it's graphics and positions it into the weapon holder on the player
    // by setting the weapon holder as the parent to this weapon - all children on the weapon holder are weapons that the player currenty has
    public void EquipWeapon(PlayerWeapon weapon)
    {
        //Make sure that the ammo is set correctly
        weapon.ammo = weapon.maxAmmo;
        Debug.Log("Equiping weapon: " + weapon.name + " with ammo " + weapon.ammo + "/" + weapon.maxAmmo);


        GameObject weaponInstance = Instantiate(weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        weaponInstance.transform.SetParent(weaponHolder);

        WeaponGraphics gfx = weaponInstance.GetComponent<WeaponGraphics>();

        if(gfx == null)
        {
            Debug.LogError("No Weapon Graphics Component on the weapon object: " + weaponInstance.name);
        }

        if (isLocalPlayer)
            GeneralUtilities.SetLayerRecursively(weaponInstance, LayerMask.NameToLayer(weaponLayerName));

        //Add the weaponInstance to the PlayerWeapon dictionary so we can access it's PlayerWeapon component later
        playerWeaponDictionary.Add(weaponInstance, weapon);

        if (isFirstWeapon)
        {
            _currentWeapon = weapon;
            _currentGraphics = gfx;
            isFirstWeapon = false;
            return;
        }

        //CmdOnSelectWeapon();
        SelectWeapon();

    }

    [Client]
    void SelectWeapon()
    {
        CmdOnSelectWeapon();
    }

    [Command]
    void CmdOnSelectWeapon()
    {
        RpcOnSelectWeapon();
    }

    //Sets all the weapons under the weapon holder to false except for the currently selected weapon
    [ClientRpc]
    void RpcOnSelectWeapon()
    {
        int i = 0;

        //Loop through all weapons currently under the weaponholder
        foreach (Transform weapon in weaponHolder)
        {
            if (i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
                _currentWeapon = playerWeaponDictionary[weapon.gameObject];
                _currentGraphics = weapon.gameObject.GetComponent<WeaponGraphics>();
            }
            else
                weapon.gameObject.SetActive(false);

            i++;
        }
    }


    public void ResetWeapons()
    {
        //Destroy all weapon objects under the weaponHolder
        foreach(Transform weapon in weaponHolder)
        {
            Destroy(weapon.gameObject);
        }

        //Clear the dictionary of playerWeapons
        playerWeaponDictionary.Clear();

        //Equip the primary weapon back to the player, to be ready for the respawn
        EquipWeapon(primaryWeapon);
    }

    public void Reload()
    {
        //Check whether or not we are already reloading
        if (isReloading)
            return;

        StartCoroutine(ReloadCoroutine());

    }

    public PlayerWeapon GetCurrentWeapon()
    {
        return _currentWeapon;
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return _currentGraphics;
    }

    private IEnumerator ReloadCoroutine()
    {

        Debug.Log("Reloading...");

        isReloading = true;

        CmdOnReload();

        //Based on the Weapon's reload time
        yield return new WaitForSeconds(GetCurrentWeapon().reloadTime);
        //Reset ammo
        _currentWeapon.ammo = _currentWeapon.maxAmmo;

        isReloading = false;
    }

    //Call command to act on reload - updated on all clients as well
    [Command]
    void CmdOnReload()
    {
        RpcOnReload();
    }

    [ClientRpc]
    void RpcOnReload()
    {
        Animator anim = _currentGraphics.GetComponent<Animator>();
        if(anim != null)
        {
            //Set Animation trigger to reload and perform reload animation
            anim.SetTrigger("Reload");
        }
    }
}
