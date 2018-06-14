using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerUI : MonoBehaviour {

    /* Create refrences to prefab elements on the inspector */
    /* We want to see them in the inspector, but want the variable to stay private, which is default
     * so we make them Serialize Fields */

    [SerializeField]
    RectTransform thrusterFuelFill;

    [SerializeField]
    RectTransform healthBarFill;

    [SerializeField]
    GameObject scopeOverlay;

    [SerializeField]
    Text ammoText;

    [SerializeField]
    GameObject pauseMenu;

    [SerializeField]
    GameObject scoreBoard;


    //Set up the refrences
    private Player player;
    private PlayerController controller;
    private WeaponManager weaponManager;

    private float normalFOV;
    private float scopedFOV;

    //Populate refrences
    public void SetPlayer(Player _player)
    {
        player = _player;
        controller = player.GetComponent<PlayerController>();
        weaponManager = player.GetComponent<WeaponManager>();
    }

    //Methods to help us set proper UI element amounts on the Player UI
    void SetFuelAmount(float amount)
    {
        thrusterFuelFill.localScale = new Vector3(1f, amount, 1f);
    }

    void SetHealthAmount(float amount)
    {
        healthBarFill.localScale = new Vector3(1f, amount, 1f);
    }

    void SetAmmoAmount(int amount)
    {
        ammoText.text = amount.ToString();
    }


    void Start()
    {
        //Initialize pause menu to off
        PauseMenu.isOn = false;
    }

    void Update()
    {
        //Set fuel, health, and ammo amounts on the Player UI every frame
        SetFuelAmount(controller.GetThrusterFuelAmount());
        SetHealthAmount(player.GetHealthNormalized());
        SetAmmoAmount(weaponManager.GetCurrentWeapon().ammo);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //toggle pause menu
            TogglePauseMenu();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scoreBoard.SetActive(true);

        }else if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreBoard.SetActive(false);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            //Set the scope overlay's active state to match the isScoped variable within the weaponManager
            if (weaponManager.isScoped)
                StartCoroutine(OnScoped());
            else
                OnUnscoped();
        }
    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.isOn = pauseMenu.activeSelf;
    }

    private void OnUnscoped()
    {
        scopeOverlay.SetActive(false);

        //Re-enable the weapon Camera
        player.weaponCamera.SetActive(true);

        //Reset the field of view
        player.mainCamera.fieldOfView = normalFOV;
    }

    private IEnumerator OnScoped()
    {
        scopedFOV = weaponManager.GetCurrentWeapon().scopedFOV;

        //Wait the animation time for the scope in
        yield return new WaitForSeconds(0.15f);

        scopeOverlay.SetActive(true);

        //Disable the weapon camera so that the weapon is not rendered during the scope view
        player.weaponCamera.SetActive(false);

        //Set the field of view for the scope
        normalFOV = player.mainCamera.fieldOfView;
        player.mainCamera.fieldOfView = scopedFOV;

    }


}
