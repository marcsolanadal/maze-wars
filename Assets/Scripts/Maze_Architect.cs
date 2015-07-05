using UnityEngine;
using System;
using System.Collections.Generic;

public enum CellType { Free, Wall, Chest, Visited, Listed, Exit };
public enum WallType { None, Isolate, Corner, Normal, Joint, Unvisible }; 
public enum CornerType { None, End, UpLeft, UpRight, DownLeft, DownRight };

public class Cell
{
    public IntVector2 position;

    private CellType cellType;
    private WallType wallType;
    private CornerType cornerType;

    public Cell(int x, int y)
    {
        this.position.x = x;
        this.position.y = y;
    }
    public Cell(int x, int y, CellType type) : this(x, y)
    {
        this.cellType = type;
    }
    public Cell(IntVector2 position)
    {
        this.position = position;
    }
    public Cell(IntVector2 position, CellType type) : this(position)
    {
        this.cellType = type;
    }

    public CellType type
    {
        get { return cellType; }
        set { cellType = value; }
    }
    public WallType wall
    {
        get { return (cellType == CellType.Wall) ? wallType : WallType.None; }
        set { wallType = value; }
    }
    public CornerType corner
    {
        get { return (wallType == WallType.Corner) ? cornerType : CornerType.None; }
        set { cornerType = value; }
    }

    public bool IsFreeable()
    {
        return (Utility.Odd(this.position.x) && Utility.Odd(this.position.y)) ? true : false;
    }

    public bool Visited()
    {
        return (this.type == CellType.Visited) ? true : false;
    }

}

public class Maze
{
    public Cell[,] map;
    private List<Cell> pendingNeighbours;

    // This width and height are counting the walls.
    private int totalWidth;
    private int totalHeight;
    private float corridorDirection;

    // Important maze positions
    private Cell algorithmStartPoint;
    private Cell entrance;

    // Pseudo RNG
    private string seed;
    private System.Random pseudoRNG;

    // Constructor overloading
    //public Maze(string seed)
    //{
    //    Debug.Log("pre - width:" + width.ToString() + " heigth: " + height.ToString());

    //    // Calculating maze size with walls
    //    totalWidth = 2 * width + 1;
    //    totalHeight = 2 * height + 1;

    //    Debug.Log("post - width:" + totalWidth.ToString() + " heigth: " + totalHeight.ToString());


    //    // Important default positions
    //    // TODO: Implement a setRandomEntryPoint
    //    // TODO: Change end variable for startPoint
    //    end = new Cell(1, 1);

    //    // Getting random seed
    //    //this.seed = seed;
    //    pseudoRNG = new System.Random(seed.GetHashCode());

    //    // Creating map skeleton
    //    map = new Cell[totalWidth, totalHeight];
    //    GenerateSkeleton();

    //    // Initialize neighbour list
    //    pendingNeighbours = new List<Cell>();
    //}
    //public Maze(int width, int height, string seed) : this(seed)
    //{
    //    this.width = width;
    //    this.height = height;
    //}
    public Maze(int width, int height, string seed, float corridorDirection)
    {
        // Check if start and stop are cells or walls
        // We use the setter because we also apply the value to the map.
        //exitCell = SetRandomEntrance();
        //this.algorithmStartPoint = algorithmStartPoint;
        this.seed = seed;
        this.corridorDirection = corridorDirection;

        // Calculating maze size with walls.
        totalWidth = 2 * width + 1;
        totalHeight = 2 * height + 1;

        // Getting random seed.
        pseudoRNG = new System.Random(seed.GetHashCode());

        // Creating map skeleton.
        map = new Cell[totalWidth, totalHeight];
        GenerateSkeleton();

        // Initialize neighbour list.
        pendingNeighbours = new List<Cell>();

        // Algorithm starting point used as entrance point.
        algorithmStartPoint = SetRandomEntrance();
    }

    // Getters and setters
    public Cell exitCell
    {
        get { return algorithmStartPoint; }
        set
        {
            algorithmStartPoint = value;
            value.type = CellType.Exit; // We assign the algorithm starting point as visited.
        }
    }
    
