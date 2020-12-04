using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Celestial.Levels
{
    public class LevelGenerator : MonoBehaviour
    {
#region Serialized_Settings
        [SerializeField]
        int mainPathLength = 4;
        [SerializeField]
        int seed = 666;
        //TODO: need a tileset of some sort.
#endregion

#region Runtime_Generation
        List<GridTile> tiles;

        Vector3Int bossRoomOffset;

        Random rng;
#endregion

        private void Start() 
        {
            InitializeGeneration();
            GenerateLevelLayout();
        }

        ///<summary>Initialize parameters for the actual generation of the level.</summary>
        private void InitializeGeneration()
        {
            rng = new Random(seed);
            //path length in X and Z direction.
            int xPathLength = rng.Next(1, mainPathLength - 1);
            int zPathLength = mainPathLength - xPathLength;
            //offset in x and z direction randomized to get more diverse level layouts.
            bossRoomOffset.x = rng.Next(2) > 0 ? -xPathLength: xPathLength;
            bossRoomOffset.z = rng.Next(2) > 0 ? -zPathLength: zPathLength;

            //The first tile.
            tiles = new List<GridTile>();
            GridTile startTile = new GridTile()
            {
                gridPosition = Vector3Int.zero,
                flags = TileFlags.Entrance,
                instance = null,
                connections = new List<TileConnection>()
            };
            //the last tile in the floor.
            GridTile lastTile = new GridTile()
            {
                gridPosition = bossRoomOffset,
                flags = TileFlags.BossRoom,
                instance = null,
                connections = new List<TileConnection>()
            };
            //Add both to the level
            tiles.Add(startTile);
            tiles.Add(lastTile);
        }

        ///<summary>Generates the level layout based on the intialized tiles list.</summary>
        private void GenerateLevelLayout()
        {
            //TODO: this.
            Vector3Int stepsTaken = Vector3Int.zero;
            Vector3Int currentPosition = Vector3Int.zero;
            //Step 1: Make a path between the start and the end.
            for(int i = 0; i < mainPathLength-1; i++)
            {
                //whether the generator can step in the given direction.
                bool canStepX = stepsTaken.x < Mathf.Abs(bossRoomOffset.x);
                bool canStepZ = stepsTaken.z < Mathf.Abs(bossRoomOffset.z);

                //if any direction is fine, randomize, otherwise go in whichever is available.
                bool walksInX = canStepX && canStepZ && rng.Next(2)>0 || canStepX && !canStepZ;
                TakeStep(walksInX, ref stepsTaken, ref currentPosition);
            }
            //THIS IS FOR DEBUGGING ONLY!
            foreach(var tile in tiles)
            {
                Transform t = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                t.position = tile.gridPosition*2;
                t.localScale = tile.flags.HasFlag(TileFlags.Entrance) || tile.flags.HasFlag(TileFlags.BossRoom) ? new Vector3(2, 2, 2): Vector3.one; 
            }
            //Step 2: branch off from the main path.
            int lastMainPathTile = tiles.Count-1;
        }

        ///<summary>Take a step in either X or Z, directly towards the boss room.</summary>
        private void TakeStep(bool inXDirection, ref Vector3Int steps, ref Vector3Int position)
        {
            Vector3Int oldPosition = position;
            Vector3Int lateral = inXDirection? Vector3Int.right : new Vector3Int(0, 0, 1); //Why is there no .forward???
            steps += lateral;
            //the actual world direction we are going in.
            Vector3Int direction = inXDirection? (position.x < bossRoomOffset.x? lateral: -lateral)
                                                : (position.z < bossRoomOffset.z? lateral: -lateral);
            position += direction;
            //Now create a gridtile at the position.
            GridTile tile = new GridTile()
            {
                gridPosition = position,
                flags = TileFlags.Standard, //for now, every tile is a normal tile, idc.
                instance = null,
                connections = new List<TileConnection>()
            };
            //add the connections.
            GridTile oldTile = tiles.Find(x => x.gridPosition == oldPosition);
            tile.connections.Add(new TileConnection(oldTile));
            oldTile.connections.Add(new TileConnection(tile));
            //add the tile to the list of all tiles.
            tiles.Add(tile);
        }
    }
}