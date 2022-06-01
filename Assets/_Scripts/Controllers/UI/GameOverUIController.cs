using UnityEngine;
using TMPro;

public class GameOverUIController : MonoBehaviour
{
   [SerializeField] private GameObject _displayContainer = null;

   [SerializeField] private TextMeshProUGUI _lastRunStatsText = null;
   [SerializeField] private TextMeshProUGUI _careerBestStatsText = null;

   public void OnClickReset()
   {
      GameManager.Instance.ResetGame();
   }

   private void Awake()
   {
      //hide everything
      _displayContainer?.SetActive(false);
   }

   private void OnGameOver()
   {
      //populate last run stats text
      _lastRunStatsText.text = string.Format("Clean streak: {0}\nLargest soap stack brought to shower: {1}\nPeople cleaned{2}\nTimes showered{3}",
         DataManager.Instance.LastRunStats?.CleanStreak,
         DataManager.Instance.LastRunStats?.LargestSoapStack,
         DataManager.Instance.LastRunStats?.PeopleCleaned,
         DataManager.Instance.LastRunStats?.TimesShowered);

      //populate best stats text
      _careerBestStatsText.text = string.Format("Clean streak: {0}\nLargest soap stack brought to shower: {1}\nPeople cleaned{2}\nTimes showered{3}",
         DataManager.Instance.CarreerStats?.CleanStreak,
         DataManager.Instance.CarreerStats?.LargestSoapStack,
         DataManager.Instance.CarreerStats?.PeopleCleaned,
         DataManager.Instance.CarreerStats?.TimesShowered);

      //show everything
      _displayContainer?.SetActive(true);
   }

   private void OnEnable()
   {
      Messaging.AddListener(Enums.MessageType.GAME_OVER, OnGameOver);
   }

   private void OnDisable()
   {
      Messaging.RemoveListener(Enums.MessageType.GAME_OVER, OnGameOver);
   }
}
