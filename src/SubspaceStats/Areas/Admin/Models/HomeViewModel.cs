namespace SubspaceStats.Areas.Admin.Models
{
    public class HomeViewModel : IAdminViewModel
    {
        public AdminSection Section => AdminSection.General;

        public string? Message { get; set; }
    }
}
