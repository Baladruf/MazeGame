using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MapUI : MonoBehaviour
{
    private GridLayoutGroup grid;

    [SerializeField]
    private CellDataUI cellPrefabUI;

    private int width, height;

    private List<List<CellDataUI>> mazeUI;

    [SerializeField]
    private RectTransform cursor;
    private RectTransform currentCursor;

    private void Awake()
    {
        grid = GetComponent<GridLayoutGroup>();
        mazeUI = new List<List<CellDataUI>>();
    }

    private void Start()
    {
        MazeGenerator.Instance.player.viewPlayerEvent += CursorDirection;
        MazeGenerator.Instance.player.playerMoveEvent += UpdatePlayerPosition;
    }

    public void InitializeMap(Vector2Int _playerPosition, Vector2Int _keyPosition, Vector2Int _trapdoorPosition)
    {
        ClearMap();
        Vector2Int sizeMaze = MazeGenerator.Instance.GetMazeSize();
        width = sizeMaze.x;
        height = sizeMaze.y;
        float sizeCell = Screen.width * 0.3f / width;
        grid.cellSize = new Vector2(sizeCell, sizeCell);
        grid.constraintCount = width;

        for(int y = 0; y < height; y++)
        {
            mazeUI.Add(new List<CellDataUI>());
            for(int x = 0; x < width; x++)
            {
                var _cellUI = Instantiate(cellPrefabUI, transform);
                mazeUI[y].Add(_cellUI);
                CellData _cell;
                if(MazeGenerator.Instance.TryGetCell(x, y, out _cell))
                {
                    MapUIElement _element = MapUIElement.none;
                    if(x == _keyPosition.x && y == _keyPosition.y)
                    {
                        _element = MapUIElement.key;
                    }
                    else if(x == _trapdoorPosition.x && y == _trapdoorPosition.y)
                    {
                        _element = MapUIElement.trapddor;
                    }

                    _cellUI.InitializeWall(_cell, _element);
                }
                else
                {
                    Debug.LogWarning("Fail load maze cell (" + x + ", " + y + ")");
                }
            }
        }
        currentCursor = Instantiate(cursor, mazeUI[_playerPosition.y][_playerPosition.x].GetPlayerPosition);
        mazeUI[_playerPosition.y][_playerPosition.x].Unhide();
    }

    private void ClearMap()
    {
        if(grid.transform.childCount > 0)
        {
            for(int i = 0; i < grid.transform.childCount; i++)
            {
                Destroy(grid.transform.GetChild(i).gameObject);
            }
        }
        mazeUI.Clear();
    }

    private void CursorDirection(Direction _dir)
    {
        switch (_dir)
        {
            case Direction.north:
                currentCursor.transform.localRotation = Quaternion.Euler(Vector3.forward * 0f);
                break;
            case Direction.west:
                currentCursor.transform.localRotation = Quaternion.Euler(Vector3.forward * 90f);
                break;
            case Direction.south:
                currentCursor.transform.localRotation = Quaternion.Euler(Vector3.forward * 180f);
                break;
            case Direction.east:
                currentCursor.transform.localRotation = Quaternion.Euler(Vector3.forward * -90f);
                break;
        }
    }

    private void UpdatePlayerPosition(int _x, int _y)
    {
        CellDataUI _cell = mazeUI[_y][_x];


        currentCursor.transform.SetParent(_cell.GetPlayerPosition);
        currentCursor.anchorMin = Vector2.zero;
        currentCursor.anchorMax = Vector2.one;
        currentCursor.pivot = new Vector2(0.5f, 0.5f);
        currentCursor.sizeDelta = Vector2.zero;
        currentCursor.anchoredPosition = Vector2.zero;
        _cell.Unhide();


        /*_mRect.anchoredPosition = _parent.position;
        _mRect.anchorMin = new Vector2(1, 0);
        _mRect.anchorMax = new Vector2(0, 1);
        _mRect.pivot = new Vector2(0.5f, 0.5f);
        _mRect.sizeDelta = _parent.rect.size;
        _mRect.transform.SetParent(_parent);*/
    }

    public void RemoveKeyMap(int x, int y)
    {
        mazeUI[y][x].SetBackground(MapUIElement.none);
    }
}
