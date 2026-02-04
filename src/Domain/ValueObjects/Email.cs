using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public sealed class Email : IEquatable<Email>
{
    public string Value { get; }
    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Email inválido");
        if (!Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) throw new ArgumentException("Email inválido");
        Value = value;
    }
    public bool Equals(Email? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => obj is Email e && Equals(e);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;
}
