using System;
using System.Collections.Generic;

[Serializable]
public class MapTileModel
{
	public float Height;
	public string TerrainId;

	public TerrainModel Terrain()
	{
		return TerrainParser.TerrainData[TerrainId];
	}
	public string KingdomId;
	public List<EntityPlacementModel> Entities = new List<EntityPlacementModel>();
}