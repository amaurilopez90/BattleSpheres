using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    Behaviour[] componentToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    [SerializeField]
    string dontDrawLayerName = "DontDraw";
    [SerializeField]
    GameObject playerGraphics;

    [SerializeField]
    GameObject playerUIPrefab;

    [HideInInspector]
    public GameObject playerUIInstance;

    void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {
            //Disable player graphics for local player
            SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            //Create player UI
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;

            //Configure player UI
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();

            if (ui == null)
                Debug.LogError("No Player UI Component on player UI prefab.");
            ui.SetPlayer(GetComponent<Player>());

            GetComponent<Player>().SetupPlayer();

            string _username = "Loading...";
            if (UserAccountManager.IsLoggedIn)
                _username = UserAccountManager.LoggedIn_Username;
            else
                _username = transform.name;

            CmdSetUsername(transform.name, _username);

        }
        
    }

    [Command]
    void CmdSetUsername(string playerID, string username)
    {
        Player player = GameManager.GetPlayer(playerID);
        if(player != null)
        {
            Debug.Log(username + " has joined!");
            player.username = username;
        }
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach(Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    void DisableComponents()
    {
        for (int i = 0; i < componentToDisable.Length; i++)
        {
            componentToDisable[i].enabled = false;
        }

    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        string netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player player = GetComponent<Player>();
        GameManager.RegisterPlayer(netID, player);
    }

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    //When we are destroyed, or disconnect
    void OnDisable()
    {
        //remove player UI on disable
        Destroy(playerUIInstance);

        if(isLocalPlayer)
            GameManager.instance.SetSceneCamera(true);

        //De-register once player is killed
        GameManager.DeRegisterPlayer(transform.name);
    }


}
