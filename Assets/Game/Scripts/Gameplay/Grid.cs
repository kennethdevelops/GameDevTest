using System;
using System.Collections.Generic;
using Game.Scripts.Util;
using UnityEngine;

namespace Game.Scripts {
    public class Grid : MonoBehaviour {

        //prefabs
        [SerializeField] private Block _greenBlockPrefab;
        [SerializeField] private Block _blueBlockPrefab;
        [SerializeField] private Block _yellowBlockPrefab;
        [SerializeField] private Block _brownBlockPrefab;
        [SerializeField] private Block _pinkBlockPrefab;
        
        [SerializeField] private Transform _blockParent;
        
        [SerializeField] private Vector2 _blockSpacing = new Vector2(1f, 1f);
        
        private Dictionary<BlockType, BlockPool> _blockPools = new Dictionary<BlockType, BlockPool>();
        
        private Block[,] _blocks;
        
        private int _width = 5;
        private int _height = 6;
        
        private bool _moveAble = true; //we use this to prevent the player from making moves while the grid is refilling


        private void Start() {
            CreatePools();
            CreateGrid();
        }

        private void CreatePools() {
            _blockPools[BlockType.Green]  = new BlockPool(_greenBlockPrefab,  _blockParent); 
            _blockPools[BlockType.Blue]   = new BlockPool(_blueBlockPrefab,   _blockParent);
            _blockPools[BlockType.Yellow] = new BlockPool(_yellowBlockPrefab, _blockParent);
            _blockPools[BlockType.Brown]  = new BlockPool(_brownBlockPrefab,  _blockParent);
            _blockPools[BlockType.Pink]   = new BlockPool(_pinkBlockPrefab,   _blockParent);
        }
        
        private void CreateGrid() {
            _blocks = new Block[_width, _height];
            
            for (int x = 0; x < _width; x++) {
                for (int y = 0; y < _height; y++) {
                    var blockType = (BlockType) UnityEngine.Random.Range(0, 5);
                    var block = CreateBlock(blockType);
                    block.X = x;
                    block.Y = y;
                    block.transform.localPosition = new Vector3(x * _blockSpacing.x, y * _blockSpacing.y, 0);
                    _blocks[x, y] = block;
                }
            }
        }
        
        public bool TryGetBlock(Camera camera, Vector2 screenPos, out Block block) {
            block = null;
            //we map the position of the tap to world coordinates and check if there's a block there
            var     worldPos   = camera.ScreenToWorldPoint(screenPos);
            Vector3 localPoint = _blockParent.InverseTransformPoint(worldPos);

            // Convert local position to “grid coordinates” (centres are at 0, spacing, 2*spacing, etc.)
            float gx = localPoint.x / _blockSpacing.x;
            float gy = localPoint.y / _blockSpacing.y;

            int x = Mathf.RoundToInt(gx);
            int y = Mathf.RoundToInt(gy);
            
            // This assumes each cell is “owned” within +/- 0.5 of its centre.
            if (Mathf.Abs(gx - x) > 0.5f || Mathf.Abs(gy - y) > 0.5f) {
                return false;
            }

            if (x < 0 || x >= _width || y < 0 || y >= _height) {
                return false;
            }

            block = _blocks[x, y];
            return block != null;
        }

        private Block CreateBlock(BlockType type) {
            return _blockPools[type].Get();
        }
        
        public int TapBlock(Block block) {
            if (!_moveAble) {
                return 0; //ignore taps while refilling
            }
            var blocksCollected = 1;
            
            CollectBlock(block);
            
            //Find adjacent blocks of the same type
            var adjacentBlocks = FindAdjacentBlocks(block);
            
            foreach (var adjacentBlock in adjacentBlocks) {
                if (adjacentBlock.Type == block.Type) {
                    CollectBlock(adjacentBlock);
                    blocksCollected++;
                }
            }
            
            _moveAble = false;
            Invoke(nameof(GridRefill), 1);
            
            return blocksCollected;
        }

        private void GridRefill() {
            for (int x = 0; x < _width; x++) {
                FillColumn(x);
            }
            
            _moveAble = true;
        }

        private void CollectBlock(Block block) {
            _blockPools[block.Type].Release(block);
            
            //update grid
            _blocks[block.X, block.Y] = null;
        }

        //Recursive function that finds all adjacent blocks of the same type. We use a HashSet to avoid infinite loops.
        private List<Block> FindAdjacentBlocks(Block block) {
            var visited = new HashSet<Block>();
            var toVisit = new Stack<Block>();
            var result  = new List<Block>();

            toVisit.Push(block);

            while (toVisit.Count > 0) {
                var current = toVisit.Pop();
                if (visited.Contains(current)) {
                    continue;
                }

                visited.Add(current);
                result.Add(current);

                //check neighbours
                foreach (var neighbour in GetNeighbours(current)) {
                    if (neighbour.Type == block.Type && !visited.Contains(neighbour)) {
                        toVisit.Push(neighbour);
                    }
                }
            }

            return result;
        }
        
        private List<Block> GetNeighbours(Block block) {
            var neighbours = new List<Block>();

            int x = block.X;
            int y = block.Y;

            if (x > 0 && _blocks[x - 1, y] != null) {
                neighbours.Add(_blocks[x - 1, y]);
            }

            if (x < _width - 1 && _blocks[x + 1, y] != null) {
                neighbours.Add(_blocks[x + 1, y]);
            }

            if (y > 0 && _blocks[x, y - 1] != null) {
                neighbours.Add(_blocks[x, y - 1]);
            }

            if (y < _height - 1 && _blocks[x, y + 1] != null) {
                neighbours.Add(_blocks[x, y + 1]);
            }

            return neighbours;
        }

        private void FillColumn(int x) {
            // Process from bottom to top so we can "pull down" blocks cleanly.
            for (int y = _height - 1; y >= 0; y--) {
                if (_blocks[x, y] != null) {
                    continue;
                }

                // Find the nearest non-null block above (smaller y).
                int sourceY = y - 1;
                while (sourceY >= 0 && _blocks[x, sourceY] == null) {
                    sourceY--;
                }

                if (sourceY >= 0) {
                    // Pull the block down.
                    MoveBlock(x, sourceY, x, y);
                } else {
                    // Nothing above -> we're effectively at the "top spill" -> spawn new block.
                    SpawnNewBlockAt(x, y);
                }
            }
        }

        private void MoveBlock(int fromX, int fromY, int toX, int toY) {
            Block block = _blocks[fromX, fromY];
            _blocks[fromX, fromY] = null;

            _blocks[toX, toY] = block;

            // Keep any coordinate metadata in sync if you store it on Block
            block.X = toX;
            block.Y = toY;

            // Snap into place (or start an animation instead)
            block.transform.localPosition = new Vector3(
                                                        toX * _blockSpacing.x,
                                                        toY * _blockSpacing.y,
                                                        0f
                                                       );
        }

        private void SpawnNewBlockAt(int x, int y) {
            // Replace this with your own random/type logic
            BlockType blockType = (BlockType)UnityEngine.Random.Range(0, 5);

            Block block = CreateBlock(blockType);

            _blocks[x, y] = block;

            block.X = x;
            block.Y = y;

            block.transform.localPosition = new Vector3(
                                                        x * _blockSpacing.x,
                                                        y * _blockSpacing.y,
                                                        0f
                                                       );
        }
    }
}