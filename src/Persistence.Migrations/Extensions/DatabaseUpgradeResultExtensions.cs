using DbUp.Engine;

namespace Persistence.Migrations.Extensions;

internal static class DatabaseUpgradeResultExtensions
{
    public static void ThrowOnError(this DatabaseUpgradeResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.Successful) return;
        throw new InvalidOperationException(result.Error.Message);
    }
}