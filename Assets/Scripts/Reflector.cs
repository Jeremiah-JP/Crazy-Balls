using UnityEngine;

public class Reflector : MonoBehaviour, IUpgradeable
{
    public bool rotatable = true;
    public float snapDegrees = 15f;
    public float boostMultiplier = 1f;
    public SpriteRenderer body;
    public GameObject highlight;
    public GameObject selectionRing;
    public Color normalColor = new Color(0.81f, 0.84f, 0.87f);
    public Color boostedColor = new Color(0.95f, 0.6f, 0.29f);

    void Awake()
    {
        Snap();
        SetHighlight(false);
        SetSelected(false);
        if (body) body.color = normalColor;
    }

    public void RotateStep(int direction)
    {
        if (!rotatable) return;
        transform.Rotate(0f, 0f, snapDegrees * direction);
        Snap();
        Physics2D.SyncTransforms();
    }

    void Snap()
    {
        Vector3 e = transform.eulerAngles;
        e.z = Mathf.Round(e.z / snapDegrees) * snapDegrees;
        transform.eulerAngles = e;
    }

    public float Angle => Mathf.Round(transform.eulerAngles.z);

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (boostMultiplier <= 1f) return;
        if (collision.rigidbody && collision.rigidbody.TryGetComponent(out Ball ball)) ball.Boost(boostMultiplier);
    }

    public bool CanApply(CardData card)
    {
        return card.effect == CardEffect.ReflectorBooster && card.value > boostMultiplier;
    }

    public void Apply(CardData card)
    {
        boostMultiplier = card.value;
        if (body) body.color = boostedColor;
    }

    public void SetHighlight(bool on)
    {
        if (highlight) highlight.SetActive(on);
    }

    public void SetSelected(bool on)
    {
        if (selectionRing) selectionRing.SetActive(on);
    }
}
