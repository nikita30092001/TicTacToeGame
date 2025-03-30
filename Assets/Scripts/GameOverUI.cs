using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private Color _winColor;
    [SerializeField] private Color _loseColor;
    [SerializeField] private Color _tieColor;
    [SerializeField] private GameManager _gameManager;

    private void Start()
    {
        ActivateUI(false);
    }

    private void OnEnable()
    {
        _gameManager.OnWinnerPlayerTypeChanged += DrawWinUI;
        _gameManager.OnRematch += Rematch;
        _gameManager.OnGameTied += DrawTieUI;
    }

    private void OnDisable()
    {
        _gameManager.OnRematch -= Rematch;
    }

    private void DrawTieUI()
    {
        ActivateUI(true);
        _resultText.text = "TIE!";
        _resultText.color = _tieColor;
        _gameManager.OnGameTied += DrawTieUI;
    }

    private void Rematch()
    {
        ActivateUI(false);
    }

    private void DrawWinUI(GameManager.PlayerType winnerPlayerType)
    {
        if (winnerPlayerType == _gameManager.GetLocalPlayerType())
        {
            _resultText.text = "YOU WIN!";
            _resultText.color = _winColor;
        }
        else
        {
            _resultText.text = "YOU LOSE!";
            _resultText.color = _loseColor;
        }

        _gameManager.OnWinnerPlayerTypeChanged -= DrawWinUI;
        ActivateUI(true);
    }

    private void ActivateUI(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
