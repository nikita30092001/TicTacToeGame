using Unity.Netcode;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    [SerializeField] private Transform _crossPrefab;
    [SerializeField] private Transform _circlePrefab;
    [SerializeField] private GameManager _gameManager;

    private void OnEnable()
    {
        _gameManager.OnGridPositionClicked += SpawnPrefab;
    }

    private void OnDisable()
    {
        _gameManager.OnGridPositionClicked -= SpawnPrefab;
    }

    private void SpawnPrefab(float x, float y, GameManager.PlayerType playerType)
    {
        SpawnObjectRpc(x, y, playerType);
    }

    [Rpc(SendTo.Server)]
    private void SpawnObjectRpc(float x, float y, GameManager.PlayerType playerType)
    {
        Transform prefab;
        switch (playerType)
        {
            default:
            case GameManager.PlayerType.Cross:
                prefab = _crossPrefab;
                break;
            case GameManager.PlayerType.Circle:
                prefab = _circlePrefab;
                break;
        }
        Transform spawnedCrossTransform = Instantiate(prefab, GetGridPosition(x, y), Quaternion.identity);
        spawnedCrossTransform.GetComponent<NetworkObject>().Spawn(true);
    }

    private Vector2 GetGridPosition(float x, float y)
    {
        return new Vector2 (x, y);
    }
}
