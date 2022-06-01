using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SpriteLayerController))]
public class SpriteLayerControllerEditor : Editor
{
   private SpriteLayerController _spriteLayerController;
   private GameObject _gameObject;
   private SerializedProperty _dynamicProp;

   private void OnEnable()
   {
      _spriteLayerController = (SpriteLayerController)target;
      _gameObject = _spriteLayerController.gameObject;

      _spriteLayerController.Initialize();
      EditorApplication.update -= OnUpdate;
      EditorApplication.update += OnUpdate;
   }

   private void OnDisable()
   {
      EditorApplication.update -= OnUpdate;
   }

   private void OnUpdate()
   {
      if (Selection.Contains(_gameObject))
      {
         _spriteLayerController.RefreshLayers();
      }
   }

   public override void OnInspectorGUI()
   {
      DrawDefaultInspector();
   }

}
