using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour {

    [SerializeField]
    private Text killCount;

    [SerializeField]
    private Text deathCount;

	// Use this for initialization
	void Start () {
        //Get user data
        if(UserAccountManager.IsLoggedIn)
            UserAccountManager.instance.GetData(OnReceivedData);
	}
    
    void OnReceivedData(string data)
    {
        if (killCount == null || deathCount == null)
            return;

        //Update the player stats based on the data using the parser
        killCount.text = DataParser.DataToKills(data).ToString() + " Kills";
        deathCount.text = DataParser.DataToDeaths(data).ToString() + " DEATHS";
    }
}
