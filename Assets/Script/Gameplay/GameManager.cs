using System;
using System.Collections.Generic;
using Mirror;
using Script.Car;
using Script.Data;
using Script.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
    public class GameManager : NetworkBehaviour
    {
        [Header("Gameplay")]
        public GameObject[] spawnPoint;
        public GameObject[] checkPoint;
        public GameObject[] players;
        public GameObject player;
        public bool allWin = false;
        public bool finishLine = false;
        [SyncVar] public float time;
        public List<GameObject> listWinInContent;
        public AudioSource winAudioSource;
        public bool isEveryoneReady = false;
        public bool isEveryoneCanGo = false;
        [SyncVar]public float timeCoolDown = 5;
        
        [Header("Gameplay UI")]
        public TextMeshProUGUI timeText;
        public TextMeshProUGUI lapsText;
        public GameObject startText;
        [Header("Finish Line UI")]
        public GameObject win;
        public GameObject winViewContent;
        public GameObject winListPrefab;
        public Button quitButton;
        
        void Start()
        {
            Destroy (GameObject.FindGameObjectWithTag("Music"));

            SetPosition();

            SetCheckpoint();
            
            timeText.text = $"{time:m\\:ss\\.fff}";
            lapsText.text = $"{player.GetComponent<CarController>().currentIndexCheckPoint} / {checkPoint.Length} Laps";
        }

        private void Update()
        {
            EveryoneReady();
            TimerAndLaps();
            CheckIsFinishLine();
            CheckWin();

            if (allWin)
            {
                quitButton.interactable = enabled;
            }
        }

        private void SetPosition()
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < players.Length; i++)
            {
                players[i].transform.position = spawnPoint[i].transform.position;
                var playerCarController = players[i].GetComponent<CarController>();
                playerCarController.checkPoints = checkPoint;
                playerCarController.isSetup = true;
                if (playerCarController.isLocalPlayer)
                {
                    player = players[i];
                }
            }
        }

        private void SetCheckpoint()
        {
            for (int i = 1; i < checkPoint.Length ; i++)
            {
                checkPoint[i].SetActive(false);
            }
        }

        private void EveryoneReady()
        {
            if (!isEveryoneCanGo)
            {
                foreach (var player in players)
                {
                    if (player.GetComponent<CarController>().isSetup)
                    {
                        isEveryoneReady = true;
                    }
                }

                if (isEveryoneReady)
                {
                    timeCoolDown -= Time.deltaTime;
                    startText.GetComponent<TextMeshProUGUI>().text = $"{(int)timeCoolDown}";
                }

                if (timeCoolDown < 0)
                {
                    foreach (var player in players)
                    {
                        player.GetComponent<CarController>().isCanGo = true;
                    }
                    startText.SetActive(false);
                    isEveryoneCanGo = true;
                    winAudioSource.Play();
                }
            }
        }

        private void TimerAndLaps()
        {
            if (isEveryoneCanGo)
            {
                time += Time.deltaTime;
                TimeSpan timeSpan = TimeSpan.FromSeconds(time);
                timeText.text = $"{timeSpan:m\\:ss\\.fff}";
                lapsText.text = $"{player.GetComponent<CarController>().currentIndexCheckPoint} / {checkPoint.Length} Laps";
            }
        }

        private void CheckIsFinishLine()
        {
            //if (carController.isFinishLine)
            {
                foreach (var VARIABLE in listWinInContent)
                {
                    Destroy(VARIABLE);
                }
            
                listWinInContent.Clear();
                
                List<PlayerObjectLobby> finishedPlayers = new List<PlayerObjectLobby>();
                foreach (var player in players)
                {
                    var a = player.GetComponent<PlayerObjectLobby>();
                    if (a.isFinish)
                    {
                        finishedPlayers.Add(a);
                    }
                }

                finishedPlayers.Sort( (player1,player2) => player1.finishTimeInSec.CompareTo (player2.finishTimeInSec));
                int rank = 1;
                foreach (PlayerObjectLobby player in finishedPlayers)
                {
                    CreateWinListInContent(player.GetComponent<PlayerObjectLobby>(),rank++);
                }
            }
            
            var carController = player.GetComponent<CarController>();
            if (carController.isFinishLine&&!finishLine)
            {
                finishLine = true;
                win.SetActive(true);
                player.GetComponent<PlayerObjectLobby>().SetIsFinish(true); 
                player.GetComponent<PlayerObjectLobby>().SetTime(time);
                winAudioSource.Play();
                //Debug.Log($"{player.name} {player.GetComponent<PlayerObjectLobby>().isFinish} , {player.GetComponent<PlayerObjectLobby>().finishTimeInSec = time}");
            }
        }

        private void CheckWin()
        {
            allWin = true;
            foreach (PlayerObjectLobby player in LobbyController.instance.Manager.gamePlayers)
            {
                if (!player.isFinish)
                {
                    allWin = false;
                }
            }
        }
        
        private void CreateWinListInContent(PlayerObjectLobby playerObjectLobby,int winRank)
        {
            GameObject newPlayerItem = Instantiate(winListPrefab);
            WinData newPlayerItemScript = newPlayerItem.GetComponent<WinData>();
            
            newPlayerItemScript.namePlayer.text = playerObjectLobby.playerName;
            newPlayerItemScript.winRank.text = $"{winRank}";
            newPlayerItemScript.playerSteamID = playerObjectLobby.playerSteamID;
            
            TimeSpan timeSpan = TimeSpan.FromSeconds(playerObjectLobby.finishTimeInSec);
            newPlayerItemScript.time.text = $"Time : {timeSpan:m\\:ss\\.fff}";

            newPlayerItemScript.SetPlayerValues();
            
            newPlayerItem.transform.SetParent(winViewContent.transform);
            newPlayerItem.transform.localScale = Vector3.one;
            
            listWinInContent.Add(newPlayerItem);
        }

        public void Quit()
        {
            LobbyController.instance.Manager.Logout();
        }
    }
}


