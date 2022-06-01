using UnityEngine;

public class AudioManager : MonoBehaviourStatic<AudioManager>
{
   [Header("Music")]
   [SerializeField] AudioClip _gameplayMusic = null;

   [Header("Voice Overs")]
   [SerializeField] AudioClip _start = null;
   [SerializeField] AudioClip _one = null;
   [SerializeField] AudioClip _two = null;
   [SerializeField] AudioClip _great = null;

   private AudioSource _soundEffectsAudioSource;
   private AudioSource _voiceOverAudioSource;
   private AudioSource _musicAudioSource;

   public void Play(AudioClip clip, float volumeScale = 1f)
   {
      if (clip != null) _soundEffectsAudioSource.PlayOneShot(clip, volumeScale);
   }

   private void Awake()
   {
      _soundEffectsAudioSource = gameObject.AddComponent<AudioSource>();
      _soundEffectsAudioSource.volume = 0.8f;

      _voiceOverAudioSource = gameObject.AddComponent<AudioSource>();
      _voiceOverAudioSource.volume = 0.1f;

      _musicAudioSource = gameObject.AddComponent<AudioSource>();
      _musicAudioSource.volume = 0.3f;
   }

   private void OnGameStarted()
   {
      _voiceOverAudioSource.PlayOneShot(_start);

      _musicAudioSource.clip = _gameplayMusic;
      _musicAudioSource.loop = true;
      _musicAudioSource.Play();
   }

   private void OnGamePaused()
   {
      _musicAudioSource.Pause();
   }

   private void OnGameResumed()
   {
      _musicAudioSource.UnPause();
   }

   private void OnShowerStarted(int count)
   {
      switch (count)
      {
         case 0:
            break;
         case 1:
            _voiceOverAudioSource.PlayOneShot(_one);
            break;
         case 2:
            _voiceOverAudioSource.PlayOneShot(_two);
            break;
         default:
            _voiceOverAudioSource.PlayOneShot(_great);
            break;
      }
   }

   private void OnEnable()
   {
      Messaging.AddListener(Enums.MessageType.GAME_STARTED, OnGameStarted);
      Messaging.AddListener(Enums.MessageType.GAME_PAUSED, OnGamePaused);
      Messaging.AddListener(Enums.MessageType.GAME_RESUMED, OnGameResumed);
      Messaging.AddListener<int>(Enums.MessageType.SHOWER_STARTED, OnShowerStarted);
   }

   private void OnDisable()
   {
      Messaging.RemoveListener(Enums.MessageType.GAME_STARTED, OnGameStarted);
      Messaging.RemoveListener(Enums.MessageType.GAME_PAUSED, OnGamePaused);
      Messaging.RemoveListener(Enums.MessageType.GAME_RESUMED, OnGameResumed);
      Messaging.RemoveListener<int>(Enums.MessageType.SHOWER_STARTED, OnShowerStarted);
   }
}
