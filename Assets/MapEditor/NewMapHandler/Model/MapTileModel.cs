using System;
using System.Collections.Generic;

[Serializable]
public class MapTileModel
{
	public float Height;
	public TerrainModel Terrain;
	public List<EntityModel> Entities;
}
