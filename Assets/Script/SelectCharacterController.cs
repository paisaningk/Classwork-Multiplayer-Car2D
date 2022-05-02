using Script.Network;
using UnityEngine;
using UnityEngine.UI;

namespace Script
{
   public class SelectCharacterController : MonoBehaviour
   {
      public static int currentIndex = 0;
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
            //PlayerPrefs.GetInt("currentIndex", currentIndex);
            currentImage.texture = playerCar[currentIndex].texture;
            LobbyController.instance.localPlayerController.CmdUpdatePlayerCarColor(currentIndex);
         }
         else
         {
            PreviousColor();
         }
      }

      public void PreviousColor()
      {
         if (currentIndex > 0)
         {
            currentIndex--;
            //PlayerPrefs.GetInt("currentIndex", currentIndex);
            currentImage.texture = playerCar[currentIndex].texture;
            LobbyController.instance.localPlayerController.CmdUpdatePlayerCarColor(currentIndex);
         }
         else
         {
            NextColor();
         }
      }
   }
}
