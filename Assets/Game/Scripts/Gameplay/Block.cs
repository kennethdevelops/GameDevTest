using UnityEngine;

namespace Game.Scripts {
    public class Block : MonoBehaviour {

        [SerializeField] private BlockType      type;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private int _maxY = 6;

        private int _x;
        private int _y;
        
        
        
        public BlockType Type => type;

        public int X {
            get => _x;
            set => _x = value;
        }

        public int Y {
            get => _y;
            set {
                _y = value;
                //higher y means lower sorting order, so blocks below are rendered on top
                spriteRenderer.sortingOrder = _maxY - _y; 
            }
        }
    }

    public enum BlockType {
        Green = 0,
        Blue = 1,
        Yellow = 2,
        Brown = 3,
        Pink = 4
    }
}