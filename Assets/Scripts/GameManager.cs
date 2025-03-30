using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private const float GRID_SIZE = 3.1f;

    public event Action<float, float, PlayerType> OnGridPositionClicked;
    public event Action OnGameStarted;
    public event Action<Vector2, Line> OnGameWin;
    public event Action OnCurrentPlayablePlayerTypeChanged;
    public event Action<PlayerType> OnWinnerPlayerTypeChanged;

    private PlayerType _localPlayerType;
    private NetworkVariable<PlayerType> _currentPlayablePlayerType = new NetworkVariable<PlayerType>();
    private PlayerType[,] _playerTypeArray;
    private List<Line> _lineList;
    private PlayerType _winnerPlayerType;

    private void Awake()
    {
        _playerTypeArray = new PlayerType[3, 3];
        _lineList = new List<Line>()
        {
            new Line()
            {
                gridVector2List = new List<Vector2Int>{ new(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) },
                centerGridPosition = new Vector2Int(1, 0),
                orientation = Orientation.Horizontal
            },
            new Line()
            {
                gridVector2List = new List<Vector2Int>{ new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1) },
                centerGridPosition = new Vector2Int(1, 1),
                orientation = Orientation.Horizontal
            },
            new Line()
            {
                gridVector2List = new List<Vector2Int>{ new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2) },
                centerGridPosition = new Vector2Int(1, 2),
                orientation = Orientation.Horizontal
            },
            new Line()
            {
                gridVector2List = new List<Vector2Int>{ new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) },
                centerGridPosition = new Vector2Int(0, 1),
                orientation = Orientation.Vertical
            },
            new Line()
            {
                gridVector2List = new List<Vector2Int>{ new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2) },
                centerGridPosition = new Vector2Int(1, 1),
                orientation = Orientation.Vertical
            },
            new Line()
            {
                gridVector2List = new List<Vector2Int>{ new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(2, 2) },
                centerGridPosition = new Vector2Int(2, 1),
                orientation = Orientation.Vertical
            },
            new Line()
            {
                gridVector2List = new List<Vector2Int>{ new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(2, 2) },
                centerGridPosition = new Vector2Int(1, 1),
                orientation = Orientation.DiagonalA
            },
            new Line()
            {
                gridVector2List = new List<Vector2Int>{ new Vector2Int(0, 2), new Vector2Int(1, 1), new Vector2Int(2, 0) },
                centerGridPosition = new Vector2Int(1, 1),
                orientation = Orientation.DiagonalB
            }
        };
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnWinnerPlayerTypeChangedRpc(PlayerType playerType)
    {
        OnWinnerPlayerTypeChanged?.Invoke(playerType);
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

        _playerTypeArray[GetPosition(x), GetPosition(y)] = playerType;
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

        TestWinner();
    }

    private void ChangeCurrentPlayablePlayerType(PlayerType oldPlayerType, PlayerType newPlayerType)
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
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

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

    private bool TestWinnerLine(Line line)
    {
        return TestWinnerLine
            (
                _playerTypeArray[line.gridVector2List[0].x, line.gridVector2List[0].y],
                _playerTypeArray[line.gridVector2List[1].x, line.gridVector2List[1].y],
                _playerTypeArray[line.gridVector2List[2].x, line.gridVector2List[2].y]
            );
    }

    private bool TestWinnerLine(PlayerType aPlayerType, PlayerType bPlayerType, PlayerType cPlayerType)
    {
        return aPlayerType != PlayerType.None && aPlayerType == bPlayerType && bPlayerType == cPlayerType;
    }

    private void TestWinner()
    {
        foreach (var line in _lineList)
        {
            if (TestWinnerLine(line))
            {
                _currentPlayablePlayerType.Value = PlayerType.None;
                Vector2 position = new Vector2(line.centerGridPosition.x * GRID_SIZE - GRID_SIZE, line.centerGridPosition.y * GRID_SIZE - GRID_SIZE);
                OnGameWin?.Invoke(position, line);
                _winnerPlayerType = _playerTypeArray[line.centerGridPosition.x, line.centerGridPosition.y];
                TriggerOnWinnerPlayerTypeChangedRpc(_winnerPlayerType);
                break;
            }
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

    public struct Line
    {
        public List<Vector2Int> gridVector2List;
        public Vector2Int centerGridPosition;
        public Orientation orientation;
    }

    public enum Orientation
    {
        Horizontal,
        Vertical,
        DiagonalA, 
        DiagonalB
    }
}
