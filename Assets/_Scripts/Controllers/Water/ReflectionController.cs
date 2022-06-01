using UnityEngine;
using UnityEngine.Tilemaps;

public class ReflectionController : MonoBehaviour
{
   private float _reflectionAlphaFactor = 0.5f;

   private void Awake()
   {
      var reflectionGameObject = new GameObject("ReflectionOf" + gameObject.name);
      reflectionGameObject.transform.parent = transform;
      reflectionGameObject.transform.position = transform.position;

      //attempt to get all the things
      var mainSpriteRenderer = GetComponent<SpriteRenderer>();

      if (mainSpriteRenderer != null)
      {
         var reflectionSpriteRenderer = reflectionGameObject.AddComponent<SpriteRenderer>();

         var localPivot = mainSpriteRenderer.sprite.pivot / mainSpriteRenderer.sprite.pixelsPerUnit;
         reflectionSpriteRenderer.sprite = mainSpriteRenderer.sprite;
         reflectionSpriteRenderer.color = new Color(mainSpriteRenderer.color.r, mainSpriteRenderer.color.g, mainSpriteRenderer.color.b, mainSpriteRenderer.color.a * _reflectionAlphaFactor);
         reflectionSpriteRenderer.flipX = mainSpriteRenderer.flipX;
         reflectionSpriteRenderer.flipY = mainSpriteRenderer.flipY;
         reflectionSpriteRenderer.material = mainSpriteRenderer.material;
         reflectionSpriteRenderer.material.shader = Shader.Find("ProjectSoap/Reflection");

         reflectionSpriteRenderer.flipY = !reflectionSpriteRenderer.flipY;
         reflectionSpriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
         reflectionSpriteRenderer.sortingLayerID = Constants.SORTING_LAYER_BACKGROUND;
         reflectionSpriteRenderer.sortingOrder = Constants.SORTING_ORDER_PUDDLE + 1;

         reflectionGameObject.transform.position -= new Vector3(0f, localPivot.y * 2f, 0f);
      }
      else
      {
         var mainTilemapRenderer = GetComponent<TilemapRenderer>();
         var mainTilemap = GetComponent<Tilemap>();

         if (mainTilemap != null && mainTilemapRenderer != null)
         {
            var tilemap = reflectionGameObject.AddComponent<Tilemap>();

            var localPivot = mainTilemap.localBounds.center;

            tilemap.transform.Rotate(new Vector3(180f, 0, 0));
            tilemap.animationFrameRate = mainTilemap.animationFrameRate;
            tilemap.color = new Color(mainTilemap.color.r, mainTilemap.color.g, mainTilemap.color.b, mainTilemap.color.a * _reflectionAlphaFactor);
            tilemap.SetTilesBlock(mainTilemap.cellBounds, mainTilemap.GetTilesBlock(mainTilemap.cellBounds));

            var tilemapRenderer = reflectionGameObject.AddComponent<TilemapRenderer>();
            tilemapRenderer.material = mainTilemapRenderer.material;
            tilemapRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            tilemapRenderer.sortingLayerID = Constants.SORTING_LAYER_BACKGROUND;

            //todo - find a section of sorting orders to safely programatically set order between reflections
            tilemapRenderer.sortingOrder = Constants.SORTING_ORDER_PUDDLE +2;

            //todo - make this work in all cases, not just this one case
            //hack - hacky and shortsighted. because tileset might be changing
            reflectionGameObject.transform.position += new Vector3(0f, localPivot.y * 3f, 0f);
         }
      }

      var mainAnimator = GetComponent<Animator>();
      if (mainAnimator)
      {
         var reflectionAnimator = reflectionGameObject.AddComponent<Animator>();
         reflectionAnimator.runtimeAnimatorController = mainAnimator.runtimeAnimatorController;
         
         //so animator frame events dont cause errors
         reflectionGameObject.AddComponent<FakeMovementController>();
      }
   }
}
