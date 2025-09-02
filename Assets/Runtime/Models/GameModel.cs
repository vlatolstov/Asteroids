using System;
using Runtime.Abstract.MVP;
using Runtime.Data;
using UnityEngine;

namespace Runtime.Models
{
    public class GameModel : BaseModel
    {
        protected override void OnDataChange(IData newValue)
        {
            if (newValue is TurnInput turnInput)
            {
                Debug.Log($"GameModel.OnDataChange: new TurnInput {turnInput.Value:F1}");
            }
        }

        protected override void OnNotify(Action action)
        { }
    }
}