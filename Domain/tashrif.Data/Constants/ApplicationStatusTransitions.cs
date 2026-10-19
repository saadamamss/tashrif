namespace tashrif.Data.Constants;

/// <summary>
/// BUG-05: legal application-status transitions. Single source of the matrix — services must
/// never duplicate it. Rules:
/// - forward-only pipeline: new → shortlisted → interview → contract_sent → accepted;
/// - refused is reachable from every active stage and can be restored to new;
/// - withdrawn is reachable ONLY via WithdrawAsync (individual, pre-contract) and is final
///   for entities — it cannot be restored or set through an entity endpoint;
/// - accepted is terminal; 
/// - self-transitions are legal no-ops (re-scheduling an interview must not fail).
/// </summary>
public static class ApplicationStatusTransitions
{
    private static readonly Dictionary<string, string[]> Map = new()
    {
        ["new"] = ["shortlisted", "refused"],
        ["shortlisted"] = ["interview", "refused"],
        ["interview"] = ["contract_sent", "refused"],
        ["contract_sent"] = ["accepted", "refused"],
        ["refused"] = ["new"],
        ["withdrawn"] = [],
        ["accepted"] = [],
    };

    public static bool IsLegal(string from, string to) =>
        from == to || (Map.TryGetValue(from, out var allowed) && allowed.Contains(to));

    /// <summary>Only a refused application can be restored to new by its owning entity.</summary>
    public static bool CanRestore(string from) =>
        Map.TryGetValue(from, out var allowed) && allowed.Contains("new");
}
