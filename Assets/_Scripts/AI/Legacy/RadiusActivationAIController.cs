using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Obsolete("Not really used anymore")]
public class RadiusActivationAIController : MonoBehaviour
{
   [SerializeField] UnityEvent _activateAction = null;
   [SerializeField] UnityEvent _deactivateAction = null;

   [SerializeField] private float _radius = 3f;
   [SerializeField] private bool _deactivateOnExit = false;
   [SerializeField] Vector2 _radiusOffset = Vector2.zero;

   private bool _isActivated;
   private Transform _player;
   private Vector2 radiusCenter;
   private float _distanceToPlayer;

   private float _initialRadius;

   private void Awake()
   {
      _initialRadius = _radius;
   }

   private void Start()
   {
      _player = FindObjectOfType<SoapThrowController>().transform;
   }

   void Update()
   {
      radiusCenter = (Vector2)transform.position + _radiusOffset;
      _distanceToPlayer = Vector2.Distance(radiusCenter, _player.position);

      if (!_isActivated && (_distanceToPlayer <= _radius))
      {
         _isActivated = true;
         _activateAction.Invoke();
      }

      else if (_deactivateOnExit && _isActivated && _distanceToPlayer > _radius)
      {
         _isActivated = false;
         _deactivateAction.Invoke();
      }
   }

   private void OnDrawGizmos()
   {
      radiusCenter = (Vector2)transform.position + _radiusOffset;
      DebugExtension.DrawPoint(radiusCenter, Color.yellow, 0.25f);
      DebugExtension.DrawCircle(radiusCenter, Vector3.back, Color.yellow, _radius);
   }

   public void ResetRadius()
   {
      _radius = _initialRadius;
   }

   public void SetRadius(float radius)
   {
      _radius = radius;
   }
}
