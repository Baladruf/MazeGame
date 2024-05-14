using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CellDataUI : MonoBehaviour
{
    [SerializeField]
    private Image[] wallUI;

    [SerializeField]
    private Sprite[] objMaze;

    private Dictionary<Direction, GameObject> walls;

    [SerializeField]
    private Image background;

    [SerializeField]
    private GameObject hideCell;

    [SerializeField]
    private RectTransform playerPosition;
    public RectTransform GetPlayerPosition { get { return playerPosition; } }

    public void InitializeWall(CellData _cell, MapUIElement _mapUIElement = MapUIElement.none)
    {
        walls = new Dictionary<Direction, GameObject>();
        for(int i = 0; i < 4; i++)
        {
            walls.Add((Direction)i, wallUI[i].gameObject);
            walls[(Direction)i].SetActive(_cell.HasWall((Direction)i));
        }
        SetBackground(_mapUIElement);
    }

    public void SetBackground(MapUIElement _mapUIElement)
    {
        background.color = Color.white;
        switch (_mapUIElement)
        {
            case MapUIElement.none:
                background.color = Color.clear;
                break;
            case MapUIElement.key:
                background.sprite = objMaze[0];
                Unhide();
                break;
            case MapUIElement.trapddor:
                background.sprite = objMaze[1];
                Unhide();
                break;
        }
    }

    public void Unhide()
    {
        hideCell.SetActive(false);
    }
}
