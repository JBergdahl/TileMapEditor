using System.Text.Json.Serialization;

namespace TileMapEditor.Models
{
    public class MainWindowModel
    {
        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("grid_rows")] public int GridHeight { get; set; }

        [JsonPropertyName("grid_cols")] public int GridWidth { get; set; }

        [JsonPropertyName("rows")] public int TilemapHeight { get; set; }

        [JsonPropertyName("cols")] public int TilemapWidth { get; set; }

        [JsonPropertyName("grid_layers")] public int[,] GridLayers { get; set; }

        [JsonPropertyName("tile_height")] public int TileHeight { get; set; }

        [JsonPropertyName("tile_width")] public int TileWidth { get; set; }

        [JsonPropertyName("tilemap_path")] public string TileMapPath { get; set; }

        [JsonPropertyName("collidable")] public int[] Collidable { get; set; }
    }
}