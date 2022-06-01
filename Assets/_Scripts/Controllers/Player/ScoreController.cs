using UnityEngine;

public class ScoreController : MonoBehaviour
{
   public float Score { get; private set; }

   private bool _tickScore;

   private void Update()
   {
      if (_tickScore) Score += Time.deltaTime;
   }

   private void OnGameResumed()
   {
      _tickScore = true;
   }

   private void OnGamePaused()
   {
      _tickScore = false;
   }

   private void OnEnable()
   {
      Messaging.AddListener(Enums.MessageType.GAME_PAUSED, OnGamePaused);
      Messaging.AddListener(Enums.MessageType.GAME_RESUMED, OnGameResumed);
   }

   private void OnDisable()
   {
      Messaging.RemoveListener(Enums.MessageType.GAME_PAUSED, OnGamePaused);
      Messaging.RemoveListener(Enums.MessageType.GAME_RESUMED, OnGameResumed);
   }
}
