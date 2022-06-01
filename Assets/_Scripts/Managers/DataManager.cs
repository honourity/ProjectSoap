using UnityEngine;

public class DataManager : MonoBehaviourStatic<DataManager>
{
   private Stats _carreerStats = new Stats();
   public Stats CarreerStats
   {
      get
      {
         return GetLatestCarrerStats();
      }
   }

   private Stats _lastRunStats = new Stats();
   public Stats LastRunStats
   {
      get
      {
         return GetLatestLastRunStats();
      }
   }

   private void Awake()
   {
      //float CleanStreak;
      //int LargestSoapStack;
      //int PeopleCleaned;
      //int TimesShowered;

      _carreerStats.CleanStreak = PlayerPrefs.GetFloat("CleanStreak", 0);
      _carreerStats.LargestSoapStack = PlayerPrefs.GetInt("LargestSoapStack", 0);
      _carreerStats.PeopleCleaned = PlayerPrefs.GetInt("PeopleCleaned", 0);
      _carreerStats.TimesShowered = PlayerPrefs.GetInt("TimesShowered", 0);
   }

   private void SaveData()
   {
      PlayerPrefs.SetFloat("CleanStreak", _carreerStats.CleanStreak);
      PlayerPrefs.SetInt("LargestSoapStack", _carreerStats.LargestSoapStack);
      PlayerPrefs.SetInt("PeopleCleaned", _carreerStats.PeopleCleaned);
      PlayerPrefs.SetInt("TimesShowered", _carreerStats.TimesShowered);
   }

   private Stats GetLatestCarrerStats()
   {
      _carreerStats.CleanStreak = Mathf.Max(_carreerStats.CleanStreak, _lastRunStats.CleanStreak);
      _carreerStats.LargestSoapStack = Mathf.Max(_carreerStats.LargestSoapStack, _lastRunStats.LargestSoapStack);
      _carreerStats.PeopleCleaned = Mathf.Max(_carreerStats.PeopleCleaned, _lastRunStats.PeopleCleaned);
      _carreerStats.TimesShowered = Mathf.Max(_carreerStats.TimesShowered, _lastRunStats.TimesShowered);

      SaveData();

      return _carreerStats;
   }

   private Stats GetLatestLastRunStats()
   {
      _lastRunStats.CleanStreak = GameManager.Instance.Player?.GetComponent<ScoreController>()?.Score ?? 0;

      return _lastRunStats;
   }

   private void OnGameStarted()
   {
      _lastRunStats.CleanStreak = 0;
      _lastRunStats.LargestSoapStack = 0;
      _lastRunStats.PeopleCleaned = 0;
      _lastRunStats.TimesShowered = 0;
   }

   private void OnShowerStartedPlayer(int soap)
   {
      _lastRunStats.TimesShowered++;
      _lastRunStats.LargestSoapStack = Mathf.Max(_lastRunStats.LargestSoapStack, soap);
   }

   private void OnNpcCleanedBySoap()
   {
      _lastRunStats.PeopleCleaned++;
   }

   private void OnEnable()
   {
      Messaging.AddListener(Enums.MessageType.GAME_STARTED, OnGameStarted);
      Messaging.AddListener<int>(Enums.MessageType.SHOWER_STARTED_PLAYER, OnShowerStartedPlayer);
      Messaging.AddListener(Enums.MessageType.NPC_CLEANED_BY_SOAP, OnNpcCleanedBySoap);
   }

   private void OnDisable()
   {
      Messaging.RemoveListener(Enums.MessageType.GAME_STARTED, OnGameStarted);
      Messaging.RemoveListener<int>(Enums.MessageType.SHOWER_STARTED_PLAYER, OnShowerStartedPlayer);
      Messaging.RemoveListener(Enums.MessageType.NPC_CLEANED_BY_SOAP, OnNpcCleanedBySoap);
   }
}

public class Stats
{
   public float CleanStreak;
   public int LargestSoapStack;
   public int PeopleCleaned;
   public int TimesShowered;
}