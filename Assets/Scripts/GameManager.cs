using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public event Action<float, float, PlayerType> OnGridPositionClicked;

    private PlayerType _localPlayerType;
    private PlayerType _currentPlayablePlayerType;
    
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
    }

    
    public enum PlayerType
    {
        None,
        Cross,
        Circle
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
            _currentPlayablePlayerType = PlayerType.Cross;
        }
    }

    public PlayerType GetLocalPlayerType()
    {
        return _localPlayerType;
    }
}
