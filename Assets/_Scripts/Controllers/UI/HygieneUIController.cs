using UnityEngine;
using UnityEngine.UI;

public class HygieneUIController : MonoBehaviour
{
   private Slider _slider;
   private HygieneComponent _hygieneComponent;

   private void Awake()
   {
      _slider = GetComponent<Slider>();

      var hygieneGameObject = GameObject.FindGameObjectWithTag("Player");
      _hygieneComponent = hygieneGameObject.GetComponent<HygieneComponent>();
   }

   private void Update()
   {
      if (_hygieneComponent != null)
      {
         _slider.value = _hygieneComponent.Hygiene;
      }
   }
}