    // This ones are only for the gizmo
    public int totalCellsWidth
    {
        get { return totalWidth; }
    }
    public int totalCellsHeight
    {
        get { return totalHeight; }
    }

    // Generation functions
    private void GenerateSkeleton()
    {
        for (int x = 0; x < totalWidth; x++)
        {
            for (int y = 0; y < totalHeight; y++)
            {
                // Outer maze walls.
                if (x == 0 || x == totalWidth - 1 || y == 0 || y == totalHeight - 1)
                {
                    map[x, y] = new Cell(x, y, CellType.Wall);
                }
                // Odd numbers will be holes.
                else if ((x % 2 != 0) && (y % 2 != 0))
                {
                    map[x, y] = new Cell(x, y, CellType.Free);
                }
                // The other cases will be cells.
                else
                {
                    map[x, y] = new Cell(x, y, CellType.Wall);
                }
            }
        }
    }

    private bool InsideMap(IntVector2 position)
    {
        return (position.x >= 0 && position.x < totalWidth && position.y >= 0 && position.y < totalHeight) ? true : false;
    }

    private bool Visited(Cell cell)
    {
        return (cell.type == CellType.Visited) ? true : false;
    }

    private bool ListedNeighbour(Cell cell)
    {
        return (InsideMap(cell.position) && cell.type == CellType.Listed) ? true : false;
    }

    private void MarkCellAsVisited(Cell cell)
    {
        // We need to check that is a cell and not a wall.
        cell.type = (cell.IsFreeable()) ? CellType.Visited : cell.type;
    }

    private void MarkNeighboursAsListed(List<Cell> neighbours)
    {
        neighbours.ForEach(delegate (Cell neighbour)
        {
            map[neighbour.position.x, neighbour.position.y].type = CellType.Listed;
        });
    }

    private Cell SetRandomEntrance()
    {
        do
        {
            int x = pseudoRNG.Next(totalCellsHeight);
            entrance = new Cell(x, 0);
        } while (map[entrance.position.x, 1].type != CellType.Free);

        map[entrance.position.x, entrance.position.y].type = CellType.Exit;

        return map[entrance.position.x, 1];

    }

    private void RemoveWall(Cell fromCell, Cell toCell)
    {
        // Detecting the direction of the move for the X axis and removing the wall acordingly.
        if (toCell.position.x - fromCell.position.x > 0)
        {
            map[toCell.position.x - 1, toCell.position.y].type = CellType.Free;
        }
        else if (toCell.position.x - fromCell.position.x < 0)
        {
            map[toCell.position.x + 1, toCell.position.y].type = CellType.Free;
        }

        // Detecting the direction of the move for the Y axis and removing the wall acordingly.
        if (toCell.position.y - fromCell.position.y > 0)
        {
            map[toCell.position.x, toCell.position.y - 1].type = CellType.Free;
        }
        else if (toCell.position.y - fromCell.position.y < 0)
        {
            map[toCell.position.x, toCell.position.y + 1].type = CellType.Free;
        }
    }

    // Detect number of walls around a cell
    private int DetectNumberWalls(Cell cell)
    {
        int wallCounter = 0;
        GetNeightbours(cell, 1).ForEach(delegate (Cell neighbour)
        {
            if (neighbour.type == CellType.Wall)
            {
                wallCounter += 1;
            }
        });

        //Debug.Log("DetectNumberWalls(cell[" + cell.position.x + "," + cell.position.y + "]) = " + wallCounter);

        return wallCounter;
    }

