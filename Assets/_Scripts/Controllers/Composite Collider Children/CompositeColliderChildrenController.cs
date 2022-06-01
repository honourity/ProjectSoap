using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeColliderChildrenController : MonoBehaviour
{
   [SerializeField] private bool _destroyOnAwake = true;

   private void Awake()
   {
      if (_destroyOnAwake)
         Destroy(this);
   }

   public void DisableUsingComposite()
   {
      SetUsingComposite(false);
   }

   public void EnableUsingComposite()
   {
      SetUsingComposite(true);
   }

   private void SetUsingComposite(bool enabled)
   {
      Collider2D[] allCollider2Ds = GetComponentsInChildren<Collider2D>();

      //Start at 1 so it doesn't include itself
      for (int i = 1; i < allCollider2Ds.Length; i++)
      {
         if (allCollider2Ds[i].GetType() != typeof(CompositeCollider2D))
            allCollider2Ds[i].usedByComposite = enabled;
      }
   }
}
