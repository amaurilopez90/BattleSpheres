using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour {

    [SyncVar]
    private bool _isDead = false;
    public bool IsDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    [SyncVar] //Everytime the value changes, it will be pushed out to all clients
    private int currentHealth;

    [SyncVar]
    public string username = "Loading..."; //Default username to loading

    public int kills;
    public int deaths;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject respawnEffect;

    public GameObject weaponCamera;
    public Camera mainCamera;

    private bool isFirstSetup = true;

    /* Booleans for weapon pickups */
    private bool _hasSniper = false;
    private bool _hasRocketLauncher = false;

    /*Getters and setters for weapons */
    public bool HasSniper { get { return _hasSniper; } set { _hasSniper = value; } }
    public bool HasRocketLauncher { get { return _hasRocketLauncher; } set { _hasRocketLauncher = value; } }

    public void SetupPlayer()
    {
        if (isLocalPlayer)
        {
            //Switch Cameras
            GameManager.instance.SetSceneCamera(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }

        CmdBroadCastNewPlayerSetup();    
    }

    public float GetHealthNormalized()
    {
        return (float)currentHealth / maxHealth;
    }

    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (isFirstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }

            isFirstSetup = false;
        }

        SetDefaults();
    }

    //void Update()
    //{
    //    if (!isLocalPlayer)
    //        return;

    //    if (Input.GetKeyDown(KeyCode.K))
    //    {
    //        RpcTakeDamage(9999, transform.name);
    //    }
    //}

    [ClientRpc] //Make sure this method is called on all clients connected
    public void RpcTakeDamage(int amount, string sourceID)
    {
        if (IsDead)
            return;

        currentHealth -= amount;

        //Debug.Log(transform.name + " now has " + currentHealth + " health.");

        if(currentHealth <= 0)
        {
            //Kill the player
            Die(sourceID);
        }
    }

    private void Die(string sourceID)
    {
        IsDead = true;

        //Get the player who killed us and increase their kill count
        Player sourcePlayer = GameManager.GetPlayer(sourceID);
        if (sourcePlayer != null)
        {
            sourcePlayer.kills++;

            //Update UI
            GameManager.instance.onPlayerKilledCallback.Invoke(username, sourcePlayer.username);
            
        }


        //Increase our death count
        deaths += 1;


        //Reset weapons
        GetComponent<WeaponManager>().ResetWeapons();

        //disable some components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        //disable Game Objects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }

        //disable collider
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        //Spawn a death effect
        GameObject deathEffectsInstance = Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(deathEffectsInstance, 3f);

        //Switch cameras
        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCamera(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        //Call respawn method
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn ()
    {
        //Wait for respawn timer
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();

        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        //Delay instantiating the particles
        yield return new WaitForSeconds(0.1f);

        SetupPlayer();

        //Debug.Log(transform.name + " respawned");
    }

    public void SetDefaults()
    {
        IsDead = false;
        HasSniper = false;
        HasRocketLauncher = false;

        currentHealth = maxHealth;

        //Enable the components
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            //default enabled states to whether or not they were originally enabled
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        //Enable Game Objects
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        //Enable collider
        Collider col = GetComponent<Collider>();
        if(col != null)
            col.enabled = true;

        //Create respawn effect
        GameObject respawnEffectsInstance = Instantiate(respawnEffect, transform.position, Quaternion.identity);
        Destroy(respawnEffectsInstance, 3f);
    }
}
