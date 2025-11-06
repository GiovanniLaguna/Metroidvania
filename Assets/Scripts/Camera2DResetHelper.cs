
using Unity.Cinemachine;
using UnityEngine;

public class Camera2DResetHelper : MonoBehaviour
{
    public Transform player;

    [ContextMenu("ResetNow")]
    public void ResetNow()
    {
        // Main Camera
        Camera cam = Camera.main;
        if (cam == null)
        {
            var go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            cam = go.AddComponent<Camera>();
        }
        cam.transform.position = new Vector3(0, 0, -10);
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        cam.cullingMask = ~0;
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 1000f;

        var brain = cam.GetComponent<CinemachineBrain>();
        if (brain == null) brain = cam.gameObject.AddComponent<CinemachineBrain>();

        // CinemachineCamera
        var vcam = FindObjectOfType<CinemachineCamera>();
        if (vcam == null)
        {
            var goV = new GameObject("CinemachineCamera");
            vcam = goV.AddComponent<CinemachineCamera>();
        }
        vcam.Priority = 100;
        if (player != null) vcam.Follow = player;

        // Noise en reposo
        var noise = vcam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        if (noise == null) noise = vcam.gameObject.AddComponent<CinemachineBasicMultiChannelPerlin>();
        noise.AmplitudeGain = 0f;
        noise.FrequencyGain = 0f;

        Debug.Log("Cámara 2D y Cinemachine 3.1 reseteadas.");
    }
}
