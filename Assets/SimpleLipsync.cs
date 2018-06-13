using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLipsync : MonoBehaviour
{

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject jaw;
    private float currentValue { get { return jawEA.x; } set { jawEA.x = value; jaw.transform.localEulerAngles = jawEA; } }

    [SerializeField] private float maxValue;
    [SerializeField] private float minValue;
    [SerializeField] private float interval = 0.1f;

    private Coroutine lipsync;
    private Vector3 defaultJawEA;
    public Vector3 jawEA;

    private void OnEnable()
    {
        jawEA = defaultJawEA = jaw.transform.localEulerAngles;
        //jawEA.y = jawEA.z = 0;

        lipsync = StartCoroutine(LipSync());
    }

    IEnumerator LipSync()
    {
        while (true)
        {
            float[] samples = new float[64];
            audioSource.GetSpectrumData(samples, 0, FFTWindow.Rectangular);
            float value = 0;
            for (int i = 0; i < 64; i++)
            {
                value += samples[i];
            }
            currentValue = Mathf.Lerp(minValue, maxValue, value);
            yield return new WaitForSeconds(interval);
        }
    }

    private void OnDisable()
    {
        StopCoroutine(lipsync);
        jaw.transform.localEulerAngles = defaultJawEA;
    }


}
