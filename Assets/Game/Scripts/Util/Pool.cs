using System;
using System.Collections.Generic;

namespace Game.Scripts.Util {
    public class Pool<T> {
        private readonly Stack<T> _objects = new Stack<T>();

        private Func<T>   createFunc;
        private Action<T> onGetFunc;
        private Action<T> onReleaseFunc;
        
        
        public Pool(Func<T> createFunc, Action<T> onGetFunc = null, Action<T> onReleaseFunc = null) {
            this.createFunc = createFunc;
            this.onGetFunc = onGetFunc;
            this.onReleaseFunc = onReleaseFunc;
        }
        
        public T Get() {
            T obj;
            if (_objects.Count > 0) {
                obj = _objects.Pop();
            } else {
                obj = createFunc();
            }
            
            onGetFunc?.Invoke(obj);
            return obj;
        }

        public void Release(T obj) {
            onReleaseFunc?.Invoke(obj);
            _objects.Push(obj);
        }

    }
}