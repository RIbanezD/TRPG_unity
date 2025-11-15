using UnityEngine;
public interface IAbilityEffect
{
    void Execute(Unit caster, Unit target, int power);
}

// DamageEffect.cs - NUEVO
public class DamageEffect : IAbilityEffect
{
    public void Execute(Unit caster, Unit target, int power)
    {
        int damage = caster.baseStats.attack + power;
        target.TakeDamage(damage);
    }
}