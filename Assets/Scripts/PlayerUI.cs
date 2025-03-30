using System;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject _crossArrowGameObject;
    [SerializeField] private GameObject _circleArrowGameObject;
    [SerializeField] private GameObject _crossTextGameObject;
    [SerializeField] private GameObject _circleTextGameObject;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private TextMeshProUGUI _crossPlayerScoreText;
    [SerializeField] private TextMeshProUGUI _circlePlayerScoreText;

    private void Awake()
    {
        _crossArrowGameObject.SetActive(false);
        _circleArrowGameObject.SetActive(false);
        _crossTextGameObject.SetActive(false);
        _circleTextGameObject.SetActive(false);
        _crossPlayerScoreText.text = "";
        _circlePlayerScoreText.text = "";
    }

    private void OnEnable()
    {
        _gameManager.OnGameStarted += DrawUI;
        _gameManager.OnCurrentPlayablePlayerTypeChanged += UpdateCurrentArrow;
        _gameManager.OnScoreChanged += DrawPlayerScore;
    }

    private void OnDisable()
    {
        _gameManager.OnGameStarted -= DrawUI;
        _gameManager.OnCurrentPlayablePlayerTypeChanged -= UpdateCurrentArrow;
        _gameManager.OnScoreChanged -= DrawPlayerScore;
    }

    private void DrawPlayerScore(int crossPlayerScore, int circlePlayerScore)
    {
        _crossPlayerScoreText.text = crossPlayerScore.ToString();
        _circlePlayerScoreText.text = circlePlayerScore.ToString();
    }

    private void DrawUI()
    {
        if (_gameManager.GetLocalPlayerType() == GameManager.PlayerType.Cross)
        {
            _crossTextGameObject.SetActive(true);
        }
        else
        {
            _circleTextGameObject.SetActive(true);
        }

        _crossPlayerScoreText.text = "0";
        _circlePlayerScoreText.text = "0";
        UpdateCurrentArrow();
    }

    private void UpdateCurrentArrow()
    {
        if (_gameManager.GetCurrentPlayablePlayerType() == GameManager.PlayerType.Cross)
        {
            _crossArrowGameObject.SetActive(true);
            _circleArrowGameObject.SetActive(false);
        }
        else
        {
            _circleArrowGameObject.SetActive(true);
            _crossArrowGameObject.SetActive(false);
        }
    }
}
