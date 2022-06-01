using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShowerController : MonoBehaviour
{
   [SerializeField] private float _showerTime = 1f;
   [SerializeField] private float _cleanExplosionRadius = 2f;

   private Animator _animator;

   public void UseShower(InventoryComponent inventoryComponent, HygieneComponent hygieneComponent, bool requireSoap = true)
   {
      var soaps = inventoryComponent.GetItemsOfType<SoapModel>();

      if (!requireSoap || soaps.Count > 0)
      {
         Messaging.SendMessage(Enums.MessageType.SHOWER_STARTED, soaps.Count);

         if (inventoryComponent?.GetComponent<PlayerController>() != null)
         {
            Messaging.SendMessage(Enums.MessageType.SHOWER_STARTED_PLAYER, soaps.Count);
         }

         foreach (var soap in soaps)
         {
            inventoryComponent.RemoveItem(soap);
         }

         StartCoroutine(UseShowerCoroutine(soaps, hygieneComponent, requireSoap));
      }
   }

   private void Awake()
   {
      _animator = GetComponent<Animator>();
   }

   private IEnumerator UseShowerCoroutine(IList<SoapModel> soaps, HygieneComponent hygieneComponent, bool requireSoap)
   {
      var hygieneToAdd = soaps.Sum(s => s.Hygiene);
      var hygieneTickAmount = hygieneToAdd / _showerTime;
      var timer = 0f;

      //hide the unit and disable movement is there is one
      var movementController = hygieneComponent?.GetComponent<MovementController>();
      if (movementController != null)
      {
         movementController.CanMove = false;
         movementController.SpecialMoveInProgress = true;
         movementController.gameObject.SetActive(false);
      }

      //refill stamina component if there is one
      var staminaComponent = hygieneComponent?.GetComponent<StaminaComponent>();
      if (staminaComponent != null)
      {
         staminaComponent.AddStamina(Constants.STAMINA_MAX);
      }

      //cleanliness explosion
      var hits = Physics2D.OverlapCircleAll(transform.position, _cleanExplosionRadius);
      foreach (var hit in hits)
      {
         var hitHygiene = hit.GetComponent<HygieneComponent>();
         if (hitHygiene != null)
         {
            hitHygiene.AddHygiene(Constants.HYGIENE_MAX);
         }
      }

      //start animation
      _animator?.SetBool("Showering", true);

      //showering loop
      while (timer < _showerTime)
      {
         //handle hygiene regen
         var hygieneTick = 0f;
         var hygieneTickAmountNormalized = hygieneTickAmount * Time.deltaTime;

         var hygieneTickDifference = hygieneToAdd - hygieneTickAmountNormalized;
         if (hygieneTickDifference < 0f)
         {
            hygieneTick = hygieneToAdd;
            hygieneToAdd = 0f;
         }
         else
         {
            hygieneTick = hygieneTickAmountNormalized;
            hygieneToAdd -= hygieneTickAmountNormalized;
         }
         
         hygieneComponent?.AddHygiene(hygieneTick);

         timer += Time.deltaTime;

         yield return null;
      }

      //stop animation
      _animator?.SetBool("Showering", false);

      //show the unit and enable movement if it has one
      if (movementController != null)
      {
         movementController.CanMove = true;
         movementController.SpecialMoveInProgress = false;
         movementController.gameObject.SetActive(true);
      }

      if (!requireSoap) hygieneComponent.AddHygiene(Constants.HYGIENE_MAX);

      Messaging.SendMessage(Enums.MessageType.SHOWER_FINISHED, soaps.Count);
   }
}
