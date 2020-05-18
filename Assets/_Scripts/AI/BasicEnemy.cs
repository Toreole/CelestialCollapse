using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.AI;

namespace Celestial
{
    public class BasicEnemy : MonoBehaviour
    {
        [SerializeField]
        protected float detectionRadius = 5, pathUpdateInterval = 0.5f;
        [SerializeField]
        protected SphereCollider detectionCollider;
        [SerializeField]
        protected float attackRange;
        [SerializeField]
        protected float attackSpeed;
        [SerializeField]
        protected float attackDamage;
        [SerializeField]
        protected float baseMaxHealth;
        [SerializeField]
        protected NavMeshAgent agent;

        protected float currentHealth, maxHealth;
        protected PlayerMove player;

        public float CurrentHealth
        {
            get => currentHealth;
            protected set
            {
                currentHealth = value;
                //UpdateHealthUI();
            }
        }

        public Transform Destination { get; protected set; }
        
        private void OnValidate()
        {
            if (detectionCollider)
                detectionCollider.radius = detectionRadius;
        }

        // Start is called before the first frame update
        void Start()
        {
            maxHealth = baseMaxHealth;
            currentHealth = maxHealth;
        }

        private void OnEnable()
        {
            Level.OnModifierChanged += Recalculate;
        }

        private void OnDisable()
        {
            Level.OnModifierChanged -= Recalculate;
        }

        private void Recalculate(Level lvl)
        {
            //%health
            var perc = currentHealth / maxHealth;
            //mod maxHealth
            maxHealth = baseMaxHealth * lvl.EnemyHealthMod;
            //reset health to %
            currentHealth = perc * maxHealth;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                Destination = other.transform;
                //get player.
            }
        }

        protected float lastUpdate;
        private void Update()
        {
            if(Destination )
            {
                if (Time.time - lastUpdate > pathUpdateInterval)
                {
                    lastUpdate = Time.time;
                    agent.SetDestination(Destination.position);
                }
                if (Vector3.Distance(transform.position, Destination.position) <= attackRange)
                {
                    Attack();
                }
            }
        }

        void Attack()
        {
            //make player take damage.
        }

        public void Damage(float amount)
        {
            CurrentHealth -= amount;
            print("ouch");
            //UpdateHealthUI();
        }
    }
}