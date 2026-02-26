using Game.Scripts.Events;
using Game.Scripts.Events.Impl;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour {
    
    [SerializeField] private TextMeshProUGUI _movesText;
    [SerializeField] private TextMeshProUGUI _scoreText;

    [SerializeField] private GameObject _gameOverScreen;

    //useful data in case we later want more complex UI updates (e.g. animations, color changes, etc.)
    private int _moves;
    private int _score;
    
    
    void Start() {
        EventManager.Subscribe<OnMovesUpdated>(MovesUpdatedHandler);
        EventManager.Subscribe<OnScoreUpdated>(ScoreUpdatedHandler);
        EventManager.Subscribe<OnGameOver>(GameOverHandler);
    }
    
    private void MovesUpdatedHandler(OnMovesUpdated eventData) {
        //Update the moves UI
        _moves = eventData.Moves;
        _movesText.text = $"{_moves}";
    }
    
    private void ScoreUpdatedHandler(OnScoreUpdated eventData) {
        //Update the score UI
        _score = eventData.score;
        _scoreText.text = $"{_score}";
    }
    
    private void GameOverHandler(OnGameOver eventData) {
        //For now, just log the game over event. In a real game, we would show a game over screen, etc.
        _gameOverScreen.SetActive(true);
    }

    private void OnDestroy() {
        EventManager.Unsubscribe<OnMovesUpdated>(MovesUpdatedHandler);
        EventManager.Unsubscribe<OnScoreUpdated>(ScoreUpdatedHandler);
    }
}
