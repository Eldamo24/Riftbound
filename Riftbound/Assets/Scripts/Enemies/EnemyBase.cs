using Riftbound.Core;
using UnityEngine;

namespace Riftbound.Enemies
{
    public abstract class EnemyBase : MonoBehaviour, IDamageable, IAttack, IMovable
    {
        [Header("Stats")]
        [SerializeField] protected float maxHealth = 30f;
        [SerializeField] protected float moveSpeed = 2f;

        protected float currentHealth;

        protected virtual void Awake()
        {
            currentHealth = maxHealth;
        }

        public abstract void Move(Vector3 direction);

        public abstract void Attack();

        public virtual void TakeDamage(float amount)
        {
            currentHealth -= amount;
            if (currentHealth <= 0f)
            {
                currentHealth = 0f;
                Die();
            }
        }

        protected virtual void Die()
        {
            Debug.Log($"{gameObject.name} murió.");
            Destroy(gameObject);
        }
    }
}