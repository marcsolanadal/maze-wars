using System.Collections.Generic;

public enum CellType { Free, Wall, Chest, Trap, Visited, Listed, Exit };
public enum WallType { None, Isolate, Corner, Normal, Joint, invisible, outter };
public enum CornerType { None, End, UpLeft, UpRight, DownLeft, DownRight };
public enum TrapType { Floor, SingleWall, DoubleWall, Ceiling };

public interface ICellable
{
    int[] getPosition();
    int[] getBoundaries();
}

public class Cell : ICellable
{
    // TODO: This should be only read only from the outside 
    // and be modifiable in this class.
    public int x { get; set; }
    public int y { get; set; }

    private CellType cellType;
    private WallType wallType;
    private CornerType cornerType;

    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public Cell(int x, int y, CellType type) : this(x, y)
    {
        cellType = type;
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

    // Interface methods.
    public int[] getPosition()
    {
        return new int[]{ x, y };
    }
    public int[] getBoundaries()
    {
        return new int[]{ x, y };
    }

    public bool IsFreeable()
    {
        return (Utility.Odd(x) && Utility.Odd(y)) ? true : false;
    }
    public bool Visited()
    {
        return (cellType == CellType.Visited) ? true : false;
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

    // Important maze positions.
    private Cell entrance;
    private Cell exit;

    // Maze info.
    private int cellNumber;
    private int chestNumber;
    private int trapNumber;

    // Pseudo RNG.
    private System.Random pseudoRNG;

    public Maze(int width, int height, string seed, float corridorDirection)
    {
        this.corridorDirection = corridorDirection;

        // Calculating maze size with walls.
        totalWidth = 2 * width + 1;
        totalHeight = 2 * height + 1;

        // Setting maze size information.
        cellNumber = totalWidth * totalHeight;

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

    public Cell Entrance
    {
        get { return entrance; }
    }
    public Cell Exit
    {
        get { return exit; }
    }

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

    // position[0] = x
    // position[1] = y
    // boundaries[0] = x + width
    // boundaries[1] = y + height
    private bool InsideMap<T>(T cellable) where T : ICellable
    {
        int[] position = cellable.getPosition();
        int[] boundaries = cellable.getBoundaries();

        return (position[0] >= 0 && position[1] >= 0 &&
                boundaries[0] < totalWidth && boundaries[1] < totalHeight) ? true : false;
    }

    private bool ListedNeighbour(Cell cell)
    {
        return (InsideMap(cell) && cell.type == CellType.Listed) ? true : false;
    }

    private void MarkNeighboursAsListed(List<Cell> neighbours)
    {
        neighbours.ForEach(delegate (Cell neighbour)
        {
            map[neighbour.x, neighbour.y].type = CellType.Listed;
        });
    }

    private void SetRandomEntrance()
    {
        do
        {
            int x = pseudoRNG.Next(totalCellsWidth);
            entrance = new Cell(x, 0);
        } while (map[entrance.x, 1].type != CellType.Free);

        // Set the entrance point into the map.
        map[entrance.x, entrance.y].type = CellType.Exit;

        // Move the entranance one cell up for the algorithm.
        entrance = map[entrance.x, 1];
    }

    private void SetRandomExit()
    {
        do
        {
            int x = pseudoRNG.Next(totalCellsWidth - 1);
            exit = new Cell(x, totalCellsHeight - 1);
        } while (map[exit.x, totalCellsHeight - 2].type != CellType.Free);

        // Current exit point as exit into the map.
        map[exit.x, exit.y].type = CellType.Exit;

        // Exit point into the upper outter wall.
        exit = map[exit.x, totalCellsHeight - 1];
    }

    private void RemoveWall(Cell fromCell, Cell toCell)
    {
        // Detecting the direction of the move for the X axis and removing the wall acordingly.
        if (toCell.x - fromCell.x > 0)
        {
            map[toCell.x - 1, toCell.y].type = CellType.Free;
        }
        else if (toCell.x - fromCell.x < 0)
        {
            map[toCell.x + 1, toCell.y].type = CellType.Free;
        }

        // Detecting the direction of the move for the Y axis and removing the wall acordingly.
        if (toCell.y - fromCell.y > 0)
        {
            map[toCell.x, toCell.y - 1].type = CellType.Free;
        }
        else if (toCell.y - fromCell.y < 0)
        {
            map[toCell.x, toCell.y + 1].type = CellType.Free;
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
        for (int i = currentCell.x - spacing; i <= currentCell.x + spacing; i += spacing)
        {
            // First we need to check if the position of the possible cell will be in map bounds.
            //if (InsideMap(new IntVector2(i, currentCell.y)))
            if(InsideMap(new Cell(i, currentCell.y)))
            //if(InsideMap(map[i, currentCell.y]))
            {
                Cell neighbour = map[i, currentCell.y];
                // Discarting the currentCell as neighbour.
                if (currentCell.x != neighbour.x)
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
        for (int n = currentCell.y - spacing; n <= currentCell.y + spacing; n += spacing)
        {
            if (InsideMap(new Cell(currentCell.x, n)))
            {
                Cell neighbour = map[currentCell.x, n];
                if (currentCell.y != neighbour.y)
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
        int nextX = nextCell.x;
        int nextY = nextCell.y;

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
            if (wallNeighbours[0].y == wallNeighbours[1].y ||
                wallNeighbours[0].x == wallNeighbours[1].x)
            {
                return CornerType.None;
            }
            else
            {
                if (wallNeighbours[0].y > wallNeighbours[1].y)
                {
                    return (wallNeighbours[0].x > wallNeighbours[1].x) 
                        ? CornerType.UpLeft 
                        : CornerType.UpRight;
                }
                else
                {
                    return (wallNeighbours[0].x > wallNeighbours[1].x) 
                        ? CornerType.DownLeft 
                        : CornerType.DownRight;
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
                map[nextCell.x, nextCell.y].type = CellType.Visited;

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

    public void AssignCellTypes(float chestProvability, float trapProvability)
    {
        chestNumber = 0;
        trapNumber = 0;

        for (int x = 0; x < totalWidth; x++)
        {
            for (int y = 0; y < totalHeight; y++)
            {
                // Generating Chests.
                if (map[x, y].type == CellType.Visited && DetectNumberWalls(map[x, y]) >= 3)
                {
                    if (pseudoRNG.NextDouble() <= chestProvability)
                    {
                        map[x, y].type = CellType.Chest;
                        chestNumber++;
                    }
                }

                // Generating Traps.
                if (map[x, y].type == CellType.Visited || map[x, y].type == CellType.Free
                    && DetectNumberWalls(map[x, y]) <= 2)
                {
                    if (pseudoRNG.NextDouble() <= trapProvability)
                    {
                        map[x, y].type = CellType.Trap;
                        trapNumber++;
                    }
                }

                // Assigning wall types.
                if (x == 0 || y == 0 || x == totalWidth - 1 || y == totalHeight - 1)
                {
                    map[x, y].wall = WallType.outter;
                }
                else
                {
                    // TODO: I don't like this switch statement. 
                    // Maybe there's a better disign option?
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

    public int[] GetMazeInfo()
    {
        return new int[] { cellNumber, chestNumber, trapNumber };
    }

}

public class CellGroup
{
    protected List<Cell> group;
}

public class Room : CellGroup, ICellable
{
    int x;
    int y;
    int height;
    int width;

    public Room(int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.height = height;
        this.width = width;
    }

    public int[] getPosition()
    {
        return new int[] { x, y };
    }

    public int[] getBoundaries()
    {
        return new int[] { x + width, y + height };
    }

}

