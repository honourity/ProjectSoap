using UnityEngine;

public class HygieneComponent : MonoBehaviour
{
   public float Hygiene { get; private set; }

   public bool IsDirty { get { return Hygiene < 0f; } }

   [SerializeField] private float _startingHygiene = Constants.HYGIENE_MAX;

   private Animator[] _animators;
   private int _animatorDirtyLayerIndex;

   public bool RemoveHygiene(float amount)
   {
      if (amount <= Hygiene)
      {
         Hygiene -= amount;
         Messaging.SendMessage(Enums.MessageType.HYGIENE_CHANGED, this);

         ApplyDirtyStatusToAnimator();

         return true;
      }
      else
      {
         return false;
      }
   }

   public float AddHygiene(float amount)
   {
      var originalHygiene = Hygiene;
      Hygiene = Mathf.Min(Hygiene + amount, Constants.HYGIENE_MAX);

      Messaging.SendMessage(Enums.MessageType.HYGIENE_CHANGED, this);

      ApplyDirtyStatusToAnimator();

      return (Hygiene == Constants.HYGIENE_MAX) ? Constants.HYGIENE_MAX - originalHygiene : amount;
   }

   private void Awake()
   {
      Hygiene = _startingHygiene;

      _animators = GetComponentsInChildren<Animator>();
      foreach (var animator in _animators)
      {
         if (animator != null) _animatorDirtyLayerIndex = animator?.GetLayerIndex("Dirty") ?? -1;
      }

      ApplyDirtyStatusToAnimator();
   }

   private void ApplyDirtyStatusToAnimator()
   {
      var dirtyLayerWeight = 0f;

      if (Hygiene < 0f) dirtyLayerWeight = 1f;

      foreach (var animator in _animators)
      {
         if (animator != null && animator.gameObject.activeInHierarchy && _animatorDirtyLayerIndex != -1) animator.SetLayerWeight(_animatorDirtyLayerIndex, dirtyLayerWeight);
      }
   }
}
