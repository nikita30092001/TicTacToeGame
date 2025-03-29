using UnityEngine;

public class GridPosition : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    
    private void OnMouseDown()
    {
        _gameManager.ClickedOnGridPosition(transform.position.x, transform.position.y);
    }
}
