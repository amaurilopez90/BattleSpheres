using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notifications : MonoBehaviour {


    [SerializeField]
    GameObject notificationItemPrefab;

	// Use this for initialization
	void Start () {
        GameManager.instance.onPlayerKilledCallback += OnKill; //add on another function, called OnKill
        GameManager.instance.onItemPickupCallback += OnItemPickup;
	}
	
    public void OnKill(string player, string source)
    {
        GameObject GO = Instantiate(notificationItemPrefab, this.transform);
        GO.GetComponent<NotificationItem>().SetupOnPlayerKillItem(player, source);

        GO.transform.SetAsFirstSibling(); //Set to have the most recent notification at the top
        Destroy(GO, 4f); //Destroy the notifications object after 4 seconds
    }

    public void OnItemPickup(string player, string sourceItem)
    {
        GameObject GO = Instantiate(notificationItemPrefab, this.transform);
        GO.GetComponent<NotificationItem>().SetupOnItemPickupItem(player, sourceItem);

        GO.transform.SetAsFirstSibling();
        Destroy(GO, 4f);
    }
}
