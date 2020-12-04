using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Celestial.Levels
{
    public class GridTile
    {
        public Vector3Int gridPosition;
        public TileFlags flags;
        public GameObject instance;
        //just for convenience
        public List<TileConnection> connections;
    }

    public class TileConnection
    {
        public GridTile other;
        public PathStyle pathStyle;
        public bool open; //Whether there is a connection

        //ctor
        public TileConnection(){}
        public TileConnection(GridTile tile)
        {
            other = tile;
        }
    }

    [System.Flags]
    public enum TileFlags
    {
        Undefined,
        Entrance = 1 << 1, 
        BossRoom = 1 << 2, 
        Standard = 1 << 3,
        Shop = 1 << 4,
        Special = 1 << 5
    }

    public enum PathStyle //specific to the tiles. preferably only have centeronly for now.
    {
        CenterOnly, LeftOnly, RightOnly, BothSides, FullWidth
    }   
}