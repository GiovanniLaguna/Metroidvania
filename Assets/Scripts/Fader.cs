using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;
public class Fader : MonoBehaviour
{
    public Image image;
    public float animationTime;
    public UnityEvent EndFadeEvent;
    public bool fadeOutOnStart;
    private Tween Tween;
    void Start()
    {
        if (fadeOutOnStart)
        {
            Fade(false);
        }
    }

    public void Fade(bool fadeIn)
    {
        Tween?.Kill();
        float targetValue = fadeIn ? 1f : 0f;

        if (image.type == Image.Type.Filled)
        {

Tween = image.DOFillAmount(targetValue, animationTime).OnComplete(EndFadeEvent.Invoke).SetDelay(2);
            return;
        }
Tween = image.DOFade(targetValue, animationTime).OnComplete(EndFadeEvent.Invoke);
    }

}
