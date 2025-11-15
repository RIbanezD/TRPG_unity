using UnityEngine;
using System;

public static class BattleEvents
{
    public static event Action<Unit, int> OnUnitDamaged;
    public static event Action<Unit, int> OnUnitHealed;
    public static event Action<Unit> OnUnitDied;
    
    public static void UnitDamaged(Unit unit, int amount) 
        => OnUnitDamaged?.Invoke(unit, amount);
}