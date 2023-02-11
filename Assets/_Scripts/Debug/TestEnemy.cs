using UnityEngine;
using System.Collections;
using Celestial;

namespace Celestial.Debugging
{
    public class TestEnemy : MonoBehaviour, IDamagable, IInteractable
    {
        public float baseMaxHealth;
        public TMPro.TextMeshProUGUI healthText;

        public UnityEngine.InputSystem.InputActionReference testAction;
        
        public float CurrentHealth { get => currentHealth;
            protected set
            {
                currentHealth = value;
                UpdateHealthUI();
            }
        }
        private float maxHealth;
        private float currentHealth;

        public void Damage(float amount)
        {
            CurrentHealth -= amount;
            print("ouch");
            UpdateHealthUI();
        }

        // Use this for initialization
        void Start()
        {
            currentHealth = baseMaxHealth;
            maxHealth = baseMaxHealth;
            Level.OnModifierChanged += Recalculate;
            UpdateHealthUI();
        }

        void UpdateHealthUI()
        {
            healthText.text = $"{currentHealth.ToString("00.0")}/{maxHealth.ToString("00.0")}";
        }

        void Recalculate(Level l)
        {
            //%health
            var perc = currentHealth / maxHealth;
            //mod maxHealth
            maxHealth = baseMaxHealth * l.EnemyHealthMod;
            //reset health to %
            currentHealth = perc * maxHealth;
            UpdateHealthUI();
        }

        public void Interact()
        {
            Damage(5);
        }
    }
}