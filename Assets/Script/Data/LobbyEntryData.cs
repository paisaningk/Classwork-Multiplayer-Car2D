using System;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Network
{
    public class LobbyEntryData : MonoBehaviour
    {
        //Data
        public CSteamID lobbySteamID;
        public string lobbyName;
        public TextMeshProUGUI lobbyNameText;
        public Button joinButton;

        public void Awake()
        {
            joinButton.onClick.AddListener(JoinLobby);
        }

        public void SetLobbyName()
        {
            if(lobbyName =="")
            {
                lobbyNameText.text = "Empty";
                lobbyNameText.color = Color.white;
                joinButton.interactable = false;
            }
            else
            {
           
                lobbyNameText.text = lobbyName;
                if(lobbyName.Contains("'S Lobby"))
                {
                    lobbyNameText.color = Color.green;
                    joinButton.interactable = true;
                }
                else
                {
                    lobbyNameText.color = Color.white;
                    joinButton.interactable = false;
                }
            
            }

        }

        public void JoinLobby()
        {
            SteamMatchmaking.JoinLobby(lobbySteamID);
        }
    }
}