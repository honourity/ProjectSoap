using UnityEngine;

public class InputManager : MonoBehaviourStatic<InputManager>
{
   PlayerController _playerController;

   private void Awake()
   {
      _playerController = FindFirstObjectByType<PlayerController>();
   }

   private void Update()
   {
      //getting input
      var northButton = Input.GetKey(KeyCode.W);
      var westButton = Input.GetKey(KeyCode.A);
      var southButton = Input.GetKey(KeyCode.S);
      var eastButton = Input.GetKey(KeyCode.D);

      var attackButton = Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0);

      var balanceSoapButton = Input.GetKey(KeyCode.K) || Input.GetMouseButton(1);

      var dashButton = Input.GetKeyDown(KeyCode.Space);

      #region MOVEMENT

      //defaults
      var moveType = Enums.MoveType.RUN_LAND;
      var moveDirection = Vector2.zero;

      //directions
      if (northButton && eastButton)
      {
         moveDirection = Vector2.up + Vector2.right;
      }
      else if (southButton && eastButton)
      {
         moveDirection = Vector2.down + Vector2.right;
      }
      else if (southButton && westButton)
      {
         moveDirection = Vector2.down + Vector2.left;
      }
      else if (westButton && northButton)
      {
         moveDirection = Vector2.up + Vector2.left;
      }
      else if (northButton)
      {
         moveDirection = Vector2.up;
      }
      else if (eastButton)
      {
         moveDirection = Vector2.right;
      }
      else if (southButton)
      {
         moveDirection = Vector2.down;
      }
      else if (westButton)
      {
         moveDirection = Vector2.left;
      }
      else
      {
         moveType = Enums.MoveType.NONE;
      }

      //dash
      if (dashButton)
      {
         moveType = Enums.MoveType.DASH_LAND;
      }

      #endregion
   
      //attack
      if (attackButton)
      {
         moveType = Enums.MoveType.ATTACK;
      }

      if (balanceSoapButton)
      {
         _playerController?.BalanceSoap();
      }

      _playerController?.Move(moveType, moveDirection);

      //debugging input
      var slowmo = Input.GetKey(KeyCode.Q);
      if (slowmo)
      {
         Time.timeScale = 0.1f;
      }
      else
      {
         Time.timeScale = 1f;
      }
   }
}
