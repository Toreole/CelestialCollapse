using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Celestial
{
    public class Level : MonoBehaviour
    {
        //static shiz
        public static Level current { get; private set; }

        public static event System.Action<Level> OnModifierChanged;
        
        //private runtime shit
        private List<LevelModifier> activeModifiers = new List<LevelModifier>();
        private float enemyHealthMod = 1.0f;

        //public props.
        public float EnemyHealthMod => enemyHealthMod;

        public void AddModifier(LevelModifier mod)
        {
            activeModifiers.Add(mod);
            enemyHealthMod *= mod.enemyHealth; //multiplicative.
        }

    }
}