using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Celestial.Levels
{
    public static partial class Util
    {
        public static readonly Vector3Int Vector3IntForward = new Vector3Int(0, 0, 1);
        public static readonly Vector3Int Vector3IntBack = new Vector3Int(0, 0, -1);

        ///<summary>Gets the worldspace Vector3Int equivalent of the cardinal.</summary>
        ///<arg name=original>Has to be a singular value, not multiple flags</arg>
        public static Vector3Int GetDirection(this Cardinals original)
        {
            switch(original)
            {
                default:
                    return Vector3Int.zero;
                case Cardinals.North:
                    return Vector3IntForward;
                case Cardinals.East:
                    return Vector3Int.right;
                case Cardinals.South:
                    return Vector3IntBack;
                case Cardinals.West:
                    return Vector3Int.left;
            }
        }

        public static Cardinals RotateBy90ClockWise(this Cardinals original)
        {
            if(original == Cardinals.Undefined) return Cardinals.Undefined;
            Cardinals rotated = 0;
            if(original.HasFlag(Cardinals.North))
                rotated |= Cardinals.East;
            if(original.HasFlag(Cardinals.East))
                rotated |= Cardinals.South;
            if(original.HasFlag(Cardinals.South))
                rotated |= Cardinals.West;
            if(original.HasFlag(Cardinals.West))
                rotated |= Cardinals.North;
            return rotated;
        }
        public static Cardinals RotateBy90CounterClockWise(this Cardinals original)
        {
            if(original == Cardinals.Undefined) return Cardinals.Undefined;
            Cardinals rotated = 0;
            if(original.HasFlag(Cardinals.North))
                rotated |= Cardinals.West;
            if(original.HasFlag(Cardinals.East))
                rotated |= Cardinals.North;
            if(original.HasFlag(Cardinals.South))
                rotated |= Cardinals.East;
            if(original.HasFlag(Cardinals.West))
                rotated |= Cardinals.South;
            return rotated;
        }

        public static List<Cardinals> Seperate(this Cardinals original)
        {
            List<Cardinals> list = new List<Cardinals>();
            if(original.HasFlag(Cardinals.North))
                list.Add(Cardinals.North);
            if(original.HasFlag(Cardinals.East))
                list.Add(Cardinals.East);
            if(original.HasFlag(Cardinals.South))
                list.Add(Cardinals.South);
            if(original.HasFlag(Cardinals.West))
                list.Add(Cardinals.West);
            return list;
        }

        //just a small helper to figure out whether this is a straight.
        public static bool DescribesStraight(this Cardinals cardinals)
         => cardinals.HasFlag(Cardinals.North | Cardinals.South) || cardinals.HasFlag(Cardinals.East | Cardinals.West);

        public static Cardinals Invert(this Cardinals original)
        {
            //this works because every cardinal has its counterpart two bits to the left/right.
            //combine the results with an or, and clear the remaining part with AND 31.
            return original == Cardinals.Undefined? Cardinals.Undefined : (Cardinals)((int)original << 2 | (int)original >> 2 & 0x1111);
        }
    }
}