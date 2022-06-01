using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreUIController : MonoBehaviour
{
   private Text _scoreText;
   private ScoreController _scoreController;

   private void Start()
   {
      _scoreController = GameManager.Instance.Player?.GetComponent<ScoreController>();
      _scoreText = GetComponent<Text>();
   }

   private void Update()
   {
      _scoreText.text = _scoreController?.Score.ToString("00.00");
   }
}
