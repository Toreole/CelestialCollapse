using UnityEngine;
using System.Collections;

namespace Celestial.Debugging
{
    public class TestModifier : MonoBehaviour , IInteractable
    {
        public LevelModifier mod;

        public void Interact()
        {
            Level.current.AddModifier(mod);
            Destroy(this);
        }
        
    }
}