using UnityEngine;
using UnityEngine.UI;

public class EventsOverlayUIController : MonoBehaviour
{
   private Text _text;
   private Animation _animation;

   private void Awake()
   {
      _text = GetComponent<Text>();
      _animation = GetComponent<Animation>();
   }

   private void EventTriggered(string name)
   {
      _text.text = name;
      _animation.Stop();
      _animation.Play();
   }

   private void OnShowerStarted(int soapCount)
   {
      if (soapCount > 0)
      {
         EventTriggered("SHOWERED!");
      }
   }

   private void OnGameStarted()
   {
      EventTriggered("START!");
   }

   private void OnEnable()
   {
      Messaging.AddListener<int>(Enums.MessageType.SHOWER_STARTED, OnShowerStarted);
      Messaging.AddListener(Enums.MessageType.GAME_STARTED, OnGameStarted);
   }

   private void OnDisable()
   {
      Messaging.RemoveListener<int>(Enums.MessageType.SHOWER_STARTED, OnShowerStarted);
      Messaging.RemoveListener(Enums.MessageType.GAME_STARTED, OnGameStarted);
   }
}
