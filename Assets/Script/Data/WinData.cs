using System;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Data
{
    public class WinData : MonoBehaviour
    {
        public TextMeshProUGUI winRank;
        public TextMeshProUGUI namePlayer;
        public TextMeshProUGUI time;
        public ulong playerSteamID;
        private bool avatarReceived = false;
        public RawImage proFile;

         protected Callback<AvatarImageLoaded_t> imageLoaded;

        private void Start()
        {
            imageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
        }
        


        public void SetPlayerValues()
        {
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
            proFile.texture = GetSteamImageAsTexture(imageID);
        }
        private void OnImageLoaded(AvatarImageLoaded_t callback)
        {
            if (callback.m_steamID.m_SteamID == playerSteamID) //player
            {
                proFile.texture = GetSteamImageAsTexture(callback.m_iImage);
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