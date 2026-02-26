using System;
using Game.Scripts.Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Scripts {
    public class ReplayButton : MonoBehaviour {

        private Button _button;
        
        private void Start() {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }
        
        private void OnClick() {
            //this prevents us from clicking the button multiple times and causing multiple scene loads
            _button.onClick.RemoveListener(OnClick);
            
            //we clear all event data, so that we don't have any lingering events from the previous game
            //that could interfere with the new game
            EventManager.Clear();
            
            // we reload the current scene, which will reset the game state and start a new game
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}