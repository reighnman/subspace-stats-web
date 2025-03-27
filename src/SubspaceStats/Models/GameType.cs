namespace SubspaceStats.Models
{
    public enum GameType : long
    {
        Nexus_Duel = 1,
        Nexus_1v1_Obstacle = 2,
        Nexus_2v2_Box = 3,
        Nexus_2v2_Obstacle = 4,
        Nexus_3v3_Obstacle = 5,
        Nexus_4v4_Obstacle = 6,
        SVS_4v4_League = 7,
        SVS_4v4_Practice = 8,
        SVS_4v4_MatchMaking = 9,
        SVS_SoloFFA = 10,
        SVS_TeamFFA_2s = 11,
        PowerBall_Traditional = 12,
        PowerBall_Proball = 13,
        PowerBall_Smallpub = 14,
        PowerBall_3h = 15,
        PowerBall_small4tmpb = 16,
        PowerBall_minipub = 17,
        PowerBall_mediumpub = 18,
        Chaos = 19,

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
                GameType.Nexus_Duel or
                GameType.Nexus_1v1_Obstacle => GameCategory.Solo,

                GameType.Nexus_2v2_Box or
                GameType.Nexus_2v2_Obstacle or
                GameType.Nexus_3v3_Obstacle or
                GameType.Nexus_4v4_Obstacle or
                GameType.SVS_4v4_League or
                GameType.SVS_4v4_Practice or
                GameType.SVS_4v4_MatchMaking or
                GameType.SVS_SoloFFA or
                GameType.SVS_TeamFFA_2s => GameCategory.TeamVersus,

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
                GameType.Nexus_Duel => "Nexus Public - Duel (box)",
                GameType.Nexus_1v1_Obstacle => "Nexus Public - 1v1 (obstacle)",
                GameType.Nexus_2v2_Box => "Nexus Public - 2v2 (box)",
                GameType.Nexus_2v2_Obstacle => "Nexus Public - 2v2 (obstacle)",
                GameType.Nexus_3v3_Obstacle => "Nexus Public - 3v3 (obstacle)",
                GameType.Nexus_4v4_Obstacle => "Nexus Public - 4v4 (obstacle)",
                GameType.SVS_4v4_League => "SVS 4v4 League ",
                GameType.SVS_4v4_Practice => "SVS 4v4 Practice",
                GameType.SVS_4v4_MatchMaking => "SVS 4v4 MatchMaking",
                GameType.SVS_SoloFFA => "SVS Solo FFA",
                GameType.SVS_TeamFFA_2s => "SVS Team FFA 2s",
                GameType.PowerBall_Traditional => "Powerball - Traditional",
                GameType.PowerBall_Proball => "Powerball - Proball",
                GameType.PowerBall_Smallpub => "Powerball - Smallpub",
                GameType.PowerBall_3h => "Powerball - smallpb3h",
                GameType.PowerBall_small4tmpb => "Powerball- small4tmpb",
                GameType.PowerBall_minipub => "Powerball - minipub",
                GameType.PowerBall_mediumpub => "Powerball - mediumpub",

                _ => gameType.ToString(),
            };
        }
    }
}
