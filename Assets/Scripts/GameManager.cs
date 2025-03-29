using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public event Action<float, float, PlayerType> OnGridPositionClicked;
    public event Action OnGameStarted;
    public event Action OnCurrentPlayablePlayerTypeChanged;

    private PlayerType _localPlayerType;
    private NetworkVariable<PlayerType> _currentPlayablePlayerType = new NetworkVariable<PlayerType>();

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

    public enum PlayerType
    {
        None,
        Cross,
        Circle
    }
}
