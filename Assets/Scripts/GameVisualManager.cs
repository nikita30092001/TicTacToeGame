using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class GameVisualManager : MonoBehaviour
{
    [SerializeField] private Transform _crossPrefab;
    [SerializeField] private GameObject _circlePrefab;
    [SerializeField] private GameManager _gameManager;

    private void OnEnable()
    {
        _gameManager.OnGridPositionClicked += SpawnPrefab;
    }

    private void OnDisable()
    {
        _gameManager.OnGridPositionClicked -= SpawnPrefab;
    }

    private void SpawnPrefab(float x, float y)
    {
        Instantiate(_crossPrefab, GetGridPosition(x, y), Quaternion.identity);
    }

    private Vector2 GetGridPosition(float x, float y)
    {
        return new Vector2 (x, y);
    }
}
