using UnityEngine;
using UnityEngine.UI;

public class NotificationItem : MonoBehaviour {

    [SerializeField]
    Text text;

    public void SetupOnPlayerKillItem(string player, string source)
    {
        text.text = "<b>" + source + "</b>" + " killed " + "<i>" + player + "</i>";
    }

    public void SetupOnItemPickupItem(string player, string sourceItem)
    {
        text.text = "<b>" + sourceItem + "</b>" + " picked up by " + "<i>" + player + "</i>";
    }
}
