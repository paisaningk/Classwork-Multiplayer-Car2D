using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.UI;
using System.Linq;
using TMPro;

namespace Script.Network
{
    public class LobbyController : MonoBehaviour
    {
        public static LobbyController instance;

        //UI Elements
        public TextMeshProUGUI lobbyNameText;

        //Player Date
        public GameObject playerListViewContent;
        public GameObject playerListItemPreFab;
        public GameObject localPlayerObject;

        //Other Date
        public ulong currentLobbyID;
        public bool playerItemCreated = false;
        private List<PlayerListItem> playerListItems = new List<PlayerListItem>();
        public PlayerObjectLobby localPlayerController;

        //Manager
        private CustomNetworkManager manager;

        private CustomNetworkManager Manager
        {
            get
            {
                if (manager != null)
                {
                    return manager;
                }

                return manager = CustomNetworkManager.singleton as CustomNetworkManager;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        public void UpdateLobbyName()
        {
            currentLobbyID = Manager.GetComponent<SteamLobby>().currentLobbyID;
            lobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(currentLobbyID), "name");
        }

        public void UpdatePlayerList()
        {
            if (!playerItemCreated)
            {
                CreateHostPlayerItem();
            } //Host

            if (playerListItems.Count < Manager.gamePlayers.Count)
            {
                CreateClientPlayerItem();
            }

            if (playerListItems.Count > Manager.gamePlayers.Count)
            {
                RemovePlayerItem();
            }

            if (playerListItems.Count == Manager.gamePlayers.Count)
            {
                UpdatePlayerItem();
            }
        }

        public void CreateHostPlayerItem()
        {
            foreach (PlayerObjectLobby player in Manager.gamePlayers)
            {
                GameObject newPlayerItem = Instantiate(playerListItemPreFab) as GameObject;
                PlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();

                newPlayerItemScript.playerName = player.playerName;
                newPlayerItemScript.connectionID = player.connectionID;
                newPlayerItemScript.playerSteamID = player.playerSteamID;
                newPlayerItemScript.SetPlayerValues();


                newPlayerItem.transform.SetParent(playerListViewContent.transform);
                newPlayerItem.transform.localScale = Vector3.one;

                playerListItems.Add(newPlayerItemScript);
            }

            playerItemCreated = true;
        }

        public void CreateClientPlayerItem()
        {
            foreach (PlayerObjectLobby player in Manager.gamePlayers)
            {
                if (playerListItems.All(b => b.connectionID != player.connectionID))
                {
                    GameObject newPlayerItem = Instantiate(playerListItemPreFab) as GameObject;
                    PlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();

                    newPlayerItemScript.playerName = player.playerName;
                    newPlayerItemScript.connectionID = player.connectionID;
                    newPlayerItemScript.playerSteamID = player.playerSteamID;
                    newPlayerItemScript.SetPlayerValues();


                    newPlayerItem.transform.SetParent(playerListViewContent.transform);
                    newPlayerItem.transform.localScale = Vector3.one;

                    playerListItems.Add(newPlayerItemScript);
                }
            }
        }

        public void UpdatePlayerItem()
        {
            foreach (PlayerObjectLobby player in Manager.gamePlayers)
            {
                foreach (PlayerListItem playerListItemScript in playerListItems)
                {
                    if (playerListItemScript.connectionID == player.connectionID)
                    {
                        playerListItemScript.playerName = player.playerName;
                        playerListItemScript.SetPlayerValues();
                        if (player == localPlayerController)
                        {

                        }
                    }
                }

            }
        }

        public void RemovePlayerItem()
        {
            List<PlayerListItem> playerListItemToRemove = new List<PlayerListItem>();

            foreach (PlayerListItem playerlistItem in playerListItems)
            {
                if (!Manager.gamePlayers.Any(b => b.connectionID == playerlistItem.connectionID))
                {
                    playerListItemToRemove.Add(playerlistItem);
                }
            }

            if (playerListItemToRemove.Count > 0)
            {
                foreach (PlayerListItem playerlistItemToRemove in playerListItemToRemove)
                {
                    if (playerlistItemToRemove)
                    {
                        GameObject ObjectToRemove = playerlistItemToRemove.gameObject;
                        playerListItems.Remove(playerlistItemToRemove);
                        Destroy(ObjectToRemove);
                        ObjectToRemove = null;
                    }

                }
            }
        }
    }
}
