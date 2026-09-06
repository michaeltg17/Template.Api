namespace Core.Extensions
{
    public static class StringExtensions
    {
        public static string JoinNonEmpty(params string?[] values) =>
            string.Join(" ", values.Where(v => !string.IsNullOrEmpty(v)));
    }
}
