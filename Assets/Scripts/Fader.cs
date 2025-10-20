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
    void Start()
    {
        if (fadeOutOnStart)
        {
            Fade(false);
        }
    }

    public void Fade(bool fadeIn)
    {
        float targetValue = fadeIn ? 1f : 0f;

        if (image.type == Image.Type.Filled)
        {
            image.DOFillAmount(targetValue, animationTime).OnComplete(EndFadeEvent.Invoke).SetDelay(2);
            return;
        }
        image.DOFade(targetValue, animationTime).OnComplete(EndFadeEvent.Invoke);
    }

}
