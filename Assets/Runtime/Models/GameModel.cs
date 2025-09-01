using System;
using Runtime.Abstract.MVP;

namespace Runtime.Models
{
    public class GameModel : BaseModel
    {
        protected override void OnDataChange(IData newValue)
        { }

        protected override void OnNotify(Action action)
        { }
    }
}