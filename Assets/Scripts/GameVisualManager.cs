using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class GameVisualManager : NetworkBehaviour
{
    [SerializeField] private Transform _crossPrefab;
    [SerializeField] private Transform _circlePrefab;
    [SerializeField] private Transform _lineCompletePrefab;
    [SerializeField] private GameManager _gameManager;

    private List<GameObject> _visualGameObjectList;

    private void Awake()
    {
        _visualGameObjectList = new List<GameObject>();
    }

    private void OnEnable()
    {
        _gameManager.OnGridPositionClicked += SpawnPrefab;
        _gameManager.OnGameWin += DrawLineComplete;
        _gameManager.OnRematch += Rematch;
    }

    private void OnDisable()
    {
        _gameManager.OnGridPositionClicked -= SpawnPrefab;
        _gameManager.OnGameWin -= DrawLineComplete;
        _gameManager.OnRematch -= Rematch;
    }

    private void Rematch()
    {
        if (!NetworkManager.IsServer)
        {
            return;
        }

        foreach (GameObject gameObject in _visualGameObjectList)
        {
            Destroy(gameObject);
        }

        _visualGameObjectList.Clear();
    }

    private void DrawLineComplete(Vector2 centerGridPosition, GameManager.Line line)
    {
        float eulerZ = 0f;
        switch (line.orientation)
        {
            default:
            case GameManager.Orientation.Horizontal: eulerZ = 0f; break;
            case GameManager.Orientation.Vertical: eulerZ = 90f; break;
            case GameManager.Orientation.DiagonalA: eulerZ = 45f; break;
            case GameManager.Orientation.DiagonalB: eulerZ = -45f; break;

        }

        Transform lineCompleteTransform = Instantiate(_lineCompletePrefab, centerGridPosition, Quaternion.Euler(0, 0, eulerZ));
        lineCompleteTransform.GetComponent<NetworkObject>().Spawn(true);
        _visualGameObjectList.Add(lineCompleteTransform.gameObject);
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
        _visualGameObjectList.Add(spawnedCrossTransform.gameObject);
    }

    private Vector2 GetGridPosition(float x, float y)
    {
        return new Vector2 (x, y);
    }
}
