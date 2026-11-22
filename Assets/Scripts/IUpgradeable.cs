public interface IUpgradeable
{
    bool CanApply(CardData card);
    void Apply(CardData card);
    void SetHighlight(bool on);
}