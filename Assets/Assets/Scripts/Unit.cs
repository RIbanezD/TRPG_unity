// Unit.cs
using UnityEngine;
using TMPro;

public class Unit : MonoBehaviour
{
    // Objeto para almacenar los datos base
    public UnitData baseStats;

    // Referencia a la UI para mostrar HP
    public TextMeshProUGUI hpText;

    // Referencia a la UI para mostrar MP
    public TextMeshProUGUI mpText;

    // Estadísticas actuales (variables que cambian durante la batalla)
    private int currentHP;
    private int currentMP;

    // REGENERACIÓN: Cantidad de MP a regenerar cada turno
    private const int MP_REGEN_AMOUNT = 5;

    // Inicializa la unidad con sus estadísticas base
    public void Initialize()
    {
        currentHP = baseStats.maxHP;
        currentMP = baseStats.maxMP;
        UpdateHPDisplay();
        UpdateMPDisplay();
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

    // Lógica para tomar daño
    public void TakeDamage(int damageAmount)
    {
        // Fórmula de daño simple (puedes complicarla después)
        int defenseFactor = 1; // Simplificamos la defensa
        int finalDamage = Mathf.Max(0, damageAmount - baseStats.defense * defenseFactor);

        currentHP -= finalDamage;
        currentHP = Mathf.Max(0, currentHP);

        // **TODO: Actualizar la interfaz de usuario (UI) aquí**
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