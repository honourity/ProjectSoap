using UnityEngine;

public class DropletController : MonoBehaviour
{
   [SerializeField] PuddleController _puddlePrefab = null;

   public float Volume { get; private set; }
   public float Hygiene { get; private set; }

   private float _fallSpeed = 1f;
   private float _fallDistance;
   private Vector2 _spawnLocation;
   private bool _flaggedForDestroy;

   public void Initialize(float fallDistance, float volume, float hygiene = Constants.HYGIENE_MAX)
   {
      _fallDistance = fallDistance;
      Hygiene = hygiene;
      Volume = volume;
   }

   private void Awake()
   {
      _spawnLocation = transform.position;
   }

   private void Update()
   {
      if (!_flaggedForDestroy)
      {
         //has to check for collision, and if the distance is enough, collide, add volume to collision, and destroy self

         if (Vector3.Distance(transform.position, _spawnLocation) > _fallDistance)
         {
            //instantiate a puddle based on current volume
            var instance = Instantiate(_puddlePrefab, transform.position, Quaternion.identity, null);
            instance.Initialize(Volume, Hygiene);

            Destroy(gameObject);
         }
         else
         {
            //accelerate falling speed
            _fallSpeed += _fallSpeed * 10f * Time.deltaTime;

            //fall
            transform.position = new Vector2(transform.position.x, transform.position.y - (_fallSpeed * Time.deltaTime));
         }
      }
   }

   private void OnTriggerEnter2D(Collider2D collision)
   {
      if (!_flaggedForDestroy)
      {
         var puddle = collision.GetComponent<PuddleController>();
         if (puddle != null)
         {
            //if droplet has fallen more than 1/2 the _fallDistance
            if ((_fallDistance - Vector3.Distance(transform.position, _spawnLocation)) < (_fallDistance / 2f))
            {
               puddle.Volume += Volume;
               _flaggedForDestroy = true;
               Destroy(gameObject);
            }
         }
      }
   }
}
