using System.Collections.Generic;
using UnityEngine;

public class HandUI : MonoBehaviour
{
    public List<CardData> startingHand = new List<CardData>();
    public CardView cardViewPrefab;
    public Transform container;

    readonly List<CardView> views = new List<CardView>();

    void Start()
    {
        foreach (var card in startingHand) AddCard(card);
        GameManager.Instance.RefreshUI();
    }

    public void AddCard(CardData card)
    {
        CardView view = Instantiate(cardViewPrefab, container);
        view.Bind(card, this);
        views.Add(view);
    }

    public void RemoveCard(CardData card)
    {
        CardView view = views.Find(v => v.Card == card);
        if (!view) return;
        views.Remove(view);
        Destroy(view.gameObject);
    }

    public void OnCardClicked(CardView view)
    {
        if (GameManager.Instance.State != GameState.Setup) return;
        if (view.Card.cost > GameManager.Instance.Money) return;
        SetupInput.Instance.SelectCard(view.Card);
    }

    public void SetSelected(CardData card)
    {
        foreach (var v in views) v.SetSelected(v.Card == card);
    }

    public void RefreshAffordability(int money, bool canPlay)
    {
        foreach (var v in views) v.SetAffordable(canPlay && v.Card.cost <= money);
    }
}
