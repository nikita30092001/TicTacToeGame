using System;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject _crossArrowGameObject;
    [SerializeField] private GameObject _circleArrowGameObject;
    [SerializeField] private GameObject _crossTextGameObject;
    [SerializeField] private GameObject _circleTextGameObject;
    [SerializeField] private GameManager _gameManager;

    private void Awake()
    {
        _crossArrowGameObject.SetActive(false);
        _circleArrowGameObject.SetActive(false);
        _crossTextGameObject.SetActive(false);
        _circleTextGameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _gameManager.OnGameStarted += DrawUI;
        _gameManager.OnCurrentPlayablePlayerTypeChanged += UpdateCurrentArrow;
    }
    private void OnDisable()
    {
        _gameManager.OnGameStarted -= DrawUI;
        _gameManager.OnCurrentPlayablePlayerTypeChanged -= UpdateCurrentArrow;
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
