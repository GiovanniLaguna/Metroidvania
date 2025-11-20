using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryMenu : MonoBehaviour
{
    [Header("Escenas a cargar")]
    [SerializeField] private string currentLevelScene = "Level_01";     // Nivel actual (por si quieres repetir)
    [SerializeField] private string mainMenuScene = "MainMenu";         // Menú principal


    /// Llamado por el botón "Repetir nivel"
  
    public void OnRetryLevelButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(currentLevelScene);
    }

  
    /// Llamado por el botón "Menú principal"
 
    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    private void Update()
    {
        

        // R → repetir nivel
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnRetryLevelButton();
        }

        // Escape → menú principal
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnMainMenuButton();
        }
    }
}
