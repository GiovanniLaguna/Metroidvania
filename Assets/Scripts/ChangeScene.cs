using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{
    private int gameScene = 1;

    public void PlayPlatformer()
    { 
     SceneManager.LoadScene(gameScene);
    }
}