    private List<Cell> GetNeightbours(Cell currentCell, int spacing)
    {
        List<Cell> currentNeighbours = new List<Cell>();

        // Getting neighbours in the X axis.
        for (int i = currentCell.position.x - spacing; i <= currentCell.position.x + spacing; i += spacing)
        {
            // First we need to check if the position of the possible cell will be in map bounds.
            if (InsideMap(new IntVector2(i, currentCell.position.y)))
            {
                Cell neighbour = map[i, currentCell.position.y];
                // Discarting the currentCell as neighbour.
                if (currentCell.position.x != neighbour.position.x)
                {
                    // Looking for seed cells?
                    if (spacing == 2)
                    {
                        if (neighbour.IsFreeable() && !neighbour.Visited())
                            currentNeighbours.Add(neighbour);
                    }
                    else
                    {
                        currentNeighbours.Add(neighbour);
                    }
                }
            }
        }

        // Getting neighbours in the Y axis.
        for (int n = currentCell.position.y - spacing; n <= currentCell.position.y + spacing; n += spacing)
        {
            if (InsideMap(new IntVector2(currentCell.position.x, n)))
            {
                Cell neighbour = map[currentCell.position.x, n];
                if (currentCell.position.y != neighbour.position.y)
                {
                    // Looking for seed cells?
                    if (spacing == 2)
                    {
                        if (neighbour.IsFreeable() && !neighbour.Visited())
                            currentNeighbours.Add(neighbour);
                    }
                    else
                    {
                        currentNeighbours.Add(neighbour);
                    }
                }
            }
        }

        return currentNeighbours;
    }

    private Cell ToRandomNeighbour(List<Cell> currentCellNeighbours, float corridorDirection)
    {
        // Parametrizing the random walk in order to influence the maze form.
        int randomIndex = 0;
        if (pseudoRNG.NextDouble() <= corridorDirection)
        {
            randomIndex = pseudoRNG.Next(currentCellNeighbours.Count / 2);
        }
        else
        {
            randomIndex = pseudoRNG.Next(currentCellNeighbours.Count / 2, currentCellNeighbours.Count);
        }

        // Extract the positional information from the next cell.
        Cell nextCell = currentCellNeighbours[randomIndex];
        int nextX = nextCell.position.x;
        int nextY = nextCell.position.y;

        // Mark the chosen next cell as visited.
        map[nextX, nextY].type = CellType.Visited;
        
        // Remove the chosen cell from the local neighbour list.
        currentCellNeighbours.RemoveAt(randomIndex);

        // We make sure we're not adding an object twice into the pending neighbours list.
        currentCellNeighbours.ForEach(delegate (Cell neighbour)
        {
            // Check if the current neighbour is in the pending list.
            int index = pendingNeighbours.IndexOf(neighbour);
            if (index == -1)
            {
                pendingNeighbours.Add(neighbour);
            }
        });

        return nextCell;
    }

    // Here is where all the magic happens ;P
    public void BestFirstOrdering()
    {
        Cell currentCell = algorithmStartPoint;
        Cell nextCell;
        List<Cell> localNeighbours;

        do
        {
            // We mark the current cell as visited.
            currentCell.type = CellType.Visited;

            // Take the next free cell neighbours. At first they're all spaced with one wall in between.
            localNeighbours = GetNeightbours(currentCell, 2);
            MarkNeighboursAsListed(localNeighbours);

            // If no local neighbours
            if (localNeighbours.Count == 0)
            {
                // Get last pending neighbour as nextCell and remove it from the pending list.
                nextCell = pendingNeighbours[pendingNeighbours.Count - 1];
                pendingNeighbours.Remove(nextCell);

                // Mark the cell as visited.
                map[nextCell.position.x, nextCell.position.y].type = CellType.Visited;

                // Get the neighbours of next cell.
                localNeighbours = GetCellNeightbours(nextCell);
                MarkNeighboursAsListed(localNeighbours);

                // Choose one with the CellType.Visited as the currentCell.
                localNeighbours.ForEach(delegate (Cell neighbour)
                {
                    // Choose one with the CellType.Visited as the currentCell.
                    if (neighbour.type == CellType.Visited)
                    {
                        currentCell = neighbour;
                    }
                });
            }
            else
            {
                // Move to a randomly chosen local neighbour and remove it from the pendingNeighbours list.
                nextCell = ToRandomNeighbour(localNeighbours, corridorDirection);
                pendingNeighbours.Remove(nextCell);
            }

            // Remove the wall between the last and the current cells.
            RemoveWall(currentCell, nextCell);

            currentCell = nextCell;

            //current = NextStep(current);
        } while (pendingNeighbours.Count != 0);

        //map[algorithmStartPoint.position.x, algorithmStartPoint.position.y].type = CellType.Exit;

    }

