namespace SubspaceStats.Models
{
    public enum GameType : long
    {
        SVS_1v1 = 1,
        SVS_2v2 = 2,
        SVS_3v3 = 3,
        SVS_4v4 = 4,
        PowerBall_Traditional = 5,
        PowerBall_Proball = 6,
        PowerBall_Smallpub = 7,
        PowerBall_3h = 8,
        PowerBall_small4tmpb = 9,
        PowerBall_minipub = 10,
        PowerBall_mediumpub = 11,
        SVS_4v4League = 12,
        SVS_SoloFFA = 13,
        SVS_TeamFFA_2s = 14,
        SVS_2v2League = 15,
    }

    public enum GameCategory
    {
        Solo = 1,
        TeamVersus = 2,
        Powerball = 3,
    }

    public static class GameTypeExtensions
    {
        public static GameCategory? GetGameCategory(this GameType gameType)
        {
            return gameType switch
            {
                GameType.SVS_1v1 or
                GameType.SVS_SoloFFA => GameCategory.Solo,

                GameType.SVS_2v2 or
                GameType.SVS_3v3 or
                GameType.SVS_4v4 or
                GameType.SVS_4v4League or
                GameType.SVS_TeamFFA_2s or
                GameType.SVS_2v2League => GameCategory.TeamVersus,

                GameType.PowerBall_Traditional or
                GameType.PowerBall_Proball or
                GameType.PowerBall_Smallpub or
                GameType.PowerBall_3h or
                GameType.PowerBall_small4tmpb or
                GameType.PowerBall_minipub or
                GameType.PowerBall_mediumpub => GameCategory.Powerball,

                _ => null,
            };
        }

        public static string GetDisplayName(this GameType gameType)
        {
            return gameType switch
            {
                GameType.SVS_1v1 => "1v1",
                GameType.SVS_2v2 => "2v2",
                GameType.SVS_3v3 => "3v3",
                GameType.SVS_4v4 => "4v4",
                GameType.PowerBall_Traditional => "Powerball - Traditional",
                GameType.PowerBall_Proball => "Powerball - Proball",
                GameType.PowerBall_Smallpub => "Powerball - Smallpub",
                GameType.PowerBall_3h => "Powerball - smallpb3h",
                GameType.PowerBall_small4tmpb => "Powerball- small4tmpb",
                GameType.PowerBall_minipub => "Powerball - minipub",
                GameType.PowerBall_mediumpub => "Powerball - mediumpub",
                GameType.SVS_4v4League => "SVS 4v4 League",
                GameType.SVS_SoloFFA => "SVS Solo FFA",
                GameType.SVS_TeamFFA_2s => "SVS Team FFA 2s",
                GameType.SVS_2v2League => "SVS 2v2 League",
                _ => gameType.ToString(),
            };
        }
    }
}
