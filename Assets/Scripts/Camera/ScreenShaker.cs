using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class ScreenShaker : MonoBehaviour
{
    public static ScreenShaker Instance;
    CinemachineCamera vCam;
    CinemachineBasicMultiChannelPerlin machine;
    float shakeAmount;
    public float shakeTime = 0.5f;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        vCam = GetComponent<CinemachineCamera>();
        machine = vCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        shakeAmount -= Time.deltaTime / shakeTime;
        shakeAmount = Mathf.Clamp01(shakeAmount);

        machine.AmplitudeGain = Curves.QuadEaseOut(0, 1, shakeAmount);
    }

    public void SmallShake()
    {
        shakeAmount = 0.4f;
    }

    public void MediumShake()
    {
        shakeAmount = 0.7f;
    }

    public void LargeShake()
    {
        shakeAmount = 1f;
    }
}
