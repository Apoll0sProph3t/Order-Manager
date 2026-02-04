namespace Domain.ValueObjects;

public readonly struct Money : IEquatable<Money>
{
    public decimal Value { get; }
    public Money(decimal value) { Value = value; }
    public bool Equals(Money other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is Money m && Equals(m);
    public override int GetHashCode() => Value.GetHashCode();
    public static implicit operator decimal(Money m) => m.Value;
    public static Money FromDecimal(decimal value) => new Money(value);
}
