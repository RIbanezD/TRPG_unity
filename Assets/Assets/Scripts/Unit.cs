// Unit.cs
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    // Lista de efectos de estado activos
    private List<AppliedStatus> activeStatusEffects = new List<AppliedStatus>();

    // Objeto para almacenar los datos base
    public UnitData baseStats;

    // Referencia a la UI para mostrar HP
    public TextMeshProUGUI hpText;

    // Referencia a la UI para mostrar MP
    public TextMeshProUGUI mpText;

    // Estadísticas actuales (variables que cambian durante la batalla)
    private int currentHP;
    private int currentMP;

    // Modificadores de Estadísticas
    private int attackModifier = 0;
    private int defenseModifier = 0;

    // Bandera para forzar el salto de turno
    public bool skipTurn = false;

    // Lista de estados activos en esta unidad
    public List<StatusEffect> activeEffects = new List<StatusEffect>();

    // Propiedad para obtener el Ataque actual (Base + Modificador)
    public int CurrentAttack => baseStats.attack + attackModifier;

    // Propiedad para obtener la Defensa actual (Base + Modificador)
    public int CurrentDefense => baseStats.defense + defenseModifier;

    // Inicializa la unidad con sus estadísticas base
    public void Initialize()
    {
        currentHP = baseStats.maxHP;
        currentMP = baseStats.maxMP;
        UpdateHPDisplay();
        UpdateMPDisplay();
    }

    [System.Serializable]
    public class AppliedStatus
    {
        public StatusEffect data;
        public int remainingDuration;

        public AppliedStatus(StatusEffect effectData)
        {
            data = effectData;
            remainingDuration = effectData.duration;
        }
    }

    // Comprueba y consume Maná
    public bool TryConsumeMP(int cost)
    {
        if (currentMP >= cost)
        {
            currentMP -= cost;
            UpdateMPDisplay();
            return true;
        }
        // No hay suficiente MP
        Debug.LogWarning(baseStats.unitName + " no tiene suficiente Maná para esta acción.");
        return false;
    }

    // Maneja la actualización visual del MP
    private void UpdateMPDisplay()
    {
        if (mpText != null)
        {
            // Formato: MP Actual / MP Máximo
            mpText.text = $"MP: {currentMP} / {baseStats.maxMP}";
        }
    }

    // Lógica para regenerar Maná
    public void RegenerateMP(int amount)
    {
        // Guardamos el MP antes de la regeneración
        int mpBeforeRegen = currentMP;

        // Aplicamos la regeneración
        currentMP = Mathf.Min(baseStats.maxMP, currentMP + amount);

        // Verificamos si hubo un cambio
        if (currentMP > mpBeforeRegen)
        {
            Debug.Log(baseStats.unitName + " regenera " + (currentMP - mpBeforeRegen) + " MP. Actual: " + currentMP);
        }

        UpdateMPDisplay();
    }

    public void ApplyStatusEffect(StatusEffect statusData)
    {
        if (statusData == null) return;

        // Verificar si el efecto ya está activo para no duplicarlo innecesariamente.
        if (activeStatusEffects.Exists(s => s.data.type == statusData.type))
        {
            Debug.LogWarning(baseStats.unitName + " ya tiene el efecto " + statusData.effectName);
            return;
        }

        activeStatusEffects.Add(new AppliedStatus(statusData));
        Debug.Log(baseStats.unitName + " fue afectado por " + statusData.effectName);

        // Lógica inmediata para Stun
        if (statusData.type == StatusEffectType.Stun)
        {
            skipTurn = true;
        }
    }

    public void ProcessStatusEffects()
    {
        // Reiniciar la bandera de salto de turno al inicio
        skipTurn = false;

        // Lista temporal para efectos a eliminar
        List<AppliedStatus> toRemove = new List<AppliedStatus>();

        // Reiniciar modificadores temporales
        attackModifier = 0;
        defenseModifier = 0;

        foreach (AppliedStatus status in activeStatusEffects)
        {
            if (status.data.type == StatusEffectType.Stun)
            {
                // Si estaba aturdido, el flag debe estar activo para el BattleManager.
                skipTurn = true;
            }

            // Simular efecto (ej: daño de veneno)
            if (status.data.type == StatusEffectType.Poison)
            {
                TakeDamage(5); // Daño fijo por veneno (ejemplo)
            }

            // Reducir duración y marcar para eliminación
            status.remainingDuration--;
            if (status.remainingDuration <= 0)
            {
                toRemove.Add(status);
            }
        }

        // Eliminar efectos expirados
        foreach (AppliedStatus status in toRemove)
        {
            Debug.Log(baseStats.unitName + ": El efecto " + status.data.effectName + " expiró.");
            // Si Stun termina, se elimina
            if (status.data.type == StatusEffectType.Stun)
            {
                skipTurn = false;
            }
            activeStatusEffects.Remove(status);
        }
    }

    // Lógica para tomar daño
    public void TakeDamage(int damageAmount)
    {
        // Fórmula de daño simple (puedes complicarla después)
        int defenseFactor = 1; // Simplificamos la defensa
        int finalDamage = Mathf.Max(0, damageAmount - CurrentDefense * defenseFactor);

        currentHP -= finalDamage;
        currentHP = Mathf.Max(0, currentHP);

        // **TODO: Actualizar la interfaz de usuario (UI) aquí**
        BattleEvents.UnitDamaged(this, finalDamage);
        Debug.Log(baseStats.unitName + " recibe " + finalDamage + " de daño. HP restante: " + currentHP);

        UpdateHPDisplay();

        if (currentHP <= 0)
        {
            Die();
        }
    }

    // Lógica para curar
    public void Heal(int healAmount)
    {
        // Verificar si la vida ya está al máximo
        if (currentHP == baseStats.maxHP)
        {
            Debug.Log(baseStats.unitName + ": ¡Vida al máximo! La curación no tuvo efecto.");
            return;
        }

        // Si no está al máximo, aplica la curación
        int hpBeforeHeal = currentHP;

        // Calcula la curación, asegurándose de que no exceda el máximo
        currentHP = Mathf.Min(baseStats.maxHP, currentHP + healAmount);

        // Calcula cuánta vida se curó realmente para el mensaje
        int actualHealed = currentHP - hpBeforeHeal;

        // Muestra el mensaje de curación y actualiza la UI
        Debug.Log(baseStats.unitName + " se cura " + actualHealed + " puntos.");
        UpdateHPDisplay();
    }

    // Maneja la actualización visual del HP
    private void UpdateHPDisplay()
    {
        if (hpText != null)
        {
            // Formato: HP Actual / HP Máximo
            hpText.text = $"HP: {currentHP} / {baseStats.maxHP}";
        }
        else
        {
            Debug.LogError(baseStats.unitName + " no tiene un componente de texto de HP asignado.");
        }
    }

    // Indica si la unidad está viva
    public bool IsAlive()
    {
        return currentHP > 0;
    }

    // Lógica de muerte
    private void Die()
    {
        Debug.Log(baseStats.unitName + " ha sido derrotado.");
        // **TODO: Notificar al BattleManager que la unidad ha muerto**
        BattleManager.Instance.CheckBattleEnd();
    }
}