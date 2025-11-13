using UnityEngine;
using Unity.Cinemachine;//importar Cinemachine
using System.Collections;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour
{
    public CinemachineCamera curentCamera;
    public List<CinemachineCamera> cameraList;
    public CinemachineBasicMultiChannelPerlin noise;
    private float defaultAmplitude = 5;
    private float defaultFrequency = 2;
    private float defaultDuration = 0.3f;
    private Coroutine shakeRoutine;

    public static CameraManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        StopShake();

        for (int i = 0; i < cameraList.Count; i++)
        {
            cameraList[i].Priority = i;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ShakeCamera();
        }
    }

    public void CameraSwitch(int priority)
    {
        int id = Mathf.Clamp(priority, 0, cameraList.Count - 1);

        for (int i = 0; i < cameraList.Count; i++)
        {
            if (cameraList[i].Priority == id)
            {
                cameraList[i].gameObject.SetActive(true);
                noise = cameraList[i].GetComponent<CinemachineBasicMultiChannelPerlin>();
            }
            else
            {
                cameraList[i].gameObject.SetActive(false);
            }
        }
    }


    public void ShakeCamera()
    {
        if (noise == null)
        {
            return;
        }

        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
        }
        shakeRoutine = StartCoroutine(ShakeRoutine());
    }

    private void StopShake()
    {
        if (noise == null)
        {
            return;
        }

        noise.AmplitudeGain = 0;
        noise.FrequencyGain = 0;
    }

    IEnumerator ShakeRoutine()
    {
        noise.AmplitudeGain = defaultAmplitude;
        noise.FrequencyGain = defaultFrequency;
        yield return new WaitForSeconds(defaultDuration);
        StopShake();
    }


}
