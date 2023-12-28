using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourStatic<GameManager>
{
   public bool GameStarted { get; private set; }
   public bool GamePaused { get; private set; }

   public PlayerController Player { get; private set; }

   private void Awake()
   {
      Player = FindFirstObjectByType<PlayerController>();
   }

   private void Start()
   {
      PauseGame();
   }

   public void PauseGame()
   {
      //hack - this MIGHT cause issues with UI input?
      // shouldnt though, controller input should use UI auto navigation independent of InputManager
      InputManager.Instance.enabled = false;

      GamePaused = true;
      Time.timeScale = 0f;
      Messaging.SendMessage(Enums.MessageType.GAME_PAUSED);
   }

   public void GameOver()
   {
      PauseGame();
      Messaging.SendMessage(Enums.MessageType.GAME_OVER);
   }

   public void ResetGame()
   {
      PauseGame();

      //reload the scene
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);

      //Messaging.SendMessage(Enums.MessageType.GAME_RESET);
   }

   public void ResumeGame()
   {
      InputManager.Instance.enabled = true;
      GamePaused = false;
      Time.timeScale = 1f;
      Messaging.SendMessage(Enums.MessageType.GAME_RESUMED);

      if (!GameStarted)
      {
         Messaging.SendMessage(Enums.MessageType.GAME_STARTED);
         GameStarted = true;
      }
   }

}
