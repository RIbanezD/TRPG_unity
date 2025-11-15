// AbilityMenuController.cs
using UnityEngine;
using TMPro;

public class AbilityMenuController : MonoBehaviour
{
    // El Prefab del botón que instanciarás
    public GameObject abilityButtonPrefab; 

    // El transform padre donde se crearán los botones (generalmente este GameObject)
    public Transform buttonsParent; 

    void Start()
    {
        // Asignar el padre si no está asignado en el Inspector (para mayor seguridad)
        if (buttonsParent == null)
        {
            buttonsParent = transform;
        }
    }

    // Función principal llamada por el BattleManager
    public void DisplayAbilities(Unit activeUnit)
    {
        // 1. Limpiar los botones antiguos (si los hay)
        ClearButtons();

        // 2. Obtener las habilidades del jugador        
        if (activeUnit.baseStats.abilities == null || activeUnit.baseStats.abilities.Length == 0)
        {
            Debug.LogError("La unidad del jugador no tiene habilidades asignadas en UnitData.");
            return;
        }

        // 3. Crear un botón para cada habilidad
        foreach (Ability ability in activeUnit.baseStats.abilities)
        {
            GameObject buttonObject = Instantiate(abilityButtonPrefab, buttonsParent);
            
            // 4. Asignar los datos al script del botón
            AbilityButton abilityButton = buttonObject.GetComponent<AbilityButton>();
            
            if (abilityButton != null)
            {
                abilityButton.abilityData = ability;
            }
        }
    }

    // Limpia los botones generados dinámicamente
    private void ClearButtons()
    {
        // Elimina todos los hijos del panel (todos los botones)
        foreach (Transform child in buttonsParent)
        {
            Destroy(child.gameObject);
        }
    }
}