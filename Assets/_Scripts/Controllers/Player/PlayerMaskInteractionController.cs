using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMaskInteractionController : MonoBehaviour
{
   private SpriteRenderer _spriteRenderer;
   private MovementController _moveController;

   private void Awake()
   {
      _spriteRenderer = GetComponent<SpriteRenderer>();
      _moveController = GetComponent<MovementController>();
   }

   private void LateUpdate()
   {
      if (_moveController.CurrentMoveType == Enums.MoveType.DASH_WATER)
      {
         _spriteRenderer.maskInteraction = SpriteMaskInteraction.None;
      }
   }

   private void OnTriggerStay2D(Collider2D collision)
   {
      if (enabled)
      {
         var waterSource = collision.GetComponent<WaterSourceController>();
         if (waterSource != null)
         {
            _spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
         }
      }
   }

   private void OnTriggerExit2D(Collider2D collision)
   {
      if (enabled)
      {
         var waterSource = collision.GetComponent<WaterSourceController>();
         if (waterSource != null)
         {
            _spriteRenderer.maskInteraction = SpriteMaskInteraction.None;
         }
      }
   }
}
