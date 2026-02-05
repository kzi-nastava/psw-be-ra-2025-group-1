namespace Explorer.Payments.Core.Domain.Cupons;

public class Coupon
{
    public long Id { get; private set; }

    public long AuthorId { get; private set; }      // vlasnik kupona (autor)
    public long? TourId { get; private set; }       // null => važi za sve ture tog autora

    public string Code { get; private set; } = string.Empty; // UNIQUE
    public decimal Percentage { get; private set; }          // (0-100]

    public DateTime ValidFromUtc { get; private set; }
    public DateTime? ValidToUtc { get; private set; }        // opciono
    public bool IsActive { get; private set; }

    public int? MaxTotalUses { get; private set; }           // null = neograničeno
    public int? MaxUsesPerUser { get; private set; }         // null = neograničeno

    private Coupon() { } // EF

    public Coupon(
        long authorId,
        long? tourId,
        string code,
        decimal percentage,
        DateTime validFromUtc,
        DateTime? validToUtc,
        bool isActive = true,
        int? maxTotalUses = null,
        int? maxUsesPerUser = null)
    {
        AuthorId = authorId;
        TourId = tourId;

        Code = NormalizeCode(code);
        Percentage = percentage;

        ValidFromUtc = validFromUtc;
        ValidToUtc = validToUtc;
        IsActive = isActive;

        MaxTotalUses = maxTotalUses;
        MaxUsesPerUser = maxUsesPerUser;

        ValidateInvariants();
    }

    public void Deactivate() => IsActive = false;

    public bool IsValidAt(DateTime nowUtc)
        => IsActive
           && nowUtc >= ValidFromUtc
           && (ValidToUtc is null || nowUtc <= ValidToUtc.Value);

    public bool AppliesTo(long authorId, long tourId)
        => AuthorId == authorId && (TourId is null || TourId.Value == tourId);

    public decimal CalculateDiscount(decimal orderTotal)
    {
        if (orderTotal <= 0) return 0m;
        return Math.Min(orderTotal, orderTotal * (Percentage / 100m));
    }

    private void ValidateInvariants()
    {
        if (AuthorId <= 0) throw new ArgumentException("AuthorId is required.");
        if (TourId is not null && TourId <= 0) throw new ArgumentException("TourId must be > 0 when set.");

        if (string.IsNullOrWhiteSpace(Code)) throw new ArgumentException("Coupon code is required.");

        if (ValidToUtc is not null && ValidToUtc.Value < ValidFromUtc)
            throw new ArgumentException("ValidTo must be >= ValidFrom.");

        if (Percentage <= 0) throw new ArgumentException("Percentage must be > 0.");
        if (Percentage > 100m) throw new ArgumentException("Percentage must be <= 100.");

        if (MaxTotalUses is not null && MaxTotalUses <= 0)
            throw new ArgumentException("MaxTotalUses must be > 0 when set.");
        if (MaxUsesPerUser is not null && MaxUsesPerUser <= 0)
            throw new ArgumentException("MaxUsesPerUser must be > 0 when set.");
    }

    public static string NormalizeCode(string code)
        => (code ?? string.Empty).Trim().ToUpperInvariant();
}
