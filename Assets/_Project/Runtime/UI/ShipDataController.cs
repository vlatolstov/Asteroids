using UnityEngine;
using UnityEngine.UIElements;

namespace _Project.Runtime.UI
{
    public sealed class ShipDataController
    {
        private readonly VisualElement _root;
        private readonly Label _positionLabel;
        private readonly Label _speedLabel;
        private readonly Label _angleLabel;

        private readonly Label _score;

        public ShipDataController(VisualElement root)
        {
            _root = root;
            _positionLabel = root.Q<Label>("pos-label");
            _speedLabel = root.Q<Label>("speed-label");
            _angleLabel = root.Q<Label>("angle-label");
            _score = root.Q<Label>("score-label");
        }

        public void SetVisible(bool v) => _root.style.display = v ? DisplayStyle.Flex : DisplayStyle.None;

        public void UpdatePose(Vector2 pos, Vector2 vel, float angleRad)
        {
            if (_positionLabel != null)
            {
                _positionLabel.text = $"POS X:{pos.x,7:0.00} Y:{pos.y,7:0.00}";
            }

            if (_speedLabel != null)
            {
                _speedLabel.text = $"SPD {vel.magnitude:0.00}";
            }

            if (_angleLabel != null)
            {
                float angDeg = Mathf.Repeat(angleRad * Mathf.Rad2Deg, 360f);
                _angleLabel.text = $"ANG {angDeg:0.0}°";
            }
        }

        public void UpdateScore(int score)
        {
            if (_score != null)
            {
                _score.text = score.ToString();
            }
        }
    }
}