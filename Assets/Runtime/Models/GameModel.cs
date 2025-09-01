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
        { }

        protected override void OnNotify(Action action)
        { }

        public void Tick()
        { }
    }
}