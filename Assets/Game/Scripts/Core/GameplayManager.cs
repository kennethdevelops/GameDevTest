using Game.Scripts.Events;
using Game.Scripts.Events.Impl;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts {
    public class GameplayManager : MonoBehaviour {

        //For testing only
        [SerializeField] private Button _makeMoveBtn;
        
        private int _moves = 5; //we start at 5 moves
        private int _score;


        private void Start() {
            _makeMoveBtn.onClick.AddListener(MakeMove);
        }

        public void MakeMove() {
            Moves--;
            Score += 10;
            
            if (Moves <= 0) {
                //Game over 
                EventManager.Publish(new OnGameOver());
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