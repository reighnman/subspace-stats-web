using SubspaceStats.Models;

namespace SubspaceStats.Areas.Admin.Models.GameTypes
{
    public class GameTypesListViewModel : IAdminViewModel
    {
        public required OrderedDictionary<long, GameType> GameTypes { get; set; }

        public AdminSection Section => AdminSection.GameTypes;
    }
}
