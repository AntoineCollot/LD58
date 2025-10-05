using TMPro;
using UnityEngine;

public class FadeInOut : MonoBehaviour
{
    TextMeshProUGUI text;
    Color col;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        col = text.color;
    }

    void Update()
    {
        col.a = Mathf.Lerp(0.4f, 0.8f,Mathf.Sin(Time.time * 6) * 0.5f + 0.5f);
        text.color = col;
    }
}
