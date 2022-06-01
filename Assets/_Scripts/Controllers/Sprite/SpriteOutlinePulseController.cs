using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteOutlinePulseController : MonoBehaviour
{
   [SerializeField] private Color _outlineColor = Color.white;
   [SerializeField] private float _pulseRate = 2f;

   private SpriteRenderer _spriteRenderer;
   private MaterialPropertyBlock _propertyBlock;
   private bool _fading;

   private void Awake()
   {
      _spriteRenderer = GetComponent<SpriteRenderer>();
      _spriteRenderer.GetPropertyBlock(_propertyBlock = _propertyBlock ?? new MaterialPropertyBlock());
      _propertyBlock.SetColor("_OutlineColor", _outlineColor);
      _spriteRenderer.SetPropertyBlock(_propertyBlock);
   }

   private void Update()
   {
      _spriteRenderer.GetPropertyBlock(_propertyBlock = _propertyBlock ?? new MaterialPropertyBlock());
      var color = _propertyBlock.GetColor("_OutlineColor");
      var newColor = color;

      if (_fading)
      {
         newColor -= new Color(0, 0, 0, _pulseRate * Time.deltaTime);
      }
      else
      {
         newColor += new Color(0, 0, 0, _pulseRate * Time.deltaTime);
      }

      if (_fading && newColor.a <= 0f)
      {
         _fading = false;
         newColor = color;
      }
      else if (!_fading && newColor.a >= _outlineColor.a)
      {
         _fading = true;
         newColor = color;
      }

      _propertyBlock.SetColor("_OutlineColor", newColor);
      _spriteRenderer.SetPropertyBlock(_propertyBlock);
   }

   private void OnEnable()
   {
      _spriteRenderer.GetPropertyBlock(_propertyBlock = _propertyBlock ?? new MaterialPropertyBlock());
      _propertyBlock.SetFloat("_Outline", 1f);
      _spriteRenderer.SetPropertyBlock(_propertyBlock);
   }

   private void OnDisable()
   {
      _spriteRenderer.GetPropertyBlock(_propertyBlock = _propertyBlock ?? new MaterialPropertyBlock());
      _propertyBlock.SetFloat("_Outline", 0f);
      _spriteRenderer.SetPropertyBlock(_propertyBlock);
   }
}
