namespace SubspaceStats.Areas.Admin.Models
{
    public interface IAdminViewModel
    {
        AdminSection Section { get; }
    }

    public enum AdminSection
    {
        General,
        GameTypes,
    }
}
