using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Monedas")]
    public int coinsCounter = 0;

    [Range(0, 10)]
    public int coinsToCollect = 1;
    public TextMeshProUGUI coinText;
    public UnityEvent onGameWinEvents;

    [Header("Player")]
    public GameObject playerGameObject;
    private PlayerController player;

    [Header("Transiciones")]
    [SerializeField] private Fader fader;

    public static GameManager instance;

    // ========= ESTADO PERSISTENTE DEL PLAYER =========
    // (Otros scripts ya usan esto, así que lo dejamos)
    public static bool hasArmor;
    public static bool hasKnifePowerup;
    public static bool hasCrossbowPowerup;
    public static bool armorInitialized = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (playerGameObject != null)
            player = playerGameObject.GetComponent<PlayerController>();

        coinsCounter = 0;
        if (coinText != null)
            coinText.text = coinsCounter.ToString();
    }

    // -----------------------------
    //        MONEDAS / WIN
    // -----------------------------
    public void AddCoins(int amount)
    {
        coinsCounter += amount;

        if (coinText != null)
            coinText.text = coinsCounter.ToString();

        if (coinsCounter >= coinsToCollect)
        {
            onGameWinEvents?.Invoke();
            GameWon();
        }
    }

    // -----------------------------
    //        GAME OVER
    // -----------------------------
    public void OnPlayerDeath()
    {
        // Sin sistema de vidas: muerte -> GameOver directo
        GameOver();
    }

    public void GameOver()
    {
        LoadSceneWithFadeOrDirect("GameOverScene");
    }

    public void GameWon()
    {
        LoadSceneWithFadeOrDirect("VictoryScene");
    }

    public void MainMenu()
    {
        LoadSceneWithFadeOrDirect("MenuScene");
    }

    public void BossFight()
    {
        LoadSceneWithFadeOrDirect("BossFight");
    }

    // -----------------------------
    //        HELPERS
    // -----------------------------
    private void LoadSceneWithFadeOrDirect(string sceneName)
    {
        // Si no hay Fader, carga directo (evitamos freeze)
        if (fader == null)
        {
            SceneManager.LoadScene(sceneName);
            return;
        }

        // Limpiar listeners previos para no acumular
        fader.EndFadeEvent.RemoveAllListeners();
        fader.EndFadeEvent.AddListener(() => SceneManager.LoadScene(sceneName));
        fader.Fade(true);
    }
}
