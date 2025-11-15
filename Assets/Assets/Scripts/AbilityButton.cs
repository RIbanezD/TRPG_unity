// AbilityButton.cs
using UnityEngine;
using UnityEngine.UI; // Necesario para la clase Button y para deshabilitar si es necesario
using TMPro; // Necesario para actualizar el texto del botón

public class AbilityButton : MonoBehaviour
{
    // Datos de la habilidad que este botón representa (Configurado en el Inspector)
    public Ability abilityData; 

    // Referencia opcional al texto del botón para mostrar el nombre
    public TextMeshProUGUI buttonText; 

    void Start()
    {
        // Actualiza el texto del botón con el nombre de la habilidad
        if (buttonText != null && abilityData != null)
        {
            buttonText.text = abilityData.abilityName;
        }
        
        // ¡Importante! Asegúrate de que el botón sepa qué hacer al hacer clic
        GetComponent<Button>().onClick.AddListener(OnClickAction);
    }
    
    // Función que se ejecuta cuando se hace clic en el botón
    public void OnClickAction()
    {
        // Verifica que los datos de la habilidad y el BattleManager existan
        if (abilityData == null || BattleManager.Instance == null) return;

        // Llamar a la función ExecuteAbility con los datos
        BattleManager.Instance.ExecuteAbility(abilityData);
    }

    // Puedes añadir funciones para actualizar el botón (ej: deshabilitar si no hay MP)
    // public void UpdateButtonState() {} 
}