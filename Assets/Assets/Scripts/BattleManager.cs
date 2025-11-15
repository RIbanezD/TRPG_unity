// BattleManager.cs
using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    // Patrón Singleton: Acceso fácil desde cualquier script (BattleManager.Instance)
    public static BattleManager Instance;

    // Referencia al controlador de UI para generar botones dinámicamente
    public AbilityMenuController abilityMenuController;

    // Rastrea la unidad cuyo turno es actual
    public Unit currentActiveUnit;

    // Cantidad de MP a regenerar cada turno
    private const int MP_REGEN_AMOUNT = 5;

    // NUEVAS LISTAS para el control de la batalla (permite múltiples unidades)
    public List<Unit> playerUnits = new List<Unit>();
    public List<Unit> enemyUnits = new List<Unit>();

    // Referencia al Panel de UI que contiene los botones de acción del jugador
    public GameObject playerActionPanel;
    private enum BattleState { Start, PlayerTurn, EnemyTurn, Victory, Defeat }

    private BattleState currentState;

    void Awake()
    {
        // Implementación Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Inicializar todas las unidades (asume que ya están asignadas en el Inspector)
        InitializeUnits();

        // TEMPORAL: Asignamos el primer jugador como unidad activa
        if (playerUnits.Count > 0)
        {
            currentActiveUnit = playerUnits[0];
        }
            currentState = BattleState.Start;
            StartBattle();
    }

    public void StartBattle()
    {
        Debug.Log("--- Batalla Iniciada ---");
        ChangeState(BattleState.PlayerTurn);
    }

    public void InitializeUnits()
    {
        // Inicializa todas las unidades del jugador
        foreach (Unit unit in playerUnits)
        {
            unit.Initialize();
        }

        // Inicializa todas las unidades enemigas
        foreach (Unit unit in enemyUnits)
        {
            unit.Initialize();
        }

        // Asigna el primer atacante (ej: la primera unidad de la lista)
        // [Implementación más compleja de turnos requerida aquí]
    }

    // Función central para gestionar el flujo de la batalla
    private void ChangeState(BattleState newState)
    {
        currentState = newState;
        Debug.Log("Estado Actual: " + newState.ToString());

        // Aseguramos que la interfaz del jugador se desactive por defecto
        playerActionPanel?.SetActive(false);

        switch (newState)
        {
            case BattleState.PlayerTurn:

                if (currentActiveUnit != null)
                {
                currentActiveUnit.ProcessStatusEffects(); 
                
                if (!currentActiveUnit.IsAlive())
                {
                    // Si murió por Veneno, el turno termina inmediatamente.
                    ChangeState(BattleState.EnemyTurn); 
                    return;
                }
                if (currentActiveUnit.skipTurn)
                {
                    // Si la unidad fue aturdida, saltamos el turno del jugador y vamos al enemigo.
                    Debug.Log(currentActiveUnit.baseStats.unitName + " pierde el turno por aturdimiento.");
                    ChangeState(BattleState.EnemyTurn);
                    return;
                }
                }

                // **TODO: Mostrar el menú de acción del jugador (UI)**
                playerActionPanel?.SetActive(true);

                abilityMenuController?.DisplayAbilities(currentActiveUnit);

                currentActiveUnit.RegenerateMP(MP_REGEN_AMOUNT);
                break;

            case BattleState.EnemyTurn:
                // TEMPORAL: Asumimos que el primer enemigo es el activo
                if (enemyUnits.Count > 0)
                {
                enemyUnits[0].ProcessStatusEffects(); 
                }
                // El enemigo ataca automáticamente después de un pequeño retraso
                Invoke("ExecuteEnemyTurn", 1f);
                break;
            case BattleState.Victory:
                Debug.Log("¡Victoria!");
                PauseSimulation();
                // **TODO: Lógica de recompensa y transición**
                break;
            case BattleState.Defeat:
                Debug.Log("Derrota...");
                PauseSimulation();
                // **TODO: Lógica de Game Over**
                break;
        }
    }

    // --- ACCIONES DEL JUGADOR (Llamadas desde los botones de la UI) ---

    private void PauseSimulation()
    {
        // Pausa la simulación (opcional)
        Time.timeScale = 0f;

        // (Solo para Pruebas en el Editor): Detener la simulación de Unity completamente.
        // #if UNITY_EDITOR 
        // UnityEditor.EditorApplication.isPlaying = false;
        // #endif
    }

    public void OnPlayerAttack(Ability attackAbility) // Ahora requiere un argumento
    {
        ExecuteAbility(attackAbility);
    }

    public void OnPlayerHeal(Ability healAbility) // Ahora requiere un argumento
    {
        ExecuteAbility(healAbility);
    }

    public void ExecuteAbility(Ability selectedAbility)
    {
        if (currentState != BattleState.PlayerTurn) return;

        // Verificar si tiene suficiente Maná
        if (!currentActiveUnit.TryConsumeMP(selectedAbility.mpCost))
        {
            return;
        }
        

        // Determinar el objetivo y ejecutar la lógica
        Unit targetUnit = null;
        Unit casterUnit = currentActiveUnit;

        // **NOTA:** La lógica de objetivo debe ser expandida para múltiples unidades en el futuro.
        if (selectedAbility.target == TargetType.SingleEnemy && enemyUnits.Count > 0)
        {
            targetUnit = enemyUnits[0]; // TEMPORAL, siempre el primer enemigo
        }
        else if (selectedAbility.target == TargetType.Self || selectedAbility.target == TargetType.SingleAlly)
        {
            targetUnit = casterUnit; // TEMPORAL, siempre uno mismo
        }

        // Aplicar el efecto de la habilidad
        switch (selectedAbility.type)
        {
            case AbilityType.Damage:
                int damage = casterUnit.CurrentAttack + selectedAbility.power;
                targetUnit.TakeDamage(damage);
                break;
            case AbilityType.Heal:
                if (targetUnit.IsAlive())
                {
                 // La curación es directa: no aplica defensa/ataque
                 targetUnit.Heal(selectedAbility.power); 
                }
                break;
            case AbilityType.Buff:
                if (selectedAbility.statusEffectData != null)
                {
                targetUnit.ApplyStatusEffect(selectedAbility.statusEffectData);
                }
                break;
        }
        
        if (currentState == BattleState.Victory || currentState == BattleState.Defeat) return;
        ChangeState(BattleState.EnemyTurn);
    }

    // --- LÓGICA DEL ENEMIGO ---
    
    private void ExecuteEnemyTurn()
    {

        if (currentState != BattleState.EnemyTurn) return; // Si ya ganamos/perdimos, salir.

        Unit enemyUnit = enemyUnits[0];
        Unit playerUnit = playerUnits[0];

        if (!enemyUnit.IsAlive())
        {
            ChangeState(BattleState.Victory);
            return;
        }

        // Verificar Salto de Turno del Enemigo**
        if (enemyUnit.skipTurn)
        {
            Debug.Log(enemyUnit.baseStats.unitName + " perdió su turno por aturdimiento.");
            ChangeState(BattleState.PlayerTurn);
            return;
        }

        // IA simple: El enemigo siempre ataca
        Ability enemyAttackAbility = enemyUnit.baseStats.abilities[0];
        int damageAmount = enemyUnit.baseStats.attack + enemyAttackAbility.power;
        Debug.Log(enemyUnit.baseStats.unitName + " ataca con " + enemyAttackAbility.abilityName + ".");
        playerUnit.TakeDamage(damageAmount);

        if (currentState == BattleState.Victory || currentState == BattleState.Defeat)
        {
            return;
        }

        // Pasa el turno al jugador
        ChangeState(BattleState.PlayerTurn);
    }

    // --- LÓGICA DE FIN DE BATALLA ---
    
    public void CheckBattleEnd()
    {
        if (!playerUnits[0].IsAlive())
        {
            ChangeState(BattleState.Defeat);
        }
        else if (!enemyUnits[0].IsAlive())
        {
            ChangeState(BattleState.Victory);
        }
    }
}