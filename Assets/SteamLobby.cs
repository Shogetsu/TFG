using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System;

namespace Prototype.NetworkLobby
{
    public class SteamLobby : NetworkBehaviour {

        protected Callback<LobbyCreated_t> Callback_lobbyCreated;
        protected Callback<LobbyMatchList_t> Callback_lobbyList;
        protected Callback<LobbyEnter_t> Callback_lobbyEnter;
        protected Callback<LobbyDataUpdate_t> Callback_lobbyInfo;
        private Callback<GameLobbyJoinRequested_t> m_GameLobbyJoinRequested;

        public LobbyManager lobbyManager;
        public GameObject serverEntryPrefab;

        public InputField matchNameInput;
        [SyncVar]
        //public GameObject lobbyPlayerPrefab;


        ulong current_lobbyID;
        List<CSteamID> lobbyIDS;

        // Use this for initialization
        void Start()
        {

            lobbyIDS = new List<CSteamID>();
            Callback_lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            Callback_lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbiesList);
            Callback_lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
            Callback_lobbyInfo = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyInfo);
            m_GameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);


            if (SteamAPI.Init())
                Debug.Log("Steam API init -- SUCCESS!");
            else
                Debug.Log("Steam API init -- failure ...");
        }

        void Update()
        {
            SteamAPI.RunCallbacks();
            /*COMANDOS PARA PROBAR LOS METODOS DE STEAMWORKS!!!!!*/
           
            // Command - Create new lobby
           /* if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("Trying to create lobby ...");
                SteamAPICall_t try_toHost = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 4);
            }*/

            // Command - List lobbies
           /* if (Input.GetKeyDown(KeyCode.L))
            {

                Debug.Log("Trying to get list of available lobbies ...");

                SteamMatchmaking.AddRequestLobbyListStringFilter("game", "shogetsuTFG", ELobbyComparison.k_ELobbyComparisonEqual);
                SteamAPICall_t try_getList = SteamMatchmaking.RequestLobbyList();



            }*/

            // Command - Join lobby at index 0 (testing purposes)
            /*if (Input.GetKeyDown(KeyCode.J))
            {
                Debug.Log("Trying to join FIRST listed lobby ...");
                SteamAPICall_t try_joinLobby = SteamMatchmaking.JoinLobby(SteamMatchmaking.GetLobbyByIndex(0));
            }*/

            // Command - List lobby members
           /* if (Input.GetKeyDown(KeyCode.Q))
            {
                int numPlayers = SteamMatchmaking.GetNumLobbyMembers((CSteamID)current_lobbyID);

                Debug.Log("\t Number of players currently in lobby : " + numPlayers);
                for (int i = 0; i < numPlayers; i++)
                {
                    Debug.Log("\t Player(" + i + ") == " + SteamFriends.GetFriendPersonaName(SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, i)));
                }
            }*/
        }

        void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t pCallback)
        {
            // Invite accepted, game is already running
            // JoinLobby(pCallback.m_steamIDLobby);

            LeftLobby();
            Debug.Log("Aceptada la siguiente partida: ");
            CSteamID lobbyID = pCallback.m_steamIDLobby;
            SteamMatchmaking.RequestLobbyData(lobbyID);

            string idMatchUNET = SteamMatchmaking.GetLobbyData(lobbyID, "idMatch");

            while (idMatchUNET.Equals("")) //A VECES se obtiene el idMatchUNET vacio... solucion 'de aquella manera'
            {
                idMatchUNET = SteamMatchmaking.GetLobbyData(lobbyID, "idMatch");
            }
           
            Debug.Log("ID MATCH UNET (invite): " + idMatchUNET);

            GameObject o = Instantiate(serverEntryPrefab) as GameObject;

            NetworkID idMatchUnet = (NetworkID)Enum.Parse(typeof(NetworkID), idMatchUNET);
            o.GetComponent<LobbyServerEntry>().JoinMatch(idMatchUnet, lobbyManager);
            /*  SteamMatchmaking.JoinLobby((CSteamID)pCallback.m_steamIDLobby); //join Steam lobby
              lobbyManager.matchMaker.JoinMatch(networkID, "", "", "", 0, 0, lobbyManager.OnMatchJoined);
              lobbyManager.backDelegate = lobbyManager.StopClientClbk;
              lobbyManager._isMatchmaking = true;
              lobbyManager.DisplayIsConnecting();*/
        }

        public void LeftLobby()
        {
            SteamMatchmaking.LeaveLobby((CSteamID)current_lobbyID);
        }

        void OnLobbyCreated(LobbyCreated_t result)
        {
            if (result.m_eResult == EResult.k_EResultOK)
                Debug.Log("Lobby created -- SUCCESS!");
            else
                Debug.Log("Lobby created -- failure ...");

            // Debug.Log("AQUI ESTA EL ID DE UNET EN STEAM: "+SteamMatchmaking.GetUNETMatchID());
            SteamMatchmaking.SetLobbyData((CSteamID)result.m_ulSteamIDLobby, "idMatch", SteamMatchmaking.GetUNETMatchID().ToString());
            SteamMatchmaking.SetLobbyData((CSteamID)result.m_ulSteamIDLobby, "name", matchNameInput.text);
            SteamMatchmaking.SetLobbyData((CSteamID)result.m_ulSteamIDLobby, "appid", "480");
            SteamMatchmaking.SetLobbyData((CSteamID)result.m_ulSteamIDLobby, "game", "shogetsuTFG");

            Debug.Log("ID MATCH UNET (create): " + SteamMatchmaking.GetLobbyData((CSteamID)result.m_ulSteamIDLobby, "idMatch"));

        }

        void OnGetLobbiesList(LobbyMatchList_t result)
        {
            Debug.Log("Found " + result.m_nLobbiesMatching + " lobbies!");
            for (int i = 0; i < result.m_nLobbiesMatching; i++)
            {
                CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
                lobbyIDS.Add(lobbyID);
                SteamMatchmaking.RequestLobbyData(lobbyID);
            }
        }

        void OnGetLobbyInfo(LobbyDataUpdate_t result)
        {
            for (int i = 0; i < lobbyIDS.Count; i++)
            {
                if (lobbyIDS[i].m_SteamID == result.m_ulSteamIDLobby)
                {
                    //ID LOBBY STEAM: (CSteamID)lobbyIDS[i].m_SteamID
                    Debug.Log("Lobby " + i + " (" + SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDS[i].m_SteamID, "idMatch") + ") :: " + SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDS[i].m_SteamID, "name"));
                    return;
                }
            }

        }

        void OnLobbyEntered(LobbyEnter_t result)
        {
            current_lobbyID = result.m_ulSteamIDLobby;

            if (result.m_EChatRoomEnterResponse == 1) {
                Debug.Log("Steam Lobby joined! "+ current_lobbyID);
            }
            else
            {
                Debug.Log("Failed to join lobby.");
            }
        }

        public void OnClickInvite()
        {
            Debug.Log("Invitar a la partida con ID: "+current_lobbyID);
            SteamFriends.ActivateGameOverlayInviteDialog((CSteamID)current_lobbyID);
        }
    }
}