using UnityEngine;
using System.Collections.Generic;

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

    // Pseudo RNG
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