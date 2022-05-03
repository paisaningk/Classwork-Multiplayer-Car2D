using Script.Network;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
   public class SelectCharacterController : MonoBehaviour
   {
      public int currentIndex = 0;
      public Sprite[] playerCar;
      public RawImage currentImage;

      private void Start()
      {
         currentIndex = PlayerPrefs.GetInt("currentIndex", 0);
         currentImage.texture = playerCar[currentIndex].texture;
      }
   

      public void NextColor()
      {
         if (currentIndex < playerCar.Length - 1)
         {
            currentIndex++;
            currentImage.texture = playerCar[currentIndex].texture;
            LobbyController.instance.localPlayerController.CmdUpdatePlayerCarColor(currentIndex);
         }
         else
         {
            currentIndex = 0;
            currentImage.texture = playerCar[currentIndex].texture;
            LobbyController.instance.localPlayerController.CmdUpdatePlayerCarColor(currentIndex);
         }
      }

      public void PreviousColor()
      {
         if (currentIndex > 0)
         {
            currentIndex--;
            currentImage.texture = playerCar[currentIndex].texture;
            LobbyController.instance.localPlayerController.CmdUpdatePlayerCarColor(currentIndex);
         }
         else
         {
            currentIndex = playerCar.Length -1;
            currentImage.texture = playerCar[currentIndex].texture;
            LobbyController.instance.localPlayerController.CmdUpdatePlayerCarColor(currentIndex);
         }
      }
   }
}
