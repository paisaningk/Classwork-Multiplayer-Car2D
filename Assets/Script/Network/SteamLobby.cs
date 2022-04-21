using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Network
{
    public class SteamLobby : MonoBehaviour
    {
        public static SteamLobby instance;
        protected Callback<LobbyCreated_t> lobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> joinRequest;
        protected Callback<LobbyEnter_t> lobbyEntered;

        public ulong currentLobbyID;
        private const string HostAddressKey = "HostAddress";
        private CustomNetworkManager networkManager;
    
        public GameObject hostButton;
        // public TextMeshProUGUI lobbyNameText;
    
        private void Awake()
        {
            networkManager = GetComponent<CustomNetworkManager>();
            if (instance == null)
            {
                instance = this;
            }
            hostButton.GetComponent<Button>().onClick.AddListener(HostLobby);
        }

        private void Start()
        {
            if (!SteamManager.Initialized)
            {
                return;
            }

            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            joinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }

        private void HostLobby()
        {
            Debug.Log("Host");
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
        }
        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                return;
            }
        
            Debug.Log("Lobby created successfully");
        
            networkManager.StartHost();

            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey,
                SteamUser.GetSteamID().ToString());
            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name",
                SteamFriends.GetPersonaName().ToString() + "'s Lobby");

        }

        private void OnJoinRequest(GameLobbyJoinRequested_t callback)
        {
            Debug.Log("Request TO join Lobby");
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        private void OnLobbyEntered(LobbyEnter_t callback)
        {
            //Everyone
            // hostButton.SetActive(false);
            currentLobbyID = callback.m_ulSteamIDLobby;
            // lobbyNameText.gameObject.SetActive(true);
            // lobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");
        
            //Clients
            if (NetworkServer.active)
            {
                return;
            }

            networkManager.networkAddress =
                SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);
        
            networkManager.StartClient();
        }
    }
}
