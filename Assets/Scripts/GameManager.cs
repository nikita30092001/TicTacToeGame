using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public event Action<float, float, PlayerType> OnGridPositionClicked;
    public event Action OnGameStarted;
    public event Action OnCurrentPlayablePlayerTypeChanged;

    private PlayerType _localPlayerType;
    private PlayerType _currentPlayablePlayerType;

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartedRpc()
    {
        OnGameStarted?.Invoke();
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRpc(float x, float y, PlayerType playerType)
    {
        if (playerType != _currentPlayablePlayerType)
        {
            return;
        }

        OnGridPositionClicked?.Invoke(x, y, playerType);

        switch (_currentPlayablePlayerType)
        {
            default:
            case PlayerType.Cross:
                _currentPlayablePlayerType = PlayerType.Circle; 
                break;
            case PlayerType.Circle:
                _currentPlayablePlayerType = PlayerType.Cross;
                break;
        }

        TriggerOnCurrentPlayablePlayerTypeChangedRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnCurrentPlayablePlayerTypeChangedRpc()
    {
        OnCurrentPlayablePlayerTypeChanged?.Invoke();
    }

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            _localPlayerType = PlayerType.Cross;
        }
        else
        {
            _localPlayerType = PlayerType.Circle;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnClientConnected(ulong obj)
    {
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            _currentPlayablePlayerType = PlayerType.Cross;
            TriggerOnGameStartedRpc();
        }
    }

    public PlayerType GetLocalPlayerType()
    {
        return _localPlayerType;
    }
    public PlayerType GetCurrentPlayablePlayerType()
    {
        return _currentPlayablePlayerType;
    }

    public enum PlayerType
    {
        None,
        Cross,
        Circle
    }
}
