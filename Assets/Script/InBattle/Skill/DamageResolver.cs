using UnityEngine;

public static class DamageResolver
{
    public static void ApplyDamage(
        CharacterManager target,
        float damage
    )
    {
        target.Stats.MHealth -= damage;
        target.Stats.MHealth = Mathf.Max(0, target.Stats.MHealth);

        target.Hud.TakeDamage(damage);

        if (target.Stats.MHealth <= 0 && !target.Hud.isDead)
        {
            target.OnDeath();
        }
    }
}
