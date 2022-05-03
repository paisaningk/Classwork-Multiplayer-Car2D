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

        //Player Data
        public GameObject playerListViewContent;
        public GameObject playerListItemPrefab;
        public GameObject localPlayerObject;

        //Other Data
        public ulong currentLobbyID;
        public bool playerItemCreated = false;
        private List<PlayerListItem> playerListItems = new List<PlayerListItem>();
        public PlayerObjectLobby localPlayerController;

        //Ready
        public Button startGameButton;
        public TextMeshProUGUI readyButtonText;

        //Manager
        private CustomNetworkManager manager;

        public CustomNetworkManager Manager
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

        public void ReadyPlayer()
        {
            localPlayerController.ChangeReady();
        }

        public void UpdateButton()
        {
            if(localPlayerController.isReady)
            {
                readyButtonText.text = "Unready";
            }
            else
            {
                readyButtonText.text = "Ready";

            }
        }

        public void CheckIfAllReady()
        {
            bool allReady = false;
            foreach (PlayerObjectLobby player in Manager.gamePlayers)
            {
                if (player.isReady)
                {
                    allReady = true;
                }
                else
                {
                    allReady = false;
                    break;
                }
            }
            if (allReady)
            {
                if (localPlayerController.playerIDNumber == 1)
                {
                    startGameButton.interactable = true;
                    foreach (PlayerObjectLobby player in Manager.gamePlayers)
                    {
                        player.isFinish = false;
                    }
                    
                }
                else
                {
                    startGameButton.interactable = false;

                }
            }
            else
            {
                startGameButton.interactable = false;
            }

        }

        private void Awake()
        {
            if(instance == null) { instance = this; }
        }

        public void UpdateLobbyName()
        {
            currentLobbyID = Manager.GetComponent<SteamLobby>().currentLobbyID;
            lobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(currentLobbyID), "name");
        }

        public void UpdatePlayerList()
        {
            if(!playerItemCreated) { CreateHostPlayerItem(); } //Host
            if(playerListItems.Count < Manager.gamePlayers.Count) { CreateClientPlayerItem();}
            if(playerListItems.Count > Manager.gamePlayers.Count) { RemovePlayerItem(); }
            if(playerListItems.Count == Manager.gamePlayers.Count) { UpdatePlayerItem(); }
        }

        public void FindLocalPlayer()
        {
            localPlayerObject = GameObject.Find("LocalGamePlayer");
            localPlayerController = localPlayerObject.GetComponent<PlayerObjectLobby>();
        }


        public void CreateHostPlayerItem()
        {
            foreach(PlayerObjectLobby player in Manager.gamePlayers)
            {
                GameObject newPlayerItem = Instantiate(playerListItemPrefab) as GameObject;
                PlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();

                newPlayerItemScript.playerName = player.playerName;
                newPlayerItemScript.connectionID = player.connectionID;
                newPlayerItemScript.playerSteamID = player.playerSteamID;
                newPlayerItemScript.isReady = player.isReady;
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
                if(!playerListItems.Any(b => b.connectionID == player.connectionID))
                {
                    GameObject newPlayerItem = Instantiate(playerListItemPrefab) as GameObject;
                    PlayerListItem newPlayerItemScript = newPlayerItem.GetComponent<PlayerListItem>();

                    newPlayerItemScript.playerName = player.playerName;
                    newPlayerItemScript.connectionID = player.connectionID;
                    newPlayerItemScript.playerSteamID = player.playerSteamID;
                    newPlayerItemScript.isReady = player.isReady;
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
                foreach(PlayerListItem playerListItemScript in playerListItems)
                {
                    if(playerListItemScript.connectionID == player.connectionID)
                    {
                        playerListItemScript.playerName = player.playerName;
                        playerListItemScript.isReady = player.isReady;
                        playerListItemScript.SetPlayerValues();
                        if(player == localPlayerController)
                        {
                            UpdateButton();
                        }
                    }
                }

            }
            CheckIfAllReady();
        }

        public void RemovePlayerItem()
        {
            List<PlayerListItem> playerListItemToRemove = new List<PlayerListItem>();

            foreach (PlayerListItem playerlistItem in playerListItems)
            {
                if(Manager.gamePlayers.All(b => b.connectionID != playerlistItem.connectionID))
                {
                    playerListItemToRemove.Add(playerlistItem);
                }
            }
            if(playerListItemToRemove.Count > 0)
            {
                foreach (PlayerListItem playerlistItemToRemove in playerListItemToRemove)
                {
                    if(playerlistItemToRemove)
                    {
                        GameObject objectToRemove = playerlistItemToRemove.gameObject;
                        playerListItems.Remove(playerlistItemToRemove);
                        Destroy(objectToRemove);
                        objectToRemove = null;
                    }
               
                }
            }
        }
    
        public void StartGame(string nameMap)
        {
            localPlayerController.CanStartGame(nameMap);
        }

        public void Quit()
        {
            Manager.Logout();
        }
    }
}
