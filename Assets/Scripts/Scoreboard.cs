using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour {

    [SerializeField]
    GameObject playerScoreboardItem;

    [SerializeField]
    Transform playerScoreboardList;

    private void OnEnable()
    {
        //Get an array of players
        Player[] players = GameManager.GetAllPlayers();

        //Loop through and set up a list item for each one
        //This includes setting the UI elements equal to the data
        foreach(Player player in players)
        {
            GameObject itemGO = Instantiate(playerScoreboardItem, playerScoreboardList);
            PlayerScoreboardItem item = itemGO.GetComponent<PlayerScoreboardItem>();
            if(item != null)
            {
                item.Setup(player.username, player.kills, player.deaths);
            }
            //Debug.Log(player.username + " | Kills:" + player.kills + " | Deaths:" + player.deaths);
        }
    }

    private void OnDisable()
    {
        //Clean up our list of items
        foreach(Transform child in playerScoreboardList)
        {
            Destroy(child.gameObject);
        }
    }
}
