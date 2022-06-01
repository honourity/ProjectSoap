using UnityEngine;

public class GameStartUIController : MonoBehaviour
{
   [SerializeField] private GameObject _startMenu;

   public void OnClickStart()
   {
      GameManager.Instance.ResumeGame();
   }

   private void Awake()
   {
      Messaging.AddListener(Enums.MessageType.GAME_OVER, OnGameOver);
      Messaging.AddListener(Enums.MessageType.GAME_RESUMED, HideMenu);
      Messaging.AddListener(Enums.MessageType.GAME_PAUSED, ShowMenu);
   }

   private void OnDestroy()
   {
      Messaging.RemoveListener(Enums.MessageType.GAME_OVER, OnGameOver);
      Messaging.RemoveListener(Enums.MessageType.GAME_RESUMED, HideMenu);
      Messaging.RemoveListener(Enums.MessageType.GAME_PAUSED, ShowMenu);
   }

   private void HideMenu()
   {
      //hide everything
      _startMenu.SetActive(false);
   }

   private void ShowMenu()
   {
      //show everything
      _startMenu.SetActive(true);
   }

   private void OnGameOver()
   {
      HideMenu();
   }

}
