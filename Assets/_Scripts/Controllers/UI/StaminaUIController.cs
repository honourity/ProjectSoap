using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUIController : MonoBehaviour
{
   [SerializeField] private RectTransform _background = null;
   [SerializeField] private Color _backgroundWarningColor = Color.red;
   [SerializeField] private float _backgroundWarningScale = 1.25f;

   private Slider _slider;

   private StaminaComponent _staminaComponent;
   private Image _backgroundImage;
   private Color _backgroundOriginalColor;
   private Vector3 _backgroundOriginalScale;

   private void Awake()
   {
      _slider = GetComponent<Slider>();

      _backgroundImage = _background.GetComponent<Image>();
      _backgroundOriginalColor = _backgroundImage.color;
      _backgroundOriginalScale = _background.localScale;

       var staminaGameObject = GameObject.FindGameObjectWithTag("Player");
      _staminaComponent = staminaGameObject.GetComponent<StaminaComponent>();
   }

   private void Update()
   {
      _slider.value = _staminaComponent?.Stamina ?? 0;
   }

   private void OnEnable()
   {
      Messaging.AddListener(Enums.MessageType.STAMINA_USE_FAILED, OnStaminaUseFailed);
   }

   private void OnDisable()
   {
      Messaging.RemoveListener(Enums.MessageType.STAMINA_USE_FAILED, OnStaminaUseFailed);
   }

   private void OnStaminaUseFailed()
   {
      //run a coroutine which flashes the stamina bar red?
      StopAllCoroutines();
      StartCoroutine(StaminaWarningCoroutine());
   }

   private IEnumerator StaminaWarningCoroutine()
   {
      var rate = 30f;

      _background.localScale = _backgroundOriginalScale;
      _backgroundImage.color = _backgroundOriginalColor;

      //do warning
      while (_backgroundImage.color != _backgroundWarningColor)
      {
         _background.localScale = new Vector3(Mathf.Lerp(_background.localScale.x, _backgroundOriginalScale.x * _backgroundWarningScale, Time.deltaTime * rate),
            Mathf.Lerp(_background.localScale.y, _backgroundOriginalScale.y  * _backgroundWarningScale * 2f, Time.deltaTime * rate * 2f)
            );
         _backgroundImage.color = Color.Lerp(_backgroundImage.color, _backgroundWarningColor, Time.deltaTime * rate);

         yield return new WaitForEndOfFrame();
      }

      _background.localScale = _backgroundOriginalScale;
      _backgroundImage.color = _backgroundOriginalColor;
   }
}
