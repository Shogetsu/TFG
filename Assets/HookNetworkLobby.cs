using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;


/* Esta clase sirve para relacionar cualquier información del jugador del lobby con la partida (LobbyPlayer -> GamePlayer)*/
public class HookNetworkLobby : LobbyHook {

    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        //A la configuracion del jugador (SetupLocalPlayer) se le asigna el nombre del lobby y el color elegido
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
        SetupLocalPlayer localPlayer = gamePlayer.GetComponent<SetupLocalPlayer>();

        localPlayer.pname = lobby.playerName;
        localPlayer.playerColor = lobby.playerColor;
    }
}
