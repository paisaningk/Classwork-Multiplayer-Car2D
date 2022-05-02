using System;
using UnityEngine;

namespace Script.Network
{
    public class PlayerData : MonoBehaviour
    {
        public SpriteRenderer car;
        public Sprite[] carColor;

        public void UpdateColor()
        {
            car.sprite = carColor[GetComponent<PlayerObjectLobby>().playerCarColor];
        }
    }
}