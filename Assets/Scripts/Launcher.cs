using UnityEngine;

public class Launcher : MonoBehaviour, IUpgradeable
{
    public int ballCount = 20;
    public float launchSpeed = 12f;
    public float interval = 0.08f;
    public Transform muzzle;
    public GameObject highlight;

    public bool Finished { get; private set; }

    bool firing;
    int fired;
    float timer;

    void Awake()
    {
        SetHighlight(false);
    }

    public void Begin()
    {
        firing = true;
        fired = 0;
        timer = 0f;
        Finished = ballCount <= 0;
    }

    public void Tick(float dt)
    {
        if (!firing) return;
        timer -= dt;
        Vector3 origin = muzzle ? muzzle.position : transform.position;
        while (timer <= 0f && fired < ballCount)
        {
            GameManager.Instance.SpawnBall(origin, (Vector2)transform.up * launchSpeed, null);
            fired++;
            timer += interval;
        }
        if (fired >= ballCount)
        {
            firing = false;
            Finished = true;
        }
    }

    public int RemainingToFire => ballCount - fired;

    public bool CanApply(CardData card)
    {
        return card.effect == CardEffect.LauncherExtraBalls;
    }

    public void Apply(CardData card)
    {
        ballCount += Mathf.RoundToInt(card.value);
    }

    public void SetHighlight(bool on)
    {
        if (highlight) highlight.SetActive(on);
    }
}