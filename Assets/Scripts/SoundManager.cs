using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Transform _placeSfxPrefab;
    [SerializeField] private Transform _winSfxPrefab;
    [SerializeField] private Transform _loseSfxPrefab;
    [SerializeField] private GameManager _gameManager;

    private void OnEnable()
    {
        _gameManager.OnPlacedObject += PlayPlaceSound;
        _gameManager.OnWinnerPlayerTypeChanged += PlayWinSound;
    }

    private void PlayWinSound(GameManager.PlayerType winnerPlayerType)
    {
        Transform sfxTransform;
        if (_gameManager.GetLocalPlayerType() == winnerPlayerType)
        {
            sfxTransform = Instantiate(_winSfxPrefab);
        }
        else
        {
            sfxTransform = Instantiate(_loseSfxPrefab);
        }

        Destroy(sfxTransform.gameObject, 5f);
    }

    private void OnDisable()
    {
        _gameManager.OnPlacedObject -= PlayPlaceSound;
    }

    private void PlayPlaceSound()
    {
        Transform sfxTransform = Instantiate(_placeSfxPrefab);
        Destroy(sfxTransform.gameObject, 5f);
    }
}
