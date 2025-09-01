using Runtime.Abstract.MVP;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.Views
{
    [RequireComponent(typeof(UIDocument))]
    public class HudView : BaseView
    {
        private UIDocument _doc;
        private Label _posLabel;
        private Label _spdLabel;
        private Label _angLabel;
        
        private void Awake()
        {
            _doc = GetComponent<UIDocument>();
        }
        
        private void OnEnable()
        {
            var root = _doc.rootVisualElement;
            
            _posLabel = root.Q<Label>("Pos");
            _spdLabel = root.Q<Label>("Speed");
            _angLabel = root.Q<Label>("Angle");
        }
        
        public void SetPose(Vector2 pos, Vector2 vel, float angleRad)
        {
            if (_posLabel != null)
                _posLabel.text = $"POS  X:{pos.x,7:0.00}  Y:{pos.y,7:0.00}";

            if (_spdLabel != null)
            {
                float spd = vel.magnitude;
                _spdLabel.text = $"SPD  {spd:0.00}";
            }

            if (_angLabel != null)
            {
                float angDeg = Mathf.Repeat(angleRad * Mathf.Rad2Deg, 360f);
                _angLabel.text = $"ANG  {angDeg:0.0}°";
            }
        }
    }
}