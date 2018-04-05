using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Steamworks;

public class PlayerName : NetworkBehaviour {

    [SyncVar(hook = "OnMyName")]
    public string playerName = "";

    public Text playerNameText;

	// Use this for initialization
	void Start () {
        OnMyName(playerName);

        if (playerName=="")
            CmdSetPlayerName(SteamFriends.GetPersonaName());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    [Command]
    void CmdSetPlayerName(string name)
    {
        playerName = name;
       //RpcUpdatePlayerName(playerName);
    }

   /* [ClientRpc]
    void RpcUpdatePlayerName(string pName)
    {
        Debug.Log("Hola");
        playerNameText.text = pName;
    }*/

    void OnMyName(string pName)
    {
        playerName = pName;
        playerNameText.text = playerName;
    }
}
