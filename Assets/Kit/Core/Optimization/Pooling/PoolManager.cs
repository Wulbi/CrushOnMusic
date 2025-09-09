using GameLogic.Core;
using UnityEngine;
using System;

namespace GameLogic.Manager
{
    /// <summary>
    /// 오브젝트 풀링을 관리하는 클래스
    /// </summary>
    [DefaultExecutionOrder(-9999), DisallowMultipleComponent]
    internal sealed class PoolManager : SingletonBehaviour<PoolManager>
    {
        [SerializeField] private PoolData poolData;

        [Serializable]
        public class PoolData : SerializableDictionary<string, PoolContainer> { public PoolData() { } }

        protected override void Awake()
        {
            base.Awake();

            foreach (var _pool in poolData)
                _pool.Value.Populate();
        }
        private GameObject GetPrefab(string key)
        {
            if (poolData == null)
                return null;

            if (poolData.TryGetValue(key, out var _pool))
                return _pool._prefab;

            return null;
        }
        public GameObject GetFromPool(string key, Vector3 position, Quaternion rotation)
        {
            if (string.IsNullOrEmpty(key))
                return null;
            if (poolData == null)
                return null;

            GameObject pref = GetPrefab(key);

            if (pref == null)
                return null;

            return pref.Reuse(position, rotation);
        }
        public GameObject GetFromPool(string key, Transform parent)
        {
            if (string.IsNullOrEmpty(key))
                return null;
            if (poolData == null)
                return null;

            GameObject pref = GetPrefab(key);

            if (pref == null)
                return null;

            return pref.Reuse(parent);
        }

        public void ReturnToPool(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;
            if (poolData == null)
                return;

            if (poolData.TryGetValue(key, out var pool))
                pool._prefab.Release();
        }
        public void ReturnToPool(GameObject instance) => instance.Release();

        [System.Serializable]
        public struct PoolContainer
        {
            public GameObject _prefab;
            [Min(1)]
            public int _startCount;

            public void Populate()
            {
                _prefab.Populate(_startCount);
            }
        }
    }
}

