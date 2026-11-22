using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SetupInput : MonoBehaviour
{
    public static SetupInput Instance { get; private set; }

    public Camera cam;
    public HandUI hand;

    CardData selectedCard;
    Reflector selectedReflector;
    readonly List<IUpgradeable> upgradeables = new List<IUpgradeable>();

    void Awake()
    {
        Instance = this;
        if (!cam) cam = Camera.main;
    }

    void Start()
    {
        foreach (var mb in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
        {
            if (mb is IUpgradeable u) upgradeables.Add(u);
        }
    }

    void Update()
    {
        var gm = GameManager.Instance;
        var mouse = Mouse.current;
        var keyboard = Keyboard.current;

        if (gm.State == GameState.Running)
        {
            gm.FastForward = keyboard.spaceKey.isPressed;
            return;
        }
        if (gm.State != GameState.Setup) return;

        if (mouse.leftButton.wasPressedThisFrame && !PointerOverUI()) HandleClick(mouse.position.ReadValue());
        if (mouse.rightButton.wasPressedThisFrame) ClearAll();

        if (selectedReflector)
        {
            int dir = 0;
            if (keyboard.qKey.wasPressedThisFrame) dir += 1;
            if (keyboard.eKey.wasPressedThisFrame) dir -= 1;
            float scroll = mouse.scroll.ReadValue().y;
            if (scroll > 0f) dir += 1;
            else if (scroll < 0f) dir -= 1;
            if (dir != 0) selectedReflector.RotateStep(dir);
        }
    }

    void HandleClick(Vector2 screenPosition)
    {
        Vector2 world = cam.ScreenToWorldPoint(screenPosition);
        Collider2D hit = Physics2D.OverlapPoint(world);

        if (selectedCard)
        {
            IUpgradeable target = hit ? hit.GetComponentInParent<IUpgradeable>() : null;
            if (target != null && target.CanApply(selectedCard) && GameManager.Instance.TrySpend(selectedCard.cost))
            {
                target.Apply(selectedCard);
                if (hand) hand.RemoveCard(selectedCard);
                SelectCard(null);
                GameManager.Instance.RefreshUI();
            }
            return;
        }

        Reflector reflector = hit ? hit.GetComponentInParent<Reflector>() : null;
        SelectReflector(reflector && reflector.rotatable ? reflector : null);
    }

    public void SelectCard(CardData card)
    {
        if (card && card == selectedCard) card = null;
        selectedCard = card;
        SelectReflector(null);
        foreach (var u in upgradeables) u.SetHighlight(card && u.CanApply(card));
        if (hand) hand.SetSelected(card);
    }

    void SelectReflector(Reflector reflector)
    {
        if (selectedReflector) selectedReflector.SetSelected(false);
        selectedReflector = reflector;
        if (selectedReflector) selectedReflector.SetSelected(true);
    }

    public void ClearAll()
    {
        SelectCard(null);
        SelectReflector(null);
    }

    bool PointerOverUI()
    {
        return EventSystem.current && EventSystem.current.IsPointerOverGameObject();
    }
}