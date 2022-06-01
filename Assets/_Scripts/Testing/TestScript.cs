using UnityEngine;
using System.Collections.Generic;

public class TestScript : MonoBehaviour
{
   [SerializeField] private GameObject _prefab;
   private ObjectPool<GameObject> _objectPool;

   private void Start()
   {
      _objectPool = new ObjectPool<GameObject>(() => Instantiate(_prefab), 5);
   }

   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.Space))
      {
         Destroy(_objectPool.GetItem());
      }
   }

}