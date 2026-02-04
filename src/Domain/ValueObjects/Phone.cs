namespace Domain.ValueObjects;

public sealed class Phone : IEquatable<Phone>
{
    public string Value { get; }
    public Phone(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Telefone inválido");
        Value = value.Trim();
    }
    public bool Equals(Phone? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => obj is Phone p && Equals(p);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;
}
