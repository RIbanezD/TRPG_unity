// UnitData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewUnit", menuName = "Battle/Unit Data")]
public class UnitData : ScriptableObject
{
    [Header("Info Básica")]
    public string unitName;
    
    [Header("Estadísticas")]
    public int maxHP;
    public int maxMP;
    public int attack;
    public int defense;
    
    [Header("Habilidades")]
    public Ability[] abilities;
}