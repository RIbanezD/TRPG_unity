// BattleManager.cs
using UnityEngine;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    // Patrón Singleton: Acceso fácil desde cualquier script (BattleManager.Instance)
    public static BattleManager Instance;

    // Referencia al controlador de UI para generar botones dinámicamente
    public AbilityMenuController abilityMenuController;

    // Cantidad de MP a regenerar cada turno
    private const int MP_REGEN_AMOUNT = 5;

    // Lista de todas las unidades en la batalla
    public List<Unit> allUnits = new List<Unit>();

    // Referencia al Panel de UI que contiene los botones de acción del jugador
    public GameObject playerActionPanel;
    private enum BattleState { Start, PlayerTurn, EnemyTurn, Victory, Defeat }
    
    // Unidades específicas para referencia fácil
    public Unit playerUnit;
    public Unit enemyUnit;

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
        playerUnit.Initialize();
        enemyUnit.Initialize();
        
        currentState = BattleState.Start;
        StartBattle();
    }

    public void StartBattle()
    {
        Debug.Log("--- Batalla Iniciada ---");
        ChangeState(BattleState.PlayerTurn);
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
                // **TODO: Mostrar el menú de acción del jugador (UI)**
                playerActionPanel?.SetActive(true);

                abilityMenuController?.DisplayAbilities(playerUnit);

                playerUnit.RegenerateMP(MP_REGEN_AMOUNT);
                break;
            case BattleState.EnemyTurn:
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

    // Deprecated - El jugador usa un Ataque Básico
    /* public void OnPlayerAttack()
    {

        if (currentState != BattleState.PlayerTurn) return;

        // Ejecuta la acción del jugador (Ataque)
        enemyUnit.TakeDamage(playerUnit.baseStats.attack);

        if (currentState == BattleState.Victory || currentState == BattleState.Defeat)
        {
            return; // Salir de la función si la batalla ya finalizó
        }

        // Pasa el turno al enemigo
        ChangeState(BattleState.EnemyTurn);
    }
 */
    /* 
    public void OnPlayerHeal()
    {
        if (currentState != BattleState.PlayerTurn) return;

        if (playerUnit.TryConsumeMP(HEAL_COST))
        {
            // Si el MP se consume con éxito:
            playerUnit.Heal(20);

            // Pasa el turno al enemigo
            ChangeState(BattleState.EnemyTurn);
        }
        else
        {
            // Si no hay suficiente MP, el turno NO pasa
            Debug.Log("¡No hay suficiente MP!");
        }

        // Llama a la función de curación en el jugador
        playerUnit.Heal(20); // Cura una cantidad fija (20 HP) por ahora

        // Pasa el turno al enemigo
        ChangeState(BattleState.EnemyTurn);
    }
    */

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
        if (!playerUnit.TryConsumeMP(selectedAbility.mpCost))
        {
            return; 
        }

        // Determinar el objetivo y ejecutar la lógica
        Unit targetUnit = null;
        if (selectedAbility.target == TargetType.SingleEnemy)
        {
            targetUnit = enemyUnit;
        }
        else if (selectedAbility.target == TargetType.Self || selectedAbility.target == TargetType.SingleAlly)
        {
            targetUnit = playerUnit;
        }

        // Aplicar el efecto de la habilidad
        switch (selectedAbility.type)
        {
            case AbilityType.Damage:
                int damage = playerUnit.baseStats.attack + selectedAbility.power;
                targetUnit.TakeDamage(damage);
                break;
            case AbilityType.Heal:
                if (targetUnit.IsAlive())
            {
                 // La curación es directa: no aplica defensa/ataque
                 targetUnit.Heal(selectedAbility.power); 
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

        if (!enemyUnit.IsAlive())
        {
            ChangeState(BattleState.Victory);
            return;
        }

        // IA simple: El enemigo siempre ataca
        Debug.Log(enemyUnit.baseStats.unitName + " ataca.");
        playerUnit.TakeDamage(enemyUnit.baseStats.attack);

        // Pasa el turno al jugador
        ChangeState(BattleState.PlayerTurn);
    }

    // --- LÓGICA DE FIN DE BATALLA ---
    
    public void CheckBattleEnd()
    {
        if (!playerUnit.IsAlive())
        {
            ChangeState(BattleState.Defeat);
        }
        else if (!enemyUnit.IsAlive())
        {
            ChangeState(BattleState.Victory);
        }
    }
}
