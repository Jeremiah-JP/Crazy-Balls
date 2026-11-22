using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CardView : MonoBehaviour
{
    public RectTransform visual;
    public TMP_Text nameText;
    public TMP_Text costText;
    public TMP_Text descriptionText;
    public Image background;
    public Image accent;
    public float selectedLift = 20f;
    public Color normalColor = new Color(0.12f, 0.15f, 0.19f);
    public Color selectedColor = new Color(0.09f, 0.17f, 0.15f);

    public CardData Card { get; private set; }

    HandUI hand;
    Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => hand.OnCardClicked(this));
    }

    public void Bind(CardData card, HandUI owner)
    {
        Card = card;
        hand = owner;
        if (nameText) nameText.text = card.cardName;
        if (costText) costText.text = "$" + card.cost;
        if (descriptionText) descriptionText.text = card.description;
        if (accent) accent.color = card.color;
        SetSelected(false);
    }

    public void SetSelected(bool on)
    {
        if (background) background.color = on ? selectedColor : normalColor;
        if (visual) visual.anchoredPosition = on ? Vector2.up * selectedLift : Vector2.zero;
    }

    public void SetAffordable(bool affordable)
    {
        if (button) button.interactable = affordable;
    }
}
