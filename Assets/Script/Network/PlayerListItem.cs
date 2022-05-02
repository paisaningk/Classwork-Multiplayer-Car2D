using System;
using UnityEngine;
using Steamworks;
using TMPro;
using UnityEngine.UI;

namespace Script.Network
{
    public class PlayerListItem : MonoBehaviour
    {
        public string playerName;
        public int connectionID;
        public ulong playerSteamID;
        private bool avatarReceived = false;

        public TextMeshProUGUI playerNameText;
        public RawImage playerIcon;
        public TextMeshProUGUI playerReadyText;
        public bool isReady;

        protected Callback<AvatarImageLoaded_t> imageLoaded;

        private void Start()
        {
            imageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
        }

        public void ChangeReadyStatus()
        {
            if (isReady)
            {
                playerReadyText.text = "Ready";
                playerReadyText.color = Color.green;
            }
            else
            {
                playerReadyText.text = "Unready";
                playerReadyText.color = Color.red;
            }
        }


        public void SetPlayerValues()
        {
            playerNameText.text = playerName;
            ChangeReadyStatus();
            if (!avatarReceived)
            {
                GetPlayerIcon();
                
            }
        }

        private void GetPlayerIcon()
        {
            int imageID = SteamFriends.GetLargeFriendAvatar((CSteamID) playerSteamID);
            if (imageID == -1)
            {
                return;
            }
            playerIcon.texture = GetSteamImageAsTexture(imageID);
        }
        private void OnImageLoaded(AvatarImageLoaded_t callback)
        {
            if (callback.m_steamID.m_SteamID == playerSteamID) //player
            {
                playerIcon.texture = GetSteamImageAsTexture(callback.m_iImage);
            }
            else //another player
            {
                return;
            }
        }

        private Texture2D GetSteamImageAsTexture(int iImage)
        {
            Texture2D texture2D = null;
            bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
            if (isValid)
            {
                Byte[] image = new byte[width * height * 4];

                isValid = SteamUtils.GetImageRGBA(iImage, image, (int) (width * height * 4));

                if (isValid)
                {
                    texture2D = new Texture2D((int) width, (int) height, TextureFormat.RGBA32, false, true);
                    texture2D.LoadRawTextureData(image);
                    texture2D.Apply();
                }
            }

            avatarReceived = false;
            return texture2D;
        }
    }
}
