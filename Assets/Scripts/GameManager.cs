using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public MatchSettings matchSettings;

    [SerializeField]
    private GameObject sceneCamera;

    //Use a callback to call multiple methods from one place later
    public delegate void OnPlayerKilledCallback(string player, string source);
    public OnPlayerKilledCallback onPlayerKilledCallback;

    public delegate void OnItemPickupCallback(string player, string sourceItem);
    public OnItemPickupCallback onItemPickupCallback;

    void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("More than once GameManager in scene.");
        }
        else
        {
            instance = this;
        }
    }

    public void SetSceneCamera(bool isActive)
    {
        if (sceneCamera == null)
            return;

        sceneCamera.SetActive(isActive);
    }

    #region Player registering

    private const string PLAYER_ID_PREFIX = "Player ";

    //Dictionary to hold players and corresponding identity
    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    public static void RegisterPlayer(string netID, Player player)
    {
        string playerID = PLAYER_ID_PREFIX + netID;
        players.Add(playerID, player);
        player.transform.name = playerID;
    }

    public static Player[] GetAllPlayers()
    {
        return players.Values.ToArray();
    }

    public static void DeRegisterPlayer(string playerID)
    {
        players.Remove(playerID);
    }

    public static Player GetPlayer(string playerID)
    {
        return players[playerID];
    }


    #endregion
}
