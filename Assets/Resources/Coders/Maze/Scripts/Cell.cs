

//public enum CellType { Free, Wall, Chest, Trap, Visited, Listed, Entrance, Exit };

//public class Cell
//{ 
//    public IntVector2 position;
//    protected CellType cellType;

//    public Cell(int x, int y)
//    {
//        position.x = x;
//        position.y = y;
//    }
//    public Cell(int x, int y, CellType type) : this(x, y)
//    {
//        cellType = type;
//    }

//    public virtual CellType Type
//    {
//        get { return cellType; }
//    }

//    public bool IsFreeable()
//    {
//        return (Utility.Odd(position.x) && Utility.Odd(position.y)) ? true : false;
//    }
//    public bool Visited()
//    {
//        return (Type == CellType.Visited) ? true : false;
//    }

//}
