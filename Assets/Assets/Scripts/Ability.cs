// Ability.cs
using UnityEngine;

[System.Serializable]
public class Ability
{
    public string abilityName;
    public string description;

    // Costos
    public int mpCost;
    
    // Poder/Curación
    public int power; 

    // Define si la habilidad es ataque, curación, o buff.
    public AbilityType type; 
    
    // Define a quién afecta (necesario para la UI posterior)
    public TargetType target; 
}

// Enumeraciones para definir el tipo y el objetivo (útil para la IA y UI)
public enum AbilityType { Damage, Heal, Buff }
public enum TargetType { SingleEnemy, SingleAlly, Self }