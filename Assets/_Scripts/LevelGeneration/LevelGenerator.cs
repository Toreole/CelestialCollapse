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
        TileFlags roomsNotGenerated;

        Vector3Int bossRoomOffset;

        Random rng;
        Random connectionRng;
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
            connectionRng = new Random(seed);
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

            //set up the set of special rooms to be generated: //only 1 of each possible with this.
            roomsNotGenerated = TileFlags.Shop | TileFlags.Special;
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
#region DEBUG_ONLY
            //THIS IS FOR DEBUGGING ONLY!
            foreach(var tile in tiles)
            {
                Transform t = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                t.position = tile.gridPosition*2;
                t.localScale = tile.flags.HasFlag(TileFlags.Entrance) || tile.flags.HasFlag(TileFlags.BossRoom) ? new Vector3(2, 2, 2): Vector3.one; 
                tile.instance = t.gameObject;
            }
#endregion
            //Step 2: branch off from the main path.
            int lastMainPathTile = tiles.Count-1;
            for(int i = 0; i <= lastMainPathTile; i++)
            {
                GridTile currentTile = tiles[i];
                if(currentTile.flags.HasFlag(TileFlags.BossRoom)) //dont add rooms onto the boss room.
                    continue;
                //2.1: check if we should start adding rooms here
                if(rng.Next(2)>0) //50%chance to add a room onto the current one.
                    AddBranchTile(currentTile);
            }

#region DEBUG_ONLY
            foreach(GridTile tile in tiles)
                foreach(TileConnection connection in tile.connections)
                {
                    Transform t = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
                    t.position = ((Vector3)(tile.gridPosition*2 + connection.other.gridPosition*2))*0.5f;
                    t.localScale = Vector3.one * 0.45f;
                }
#endregion
        }

        private void AddBranchTile(GridTile currentTile)
        {
            //2.2: check for a direction add a room in.
            List<Vector3Int> directions = new List<Vector3Int>(); //list of available directions that have not been occupied yet.
            //this may not be nice looking or good code, but it works and is reasonably fast.
            if(!currentTile.connections.Exists(x => x.other.gridPosition == currentTile.gridPosition + Vector3Int.right)) 
                directions.Add(Vector3Int.right);
            if(!currentTile.connections.Exists(x => x.other.gridPosition == currentTile.gridPosition + Vector3Int.left))
                directions.Add(Vector3Int.left);
            if(!currentTile.connections.Exists(x => x.other.gridPosition == currentTile.gridPosition + new Vector3Int(0,0,1)))
                directions.Add(new Vector3Int(0,0,1));
            if(!currentTile.connections.Exists(x => x.other.gridPosition == currentTile.gridPosition + new Vector3Int(0,0,-1)))
                directions.Add(new Vector3Int(0,0,-1));
            //Is there a free direction we can add something to?
            if(directions.Count > 0)
            {
                //Generate the direction we want.
                Vector3Int direction = directions[rng.Next(directions.Count)];
                GridTile newTile = new GridTile()
                {
                    gridPosition = currentTile.gridPosition + direction,
                    flags = (roomsNotGenerated.HasFlag(TileFlags.Shop) && rng.Next(2)>0)? TileFlags.Shop : TileFlags.Standard,
                    instance = null,
                    connections = new List<TileConnection>()
                };
                newTile.connections.Add(new TileConnection(currentTile));
                currentTile.connections.Add(new TileConnection(newTile));
#region DEBUG_ONLY
                Transform t = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                t.position = newTile.gridPosition * 2;
                t.localScale = Vector3.one * 0.5f;
                newTile.instance = t.gameObject;
#endregion
                //add the newly generated tile to the list.
                tiles.Add(newTile);
                //2.3: check for other neighbours that already exist => chance to connect them all together.
                AttemptConnections(newTile);
            }
        }

        private void AttemptConnections(GridTile tile)
        {
            AttemptConnectionInDirection(tile, Vector3Int.right);
            AttemptConnectionInDirection(tile, Vector3Int.left);
            AttemptConnectionInDirection(tile, new Vector3Int(0,0,1));
            AttemptConnectionInDirection(tile, new Vector3Int(0,0,-1));
        }

        private void AttemptConnectionInDirection(GridTile tile, Vector3Int direction)
        {
            Vector3Int position = tile.gridPosition + direction;
            GridTile otherTile = tiles.Find(x => x.gridPosition == position);
            //other must exist, and not be connected already.
            if(otherTile != null && !tile.connections.Exists(x => x.other == otherTile) && connectionRng.Next(2) > 0)
            {
                tile.connections.Add(new TileConnection(otherTile));
                otherTile.connections.Add(new TileConnection(tile));
            }
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