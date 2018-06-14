using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;
public class RoomListItem : MonoBehaviour {

    //Create delegate as a callback so that when we invoke it, all functions subscribed to it will call it
    public delegate void JoinRoomDelegate(MatchInfoSnapshot match);
    private JoinRoomDelegate joinRoomCallback;


    [SerializeField]
    private Text roomNameText;

    private MatchInfoSnapshot match;

    public void Setup(MatchInfoSnapshot _match, JoinRoomDelegate _joinRoomCallback)
    {
        //Set up the room button
        match = _match;
        joinRoomCallback = _joinRoomCallback;

        roomNameText.text = match.name + " (" + match.currentSize + "/" + match.maxSize + ")";
        
    }

    public void JoinRoom()
    {
        //Link to whatever game we want to join
        joinRoomCallback.Invoke(match);
    }
}
