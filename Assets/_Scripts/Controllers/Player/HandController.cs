using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HandController : MonoBehaviour
{
   [SerializeField] private HandSoapController _handSoapPrefab = null;
   [SerializeField] private Transform _handSoapAnchor = null;
   private float _dropAngleX = 60f;
   private float _dropAngleZ = 30f;

   private SpriteRenderer _spriteRenderer;
   private List<HandSoapController> _handSoaps = new List<HandSoapController>();

   [SerializeField] private Vector3 _handSoapTopAnchorTarget;
   private Transform _handSoapTopAnchor;

   private void Awake()
   {
      _spriteRenderer = GetComponent<SpriteRenderer>();
      _spriteRenderer.enabled = false;

      _handSoapTopAnchor = new GameObject("HandSoapTopAnchor").transform;
      _handSoapTopAnchor.SetParent(_handSoapAnchor ?? transform);
      _handSoapTopAnchor.localPosition = Vector3.zero;
   }

   private void OnEnable()
   {
      Messaging.AddListener<SoapModel>(Enums.MessageType.INVENTORY_SOAP_ADDED, OnInventorySoapAdded);
      Messaging.AddListener<SoapModel>(Enums.MessageType.INVENTORY_SOAP_REMOVED, OnInventorySoapRemoved);
   }

   private void OnDisable()
   {
      Messaging.RemoveListener<SoapModel>(Enums.MessageType.INVENTORY_SOAP_ADDED, OnInventorySoapAdded);
      Messaging.RemoveListener<SoapModel>(Enums.MessageType.INVENTORY_SOAP_REMOVED, OnInventorySoapRemoved);
   }

   private void Update()
   {
      //randomly change anchor target location
      _handSoapTopAnchorTarget = new Vector3(
         _handSoapTopAnchorTarget.x + Random.Range(-0.1f, 0.1f) * Time.deltaTime,
         _handSoapAnchor.position.y,
         _handSoapTopAnchorTarget.z + Random.Range(-0.1f, 0.1f) * Time.deltaTime);

      _handSoapTopAnchor.localPosition = new Vector3(
         Mathf.Lerp(_handSoapTopAnchorTarget.x, _handSoapTopAnchorTarget.x + _handSoapTopAnchorTarget.x, Time.deltaTime),
         _handSoapTopAnchor.localPosition.y,
         Mathf.Lerp(_handSoapTopAnchorTarget.z, _handSoapTopAnchorTarget.z + _handSoapTopAnchorTarget.z, Time.deltaTime)
         );

      ProcessHandSoaps();
   }

   //todo - this is no longer running after camera is updating its position.
   // Maybe something to do with new pixel perfect camera?
   private void LateUpdate()
   {
      var cameraPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f));
      transform.position = new Vector2(cameraPosition.x, cameraPosition.y);
   }

   public void Center(float balancingStrength)
   {
      _handSoapTopAnchorTarget = Vector3.Lerp(_handSoapTopAnchorTarget, new Vector3(0f, _handSoapTopAnchorTarget.y, 0f), balancingStrength * Time.deltaTime);
   }

   private void CenterCompletely()
   {
      _handSoapTopAnchorTarget = new Vector3(0f, _handSoapTopAnchorTarget.y, 0f);
   }

   //y direction is translated to z direction
   public void Balance(Vector2 direction, float strength = 1f)
   {
      if (_handSoaps.Count > 0)
      {
         direction = -direction.normalized * strength * Time.deltaTime * 0.5f;

         _handSoapTopAnchorTarget = new Vector3(
            _handSoapTopAnchorTarget.x + direction.x,
            _handSoapAnchor.position.y,
            _handSoapTopAnchorTarget.z + direction.y);
      }
      else
      {
         CenterCompletely();
      }
   }

   private void OnInventorySoapAdded(SoapModel soap)
   {
      var instance = Instantiate(_handSoapPrefab, _handSoapAnchor.position, Quaternion.identity, transform);
      var instanceSpriteRenderer = instance.GetComponent<SpriteRenderer>();
      instanceSpriteRenderer.sortingLayerID = _spriteRenderer.sortingLayerID;
      instanceSpriteRenderer.sortingOrder = _spriteRenderer.sortingOrder + 1;

      instance.InitializeData(soap);

      //hack - lossy scale is pretty lossy, so have to hardcode this scaling factor
      var adjustment = instance.GetHeight() / (1.5f * 4f);
      //var adjustment = instance.GetHeight() / instance.transform.lossyScale.magnitude;

      _handSoapTopAnchor.localPosition += new Vector3(0, adjustment, 0);
      _handSoapTopAnchorTarget = _handSoapTopAnchor.localPosition;

      _handSoaps.Add(instance);

      if (_handSoaps.Count > 0) _spriteRenderer.enabled = true;
   }

   private void OnInventorySoapRemoved(SoapModel soap)
   {
      var handSoap = _handSoaps.FirstOrDefault(s => s.Soap.Equals(soap));

      if (handSoap != null)
      {
         //hack - lossy scale is pretty lossy, so have to hardcode this scaling factor
         var adjustment = handSoap.GetHeight() / (0.75f * 4f);
         //var adjustment = handSoap.GetHeight() / handSoap.transform.lossyScale.magnitude;

         _handSoapTopAnchor.localPosition -= new Vector3(0, adjustment, 0);
         _handSoapTopAnchorTarget = _handSoapTopAnchor.localPosition;

         _handSoaps.Remove(handSoap);
         Destroy(handSoap.gameObject);
      }

      if (_handSoaps.Count <= 0)
      {
         _spriteRenderer.enabled = false;

         CenterCompletely();
      }
   }

   private void ProcessHandSoaps()
   {
      if (_handSoaps.Count > 0)
      {
         var differenceVector = _handSoapTopAnchor.transform.position - _handSoapAnchor.position;

         //organise all soaps x,y,z and scale
         for (var i = 0; i < _handSoaps.Count; i++)
         {
            var previousPosition = (i == 0) ? _handSoapAnchor.position : _handSoaps[i - 1].transform.position;
            var iSafe = (i == 0) ? 1 : i;
            var stretched = differenceVector * ((iSafe * Mathf.Log10(_handSoaps.Count)) / _handSoaps.Count);

            _handSoaps[i].transform.position = new Vector3(
               previousPosition.x + stretched.x,
               _handSoaps[i].transform.position.y,
               previousPosition.z + stretched.z);

            //set y position based on previous soaps in stack
            var heightOffset = _handSoaps[i].GetHeight() / 2f;
            if (i > 0) heightOffset += (_handSoaps[i - 1].GetHeight() / 2f);

            _handSoaps[i].transform.position = new Vector3(
               _handSoaps[i].transform.position.x,
               previousPosition.y + heightOffset,
               _handSoaps[i].transform.position.z);

            //set fake depth scaling
            //todo - extract this hardcoded value out or make it dynamic
            var hardcodedOriginalScale = 0.5f;
            var scale = hardcodedOriginalScale - (1f * (_handSoaps[i].transform.localPosition.z - _handSoapAnchor.localPosition.z));
            _handSoaps[i].transform.localScale = new Vector3(scale, scale, _handSoaps[i].transform.localScale.z);

            //process dropping
            if (i == _handSoaps.Count - 1)
            {
               var mainVector = _handSoaps.LastOrDefault().transform.position - _handSoapAnchor.position;

               //todo - add a cooldown on dropping so it doesnt spam, player has time to recover
               //doing X,Z separately since a combined angle seems to not work right in Z
               var angleX = Vector3.Angle(_handSoapAnchor.up, new Vector3(mainVector.x, _handSoapAnchor.up.y, _handSoapAnchor.up.z));
               var angleZ = Vector3.Angle(_handSoapAnchor.right, new Vector3(_handSoapAnchor.right.x, _handSoapAnchor.right.y, mainVector.z));

               if (angleX > _dropAngleX || angleZ > _dropAngleZ)
               {
                  _handSoaps.LastOrDefault()?.Drop();
               }
            }
         }
      }
   }
}