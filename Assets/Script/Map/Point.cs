using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{
    public int x;
    public int y;
}

public class SquPoint : Point
{
    public SquPoint(int xValue, int yValue)
    {
        x = xValue;
        y = yValue;
    }

    public override string ToString()
    {
        return x + " " + y;
    }

    public static bool operator ==(SquPoint p1, SquPoint p2)
    {
        return (p1.x == p2.x && p1.y == p2.y);
    }

    public static bool operator !=(SquPoint p1, SquPoint p2)
    {
        return (p1.x != p2.x || p1.y != p2.y);
    }

    public static SquPoint operator +(SquPoint p1, SquPoint p2)
    {
        return new SquPoint(p1.x + p2.x, p1.y + p2.y);
    }

    public static SquPoint operator -(SquPoint p1, SquPoint p2)
    {
        return new SquPoint(p1.x - p2.x, p1.y - p2.y);
    }
}

public class HexPoint : Point
{
    public int z;

    public HexPoint(int xValue, int yValue, int zValue)
    {
        x = xValue;
        y = yValue;
        z = zValue;
    }

    public override string ToString()
    {
        return x + " " + y + " " + z;
    }

    public static HexPoint operator +(HexPoint p1, HexPoint p2)
    {
        return new HexPoint(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);
    }

    public static HexPoint operator -(HexPoint p1, HexPoint p2)
    {
        return new HexPoint(p1.x - p2.x, p1.y - p2.y, p1.z - p2.z);
    }

    public HexPoint Normalize
    {
        get
        {
            int xValue = x == 0 ? 0 : (x < 0 ? -1 : 1);
            int yValue = y == 0 ? 0 : (y < 0 ? -1 : 1);
            int zValue = z == 0 ? 0 : (z < 0 ? -1 : 1);
            return new HexPoint(xValue, yValue, zValue);
        }
    }

    public static bool operator ==(HexPoint p1, HexPoint p2)
    {
        return (p1.x == p2.x && p1.y == p2.y && p1.z == p2.z);
    }

    public static bool operator !=(HexPoint p1, HexPoint p2)
    {
        return (p1.x != p2.x || p1.y != p2.y || p1.z != p2.z);
    }

    public static HexPoint operator *(HexPoint p1, int value)
    {
        return new HexPoint(p1.x * value, p1.y * value, p1.z * value);
    }
}

