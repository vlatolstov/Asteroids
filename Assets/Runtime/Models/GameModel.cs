using System;
using Runtime.Abstract.MVP;
using Runtime.Data;
using UnityEngine;
using Zenject;

namespace Runtime.Models
{
    public class GameModel : BaseModel, ITickable
    {
        protected override void OnDataChange(IData newValue)
        {
            Debug.Log($"GameModel.OnDataChange: {newValue.GetType()}");
        }

        protected override void OnNotify(Action action)
        {
            Debug.Log($"GameModel.OnNotify: {action}");
        }

        public void Tick()
        {
            if (TryGet<ShipPose>(out var shipPose))
            {
                Debug.Log($"GameModel.Tick: {shipPose}");
            }
        }
    }
}