using UnityEngine;

public enum StatusEffectType 
{ 
    Stun, 
    Poison, 
    BuffAttack 
} 

[CreateAssetMenu(fileName = "NewStatusEffect", menuName = "Battle/Status Effect")]
public class StatusEffect : ScriptableObject
{
    public string effectName;
    public string description;
    public int duration = 1; 
    public StatusEffectType type; // Ahora StatusEffectType es reconocido
}