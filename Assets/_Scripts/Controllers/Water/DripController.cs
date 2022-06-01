using System.Linq;
using UnityEngine;

public class DripController : MonoBehaviour
{
   public float VolumePerSecond = 3f;

   [SerializeField] private DropletController _dropletPrefab = null;

   private SpriteRenderer _spriteRenderer;
   private InventoryComponent _inventory;
   private HygieneComponent _hygieneComponent;
   private float _timer;
   private float _dropletVolume = 1f;

   private void Awake()
   {
      _spriteRenderer = GetComponent<SpriteRenderer>();
      _inventory = GetComponent<InventoryComponent>();
      _hygieneComponent = GetComponent<HygieneComponent>();
   }

   private void Update()
   {
      //only do stuff if there is water in inventory
      var waterItems = _inventory?.GetItemsOfType<WaterModel>();
      if (waterItems?.Count > 0 && waterItems?.Sum(w => w.Volume) >= _dropletVolume)
      {
         _timer += Time.deltaTime;
         var cooldown = _dropletVolume / VolumePerSecond;

         if (_timer > cooldown)
         {
            //remove necessary water from inventory
            var volumeToRemove = VolumePerSecond * _timer;
            foreach (var waterItem in waterItems)
            {
               if (volumeToRemove > 0)
               {
                  volumeToRemove -= waterItem.RemoveWater(volumeToRemove);

                  if (waterItem.Volume == 0f) _inventory.RemoveItem(waterItem);
               }
               else
               {
                  //we removed all the water we want, so stop looping through inventory waters
                  break;
               }
            }

            var volumeRemoved = (VolumePerSecond * _timer) - volumeToRemove;

            //spawn droplet
            var spawnPosition = transform.position;
            if (_spriteRenderer != null)
            {
               spawnPosition = new Vector2(_spriteRenderer.bounds.center.x + (_spriteRenderer.bounds.extents.x * Random.Range(-0.5f, 0.5f)), _spriteRenderer.bounds.center.y + (_spriteRenderer.bounds.extents.y * Random.Range(-0.5f, 0.5f)));
            }
            var instance = Instantiate(_dropletPrefab, spawnPosition, Quaternion.identity, null);
            instance.Initialize(spawnPosition.y - (_spriteRenderer.bounds.center.y - _spriteRenderer.bounds.extents.y), volumeRemoved, _hygieneComponent?.Hygiene ?? Constants.HYGIENE_MAX);

            _timer = 0f;
         }
      }
      else
      {
         _timer = 0f;
      }
   }
}
