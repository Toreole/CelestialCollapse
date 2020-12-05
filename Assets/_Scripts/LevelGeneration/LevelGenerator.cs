﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Random = System.Random;
using Debug = UnityEngine.Debug;

using static Celestial.Levels.Util;

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
        [SerializeField]
        TileSet tileSet;
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
            //PlaceLevel();
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
                instance = null
            };
            //the last tile in the floor.
            GridTile lastTile = new GridTile()
            {
                gridPosition = bossRoomOffset,
                flags = TileFlags.BossRoom,
                instance = null
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
            //temp stat
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
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
            //INBETWEEN: make sure the boss room is connected!
            {
                GridTile bossRoom = tiles[1];
                GridTile lastPath = tiles[tiles.Count-1];
                bossRoom.AddConnection(new TileConnection(lastPath));
                lastPath.AddConnection(new TileConnection(bossRoom));
            }
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
            //readonly:
            Cardinals[] cards = {Cardinals.North, Cardinals.East, Cardinals.South, Cardinals.West};
            //Step 3: mark required walls! (rooms next to each other without connection)
            foreach(GridTile gridTile in tiles)
            {
                foreach(Cardinals cardinal in cards)
                {
                    Vector3Int otherPos = gridTile.gridPosition + cardinal.GetDirection();
                    //if the tile exists, but we dont have a connection to it
                    if(tiles.Exists(x => x.gridPosition == otherPos) && !gridTile.HasConnectionAtLocation(otherPos))
                        gridTile.RequiresWalls |= cardinal;
                }
            }
            stopwatch.Stop();
            Debug.Log($"Finished generating the layout in {stopwatch.ElapsedMilliseconds}ms");
        }

        ///<summary>Adds a branch tile to the current one. </summary>
        private void AddBranchTile(GridTile currentTile)
        {
            //2.2: check for a direction add a room in.
            List<Vector3Int> directions = new List<Vector3Int>(); //list of available directions that have not been occupied yet.
            //this may not be nice looking or good code, but it works and is reasonably fast.
            if(!currentTile.HasConnectionAtLocation(currentTile.gridPosition + Vector3Int.right)) 
                directions.Add(Vector3Int.right);
            if(!currentTile.HasConnectionAtLocation(currentTile.gridPosition + Vector3Int.left))
                directions.Add(Vector3Int.left);
            if(!currentTile.HasConnectionAtLocation(currentTile.gridPosition + Vector3IntForward))
                directions.Add(Vector3IntForward);
            if(!currentTile.HasConnectionAtLocation(currentTile.gridPosition + Vector3IntBack))
                directions.Add(Vector3IntBack);
            //Is there a free direction we can add something to?
            if(directions.Count > 0)
            {
                //Generate the direction we want.
                Vector3Int direction = directions[rng.Next(directions.Count)];
                GridTile newTile = new GridTile()
                {
                    gridPosition = currentTile.gridPosition + direction,
                    //TODO: this does not guarantee a shop to exist in the level!
                    flags = (roomsNotGenerated.HasFlag(TileFlags.Shop) && rng.Next(2)>0)? TileFlags.Shop : TileFlags.Standard,
                    instance = null
                };
                newTile.AddConnection(new TileConnection(currentTile));
                currentTile.AddConnection(new TileConnection(newTile));
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
            AttemptConnectionInDirection(tile, Vector3IntForward);
            AttemptConnectionInDirection(tile, Vector3IntBack);
        }

        private void AttemptConnectionInDirection(GridTile tile, Vector3Int direction)
        {
            Vector3Int position = tile.gridPosition + direction;
            GridTile otherTile = tiles.Find(x => x.gridPosition == position);
            //other must exist, and not be connected already.
            if(otherTile != null && !tile.HasConnectionTo(otherTile) && connectionRng.Next(2) > 0)
            {
                tile.AddConnection(new TileConnection(otherTile));
                otherTile.AddConnection(new TileConnection(tile));
            }
        }

        ///<summary>Take a step in either X or Z, directly towards the boss room.</summary>
        private void TakeStep(bool inXDirection, ref Vector3Int steps, ref Vector3Int position)
        {
            Vector3Int oldPosition = position;
            Vector3Int lateral = inXDirection? Vector3Int.right : Vector3IntForward;
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
                instance = null
            };
            //add the connections.
            GridTile oldTile = tiles.Find(x => x.gridPosition == oldPosition);
            tile.AddConnection(new TileConnection(oldTile));
            oldTile.AddConnection(new TileConnection(tile));
            //add the tile to the list of all tiles.
            tiles.Add(tile);
        }
    
        ///<summary>Place the parts of the level in the scene.</summary>
        private void PlaceLevel()
        {
            List<Tile> twoEntrances = tileSet.GetTilesWith(x=>x.entranceCount == 2); //straights and corners
            List<Tile> threeEntrances = tileSet.GetTilesWith(x=>x.entranceCount==3); // T-crossings
            List<Tile> fourEntrances = tileSet.GetTilesWith(x=>x.entranceCount==4); // +

            foreach(GridTile gridTile in tiles)
            {
                TileFlags flags = gridTile.flags;
                if(flags.HasFlag(TileFlags.BossRoom))
                {
                    
                }
                else if(flags.HasFlag(TileFlags.Entrance))
                {
                    //1. get possible tiles for the entrance.
                    tileSet.GetEntranceTiles().FindAll(x => x.entranceCount >= gridTile.ConnectionCount);
                }
                else if(flags.HasFlag(TileFlags.Shop))
                {

                }
                else
                {
                    
                }
            }
        }
    }
}