
public enum CellType { Free, Wall, Chest, Trap, Visited, Listed, Exit };
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
        position.x = x;
        position.y = y;
    }
    public Cell(int x, int y, CellType type) : this(x, y)
    {
        cellType = type;
    }
    public Cell(IntVector2 position)
    {
        this.position = position;
    }
    public Cell(IntVector2 position, CellType type) : this(position)
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

    public bool IsFreeable()
    {
        return (Utility.Odd(position.x) && Utility.Odd(position.y)) ? true : false;
    }

    public bool Visited()
    {
        return (type == CellType.Visited) ? true : false;
    }

}