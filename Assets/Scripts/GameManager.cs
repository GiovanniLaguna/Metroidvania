using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;



public class GameManager : MonoBehaviour
{
    public int coinsCounter = 0;

    [Range(0, 10)]
    public int coinsToCollect = 1;

    public GameObject playerGameObject;
    private PlayerController player;
    public TextMeshProUGUI coinText;
    public UnityEvent onGameWinEvents;

    public static GameManager instance;

    // ------------- VIDAS -------------
    [Header("Vidas")]
    [SerializeField] private int startingLives = 3;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private float respawnDelay = 1f;
    [SerializeField] private Transform respawnPoint; // coloca aquí un Empty en la escena

    private int currentLives;
    public UnityEvent<int> onLivesChanged;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        player = playerGameObject.GetComponent<PlayerController>();

        // Monedas
        coinsCounter = 0;
        if (coinText != null)
            coinText.text = coinsCounter.ToString();

        // Vidas
        currentLives = Mathf.Max(1, startingLives);
        UpdateLivesUI();
    }


    public void AddCoins(int amount)
        {
            coinsCounter += amount;
            coinText.text = coinsCounter.ToString();
            if (coinsCounter >= coinsToCollect)
            {
                GameWon();
            }
        }

    public void GameOver()
    {
        // Puedes poner un pequeño delay o animación antes:
        SceneManager.LoadScene("GameOverScene");
    }

    public void GameWon()
    {
        SceneManager.LoadScene("VictoryScene");
    }

    IEnumerator LoadSceneRoutine(float waitTime, int sceneToLoad)
        {
            yield return new WaitForSeconds(waitTime);
            SceneManager.LoadScene(sceneToLoad);
        }

    // -----------------------------
    //          VIDAS
    // -----------------------------
    private void UpdateLivesUI()
    {
        if (livesText != null)
            livesText.text = currentLives.ToString();

        onLivesChanged?.Invoke(currentLives);
    }

    /// <summary>
    /// Llamar esto cuando el jugador "muere" (HP llega a 0).
    /// </summary>
    public void OnPlayerDeath()
    {
        currentLives--;

        if (currentLives > 0)
        {
            UpdateLivesUI();
            StartCoroutine(RespawnRoutine());
        }
        else
        {
            GameOver();
        }
    }

    private IEnumerator RespawnRoutine()
    {
        // Espera para que se vea la anim de muerte
        yield return new WaitForSeconds(respawnDelay);

        if (playerGameObject == null)
            yield break;

        // Reactivar objeto jugador
        playerGameObject.SetActive(true);

        // Reset posición
        if (respawnPoint != null)
            playerGameObject.transform.position = respawnPoint.position;

        // Reset HP
        HpPlayer hp = playerGameObject.GetComponent<HpPlayer>();
        if (hp != null)
        {
            hp.ResetHpToFull();
        }

        // Volver a activar el control del jugador
        player?.EnableControl();
    }



}

