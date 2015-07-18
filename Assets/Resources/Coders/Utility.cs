
public static class Utility
{
    // Math functions for identifying odd and even numbers.
    public static bool Even(int num)
    {
        return (num % 2 == 0) ? true : false;
    }
    public static bool Odd(int num)
    {
        return (num % 2 != 0) ? true : false;
    }

}

// Struct used to specify postions in the maze map.
public struct IntVector2
{
    public int x;
    public int y;
    public IntVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class PseudoRNG : System.Random
{

}
