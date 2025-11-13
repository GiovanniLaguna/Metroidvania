using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;


public class TimeManager : MonoBehaviour
{
    public KeyCode pauseKey;
    private bool gamePaused;
    public CanvasGroup pauseScreen;
    private Tween pauseTween;
    private float pauseTweenTime = 0.5f;
    public static TimeManager instance;
    private Coroutine freezeFrameRotine;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        pauseScreen.alpha = 0;
    }

    public void SetTimescale(float value)
    {
        Time.timeScale = value;
    }

    public void TogglePause(bool paused)
    {
        if (freezeFrameRotine != null)
        {
            StopCoroutine(freezeFrameRotine);
        }
        gamePaused = paused;
        pauseTween?.Kill();
        if (paused)
        {
            SetTimescale(0);
            pauseTween = pauseScreen.DOFade(1, pauseTweenTime).SetUpdate(true);
        }
        else
        {
            pauseTween = pauseScreen.DOFade(0, pauseTweenTime).OnComplete(() => { SetTimescale(1); Debug.Log("WAZA"); }).SetUpdate(true);
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            TogglePause(!gamePaused);
        }
    }

    public void FreezeFrame(float timeScale, float duration)
    {
        if (freezeFrameRotine != null)
        {
            StopCoroutine(freezeFrameRotine);
        }
        freezeFrameRotine = StartCoroutine(FreezeFrameRoutine(timeScale, duration));
    }

    IEnumerator FreezeFrameRoutine(float timeScale, float duration)
    {
        SetTimescale(timeScale);
        yield return new WaitForSecondsRealtime(duration);
        SetTimescale(1);
    }
}
