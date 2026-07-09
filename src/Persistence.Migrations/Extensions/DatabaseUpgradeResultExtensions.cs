using DbUp;
using DbUp.Engine;

namespace Persistence.Migrations.Extensions;

public static class DatabaseUpgradeResultExtensions
{
    public static void ThrowOnError(this DatabaseUpgradeResult result)
    {
        if (result.Successful) return;
        throw new InvalidOperationException(result.Error.Message);
    }
}