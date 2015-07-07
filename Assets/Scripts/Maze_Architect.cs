using UnityEngine;
using System;
using System.Collections.Generic;

public enum CellType { Free, Wall, Chest, Visited, Listed, Exit };
public enum WallType { None, Isolate, Corner, Normal, Joint, invisible, outter }; 
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
    private Cell entrance;
    private Cell exit;

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
        SetRandomEntrance();
        SetRandomExit();
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
        cell.type = (cell.IsFreeable()) ? CellType.Visited : cell.type;
    }

    private void MarkNeighboursAsListed(List<Cell> neighbours)
    {
        neighbours.ForEach(delegate (Cell neighbour)
        {
            map[neighbour.position.x, neighbour.position.y].type = CellType.Listed;
        });
    }

    private void SetRandomEntrance()
    {
        do
        {
            int x = pseudoRNG.Next(totalCellsWidth);
            entrance = new Cell(x, 0);
        } while (map[entrance.position.x, 1].type != CellType.Free);

        // Set the entrance point into the map.
        map[entrance.position.x, entrance.position.y].type = CellType.Exit;

        // Displace the entranance one cell up for the algorithm.
        entrance = map[entrance.position.x, 1];
    }

    private void SetRandomExit()
    {
        do
        {
            int x = pseudoRNG.Next(totalCellsWidth - 1);
            exit = new Cell(x, totalCellsHeight - 1);
        } while (map[exit.position.x, totalCellsHeight - 2].type != CellType.Free);

        // Current exit point as exit into the map.
        map[exit.position.x, exit.position.y].type = CellType.Exit;

        // Exit point into the upper outter wall.
        exit = map[exit.position.x, totalCellsHeight - 1];
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

    private CornerType DetectCornerType(Cell cell)
    {
        // Get all the current neighbours of the current cell.
        List<Cell> neighbours = GetNeightbours(cell, 1);

        // Get only the wall neighbours.
        List<Cell> wallNeighbours = new List<Cell>();
        neighbours.ForEach(delegate (Cell neighbour)
        {
            if (neighbour.type == CellType.Wall)
            {
                wallNeighbours.Add(neighbour);
            }
        });

        // Detecting the order of the positions of the wall neighbours.
        if (wallNeighbours.Count == 1)
        {
            return CornerType.End;
        }
        else
        {
            if (wallNeighbours[0].position.y == wallNeighbours[1].position.y ||
                wallNeighbours[0].position.x == wallNeighbours[1].position.x)
            {
                return CornerType.None;
            }
            else
            {
                if (wallNeighbours[0].position.y > wallNeighbours[1].position.y)
                {
                    return (wallNeighbours[0].position.x > wallNeighbours[1].position.x) ? CornerType.UpLeft : CornerType.UpRight;
                }
                else
                {
                    return (wallNeighbours[0].position.x > wallNeighbours[1].position.x) ? CornerType.DownLeft : CornerType.DownRight;
                }
            }
        }
    }

    // Here is where all the magic happens ;P
    public void BestFirstOrdering()
    {
        Cell currentCell = entrance;
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
                localNeighbours = GetNeightbours(nextCell, 2);
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

        } while (pendingNeighbours.Count != 0);

    }

    // TODO: Operations like spawning chest or assigning wall types in the same loop.
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

    // TODO: Operations like spawning chest or assigning wall types in the same loop.
    public void AssignWallTypes()
    {
        Debug.Log("assigning wall types...");
        for (int x = 0; x < totalWidth; x++)
        {
            for (int y = 0; y < totalHeight; y++)
            {
                // Checking if it is an outter wall.
                if (x == 0 || y == 0 || x == totalWidth - 1 || y == totalHeight - 1)
                {
                    map[x, y].wall = WallType.outter;
                }
                else
                {
                    switch (DetectNumberWalls(map[x, y]))
                    {
                        case 0:
                            map[x, y].wall = WallType.Isolate;
                            break;
                        case 1:
                            map[x, y].wall = WallType.Corner;
                            map[x, y].corner = CornerType.End;
                            break;
                        case 2:
                            CornerType ct = DetectCornerType(map[x, y]);
                            if (ct != CornerType.None)
                            {
                                map[x, y].wall = WallType.Corner;
                                map[x, y].corner = ct;
                            }
                            else
                            {
                                map[x, y].wall = WallType.Normal;
                                map[x, y].corner = CornerType.None;
                            }
                            break;
                        case 3:
                            map[x, y].wall = WallType.Joint;
                            break;
                        case 4:
                            map[x, y].wall = WallType.invisible;
                            break;
                        default:
                            map[x, y].wall = WallType.None;
                            break;
                    }
                }
            }
        }
    }

}

public class ZoneMeshes
{
    private string zoneDirectoryPath = "./Assets/Resources/Zones/";
    private System.Random pseudoRNG;

    // Mesh lists for ground and chests.
    List<Mesh> groundList = new List<Mesh>();
    List<Mesh> chestList = new List<Mesh>();

