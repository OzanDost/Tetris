using System;

namespace DefaultNamespace
{
    public interface IPoolableItem
    {
        public bool IsInPool { get; set; }
        public void OnSpawn();
        public void OnReturnToPool();
    }
}