    // Spawn chests in the corners of the maze
    // TODO: We can do all the operations like spawning chest or assigning wall types in the same loop.
    public void SpawnChests(float provability)
    {
        Debug.Log("spawning chests...");
        for (int x = 0; x < totalWidth; x++)
        {
            for (int y = 0; y < totalHeight; y++)
            {
                if (map[x, y].type == CellType.Visited && DetectNumberWalls(map[x, y]) >= 3)
                {
                    if (pseudoRNG.NextDouble() <= provability)
                    {
                        map[x, y].type = CellType.Chest;
                    }
                }    
            }
        }
    }

    // TODO
    // Procedurally create wall combinations
    // Detect how many wall neighbours we have in the wall cell.
    // Assing WallType depending on the number of walls for each cell.

    // TODO: We can do all the operations like spawning chest or assigning wall types in the same loop.
    public void AssignWallTypes()
    {
        Debug.Log("assigning wall types...");
        for (int x = 0; x < totalWidth; x++)
        {
            for (int y = 0; y < totalHeight; y++)
            {
                //if (map[x, y].type == CellType.Wall)
                //{
                    switch (DetectNumberWalls(map[x, y]))
                    {
                        case 0:
                            map[x, y].wall = WallType.Isolate;
                            break;
                        case 1:
                            map[x, y].wall = WallType.Corner;
                            break;
                        case 2:
                            map[x, y].wall = WallType.Normal;
                            break;
                        case 3:
                            map[x, y].wall = WallType.Joint;
                            break;
                        case 4:
                            map[x, y].wall = WallType.Unvisible;
                            break;
                        default:
                            map[x, y].wall = WallType.None;
                            break;
                    }
                    Debug.Log("[" + x + "," + y + "] = " + map[x,y].wall);
                //}
            }
        }
    }
}

public class Maze_Architect : MonoBehaviour
{
    [SerializeField][Range(4, 80)] int cellWidth = 0;
    [SerializeField][Range(4, 80)] int cellHeight = 0;
    [SerializeField][Range(0, 1)] float corridorDirection = 0.5f;
    [SerializeField][Range(0, 1)] float chestSpawnProvability = 0.5f;
    [SerializeField] string seed = "lolerpoper";
    [SerializeField] bool useRandomSeed = true;

    private Maze maze;

    void Start()
    {
        if (useRandomSeed)   
            seed = DateTime.Now.Ticks.ToString();

        GenerateMaze();
    }

    void GenerateMaze()
    {
        maze = new Maze(cellWidth, cellHeight, seed, corridorDirection);
        maze.BestFirstOrdering();
        maze.SpawnChests(chestSpawnProvability);
        maze.AssignWallTypes();
    }

    // Helper function for visualization
    void OnDrawGizmos()
    {
        for (int x = 0; x < maze.totalCellsWidth; x++)
        {
            for (int y = 0; y < maze.totalCellsHeight; y++)
            {
                switch (maze.map[x, y].type)
                {
                    case CellType.Free:
                        Gizmos.color = Color.white;
                        break;
                    case CellType.Wall:
                        Gizmos.color = Color.black;
                        break;
                    case CellType.Chest:
                        Gizmos.color = Color.green;
                        break;
                    case CellType.Visited:
                        Gizmos.color = Color.white;
                        break;
                    case CellType.Listed:
                        Gizmos.color = Color.yellow;
                        break;
                    case CellType.Exit:
                        Gizmos.color = Color.red;
                        break;
                }

                //switch (maze.map[x, y].wall)
                //{
                //    case WallType.Isolate:
                //        Gizmos.color = Color.yellow;
                //        break;
                //    case WallType.Corner:
                //        Gizmos.color = Color.cyan;
                //        break;
                //    case WallType.Normal:
                //        Gizmos.color = Color.black;
                //        break;
                //    case WallType.Joint:
                //        Gizmos.color = Color.magenta;
                //        break;
                //    case WallType.Unvisible:
                //        Gizmos.color = Color.red;
                //        break;
                //    case WallType.None:
                //        Gizmos.color = Color.gray;
                //        break;
                //}

                Vector3 pos = new Vector3(-maze.totalCellsWidth / 2 + x + .5f, 0, -maze.totalCellsHeight / 2 + y + .5f);
                Gizmos.DrawCube(pos, Vector3.one);
            }
        }
    }

}
