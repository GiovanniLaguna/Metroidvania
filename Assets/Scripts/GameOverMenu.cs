using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [Header("Escenas a cargar")]
    [SerializeField] private string levelToReload = "GameScene";   
    [SerializeField] private string mainMenuScene = "MenuScene";   

    
    /// Llamado por el botón "Reintentar"
   
    public void OnRetryButton()
    {
        Time.timeScale = 1f; // Asegurarse de que el tiempo vuelva a la normalidad
        SceneManager.LoadScene(levelToReload);
    }

 
    /// Llamado por el botón "Menú principal"
   
    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    /// Atajo rápido con 
    
    private void Update()
    {
        // Enter para reintentar
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnRetryButton();
        }

        // Escape para ir al menú principal
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnMainMenuButton();
        }
    }
}
