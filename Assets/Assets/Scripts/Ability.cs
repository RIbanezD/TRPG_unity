// Ability.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Battle/Ability")]
public class Ability : ScriptableObject
{
    public string abilityName;
    public string description;

    // Costos
    public int mpCost;
    
    // Poder/Curación
    public int power; 

    // Referencia al efecto de estado que esta habilidad aplica
    public StatusEffect statusEffectData;

    // Define si la habilidad es ataque, curación, o buff.
    public AbilityType type;

    // Define a quién afecta (necesario para la UI posterior)
    public TargetType target; 
    
    public IAbilityEffect effect;
}

// Enumeraciones para definir el tipo y el objetivo (útil para la IA y UI)
public enum AbilityType { Damage, Heal, Buff }
public enum TargetType { SingleEnemy, SingleAlly, Self }