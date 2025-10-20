using UnityEngine;
using DG.Tweening; //Importar DOTween
using UnityEngine.Events;
using System; //Importar UnityEvents

public class Flicker : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Color targetColor;
    public float tweenTime;
    public int loops;
    private int playerLayer;
    public int inmuneLayer = 8;
    public KeyCode debugKey;
    private Tween flickerTween;
    public UnityEvent onTweenEvent;

    void Start()
    {
        playerLayer = spriteRenderer.gameObject.layer;
    }
    public void FlickerEffect()
    {
        /*if(flickerTween != null)
        {
            flickerTween.Kill();
        }*/
        onTweenEvent.Invoke(); //llamar UnityEvent
        flickerTween?.Kill(true); //Detener el tween antes de volver a llamarlo
        flickerTween = spriteRenderer.DOColor(targetColor, tweenTime).SetLoops(loops, LoopType.Yoyo)
        .OnStart(() => { SetInmunity(true); }) //usar expresión lambda para poder llamar funciones con parámetros dentro del tween
        .OnComplete(() => { SetInmunity(false); });
    }

    void Update()
    {
        if (Input.GetKeyDown(debugKey))
        {
            FlickerEffect();
        }
    }

    private void SetInmunity(bool inmune)
    {
        //int targetLayer = 0;
        /*if(inmune)
        {
            targetLayer = inmuneLayer;
        }
        else
        {
            targetLayer = playerLayer;
        }*/

        int targetLayer = inmune ? inmuneLayer : playerLayer;
        gameObject.layer = targetLayer;
    }
}
