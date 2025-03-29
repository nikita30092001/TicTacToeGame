using Unity.Netcode;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
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
        SpawnObjectRpc(x, y);
    }

    [Rpc(SendTo.Server)]
    private void SpawnObjectRpc(float x, float y)
    {
        Transform spawnedCrossTransform = Instantiate(_crossPrefab, GetGridPosition(x, y), Quaternion.identity);
        spawnedCrossTransform.GetComponent<NetworkObject>().Spawn(true);
    }

    private Vector2 GetGridPosition(float x, float y)
    {
        return new Vector2 (x, y);
    }
}
