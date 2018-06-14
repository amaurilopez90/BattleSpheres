using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerScore : MonoBehaviour {

    private int lastKills = 0;
    private int lastDeaths = 0;

    Player player;

	// Use this for initialization
	void Start () {
        player = GetComponent<Player>();
        StartCoroutine(SyncScoreLoop());
	}

    void OnDestroy()
    {
        if(player != null)
            SyncNow();
    }

    IEnumerator SyncScoreLoop()
    {
        //Sync scores every 20 seconds
        while (true)
        {
            yield return new WaitForSeconds(20f);

            SyncNow();
        }
    }

    void SyncNow()
    {
        if (UserAccountManager.IsLoggedIn)
            UserAccountManager.instance.GetData(OnDataReceived);
    }

    void OnDataReceived(string data)
    {
        //Only sync while it's necessary, not only when a number has changed
        if (player.kills <= lastKills && player.deaths <= lastDeaths)
            return;

        int killsSinceLast = player.kills - lastKills;
        int deathsSinceLast = player.deaths - lastDeaths;

        int kills = DataParser.DataToKills(data);
        int deaths = DataParser.DataToDeaths(data);

        int newKillCount = killsSinceLast + kills;
        int newDeathCount = deathsSinceLast + deaths;

        string newData = DataParser.ValuesToData(newKillCount, newDeathCount);

        Debug.Log("Syncing: " + newData);

        //Reset counts
        lastKills = player.kills;
        lastDeaths = player.deaths;

        UserAccountManager.instance.SendData(newData);
    }
}
