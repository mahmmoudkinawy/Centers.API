namespace Centers.API.Helpers;
public static class UtcDateAnnotation
{
    private const string _isUtcAnnotation = "IsUtc";
    private static readonly ValueConverter<DateTime, DateTime> _utcConverter =
      new(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

    private static readonly ValueConverter<DateTime?, DateTime?> _utcNullableConverter =
      new(v => v, v => v == null ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));

    public static PropertyBuilder<TProperty> IsUtc<TProperty>(this PropertyBuilder<TProperty> builder, bool isUtc = true) =>
      builder.HasAnnotation(_isUtcAnnotation, isUtc);

    public static bool IsUtc(this IMutableProperty property) =>
      ((bool?)property.FindAnnotation(_isUtcAnnotation)?.Value) ?? true;

    /// <summary>
    /// Make sure this is called after configuring all your entities.
    /// </summary>
    public static void ApplyUtcDateTimeConverter(this ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (!property.IsUtc())
                {
                    continue;
                }

                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(_utcConverter);
                }

                if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(_utcNullableConverter);
                }
            }
        }
    }
}
