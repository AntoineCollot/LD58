using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MusicManager : MonoBehaviour
{
    AudioSource source;
    bool isMuted;

    [SerializeField] TextMeshProUGUI text;

    public static MusicManager Instance;

    InputMap inputMap;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        source = GetComponent<AudioSource>();

        UpdateText();

        inputMap = new InputMap();
        inputMap.Enable();
        inputMap.Main.MuteMusic.performed += MuteMusic_performed;
    }

    private void MuteMusic_performed(InputAction.CallbackContext obj)
    {
        ToggleMute();
    }

    private void OnDestroy()
    {
        if (inputMap != null)
            inputMap.Main.MuteMusic.performed -= MuteMusic_performed;
        inputMap.Dispose();
    }

    private void Update()
    {
    }

    public void Mute(bool value)
    {
        isMuted = value;
        source.mute = isMuted;

        UpdateText();
    }

    public void ToggleMute()
    {
        Mute(!isMuted);
    }

    private void UpdateText()
    {
        if (isMuted)
            text.text = "Music Muted [M]";
        else
            text.text = "Mute Music [M]";
    }
}
