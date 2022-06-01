using UnityEngine;

public class StaminaComponent : MonoBehaviour
{
   public float Stamina { get; private set; }

   [SerializeField, Range(0.01f, 5f)] private float _regenInSeconds = 1f;
   [SerializeField, Range(0, 5f)] private float _regenCooldownInSeconds = 0.5f;

   private float _timer = 0;

   /// <summary>
   /// 
   /// </summary>
   /// <param name="amount">quantity of stamina to consume</param>
   /// <returns>true if stamina was used, false if failure</returns>
   public bool UseStamina(float amount)
   {
      if (amount <= Stamina)
      {
         Stamina -= Mathf.Max(0, amount);
         _timer = 0;

         return true;
      }
      else
      {
         Messaging.SendMessage(Enums.MessageType.STAMINA_USE_FAILED);

         return false;
      }
   }

   public void AddStamina(float amount)
   {
      Stamina = Mathf.Min(Stamina + amount, Constants.STAMINA_MAX);
   }

   private void Awake()
   {
      Stamina = Constants.STAMINA_MAX;
   }

   private void Update()
   {
      if (_timer > _regenCooldownInSeconds && Stamina < Constants.STAMINA_MAX)
      {
         Stamina += (Constants.STAMINA_MAX * Time.deltaTime) / _regenInSeconds;
      }

      _timer += Time.deltaTime;
   }
}
