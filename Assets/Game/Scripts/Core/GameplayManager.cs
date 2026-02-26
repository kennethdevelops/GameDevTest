using System;
using Game.Scripts.Events;
using Game.Scripts.Events.Impl;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts {
    public class GameplayManager : MonoBehaviour {

        //For testing only
        [SerializeField] private Button _makeMoveBtn;

        [SerializeField] private Grid _grid;
        
        private Camera _camera;
        
        private int _moves = 5; //we start at 5 moves
        private int _score;


        private void Start() {
            _camera = Camera.main;
            _makeMoveBtn.onClick.AddListener(MakeMove);
        }

        public void MakeMove() {
            Moves--;
            
            if (Moves <= 0) {
                //Game over 
                EventManager.Publish(new OnGameOver());
            }
        }

        private void Update() {
            if (Input.touchCount > 0) {
                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Began) {
                    HandleTap(t.position);
                }
                return;
            }

            if (Input.GetMouseButtonDown(0)) {
                HandleTap(Input.mousePosition);
            }
        }

        private void HandleTap(Vector2 screenPos) {
            if (_grid.TryGetBlock(_camera, screenPos, out var block)) {
                Debug.Log($"Tapped block at ({block.X},{block.Y})");
                
                //
                var collectedBlocks = _grid.TapBlock(block);
                if (collectedBlocks == 0) return;
                Score += collectedBlocks;
                MakeMove();
            }
            else {
                Debug.Log("Tapped empty space");
            }
        }

        private int Moves {
            get => _moves;
            set {
                _moves = value;
                EventManager.Publish(new OnMovesUpdated {Moves = _moves});
            }
        }
        
        private int Score {
            get => _score;
            set {
                _score = value;
                EventManager.Publish(new OnScoreUpdated {score = _score});
            }
        }
        
    }
}