    // Mesh lists for different wall types.
    List<Mesh> isolateWallList = new List<Mesh>();
    List<Mesh> normalWallList = new List<Mesh>();
    List<Mesh> jointWallList = new List<Mesh>();
    List<Mesh> invisibleWallList = new List<Mesh>();
    List<Mesh> outterWallList = new List<Mesh>();

    // Mesh lists for correnr types.
    List<Mesh> normalCornerList = new List<Mesh>();
    List<Mesh> endCornerList = new List<Mesh>();

    // Used to convert to List<Mesh>.
    Mesh[] meshList;

    // Load all the Mesh objects into the corresponding lists. 
    public ZoneMeshes(string zoneType, string seed)
    {
        // Getting random seed.
        pseudoRNG = new System.Random(seed.GetHashCode());

        // Throw exception if the zone doesn't exist.

        string localPath = "Zones/" + zoneType + "/Maze/";

        // Loading all ground meshes.
        meshList = Resources.LoadAll<Mesh>(localPath + "Ground");
        foreach (Mesh mesh in meshList)
        {
            groundList.Add(mesh);
        }

        // Loading all chest meshes.
        meshList = Resources.LoadAll<Mesh>(localPath + "Chests");
        foreach (Mesh mesh in meshList)
        {
            chestList.Add(mesh);
        }

        // Loading all isolated/column wall meshes.
        meshList = Resources.LoadAll<Mesh>(localPath + "Walls/Isolate");
        foreach (Mesh mesh in meshList)
        {
            isolateWallList.Add(mesh);
        }

        // Loading all normal wall meshes.
        meshList = Resources.LoadAll<Mesh>(localPath + "Walls/Normal");
        foreach (Mesh mesh in meshList)
        {
            normalWallList.Add(mesh);
        }

        // Loading all joint wall meshes.
        meshList = Resources.LoadAll<Mesh>(localPath + "Walls/Joint");
        foreach (Mesh mesh in meshList)
        {
            jointWallList.Add(mesh);
        }

        // Loading all invisible wall meshes.
        meshList = Resources.LoadAll<Mesh>(localPath + "Walls/invisible");
        foreach (Mesh mesh in meshList)
        {
            invisibleWallList.Add(mesh);
        }

        // Loading all outter wall meshes.
        meshList = Resources.LoadAll<Mesh>(localPath + "Walls/Outter");
        foreach (Mesh mesh in meshList)
        {
            outterWallList.Add(mesh);
        }

        // Loading all normal corners meshes.
        meshList = Resources.LoadAll<Mesh>(localPath + "Walls/Corner/Normal");
        foreach (Mesh mesh in meshList)
        {
            normalCornerList.Add(mesh);
        }

        // Loading all end corners meshes.
        meshList = Resources.LoadAll<Mesh>(localPath + "Walls/Corner/End");
        foreach (Mesh mesh in meshList)
        {
            endCornerList.Add(mesh);
        }

    }

    // Getters for all the types of meshes. Returning random mesh or null in case 
    public Mesh ground
    {
        get
        {
            if (groundList.Count != 0)
            {
                int index = pseudoRNG.Next(groundList.Count - 1);
                return groundList[index];
            }
            else
            {
                return null;
            }
        }
    }
    public Mesh chest
    {
        get
        {
            if (chestList.Count != 0)
            {
                int index = pseudoRNG.Next(chestList.Count - 1);
                return chestList[index];
            }
            else
            {
                return null;
            }
        }
    }
    public Mesh isolateWall
    {
        get
        {
            if (isolateWallList.Count != 0)
            {
                int index = pseudoRNG.Next(isolateWallList.Count - 1);
                return isolateWallList[index];
            }
            else
            {
                return null;
            }
        }
    }
    public Mesh normalWall
    {
        get
        {
            if (normalWallList.Count != 0)
            {
                int index = pseudoRNG.Next(normalWallList.Count - 1);
                return normalWallList[index];
            }
            else
            {
                return null;
            }
        }
    }
    public Mesh jointWall
    {
        get
        {
            if (jointWallList.Count != 0)
            {
                int index = pseudoRNG.Next(jointWallList.Count - 1);
                return jointWallList[index];
            }
            else
            {
                return null;
            }
        }
    }
    public Mesh invisibleWall
    {
        get
        {
            if (invisibleWallList.Count != 0)
            {
                int index = pseudoRNG.Next(invisibleWallList.Count - 1);
                return invisibleWallList[index];
            }
            else
            {
                return null;
            }
        }
    }
    public Mesh outterWall
    {
        get
        {
            if (outterWallList.Count != 0)
            {
                int index = pseudoRNG.Next(outterWallList.Count - 1);
                return outterWallList[index];
            }
            else
            {
                return null;
            }
        }
    }
    public Mesh normalCorner
    {
        get
        {
            if (normalCornerList.Count != 0)
            {
                int index = pseudoRNG.Next(normalCornerList.Count - 1);
                return normalCornerList[index];
            }
            else
            {
                return null;
            }
        }
    }
    public Mesh endCorner
    {
        get
        {
            if (endCornerList.Count != 0)
            {
                int index = pseudoRNG.Next(endCornerList.Count - 1);
                return endCornerList[index];
            }
            else
            {
                return null;
            }
        }
    }

}


