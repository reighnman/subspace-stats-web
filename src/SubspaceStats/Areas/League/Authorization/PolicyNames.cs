namespace SubspaceStats.Areas.League.Authorization
{
    public static class PolicyNames
    {
        /// <summary>
        /// An overall manager of a resource.
        /// </summary>
        public const string Manager = "ManagerPolicy";

        /// <summary>
        /// A limited manager of a resource that only can manage permits.
        /// </summary>
        public const string PermitManager = "PermitMangerPolicy";
    }
}
