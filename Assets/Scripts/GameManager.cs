using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action<float, float> OnGridPositionClicked;
    public void ClickedOnGridPosition(float x, float y)
    {
        OnGridPositionClicked?.Invoke(x, y);
    }
}
