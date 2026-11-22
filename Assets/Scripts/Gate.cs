using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Gate: MonoBehaviour, IUpgradeable
{
    public int multiplier = 1;
    public float spreadAngle = 10f;
    public SpriteRenderer body;
    public TMP_Text label;
    public GameObject highlight;
    public Color frameColor = new Color(0.42f, 0.46f, 0.51f);
    public Color upgradedColor = new Color(0.44f, 0.83f, 0.69f);

    void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        SetHighlight(false);
        Refresh();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance.State != GameState.Running) return;
        if (multiplier <= 1) return;
        if (!other.TryGetComponent(out Ball ball)) return;
        if (!ball.TryPass(this)) return;

        Vector2 v = ball.Body.linearVelocity;
        float step = spreadAngle / (multiplier - 1);
        float start = -spreadAngle * 0.5f;

        for (int i = 0; i < multiplier; i++)
        {
            Vector2 dir = Rotate(v, start + step * i);
            if (i == 0) ball.Body.linearVelocity = dir;
            else GameManager.Instance.SpawnBall(ball.transform.position, dir, ball.PassedGates);
        }
    }

    static Vector2 Rotate(Vector2 v, float degrees)
    {
        float r = degrees * Mathf.Deg2Rad;
        float c = Mathf.Cos(r);
        float s = Mathf.Sin(r);
        return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
    }

    public bool CanApply(CardData card)
    {
        return card.effect == CardEffect.GateMultiplier && Mathf.RoundToInt(card.value) > multiplier;
    }

    public void Apply(CardData card)
    {
        multiplier = Mathf.RoundToInt(card.value);
        Refresh();
    }

    public void SetHighlight(bool on)
    {
        if (highlight) highlight.SetActive(on);
    }

    void Refresh()
    {
        if (label) label.text = "×" + multiplier;
        if (body) body.color = multiplier > 1 ? upgradedColor : frameColor;
    }
}