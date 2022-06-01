using UnityEngine;

public class CameraController : MonoBehaviour
{
   Transform _target;

   private void Awake()
   {
      var targetGameObject = GameObject.FindGameObjectWithTag("Player");
      _target = targetGameObject?.transform;
   }

   private void LateUpdate()
   {
      if (_target != null) transform.position = new Vector3(_target.position.x, _target.position.y, transform.position.z);
   }
}
