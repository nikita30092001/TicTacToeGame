using UnityEngine;

public class GridPosition : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    
    private void OnMouseDown()
    {
        _gameManager.ClickedOnGridPositionRpc(transform.position.x, transform.position.y, _gameManager.GetLocalPlayerType());
    }
}
