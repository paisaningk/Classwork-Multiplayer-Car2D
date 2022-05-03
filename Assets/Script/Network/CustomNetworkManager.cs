using System.Collections.Generic;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Network
{
   public class CustomNetworkManager : NetworkManager
   {
      [SerializeField] private PlayerObjectLobby gamePlayerPrefab;
      public List<PlayerObjectLobby> gamePlayers { get; } = new List<PlayerObjectLobby>();

      public override void OnServerAddPlayer(NetworkConnectionToClient conn)
      {
         if (SceneManager.GetActiveScene().name == "Lobby")
         {
            PlayerObjectLobby gamePlayerInstance = Instantiate(gamePlayerPrefab);

            gamePlayerInstance.connectionID = conn.connectionId;
            gamePlayerInstance.playerIDNumber = gamePlayers.Count + 1;
            gamePlayerInstance.playerSteamID =
               (ulong) SteamMatchmaking.GetLobbyMemberByIndex((CSteamID) SteamLobby.instance.currentLobbyID,
                  gamePlayers.Count);

            NetworkServer.AddPlayerForConnection(conn, gamePlayerInstance.gameObject);
         }
      }
      
      public void StartGame(string SceneName)
      {
         ServerChangeScene(SceneName);
      }

      
      public void Logout()
      {
         StopHost();
         StopClient();
      }
   }
}
