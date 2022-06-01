using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class VersionTextUIController : MonoBehaviour
{
   private const string _pretext = "Development Build - Version ";
   private TextMeshProUGUI _tmp;

   private void OnEnable()
   {
      if (_tmp == null)
      {
         _tmp = GetComponent<TextMeshProUGUI>();
      }

      _tmp.text = _pretext + Application.version;
   }

}
