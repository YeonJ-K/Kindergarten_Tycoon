using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FindCardPlay_Card : MonoBehaviour, IPointerClickHandler
{
    private bool flipped = false;
    [SerializeField] private Image cardImage;
    private string cardName;
    Action<FindCardPlay_Card> onCardClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        onCardClick?.Invoke(this);
    }

    public void Init(Action<FindCardPlay_Card> onCardClicked)
    {
        onCardClick = onCardClicked;
    }

    public void SetCardImg(Sprite sprite)
    {
        cardName = sprite.name;
        cardImage.sprite = sprite;
    }

    public string GetCardName() => cardName;

    public void Flip()
    {
        flipped = !flipped;
        transform.DORotate(new(0, flipped ? 0f : 180f, 0), 0.25f);
    }
}
