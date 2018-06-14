using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;

public class JoinGame : MonoBehaviour {

    //Make a list to keep track of open rooms
    List<GameObject> roomList = new List<GameObject>();

    [SerializeField]
    private Text status;

    [SerializeField]
    private GameObject roomListItemPrefab;

    [SerializeField]
    private Transform roomListParent;

    private NetworkManager networkManager;

    void Start()
    {
        //Start the matchmaker and refresh the room list
        networkManager = NetworkManager.singleton;
        if (networkManager.matchMaker == null)
            networkManager.StartMatchMaker();

        RefreshRoomList();
    }

    public void RefreshRoomList()
    {
        ClearRoomList();

        //If matchmaker is equal to null, it may have been terminated. So restart it
        if(networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }
        networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList);
        status.text = "Loading...";
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        status.text = "";
        if (!success || matches == null)
        {
            status.text = "Couldn't get room list.";
            return;
        }

        foreach(MatchInfoSnapshot match in matches)
        {
            GameObject roomListItem = Instantiate(roomListItemPrefab);
            roomListItem.transform.SetParent(roomListParent);

            RoomListItem _roomListItem = roomListItem.GetComponent<RoomListItem>();
            if(_roomListItem != null)
            {
                _roomListItem.Setup(match, JoinRoom);
            }

            roomList.Add(roomListItem);
        }

        if(roomList.Count == 0)
        {
            status.text = "No rooms available";
        }
    }

    void ClearRoomList()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }

        roomList.Clear();
    }

    //Called whenever we select a room to join
    public void JoinRoom(MatchInfoSnapshot _match)
    {
        networkManager.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined);
        StartCoroutine(WaitForJoin());
    }

    //Set up an interface enumerator to implement a join game timer
    IEnumerator WaitForJoin()
    {
        ClearRoomList();

        int countdown = 10;

        //Display a countdown for joining the game, in case of time-out
        while (countdown > 0)
        {
            status.text = "JOINING... (" + countdown + ")";

            yield return new WaitForSeconds(1f);
            countdown--;

        }

        //If we haven't connected at this point, then we have failed to connect
        status.text = "Failed to connect.";
        yield return new WaitForSeconds(1f);
        
        //Make sure to drop the connection request in the background if we "failed to connect"
        //When host leaves a game, the game may still be available in the room list because Unity may take some time to clean up the room
        //This is why we implement a join game timer, to "wait" for unity to clean everything up before we are able to successfully join again
        MatchInfo matchInfo = networkManager.matchInfo;
        if(matchInfo != null)
        {
            networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection);
            networkManager.StopHost();
        }
        
        RefreshRoomList();
    }
}
