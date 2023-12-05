using System.Text.Json.Serialization;

namespace SubspaceStats.Models.GameDetails
{
	[JsonSerializable(typeof(Game), GenerationMode = JsonSourceGenerationMode.Metadata)]
	internal partial class SourceGenerationContext : JsonSerializerContext
	{
	}
}
