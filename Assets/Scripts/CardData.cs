using UnityEngine;

public enum CardEffect { GateMultiplier, ReflectorBooster, LauncherExtraBalls }

[CreateAssetMenu(menuName = "Chain Shot/Card")]
public class CardData : ScriptableObject
{
    public string cardName = "New Card";
    [TextArea] public string description;
    public int cost = 2;
    public CardEffect effect;
    public float value = 2f;
    public Color color = Color.white;
}
