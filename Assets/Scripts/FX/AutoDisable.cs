using UnityEngine;

public class AutoDisable : MonoBehaviour
{
    float duration;

    private void Awake()
    {
        duration = GetComponent<ParticleSystem>().main.duration;
    }

    private void OnEnable()
    {
        Invoke("DisableDelayed", duration);
    }

    void DisableDelayed()
    {
        gameObject.SetActive(false);
    }
}
