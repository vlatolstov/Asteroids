using UnityEngine;

namespace _Project.Runtime.Abstract.MVP
{
    public class BaseView : MonoBehaviour
    {
        public uint ViewId { get; private set; }

        public void SetId(uint viewId)
        {
            ViewId = viewId;
        }
    }
}