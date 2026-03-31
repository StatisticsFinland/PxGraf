using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Major Code Smell", "S3267:Loops should be simplified with \"LINQ\" expressions", Justification = "Explicit loops are preferred for clarity.")]
[assembly: SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging", Justification = "Performance impact in our use cases is negligible.")]
