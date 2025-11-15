// UnitData.cs
using UnityEngine;

// [System.Serializable] permite ver y editar esta clase en el Inspector de Unity
[System.Serializable]
public class UnitData
{
    // Las estadísticas mínimas para tu MVP
    public string unitName;
    public int maxHP;
    public int maxMP;
    public int attack;
    public int defense;

    public Ability[] abilities;
}