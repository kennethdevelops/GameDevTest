using UnityEngine;

namespace Game.Scripts.Util {
    public class BlockPool {
        
        private Pool<Block> _pool;

        private Block     _prefab;
        private Transform _parent;

        public BlockPool(Block prefab, Transform parent) {
            _prefab = prefab;
            _parent = parent;
            _pool = new Pool<Block>(Create, OnGet, OnRelease);
        }
        
        public Block Get() {
            return _pool.Get();
        }
        
        public void Release(Block obj) {
            _pool.Release(obj);
        }

        private Block Create() {
            var obj = Object.Instantiate<Block>(_prefab, _parent, true);

            return obj;
        }

        private void OnGet(Block obj) {
            obj.gameObject.SetActive(true);
        }

        private void OnRelease(Block obj) {
            obj.gameObject.SetActive(false);
        }
        
    }
}