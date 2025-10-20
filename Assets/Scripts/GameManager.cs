using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public int playerLifes = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    public void LoadLevel (int value)
    {
        SceneManager.LoadScene(value);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerLifes <= 0)
        {
            LoadLevel(0);
            playerLifes = 3;
        }
    }
}
