using System.Collections.Generic;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Network
{
    public class SteamLobby : MonoBehaviour
    {
        protected Callback<LobbyCreated_t> lobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> joinRequested;
        protected Callback<LobbyEnter_t> lobbyEnter;

        //Lobby List Callback
        protected Callback<LobbyMatchList_t> lobbyList;
        protected Callback<LobbyDataUpdate_t> lobbyListUpdated;

        List<CSteamID> lobbyIDs = new List<CSteamID>();


        public ulong currentLobbyID;
        private const string HostAddredKey = "HostAddress";
        private CustomNetworkManager manager;
        

        public static SteamLobby instance;

        public void Start()
        {
            if (!SteamManager.Initialized)
            {
                return;
            }

            if (instance == null)
            {
                instance = this;
            }

            manager = GetComponent<CustomNetworkManager>();

            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            joinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
            lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

            lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
            lobbyListUpdated = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
        
        }

        private void OnDestroy()
        {
            if (SteamManager.Initialized)
                SteamMatchmaking.LeaveLobby((CSteamID)currentLobbyID);
            lobbyCreated.Unregister();
            joinRequested.Unregister();
            lobbyEnter.Unregister();
            lobbyList.Unregister();
            lobbyListUpdated.Unregister();
        }

        public void HostLobby()
        {
            //FriendOnly
            // SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, Manager.maxConnections);
            //Public
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, manager.maxConnections);
        }

        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                return;
            }
            Debug.Log("Lobby Created Successfully");
            if (!this) { return; }
            
            manager.StartHost();
            
            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby),
                HostAddredKey, SteamUser.GetSteamID().ToString());
            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby),
                "name", SteamFriends.GetPersonaName().ToString() + "'S Lobby");
            
        }

        private void OnJoinRequest(GameLobbyJoinRequested_t callback)
        {
            Debug.Log("Request to join Lobby");
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        private void OnLobbyEntered(LobbyEnter_t callback)
        {
            // HostButton.SetActive(false);
            currentLobbyID = callback.m_ulSteamIDLobby;
            // LobbyNameText.gameObject.SetActive(true);
            //LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "Name");

            if (NetworkServer.active)
            {
                return;
            }
        
            manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddredKey);
            if (!this) { return; }
            if(!string.IsNullOrWhiteSpace(manager.networkAddress))
                manager.StartClient();
        }

        public void JoinLobby(CSteamID lobbyID)
        {
            SteamMatchmaking.JoinLobby(lobbyID);
        }

        public void GetListOfLobbies()
        {
            if(lobbyIDs.Count > 0)
            {
                lobbyIDs.Clear();
            }
            SteamMatchmaking.AddRequestLobbyListResultCountFilter(50);
            SteamAPICall_t tryGetList = SteamMatchmaking.RequestLobbyList();
        }

        void OnGetLobbyData(LobbyDataUpdate_t result)
        {
            LobbyListManager.instance.DisplayLobbies(lobbyIDs, result);
        }

        void OnGetLobbyList(LobbyMatchList_t result)
        {
            if(LobbyListManager.instance.listOfLobbies.Count > 0)
            {
                LobbyListManager.instance.DestroyLobbies();
            }

            for(int i = 0; i < result.m_nLobbiesMatching; i++)
            {
                CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
                lobbyIDs.Add(lobbyID);
                SteamMatchmaking.RequestLobbyData(lobbyID);
            }
        }
    }
}
