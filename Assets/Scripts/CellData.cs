using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellData : MonoBehaviour
{
    //cell
    [SerializeField]
    private GameObject[] walls;
    [SerializeField]
    private GameObject[] pillarsPrefab;

    [SerializeField]
    private MeshRenderer ground;

    [SerializeField]
    private Material[] colorGroundPrefab;

    [SerializeField]
    private Transform playerPosition;

    public Vector3 GetPositionForPlayer { get { return playerPosition.position; } }
    public int X {get; private set;}
    public int Y {get; private set;}

    public bool visited {get; private set;}
    private Dictionary<Direction, Data> cells;
    private Dictionary<InterCardinal, GameObject> pillars;

    private Dictionary<ColorType, Material> colorGround;

#if UNITY_EDITOR
    public CellData[] visualizerCells;
#endif


    public void Initialize(string _name, int _x, int _y)
    {
        name = _name;
        X = _x;
        Y = _y;
        visited = false;
        cells = new Dictionary<Direction, Data>();
        for(int i = 0; i < 4; i++)
        {
            cells.Add(
                (Direction)i,
                new Data { wall = walls[i]}
                );
        }

        pillars = new Dictionary<InterCardinal, GameObject>();
        for (int i = 0; i < 4; i++)
        {
            pillars.Add(
                (InterCardinal)i,
                pillarsPrefab[i]
                );
        }

        colorGround = new Dictionary<ColorType, Material>();
        for (int i = 0; i < colorGroundPrefab.Length; i++)
        {
            colorGround.Add(
                (ColorType)i,
                colorGroundPrefab[i]
                );
        }
    }

#if UNITY_EDITOR
    public void Visualize()
    {
        visualizerCells = new CellData[4];
        for(int i = 0; i < 4; i++)
        {
            visualizerCells[i] = GetNeighbour((Direction)i);
        }
    }
#endif

    public void SetNeighbour(CellData neighbour, Direction dir)
    {
        Debug.Assert(cells.ContainsKey(dir));
        cells[dir].neighbour = neighbour;
    }

    public void Visiting()
    {
        visited = true;
    }

    public CellData GetNeighbour(Direction dir)
    {
        Debug.Assert(cells.ContainsKey(dir));
        return cells[dir].neighbour;
    }

    public void RemoveWall(Direction dir)
    {
        Debug.Assert(cells.ContainsKey(dir));
        cells[dir].wall.SetActive(false);
    }

    public class Data
    {
        public GameObject wall;
        public CellData neighbour;
    }

    public void SetGroungColor(ColorType _color)
    {
        ground.material = colorGround[_color];
    }

    public bool HasWall(Direction _dir)
    {
        bool _hasWall = cells[_dir].wall.activeSelf;
        return _hasWall;
    }

    public void SetActivePillar(InterCardinal _dir, bool _enable)
    {
        pillars[_dir].SetActive(_enable);
    }

    public void CorrectifWall()
    {
        if(!HasWall(Direction.north) && !HasWall(Direction.east))
        {
            SetActivePillar(InterCardinal.NE, false);
        }

        if (!HasWall(Direction.north) && !HasWall(Direction.west))
        {
            SetActivePillar(InterCardinal.NW, false);
        }

        if (!HasWall(Direction.south) && !HasWall(Direction.east))
        {
            SetActivePillar(InterCardinal.SE, false);
        }

        if (!HasWall(Direction.south) && !HasWall(Direction.west))
        {
            SetActivePillar(InterCardinal.SW, false);
        }
    }
}
