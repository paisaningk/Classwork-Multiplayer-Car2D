using System;
using Mirror;
using Steamworks;
using UnityEngine;

namespace Script.Network
{
    public class PlayerObjectLobby : NetworkBehaviour
    {
         //Player Data
        [SyncVar] public int connectionID;
        [SyncVar] public int playerIDNumber;
        [SyncVar] public int characterIndex;
        [SyncVar] public ulong playerSteamID;
        [SyncVar] public bool isFinish;
        [SyncVar] public float finishTimeInSec;

        [SyncVar(hook = nameof(PlayerNameUpdate))]
        public string playerName;
        [SyncVar(hook = nameof(PlayerReadyUpdate))]
        public bool isReady;

        [SyncVar(hook = nameof(SendPlayerCarColor))]
        public int playerCarColor;
       
    
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

        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        private void PlayerReadyUpdate(bool oldValue, bool newValue)
        {
            if (isServer)
            {
                this.isReady = newValue;
            }

            if (isClient)
            {
                LobbyController.instance.UpdatePlayerList();
            }
        }

        [Command]
        private void CmdPlayerReady()
        {
            this.PlayerReadyUpdate(this.isReady,!this.isReady);
        }

        public void ChangeReady()
        {
            if (hasAuthority)
            {
                CmdPlayerReady();
            }
        }

        public override void OnStartAuthority()
        {
            CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
            gameObject.name = "LocalGamePlayer";
            //LobbyController.instance.FindLocalPlayer();
            LobbyController.instance.FindLocalPlayer();
            LobbyController.instance.UpdateLobbyName();
            
        }

        public override void OnStartClient()
        {
            Manager.gamePlayers.Add(this);
            LobbyController.instance.UpdateLobbyName();
            LobbyController.instance.UpdatePlayerList();
           
        }

        public override void OnStopClient()
        {
            Manager.gamePlayers.Remove(this);
            LobbyController.instance.UpdatePlayerList();
        }

        [Command]
        private void CmdSetPlayerName(string playerName)
        {
            this.PlayerNameUpdate(this.playerName,playerName);
        }
        
        public void PlayerNameUpdate(string oldValue, string newValue)
        {
            if (isServer) //Host
            {
                this.playerName = newValue;
            }

            if (isClient)
            {
                LobbyController.instance.UpdatePlayerList();
            }
        }

        public void CanStartGame(string sceneName)
        {
            if (hasAuthority)
            {
                CmdCanStartGame(sceneName);
            }
        }

        [Command]
        private void CmdCanStartGame(string sceneName)
        {
            Manager.StartGame(sceneName);
        }


        [Command]
        public void CmdUpdatePlayerCarColor(int newValue)
        {
            SendPlayerCarColor(playerCarColor,newValue);
        }

        public void SendPlayerCarColor(int oldValue, int newValue)
        {
            if (isServer)
            {
                playerCarColor = newValue;
            }

            if (isClient && (oldValue != newValue))
            {
                UpdateCarColor(newValue);
            }
        }

        public void UpdateCarColor(int message)
        {
            playerCarColor = message;
        }

        [Command]
        public void SetIsFinish(bool finish)
        {
            isFinish = finish;
        }

        [Command]
        public void SetTime(float time)
        {
            finishTimeInSec = time;
        }
    }
}
