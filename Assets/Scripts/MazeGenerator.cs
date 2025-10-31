using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Size")]
    public int width = 15;
    public int height = 15;
    public float tileSize = 2f;

    [Header("Prefabs")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject candyPrefab;
    public GameObject ghostPrefab;
    public GameObject exitGatePrefab;

    [Header("Gameplay")]
    public int candyCount = 12;
    public int ghostCount = 3;

    [Header("Parents")]
    public Transform mazeRoot;

    public Vector3 PlayerSpawnWorldPos { get; private set; }
    public Vector3 ExitWorldPos { get; private set; }

    private bool[,] grid; // true = cell carved/visited

    struct Cell { public int x, y; public Cell(int X,int Y){x=X;y=Y;} }

    public void Generate()
    {
        ClearMaze();
        grid = new bool[width, height];

        // Carve maze using iterative DFS (backtracker)
        Stack<Cell> stack = new Stack<Cell>();
        Cell start = new Cell(1, 1);
        grid[start.x, start.y] = true;
        stack.Push(start);

        System.Random rng = new System.Random();
        List<Vector2Int> dirs = new List<Vector2Int>{ new Vector2Int(1,0), new Vector2Int(-1,0), new Vector2Int(0,1), new Vector2Int(0,-1)};

        while (stack.Count > 0)
        {
            Cell current = stack.Peek();
            // Shuffle directions
            for (int i = 0; i < dirs.Count; i++) {
                int j = rng.Next(i, dirs.Count);
                (dirs[i], dirs[j]) = (dirs[j], dirs[i]);
            }
            bool moved = false;
            foreach (var d in dirs)
            {
                int nx = current.x + d.x * 2;
                int ny = current.y + d.y * 2;
                if (InBounds(nx, ny) && !grid[nx, ny])
                {
                    grid[nx, ny] = true;
                    grid[current.x + d.x, current.y + d.y] = true; // carve wall between
                    stack.Push(new Cell(nx, ny));
                    moved = true; break;
                }
            }
            if (!moved) stack.Pop();
        }

        // Build geometry
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            // Floor everywhere
            Vector3 pos = ToWorld(x, y);
            Instantiate(floorPrefab, pos, Quaternion.identity, mazeRoot);

            // Walls where grid is false
            if (!grid[x, y])
            {
                Vector3 wpos = ToWorld(x, y) + Vector3.up; // 2 units tall via scale
                var wall = Instantiate(wallPrefab, wpos, Quaternion.identity, mazeRoot);
                wall.transform.localScale = new Vector3(tileSize, 2f, tileSize);
            }
        }

        // Spawn candies in carved cells
        List<Vector2Int> carved = new List<Vector2Int>();
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (grid[x, y]) carved.Add(new Vector2Int(x, y));

        for (int i = 0; i < Mathf.Min(candyCount, carved.Count); i++)
        {
            int idx = rng.Next(carved.Count);
            var c = carved[idx];
            carved.RemoveAt(idx);
            Instantiate(candyPrefab, ToWorld(c.x, c.y) + Vector3.up * 0.5f, Quaternion.identity, mazeRoot);
        }

        // Player spawn = near (1,1)
        PlayerSpawnWorldPos = ToWorld(1, 1) + Vector3.up * 0.1f;

        // Exit at far corner on a carved cell
        Vector2Int exit = new Vector2Int(width - 2, height - 2);
        while (!grid[exit.x, exit.y])
            exit = new Vector2Int(Mathf.Max(1, exit.x - 1), Mathf.Max(1, exit.y - 1));
        var gate = Instantiate(exitGatePrefab, ToWorld(exit.x, exit.y), Quaternion.identity, mazeRoot);
        ExitWorldPos = gate.transform.position;

        // Ghosts
        for (int g = 0; g < ghostCount; g++)
        {
            int idx = rng.Next(carved.Count);
            var cell = carved[idx];
            Vector3 gpos = ToWorld(cell.x, cell.y) + Vector3.up * 0.5f;
            var ghost = Instantiate(ghostPrefab, gpos, Quaternion.identity, mazeRoot);
        }
    }

    public void ClearMaze()
    {
        for (int i = mazeRoot.childCount - 1; i >= 0; i--)
            DestroyImmediate(mazeRoot.GetChild(i).gameObject);
    }

    bool InBounds(int x, int y) => x > 0 && y > 0 && x < width && y < height;

    Vector3 ToWorld(int x, int y) => new Vector3(x * tileSize, 0, y * tileSize);
}
