using System;
using UnityEngine;
using UnityEngine.UI;

public class WorldUIButton : MonoBehaviour, IWorldUISelectable
{
    protected Image image;
    [SerializeField] protected Sprite hoverSprite;
    protected Sprite idleSprite;

    public event Action onClick;

    protected void Awake()
    {
        image = GetComponent<Image>();
        idleSprite = image.sprite;
    }

    public virtual void OnClick()
    {
        onClick?.Invoke();
    }

    public void OnHoverEnter()
    {
        image.sprite = hoverSprite;
    }

    public void OnHoverExit()
    {
        image.sprite = idleSprite;
    }

}
