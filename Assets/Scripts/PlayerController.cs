using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private int viewDirection = 0;
    private Direction viewPlayer = Direction.north;
    private CellData currentCell;
    public delegate void PlayerViewDirection(Direction _dir);
    public event PlayerViewDirection viewPlayerEvent;
    public delegate void PlayerMoveDirecton(int _x, int _y);
    public event PlayerMoveDirecton playerMoveEvent;

    public void Initialize(CellData _cell)
    {
        currentCell = _cell;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            viewDirection = (4 + viewDirection - 1) % 4;
            SetDirection();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            viewDirection = (4 + viewDirection + 1) % 4;
            SetDirection();
        }
        else if(Input.GetKeyDown(KeyCode.Z))
        {
            if (currentCell.HasWall(viewPlayer) || currentCell.GetNeighbour(viewPlayer) == null)
                return;

            currentCell = currentCell.GetNeighbour(viewPlayer);
            playerMoveEvent?.Invoke(currentCell.X, currentCell.Y);
            transform.position = currentCell.GetPositionForPlayer;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            Direction _inverseDir = Direction.north;
            switch (viewPlayer)
            {
                case Direction.north:
                    _inverseDir = Direction.south;
                    break;
                case Direction.south:
                    _inverseDir = Direction.north;
                    break;
                case Direction.east:
                    _inverseDir = Direction.west;
                    break;
                case Direction.west:
                    _inverseDir = Direction.east;
                    break;
            }

            if (currentCell.HasWall(_inverseDir) || currentCell.GetNeighbour(_inverseDir) == null)
                return;

            currentCell = currentCell.GetNeighbour(_inverseDir);
            playerMoveEvent?.Invoke(currentCell.X, currentCell.Y);
            transform.position = currentCell.GetPositionForPlayer;
        }
    }

    private void SetDirection()
    {
        switch (viewDirection)
        {
            case 0:
                viewPlayer = Direction.north;
                transform.localRotation = Quaternion.Euler(Vector3.up * 0f);
                break;
            case 1:
                viewPlayer = Direction.east;
                transform.localRotation = Quaternion.Euler(Vector3.up * 90f);
                break;
            case 2:
                viewPlayer = Direction.south;
                transform.localRotation = Quaternion.Euler(Vector3.up * 180f);
                break;
            case 3:
                viewPlayer = Direction.west;
                transform.localRotation = Quaternion.Euler(Vector3.up * -90f);
                break;
        }
        viewPlayerEvent?.Invoke(viewPlayer);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Key")
        {
            MazeGenerator.Instance.TakeKey(currentCell.X, currentCell.Y);
            //play sound
            Destroy(other.gameObject);
            print("Take key !");
        }
        else if(other.tag == "Exit")
        {
            MazeGenerator.Instance.TryOpenDoor();
        }
    }

    public void PlayerPosition()
    {
        playerMoveEvent?.Invoke(currentCell.X, currentCell.Y);
    }

    public void ForcePlayerView()
    {
        viewPlayerEvent?.Invoke(viewPlayer);
    }
}
