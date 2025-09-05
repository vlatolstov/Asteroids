using System;
using Runtime.Abstract.MVP;
using UnityEngine;

namespace Runtime.Models
{
    public class GameModel : BaseModel
    {
        protected override void OnDataChange(IData newValue)
        {
            // Debug.Log($"GameModel.OnDataChange: {newValue.GetType()} changed");
        }

        protected override void OnNotify(Action action)
        { }
    }
}