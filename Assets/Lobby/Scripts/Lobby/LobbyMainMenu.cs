using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Steamworks;

/*
using System.Collections.Generic;
using UnityEngine.Networking.Match;*/

namespace Prototype.NetworkLobby
{
    //Main menu, mainly only a bunch of callback called by the UI (setup throught the Inspector)
    public class LobbyMainMenu : MonoBehaviour 
    {
        protected Callback<LobbyCreated_t> Callback_lobbyCreated;
       // protected Callback<LobbyEnter_t> Callback_lobbyEnter;

        public LobbyManager lobbyManager;

        public RectTransform lobbyServerList;
        public RectTransform lobbyPanel;

        public InputField ipInput;
        public InputField matchNameInput;

       

        // ulong current_lobbyID;

        public void OnEnable()
        {
            // Callback_lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            //Callback_lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

            lobbyManager.topPanel.ToggleVisibility(true);

            ipInput.onEndEdit.RemoveAllListeners();
            ipInput.onEndEdit.AddListener(onEndEditIP);

            matchNameInput.onEndEdit.RemoveAllListeners();
            matchNameInput.onEndEdit.AddListener(onEndEditGameName);
        }

        public void OnClickHost()
        {
            lobbyManager.StartHost();
        }

        public void OnClickJoin()
        {
            lobbyManager.ChangeTo(lobbyPanel);

            lobbyManager.networkAddress = ipInput.text;
            lobbyManager.StartClient();

            lobbyManager.backDelegate = lobbyManager.StopClientClbk;
            lobbyManager.DisplayIsConnecting();

            lobbyManager.SetServerInfo("Connecting...", lobbyManager.networkAddress);
        }

        public void OnClickDedicated()
        {
            lobbyManager.ChangeTo(null);
            lobbyManager.StartServer();

            lobbyManager.backDelegate = lobbyManager.StopServerClbk;

            lobbyManager.SetServerInfo("Dedicated Server", lobbyManager.networkAddress);
        }

        public void OnClickCreateMatchmakingGame()
        {

            lobbyManager.StartMatchMaker();
           lobbyManager.matchMaker.CreateMatch(
                  matchNameInput.text,
                  (uint)lobbyManager.maxPlayers,
                  true,
                  "", "", "", 0, 0,
                  lobbyManager.OnMatchCreate);


            //SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, lobbyManager.maxPlayers);

            lobbyManager.backDelegate = lobbyManager.StopHost;
            lobbyManager._isMatchmaking = true;
            //lobbyManager.ChangeTo(lobbyPanel);
           lobbyManager.DisplayIsConnecting();

            lobbyManager.SetServerInfo("Matchmaker Host", lobbyManager.matchHost);

        }

       /* void OnLobbyCreated(LobbyCreated_t result)
        {
            if (result.m_eResult == EResult.k_EResultOK)
                Debug.Log("Lobby created -- SUCCESS!");
            else
                Debug.Log("Lobby created -- failure ...");

            SteamMatchmaking.SetLobbyData((CSteamID)result.m_ulSteamIDLobby, "name", matchNameInput.text);
            SteamMatchmaking.SetLobbyData((CSteamID)result.m_ulSteamIDLobby, "appid", "480");

        }*/

        /*void OnLobbyEntered(LobbyEnter_t result)
        {
            current_lobbyID = result.m_ulSteamIDLobby;

            if (result.m_EChatRoomEnterResponse == 1)
                Debug.Log("Lobby joined!");
            else
                Debug.Log("Failed to join lobby.");
        }*/

        public void OnClickOpenServerList()
        {
            lobbyManager.StartMatchMaker();
            lobbyManager.backDelegate = lobbyManager.SimpleBackClbk;
            lobbyManager.ChangeTo(lobbyServerList);
        }

        void onEndEditIP(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnClickJoin();
            }
        }

        void onEndEditGameName(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnClickCreateMatchmakingGame();
            }
        }

    }
}
