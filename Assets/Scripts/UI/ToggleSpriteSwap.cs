using UnityEngine;
using UnityEngine.UI;

public class ToggleSpriteSwap : MonoBehaviour
{
[SerializeField] private Toggle _targetToggle;
[SerializeField] private Image _targetImage;
[SerializeField] private Sprite _isOnSprite;
[SerializeField] private Sprite _isOffSprite;
private void Start()
{
_targetToggle.toggleTransition = Toggle.ToggleTransition.None;
_targetToggle.onValueChanged.AddListener(OnToggled);
}
private void OnToggled(bool isOn)
{
if (_targetImage == null)
return;
var sprite = isOn ? _isOnSprite : _isOffSprite;
_targetImage.overrideSprite = sprite;
}
}
