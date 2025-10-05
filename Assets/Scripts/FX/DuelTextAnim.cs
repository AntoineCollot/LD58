using System.Collections;
using TMPro;
using UnityEngine;

public class DuelTextAnim : MonoBehaviour
{
    Material mat;
    TextMeshProUGUI text;

    const string DILATE_PARAM = "_FaceDilate";

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        mat = text.fontMaterial;
    }

    private void OnEnable()
    {
        StartCoroutine(AnimText());
    }

    IEnumerator AnimText()
    {
        float t = 0;
        while(t<1)
        {
            t += Time.deltaTime / 0.2f;

            mat.SetFloat(DILATE_PARAM, Curves.QuadEaseOut(-1,1,Mathf.Clamp01(t)));

            yield return null;
        }

        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / 0.2f;

            mat.SetFloat(DILATE_PARAM, Curves.QuadEaseInOut(1, 0, Mathf.Clamp01(t)));

            yield return null;
        }

        float startTime = Time.time;
        while(true)
        {
            mat.SetFloat(DILATE_PARAM, (Mathf.Sin((Time.time-startTime) * 10)+1) *0.2f);

            yield return null;
        }
    }
}
