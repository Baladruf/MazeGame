using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    private CellData cellPrefab;

    [SerializeField]
    private MapUI map;

    [SerializeField]
    private PlayerController playerPrefab;
    public PlayerController player { get; private set; }

    [SerializeField]
    private GameObject keyPrefab;
    private bool hasKey;

    [SerializeField]
    private GameObject trapdoorPrefab;
    private GameObject trapdoor;

    [SerializeField]
    private int widthStart = 4, heightStart = 7, offset = 2;


    private List<List<CellData>> maze;
    public static MazeGenerator Instance { get; private set; }

    [Header("Visulaze")]
    [SerializeField]
    private float timerBuilderMaze = 0.5f;

    private void Start()
    {
        Instance = this;
        player = Instantiate<PlayerController>(playerPrefab);
        CreateMaze();
    }

    private void CreateMaze()
    {
        //Init maze

        if(transform.childCount != 0)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
        if(trapdoor != null)
        {
            Destroy(trapdoor.gameObject);
        }
        hasKey = false;

        GameObject _mazeHolder = new GameObject("Maze");
        _mazeHolder.transform.parent = transform;

        maze = new List<List<CellData>>();
        for (int y = 0; y < heightStart; y++)
        {
            maze.Add(new List<CellData>());
            for (int x = 0; x < widthStart; x++)
            {
                Vector3 pos = new Vector3(x * offset, 0f, -y * offset);
                var cell = Instantiate(cellPrefab, pos, Quaternion.identity, _mazeHolder.transform);
                cell.Initialize("c" + x + "." + y, x, y);
                maze[y].Add(cell);

                if (x > 0)
                {
                    maze[y][x].SetNeighbour(maze[y][x - 1], Direction.west);
                    maze[y][x - 1].SetNeighbour(maze[y][x], Direction.east);
                }

                if (y > 0)
                {
                    maze[y][x].SetNeighbour(maze[y - 1][x], Direction.north);
                    maze[y - 1][x].SetNeighbour(maze[y][x], Direction.south);
                }
            }
        }
        /*
        #if UNITY_EDITOR
        for(int i = 0; i < heightStart; i++)
        {
            for(int j = 0; j < widthStart; j++)
            {
                maze[i][j].Visualize();
            }
        }
        #endif
        */
        //building maze
        Vector2Int alea = new Vector2Int(Random.Range(0, widthStart), Random.Range(0, heightStart));
        //RecursiveBacktracker(maze[alea.y][alea.x]);
        NonRecursiveBacktracker(maze[alea.y][alea.x]);
        CorrectifWallTest();
        //StartCoroutine(NonRecursiveBacktrackerCorou(maze[alea.y][alea.x]));
        player.transform.position = maze[alea.y][alea.x].GetPositionForPlayer;
        player.Initialize(maze[alea.y][alea.x]);

        Vector2Int aleaKey = new Vector2Int(
            (Random.Range(0, widthStart) + alea.x) % widthStart,
            (Random.Range(0, heightStart) + alea.y) % heightStart
            );
        var _key = Instantiate(keyPrefab, maze[aleaKey.y][aleaKey.x].GetPositionForPlayer, Quaternion.identity);

        Vector2Int aleaTrap = new Vector2Int(
            (widthStart + Random.Range(0, widthStart) - alea.x) % widthStart,
            (heightStart + Random.Range(0, heightStart) - alea.y) % heightStart
            );
        trapdoor = Instantiate(trapdoorPrefab, maze[aleaTrap.y][aleaTrap.x].GetPositionForPlayer, Quaternion.identity);

        map.InitializeMap(alea, aleaKey,  aleaTrap);
        player.ForcePlayerView();
    }

    private void RecursiveBacktracker(CellData cell)
    {
        cell.Visiting();
        List<CardinalCell> neighboursUnvisited = new List<CardinalCell>();
        for(int i = 0; i < 4; i++)
        {
            var neighbour = cell.GetNeighbour((Direction)i);
            if(neighbour != null && !neighbour.visited)
            {
                neighboursUnvisited.Add(new CardinalCell
                {
                    direction = (Direction)i,
                    cell = neighbour
                });
            }
        }

        while(neighboursUnvisited.Count != 0)
        {
            var neighour = neighboursUnvisited.GetRandomElement();
            neighboursUnvisited.Remove(neighour);

            //remove 2 walls
            cell.RemoveWall(neighour.direction);
            neighour.cell.RemoveWall(neighour.direction.Invert());

            //next cell
            RecursiveBacktracker(neighour.cell);

            for(int i = (neighboursUnvisited.Count - 1); i >= 0; i--)
            {
                if(neighboursUnvisited[i].cell.visited)
                {
                    neighboursUnvisited.RemoveAt(i);
                }
            }
        }
    }

    private void NonRecursiveBacktracker(CellData firstCell)
    {
        Stack<CellData> cellDataStack = new Stack<CellData>();

        firstCell.Visiting();
        cellDataStack.Push(firstCell);
        while(cellDataStack.Count != 0)
        {
            CellData currentCell = cellDataStack.Pop();

            List<CardinalCell> neighboursUnvisited = new List<CardinalCell>();
            for (int i = 0; i < 4; i++)
            {
                var neighbour = currentCell.GetNeighbour((Direction)i);
                if (neighbour != null && !neighbour.visited)
                {
                    neighboursUnvisited.Add(new CardinalCell
                    {
                        direction = (Direction)i,
                        cell = neighbour
                    });
                }
            }

            if(neighboursUnvisited.Count != 0)
            {
                cellDataStack.Push(currentCell);
                var neighbour = neighboursUnvisited.GetRandomElement();

                //remove 2 walls
                currentCell.RemoveWall(neighbour.direction);
                neighbour.cell.RemoveWall(neighbour.direction.Invert());

                neighbour.cell.Visiting();
                cellDataStack.Push(neighbour.cell);
            }
        }
    }

    private IEnumerator NonRecursiveBacktrackerCorou(CellData firstCell)
    {
        Stack<CellData> cellDataStack = new Stack<CellData>();

        firstCell.Visiting();
        cellDataStack.Push(firstCell);
        while (cellDataStack.Count != 0)
        {
            CellData currentCell = cellDataStack.Pop();

            List<CardinalCell> neighboursUnvisited = new List<CardinalCell>();
            for (int i = 0; i < 4; i++)
            {
                var neighbour = currentCell.GetNeighbour((Direction)i);
                if (neighbour != null && !neighbour.visited)
                {
                    neighboursUnvisited.Add(new CardinalCell
                    {
                        direction = (Direction)i,
                        cell = neighbour
                    });
                }
            }

            if (neighboursUnvisited.Count != 0)
            {
                cellDataStack.Push(currentCell);
                var neighbour = neighboursUnvisited.GetRandomElement();

                //remove 2 walls
                currentCell.RemoveWall(neighbour.direction);
                neighbour.cell.RemoveWall(neighbour.direction.Invert());

                neighbour.cell.Visiting();
                cellDataStack.Push(neighbour.cell);
                yield return new WaitForSeconds(timerBuilderMaze);
            }
        }
    }

    public struct CardinalCell
    {
        public Direction direction;
        public CellData cell;
    }

    private void CorrectifWallTest()
    {
        /*for (int y = 0; y < heightStart; y++)
        {
            for (int x = 0; x < widthStart; x++)
            {
                if (y < heightStart - 1)
                    maze[y][x].RemoveWall(Direction.south);

                if(x < widthStart - 1)
                    maze[y][x].RemoveWall(Direction.east);
            }
        }*/

        for (int y = 0; y < heightStart; y++)
        {
            for (int x = 0; x < widthStart; x++)
            {
                var _cell = maze[y][x];

                _cell.CorrectifWall();

                if (_cell.GetNeighbour(Direction.north) != null && _cell.GetNeighbour(Direction.east) != null)
                {
                    if (_cell.GetNeighbour(Direction.north).HasWall(Direction.east) && _cell.GetNeighbour(Direction.east).HasWall(Direction.north))
                    {
                        _cell.SetActivePillar(InterCardinal.NE, true);
                    }
                }

                if (_cell.GetNeighbour(Direction.north) != null && _cell.GetNeighbour(Direction.west) != null)
                {
                    if (_cell.GetNeighbour(Direction.north).HasWall(Direction.west) && _cell.GetNeighbour(Direction.west).HasWall(Direction.north))
                    {
                        _cell.SetActivePillar(InterCardinal.NW, true);
                    }
                }

                if (_cell.GetNeighbour(Direction.south) != null && _cell.GetNeighbour(Direction.east) != null)
                {
                    if (_cell.GetNeighbour(Direction.south).HasWall(Direction.east) && _cell.GetNeighbour(Direction.east).HasWall(Direction.south))
                    {
                        _cell.SetActivePillar(InterCardinal.SE, true);
                    }
                }

                if (_cell.GetNeighbour(Direction.south) != null && _cell.GetNeighbour(Direction.west) != null)
                {
                    if (_cell.GetNeighbour(Direction.south).HasWall(Direction.west) && _cell.GetNeighbour(Direction.west).HasWall(Direction.south))
                    {
                        _cell.SetActivePillar(InterCardinal.SW, true);
                    }
                }
            }
        }
    }


    public void TakeKey(int xPlayer, int yPlayer)
    {
        hasKey = true;
        map.RemoveKeyMap(xPlayer, yPlayer);
    }

    public void TryOpenDoor()
    {
        if (hasKey)
        {
            CreateMaze();
        }
    }
    public bool TryGetCell(int _x, int _y, out CellData _cell)
    {
        if(_x >= 0 && _y >= 0)
        {
            if(_y < maze.Count)
            {
                if(_x < maze[_y].Count)
                {
                    _cell = maze[_y][_x];
                    return true;
                }
            }
        }
        _cell = null;
        return false;
    }
    public Vector2Int GetMazeSize()
    {
        return new Vector2Int(widthStart, heightStart);
    }
}
