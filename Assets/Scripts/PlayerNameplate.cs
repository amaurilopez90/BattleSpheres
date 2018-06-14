using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameplate : MonoBehaviour {

    [SerializeField]
    private Text usernameText;

    [SerializeField]
    private RectTransform healthBarFill;

    [SerializeField]
    private Player player;
	
	// Update is called once per frame
	void Update () {
        usernameText.text = player.username;
        healthBarFill.localScale = new Vector3(player.GetHealthNormalized(), 1f, 1f);

        //Camera facing billboard, so nameplate always faces the same direction that the camera looking at it is lookin
        Camera cam = Camera.main;
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);

	}
}
