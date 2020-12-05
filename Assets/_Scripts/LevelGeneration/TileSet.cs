using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NaughtyAttributes;

using Random = System.Random;

namespace Celestial.Levels
{
    [CreateAssetMenu(menuName="SO/Levels/TileSet")]
    public class TileSet : ScriptableObject
    {
        [SerializeField]
        float tileSize;

        [SerializeField]
        List<Tile> standardTiles;
        [SerializeField]
        GameObject[] bossTiles;
        [SerializeField]
        List<Tile> entranceTiles;
        [SerializeField]
        GameObject[] shopTiles;
        [SerializeField, Tooltip("Tiles with 1 connection, but no room.")]
        GameObject[] deadEndTiles;

        ///<summary>The width/length of all tiles in this set given in units</summary>
        public float TileSize => tileSize;

        ///<summary>Get a List of standard tiles that match the predicate.</summary>
        public List<Tile> GetTilesWith(Predicate<Tile> match)
            => standardTiles.FindAll(match);
        public List<Tile> GetStandardTiles() => standardTiles;

        public GameObject GetBossTile(Random rngProvider)
            => bossTiles[rngProvider.Next(bossTiles.Length)];
        public GameObject GetShopTile(Random rngProvider)
            => shopTiles[rngProvider.Next(shopTiles.Length)];
        public List<Tile> GetEntranceTiles()
            => entranceTiles;
        public GameObject GetDeadEnd(Random rngProvider)
            => deadEndTiles[rngProvider.Next(deadEndTiles.Length)];
    }

    public class Tile
    {
        public GameObject tileMeshPrefab;
        public int entranceCount;
        [EnumFlags]
        public Cardinals entrancesOnSides;

        public static implicit operator Tile(GameObject source)
        {
            return new Tile()
            {
                tileMeshPrefab = source,
                entranceCount = 0,
                entrancesOnSides = Cardinals.Undefined
            };
        }
    }

    [Flags]
    ///<summary>
    /// North = +z | East = +x
    /// South = -z | West = -x
    ///</summary>
    public enum Cardinals
    {
        Undefined = 0,
        ///<summary>North = +z </summary>
        North = 1, 
        ///<summary>East = +x </summary>
        East = 2, 
        ///<summary>South = -z </summary>
        South = 4, 
        ///<summary>West = -x </summary>
        West = 8
    }
}