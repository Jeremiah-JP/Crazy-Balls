using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class Ball : MonoBehaviour
{
    public float minSpeed = 1.5f;
    public float maxSpeed = 20f;
    public float maxLifetime = 20f;

    Rigidbody2D rb;
    float life;
    bool broken;
    readonly HashSet<Gate> passedGates = new HashSet<Gate>();

    public Rigidbody2D Body => rb;
    public IEnumerable<Gate> PassedGates => passedGates;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    public void Init(Vector2 velocity, IEnumerable<Gate> inheritedGates)
    {
        broken = false;
        life = 0f;
        passedGates.Clear();
        if (inheritedGates != null)
        {
            foreach (var g in inheritedGates) passedGates.Add(g);
        }
        rb.linearVelocity = velocity;
    }

    public bool TryPass(Gate gate)
    {
        return passedGates.Add(gate);
    }

    public void Boost(float multiplier)
    {
        rb.linearVelocity *= multiplier;
        ClampSpeed();
    }

    public void Step(float dt)
    {
        if (broken) return;
        life += dt;
        if (life >= maxLifetime)
        {
            Break();
            return;
        }
        ClampSpeed();
    }

    void ClampSpeed()
    {
        Vector2 v = rb.linearVelocity;
        float s = v.magnitude;
        if (s > maxSpeed) rb.linearVelocity = v / s * maxSpeed;
        else if (s > 0.0001f && s < minSpeed) rb.linearVelocity = v / s * minSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (broken) return;
        if (collision.collider.TryGetComponent(out Target target))
        {
            target.Hit();
            Break();
        }
    }

    public void Break()
    {
        if (broken) return;
        broken = true;
        GameManager.Instance.UnregisterBall(this);
        Destroy(gameObject);
    }
}