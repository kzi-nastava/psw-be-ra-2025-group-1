namespace Explorer.Payments.Core.Domain.Cupons;

public enum CouponDiscountType
{
    Percentage = 0,
    FixedAmount = 1
}

public class Coupon
{
    public long Id { get; private set; }

    public string Code { get; private set; } = string.Empty; // UNIQUE
    public CouponDiscountType DiscountType { get; private set; }
    public decimal Amount { get; private set; }              // % (0-100] ili fiksno

    public DateTime ValidFromUtc { get; private set; }
    public DateTime ValidToUtc { get; private set; }
    public bool IsActive { get; private set; }

    public int? MaxTotalUses { get; private set; }           // null = neograničeno
    public int? MaxUsesPerUser { get; private set; }         // null = neograničeno

    // EF
    private Coupon() { }

    public Coupon(
        string code,
        CouponDiscountType discountType,
        decimal amount,
        DateTime validFromUtc,
        DateTime validToUtc,
        bool isActive = true,
        int? maxTotalUses = null,
        int? maxUsesPerUser = null)
    {
        Code = NormalizeCode(code);
        DiscountType = discountType;
        Amount = amount;
        ValidFromUtc = validFromUtc;
        ValidToUtc = validToUtc;
        IsActive = isActive;
        MaxTotalUses = maxTotalUses;
        MaxUsesPerUser = maxUsesPerUser;

        ValidateInvariants();
    }

    public void Deactivate() => IsActive = false;

    public bool IsValidAt(DateTime nowUtc)
        => IsActive && nowUtc >= ValidFromUtc && nowUtc <= ValidToUtc;

    public decimal CalculateDiscount(decimal orderTotal)
    {
        if (orderTotal <= 0) return 0;

        return DiscountType switch
        {
            CouponDiscountType.Percentage => Math.Min(orderTotal, orderTotal * (Amount / 100m)),
            CouponDiscountType.FixedAmount => Math.Min(orderTotal, Amount),
            _ => 0
        };
    }

    private void ValidateInvariants()
    {
        if (string.IsNullOrWhiteSpace(Code)) throw new ArgumentException("Coupon code is required.");
        if (ValidToUtc < ValidFromUtc) throw new ArgumentException("ValidTo must be >= ValidFrom.");
        if (Amount <= 0) throw new ArgumentException("Amount must be > 0.");

        if (DiscountType == CouponDiscountType.Percentage && Amount > 100m)
            throw new ArgumentException("Percentage amount must be <= 100.");
        if (MaxTotalUses is not null && MaxTotalUses <= 0)
            throw new ArgumentException("MaxTotalUses must be > 0 when set.");
        if (MaxUsesPerUser is not null && MaxUsesPerUser <= 0)
            throw new ArgumentException("MaxUsesPerUser must be > 0 when set.");
    }

    public static string NormalizeCode(string code)
        => (code ?? string.Empty).Trim().ToUpperInvariant();
}
