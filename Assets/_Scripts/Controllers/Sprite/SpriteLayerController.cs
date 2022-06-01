using UnityEngine;

public class SpriteLayerController : MonoBehaviour
{
   [SerializeField] public bool Dynamic = false;

   [SerializeField] private bool _includeChildren = true;
   private Renderer[] _renderers;

   private void Start()
   {
      Initialize();
   }

   private void LateUpdate()
   {
      if (Dynamic)
      {
         RefreshLayers();
      }
      else
      {
         RefreshLayers();
         enabled = false;
      }
   }

   public void Initialize()
   {
      if (_includeChildren)
      {
         _renderers = GetComponentsInChildren<Renderer>();
      }
      else
      {
         _renderers = new Renderer[1];
         _renderers[0] = GetComponent<Renderer>();
      }

   }

   public void RefreshLayers()
   {
      foreach (var renderer in _renderers)
      {
         if (renderer.sortingLayerID != Constants.SORTING_LAYER_BACKGROUND)
         {
            var collider = renderer.GetComponent<Collider2D>();
            renderer.sortingOrder = (collider != null && collider.enabled && !Dynamic) ? -(int)(collider.bounds.max.y * 100f) : -(int)(renderer.bounds.min.y * 100f);
         }
      }
   }
}
