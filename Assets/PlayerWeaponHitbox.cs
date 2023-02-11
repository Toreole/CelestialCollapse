using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Celestial
{
    public class PlayerWeaponHitbox : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var enemy = other.GetComponent<BasicEnemy>();
            if(enemy)
            {
                //TODO: input damage and knockback, etc. from player.
                enemy.Damage(5);
                //TODO: knockback should depend on the direction the damage is coming from (for things like explosions or whatever)
                enemy.KnockBack(1);
            }
        }
    }
}