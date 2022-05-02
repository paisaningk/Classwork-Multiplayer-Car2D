using System;
using Mirror;
using Script.Car;
using TMPro;
using UnityEngine;

namespace Script
{
    public class GameManager : NetworkBehaviour
    {
        [Header("Gameplay")]
        public GameObject[] spawnPoint;
        public GameObject[] checkPoint;
        public GameObject[] players;
        public GameObject win;
        public GameObject player;
        public bool allWin = false;
        [SyncVar] public float time;
    
        [Header("UI")]
        [Header("UI")]
        public TextMeshProUGUI timeText;
        public TextMeshProUGUI lapsText;
    

        void Start()
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
            for (int i = 1; i < checkPoint.Length ; i++)
            {
                checkPoint[i].SetActive(false);
            }
        }

        private void Update()
        {
            TimerAndLaps();
            CheckIsFinishLine();
            CheckWin();
        }

        private void TimerAndLaps()
        {
            time += Time.deltaTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            timeText.text = $"{timeSpan:m\\:ss\\.fff}";
            lapsText.text = $"{player.GetComponent<CarController>().currentIndexCheckPoint} / {checkPoint.Length}";
        }

        private void CheckIsFinishLine()
        {
            var carController = player.GetComponent<CarController>();
            if (carController.isFinishLine)
            {
                win.SetActive(true);
            }
        }

        private void CheckWin()
        {
            foreach (var player in players)
            {
                var carController = player.GetComponent<CarController>();
                if (carController.isFinishLine)
                {
                    allWin = true;
                }
                else
                {
                    allWin = false;
                }
            }
        }
    }
}