public class Maze_Architect : MonoBehaviour
{
    // Parameters for the maze generator.
    [SerializeField][Range(4, 80)] int cellWidth = 0;
    [SerializeField][Range(4, 80)] int cellHeight = 0;
    [SerializeField][Range(0, 1)] float corridorDirection = 0.5f;
    [SerializeField][Range(0, 1)] float chestSpawnProvability = 0.5f;
    [SerializeField] string seed = "lolerpoper";
    [SerializeField] bool useRandomSeed = true;

    // Parameters for the maze theme.
    // TODO: Dinamically get the zones names inside the zones folder.
    [SerializeField] List<string> theme;

    // Parameters for the maze builder.

    private Maze maze;
    private ZoneMeshes zoneMeshes;

    void Start()
    {

        if (useRandomSeed)   
            seed = DateTime.Now.Ticks.ToString();

        GenerateMaze();
        GenerateZoneAssets("Ruins");
        BuildMaze();
       
    }

    void GenerateMaze()
    {
        Debug.Log("generating maze map...");
        maze = new Maze(cellWidth, cellHeight, seed, corridorDirection);
        maze.BestFirstOrdering();
        maze.SpawnChests(chestSpawnProvability);
        maze.AssignWallTypes();
    }

    void GenerateZoneAssets(string theme)
    {
        Debug.Log("generating zone meshes...");
        zoneMeshes = new ZoneMeshes(theme, seed);
    }

    void BuildMaze()
    {
        Debug.Log("building maze...");

        GameObject wall = Resources.Load("Wall", typeof(GameObject)) as GameObject;
        Mesh mesh = zoneMeshes.normalWall;
        wall.GetComponent<MeshFilter>().mesh = mesh;
        wall.GetComponent<MeshCollider>().sharedMesh = mesh;

        // Object creation testing.
        //GameObject go = new GameObject("Wall");
        //go.transform.position = new Vector3(0, 0, 0);
        //go.transform.rotation = new Quaternion(0, 90, 90, 1);
  
        //go.AddComponent<MeshFilter>();
        //go.AddComponent<MeshRenderer>();
        //go.AddComponent<MeshCollider>();

        //go.GetComponent<MeshRenderer>().material = Resources.Load("Materials/Gray", typeof(Material)) as Material;
        //go.GetComponent<MeshCollider>().convex = true;
        //go.GetComponent<MeshFilter>().mesh = zoneMeshes.normalWall;

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
                        Gizmos.color = Color.gray;
                        break;
                    case CellType.Wall:
                        switch (maze.map[x, y].wall)
                        {
                            case WallType.Isolate:
                                Gizmos.color = new Color(0.73f, 0.67f, 0.26f);
                                break;
                            case WallType.Corner:
                                switch (maze.map[x, y].corner)
                                {
                                    case CornerType.None:
                                        Gizmos.color = new Color(0, 1, 0, 0.2f);
                                        break;
                                    case CornerType.DownLeft:
                                        Gizmos.color = new Color(0, 50f, 0, 0.5f);
                                        break;
                                    case CornerType.DownRight:
                                        Gizmos.color = new Color(0, 100f, 0, 0.5f);
                                        break;
                                    case CornerType.UpLeft:
                                        Gizmos.color = new Color(0, 1, 0, 0.5f);
                                        break;
                                    case CornerType.UpRight:
                                        Gizmos.color = new Color(0, 1, 0, 0.4f);
                                        break;
                                    case CornerType.End:
                                        Gizmos.color = new Color(0, 1, 0.5f, 0.7f);
                                        break;
                                    default:
                                        Gizmos.color = new Color(0, 1, 0, 0.5f);
                                        break;
                                }
                                break;
                            case WallType.Normal:
                                Gizmos.color = new Color(0, 1, 0, 0.5f);
                                break;
                            case WallType.Joint:
                                Gizmos.color = new Color(0, 1, 0, 0.9f);
                                break;
                            case WallType.invisible:
                                Gizmos.color = new Color(0, 1, 1, 0.9f);
                                break;
                            case WallType.outter:
                                Gizmos.color = new Color(0.84f, 0.63f, 0.38f);
                                break;
                            case WallType.None:
                                Gizmos.color = Color.gray;
                                break;
                        }
                        break;
                    case CellType.Chest:
                        Gizmos.color = Color.yellow;
                        break;
                    case CellType.Visited:
                        Gizmos.color = Color.gray;
                        break;
                    case CellType.Listed:
                        Gizmos.color = Color.gray;
                        break;
                    case CellType.Exit:
                        Gizmos.color = Color.gray;
                        break;
                }

                Vector3 pos = new Vector3(-maze.totalCellsWidth / 2 + x + .5f, 0, -maze.totalCellsHeight / 2 + y + .5f);
                Gizmos.DrawCube(pos, Vector3.one);
            }
        }
    }

}
