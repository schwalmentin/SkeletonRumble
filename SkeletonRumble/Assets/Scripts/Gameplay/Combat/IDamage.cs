using UnityEngine;

public interface IDamage
{
    void GetDamage(float damage, Vector2 knockBack, float stunTime, bool weightless, MeleeAttack meleeAttack);

    void Die();

    void GetStunned(float stunTime);
}
