using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PixelPerfect2DCamera : MonoBehaviour
{
   [SerializeField] private int _targetPPU = 32;
   [Range(2, 16)]
   [SerializeField]
   private int _pixelScale = 1;
   private Camera _camera;


   void Update()
   {
      if (_camera == null)
         _camera = GetComponent<Camera>();

      UpdateOrthographicSize();
   }

   private void UpdateOrthographicSize()
   {
      _camera.orthographicSize = Screen.height / (_targetPPU * _pixelScale) / 2f;
   }
}
