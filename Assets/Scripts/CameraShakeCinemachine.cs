using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShakeCinemachine : MonoBehaviour
{
    public static CameraShakeCinemachine Instance;

    [Header("Refs")]
    [SerializeField] private CinemachineCamera vcam;   // <- NUEVO en 3.1
    [SerializeField] private Transform followTarget;   // tu Player transform

    [Header("Defaults")]
    [SerializeField] private float defaultIntensity = 2f;
    [SerializeField] private float defaultDuration = 0.2f;

    private CinemachineBasicMultiChannelPerlin noise;

    private void Awake()
    {
        Instance = this;

        if (vcam == null)
            vcam = FindObjectOfType<CinemachineCamera>();

        if (vcam == null)
        {
            Debug.LogError("No hay CinemachineCamera en la escena.");
            enabled = false;
            return;
        }

        // Asegurar Follow (opcional si ya lo pusiste en el inspector)
        if (followTarget != null)
            vcam.Follow = followTarget;

        // En 3.1 los componentes Cinemachine se agregan a la propia CinemachineCamera:
        noise = vcam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        if (noise == null)
            noise = vcam.gameObject.AddComponent<CinemachineBasicMultiChannelPerlin>(); // 3.1

        // Dejar en reposo
        noise.AmplitudeGain = 0f;
        noise.FrequencyGain = 0f;
    }

    public void Shake(float intensity, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(DoShake(
            intensity <= 0 ? defaultIntensity : intensity,
            duration <= 0 ? defaultDuration : duration
        ));
    }

    private IEnumerator DoShake(float intensity, float duration)
    {
        if (noise == null) yield break;

        noise.AmplitudeGain = intensity;
        noise.FrequencyGain = 1.5f;   // ajusta al gusto

        yield return new WaitForSeconds(duration);

        noise.AmplitudeGain = 0f;
        noise.FrequencyGain = 0f;
    }
}
