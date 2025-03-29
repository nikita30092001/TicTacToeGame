using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private const float GRID_SIZE = 3.1f;

    public event Action<float, float, PlayerType> OnGridPositionClicked;
    public event Action OnGameStarted;
    public event Action OnCurrentPlayablePlayerTypeChanged;

    private PlayerType _localPlayerType;
    private NetworkVariable<PlayerType> _currentPlayablePlayerType = new NetworkVariable<PlayerType>();
    private PlayerType[,] _playerTypeArray;

    private void Awake()
    {
        _playerTypeArray = new PlayerType[3, 3];
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartedRpc()
    {
        OnGameStarted?.Invoke();
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRpc(float x, float y, PlayerType playerType)
    {
        if (playerType != _currentPlayablePlayerType.Value)
        {
            return;
        }

        if (_playerTypeArray[GetPosition(x), GetPosition(y)] != PlayerType.None)
        {
            return;
        }

        _playerTypeArray[GetPosition(x), GetPosition(y)] = _localPlayerType;
        OnGridPositionClicked?.Invoke(x, y, playerType);
        switch (_currentPlayablePlayerType.Value)
        {
            default:
            case PlayerType.Cross:
                _currentPlayablePlayerType.Value = PlayerType.Circle; 
                break;
            case PlayerType.Circle:
                _currentPlayablePlayerType.Value = PlayerType.Cross;
                break;
        }
    }

    private void ChangeCurrentPlayablePlayerType(PlayerType oldPlayerType, PlayerType newPlayerType)
    {
        OnCurrentPlayablePlayerTypeChanged?.Invoke();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            _localPlayerType = PlayerType.Cross;
        }
        else
        {
            _localPlayerType = PlayerType.Circle;
        }

        _currentPlayablePlayerType.OnValueChanged += ChangeCurrentPlayablePlayerType;
    }

    private void OnClientConnected(ulong obj)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            _currentPlayablePlayerType.Value = PlayerType.Cross;
            TriggerOnGameStartedRpc();
        }
    }

    public PlayerType GetLocalPlayerType()
    {
        return _localPlayerType;
    }
    public PlayerType GetCurrentPlayablePlayerType()
    {
        return _currentPlayablePlayerType.Value;
    }

    private int GetPosition(float x)
    {
        return (int)((x + GRID_SIZE) / GRID_SIZE);
    }

    public enum PlayerType
    {
        None,
        Cross,
        Circle
    }
}
