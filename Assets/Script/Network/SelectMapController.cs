using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Network
{
    [System.Serializable]
    public class Map
    {
        public string mapName;
        public Sprite mapSprites;
    }
    public class SelectMapController : NetworkBehaviour
    {
        public Map[] map;

        [Header("Var Select Map")]
        public int currentIndex = 0;
        [SyncVar]public string nameMap;
        public RawImage currentImage;
        
        [Header("Button")]
        public Button nextButton;
        public Button previousButton;
        public Button startGameButton;

        private void Start()
        {
            currentImage.texture = map[currentIndex].mapSprites.texture;
            nameMap = map[currentIndex].mapName;
            nextButton.onClick.AddListener(NextMap);
            previousButton.onClick.AddListener(PreviousMap);
            startGameButton.onClick.AddListener(StartGame);

            if (!isClientOnly) return;
            nextButton.interactable = false;
            previousButton.interactable = false;

        }

        private void StartGame()
        {
            LobbyController.instance.Manager.StartGame(nameMap);
        }


        private void NextMap()
        {
            if (currentIndex <= map.Length -1)
            {
                currentIndex++;
                currentImage.texture = map[currentIndex].mapSprites.texture;
                nameMap = map[currentIndex].mapName;
            }
            else
            {
                currentIndex = 0;
                currentImage.texture = map[currentIndex].mapSprites.texture;
                nameMap = map[currentIndex].mapName;
            }
        }

        private void PreviousMap()
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                currentImage.texture = map[currentIndex].mapSprites.texture;
                nameMap = map[currentIndex].mapName;
            }
            else
            {
                currentIndex = map.Length -1;
                currentImage.texture = map[currentIndex].mapSprites.texture;
                nameMap = map[currentIndex].mapName;
            }
        }
    }
}