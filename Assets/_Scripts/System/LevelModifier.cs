using UnityEngine;

namespace Celestial
{
    [CreateAssetMenu(fileName = "new Level Modifier", menuName = "SO/Level/Modifier")]
    public class LevelModifier : ScriptableObject
    {
        [Min(1)]
        public float enemyHealth;
    }
}
