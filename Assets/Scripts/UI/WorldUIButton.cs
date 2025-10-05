using System;
using UnityEngine;
using UnityEngine.UI;

public class WorldUIButton : MonoBehaviour, IWorldUISelectable
{
    protected Image image;
    [SerializeField] protected Sprite hoverSprite;
    [SerializeField] bool playAudio = true;
    protected Sprite idleSprite;

    public event Action onClick;

    protected void Awake()
    {
        image = GetComponent<Image>();
        idleSprite = image.sprite;
    }

    public virtual void OnClick()
    {
        if (playAudio)
            SFXManager.PlaySound(GlobalSFX.ButtonClick);
        onClick?.Invoke();
    }

    public void OnHoverEnter()
    {
        if (playAudio)
            SFXManager.PlaySound(GlobalSFX.ButtonHover);
        image.sprite = hoverSprite;
    }

    public void OnHoverExit()
    {
        image.sprite = idleSprite;
    }

}
