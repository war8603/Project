using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public Path Parent;
    public Tile Tile;

    public int F; // H + G
    public int G; // 시작점으로부터 현재까지 거리값
    public int H; // 현재부터 도착점까지의 거리값
}

public class HexPath : Path
{
    public new HexPath Parent;
    public new HexTile Tile;

    public HexPath(HexPath parent, HexTile tile, int g, int h)
    {
        Parent = parent;
        Tile = tile;
        F = g + h;
        G = g;
        H = h;
    }
}

public class SquPath : Path
{
    public new SquPath Parent;
    public new SquTile Tile;

    public SquPath(SquPath parent, SquTile tile, int g, int h)
    {
        Parent = parent;
        Tile = tile;
        F = g + h;
        G = g;
        H = h;
    }
}
