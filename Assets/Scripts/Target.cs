using TMPro;
using UnityEngine;

public class Target : MonoBehaviour
{
    public int health = 30;
    public TMP_Text label;

    public bool Destroyed => health <= 0;

    void Awake()
    {
        Refresh();
    }

    public void Hit()
    {
        if (Destroyed) return;
        health--;
        Refresh();
        if (Destroyed)
        {
            gameObject.SetActive(false);
        }
        GameManager.Instance.OnTargetChanged();
    }

    void Refresh()
    {
        if (label) label.text = health.ToString();
    }
}
