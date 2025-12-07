using System.Collections.Generic;
using System;

namespace _Project.Runtime.Tools
{
    public class ReusableIdContainer
    {
        private readonly Queue<uint> _availableId = new();
        private readonly uint _chunkSize;

        private uint _lastId;

        public ReusableIdContainer(uint startId = 1, uint chunkSize = 100)
        {
            if (chunkSize == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(chunkSize));
            }

            _lastId = startId;
            _chunkSize = chunkSize;
        }

        public uint Acquire()
        {
            if (_availableId.Count == 0)
            {
                Fill();
            }

            var id = _availableId.Dequeue();
            return id;
        }

        public void Release(uint id)
        {
            if (id == 0)
            {
                return;
            }

            _availableId.Enqueue(id);
        }

        private void Fill()
        {
            for (var i = 0; i < _chunkSize; i++)
            {
                _availableId.Enqueue(_lastId++);
            }
        }
    }
}
