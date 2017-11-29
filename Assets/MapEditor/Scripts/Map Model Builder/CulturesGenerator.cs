using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CulturesGenerator
{
	protected MapModel Map;
	private Dictionary<Int2, SettlementInstance> settlements = new Dictionary<Int2, SettlementInstance>();
	private Dictionary<Int2, PortInstance> ports = new Dictionary<Int2, PortInstance>();

	private class SettlementInstance
	{
		public Int2 SettlementTile;
		public CultureModel Culture;
		public SettlementTypeModel SettlementType;
		public List<Int2> AdjacentSettlements = new List<Int2>();

		public SettlementInstance(Int2 tile, CultureModel culture, SettlementTypeModel type)
		{
			SettlementTile = tile;
			Culture = culture;
			SettlementType = type;
		}

		public Int2 GetAdjacent(List<Int2> including, List<Int2> excluding, Dictionary<Int2, SettlementInstance> settlements)
		{
			foreach (Int2 adjacenctSettlement in AdjacentSettlements)
			{
				if (including.Contains(adjacenctSettlement) && !excluding.Contains(adjacenctSettlement) &&
				    settlements[adjacenctSettlement].Culture == Culture)
					return adjacenctSettlement;
			}
			return null;
		}
	}

	private class PortInstance
	{
		public Int2 PortTile;
		public List<List<Int2>> SeaLanes = new List<List<Int2>>();

		public PortInstance(Int2 tile)
		{
			PortTile = tile;
		}
	}

	public MapModel GenerateMap(MapModel map, WorldModel world)
	{
		Map = map;
		foreach (CulturePlacementModel culture in world.Cultures)
		{
			BuildCulture(culture);
		}

		BuildAdjacencies();

		foreach (PortInstance port in ports.Values)
		{
			port.SeaLanes = FindShippingRoutesFromPort(port.PortTile, new Dictionary<Int2, List<Int2>>()).Values.ToList();
			Map.Map.Get(port.PortTile).Port = new PortModel()
			{
				SeaLanes = port.SeaLanes
			};
		}

		return Map;
	}

	private void BuildCulture(CulturePlacementModel culture)
	{
		foreach (SettlementPlacementInfo settlementType in culture.Culture.SettlementTypes)
		{
			PlaceSettlementType(settlementType, culture.Culture, (int)(Map.Map.Height * culture.MinLatitude), (int)(Map.Map.Height * culture.MaxLatitude));
		}
	}

	private void PlaceSettlementType(SettlementPlacementInfo settlementPlacementInfo, CultureModel culture, int minH, int maxH)
	{
		int numToPlace = (int)(settlementPlacementInfo.PlacementsPer20Square * Map.OccurancesPer20Scaler(minH, maxH));
		for (int i = 0; i < numToPlace; i++)
		{
			Int2 pos = GetSettlementPlacementPos(SettlementTypeParser.SettlementsData[settlementPlacementInfo.SettlementType], minH, maxH);
			if (pos == null)
				continue;

			Map.Map.Get(pos).Traits.Add(MapTileModel.TileTraits.Settled.ToString());
			Map.Map.Get(pos).SetMaxHeight(0);

			SettlementTypeModel SettlementType = SettlementTypeParser.SettlementsData[settlementPlacementInfo.SettlementType];

			EntityPlacementModel settlement = new EntityPlacementModel()
			{
				min = 1,
				max = 1,
				model = SettlementType.Entity.model,
				placementMode = EntityPlacementModel.PlacementMode.Center.ToString()
			};
			settlement.PreBakeModelIndex();

			Map.Map.Get(pos).Entities.Add(settlement);
			settlements[pos] = new SettlementInstance(pos, culture, SettlementType);

			if (SettlementType.PortEntity.model != null)
			{
				var adjRand = Map.Map.GetAdjacentPoints(pos).RandomEnumerate();
				foreach (Int2 adjacentPoint in adjRand)
				{
					if (Map.Map.Get(adjacentPoint).HasTrait(MapTileModel.TileTraits.Ocean) &&
						Map.Map.Get(adjacentPoint).Entities.Count == 0)
					{
						EntityPlacementModel port = new EntityPlacementModel()
						{
							min = 1, max = 1,
							model = SettlementType.PortEntity.model,
						};
						port.PreBakeModelIndex();
						if((pos - adjacentPoint).Equals(new Int2(-1, 0)))
							port.placementMode = EntityPlacementModel.PlacementMode.Rot180.ToString();
						if ((pos - adjacentPoint).Equals(new Int2(0, -1)))
							port.placementMode = EntityPlacementModel.PlacementMode.Rot90.ToString();
						if ((pos - adjacentPoint).Equals(new Int2(1, 0)))
							port.placementMode = EntityPlacementModel.PlacementMode.Rot0.ToString();
						if ((pos - adjacentPoint).Equals(new Int2(0, 1)))
							port.placementMode = EntityPlacementModel.PlacementMode.Rot270.ToString();
						Map.Map.Get(adjacentPoint).Entities.Add(port);
						Map.Map.Get(adjacentPoint).Traits.Add(MapTileModel.TileTraits.Port.ToString());
						ports[adjacentPoint] = new PortInstance(adjacentPoint);

						break;
					}
				}
			}

			List<string> traits = new List<string>();
			traits = traits.Concat(Map.Map.Get(pos).GetTraits()).ToList();
			foreach (MapTileModel adjacentValue in Map.Map.GetAdjacentValues(pos))
			{
				traits = traits.Concat(adjacentValue.GetTraits()).ToList();
			}

			Map.Map.Get(pos).TextEntry = new SettlementTextModel()
			{
				Text = TextChunkParser.TextData[SettlementType.NameChunk].GetText(traits),
				SettlementDescription = culture.CultureName + SettlementType.SettlementTypeName,
				SettlementHeraldry = new HeraldryModel()
				{
					BackgroundTexture = culture.HeraldryBackgrounds[Random.Range(0, culture.HeraldryBackgrounds.Count)],
					ForegroundTexture = culture.HeraldryForegrounds[Random.Range(0, culture.HeraldryForegrounds.Count)],
					OverlayTexture = culture.HeraldryOverlayImage,
					BackgroundColor1 = ColorOptionsParser.ColorOptions[culture.HeraldryBackgroundColorSource].GetRandColor(),
					BackgroundColor2 = ColorOptionsParser.ColorOptions[culture.HeraldryBackgroundColorSource].GetRandColor(),
					ForegroundColor = ColorOptionsParser.ColorOptions[culture.HeraldryForegroundColorSource].GetRandColor(),
				}
			};
		}
	}

	private Int2 GetSettlementPlacementPos(SettlementTypeModel settlementType, int minH, int maxH)
	{
		SortedDupList<Int2> possiblePoses = new SortedDupList<Int2>();
		for (int i = 0; i < 100; i++)
		{
			Int2 randPos = new Int2(Random.Range(0, Map.Map.Width - 1), Random.Range(minH, maxH - 1));
			if (Map.Map.Get(randPos).HasTrait(MapTileModel.TileTraits.Impassable) || 
				Map.Map.Get(randPos).HasTrait(MapTileModel.TileTraits.Water) ||
			    Map.Map.Get(randPos).HasTrait(MapTileModel.TileTraits.Settled))
				continue;
			float value = GetSettlementPosValue(randPos, settlementType);
			possiblePoses.Insert(randPos, value);
		}
		return possiblePoses.TopValue();
	}

	private float GetSettlementPosValue(Int2 pos, SettlementTypeModel settlementType)
	{
		float val = 0f;
		foreach (MapTileModel tile in Map.Map.GetAdjacentValues(pos))
			val += GetTileValue(tile, settlementType.TraitPreferences);
		foreach (MapTileModel tile in Map.Map.GetDiagonalValues(pos))
			val += GetTileValue(tile, settlementType.TraitPreferences) * .5f;
		foreach (MapTileModel tile in Map.Map.GetTwoAwayAdjacentValues(pos))
			val += GetTileValue(tile, settlementType.TraitPreferences) * .5f;
		return val;
	}

	private float GetTileValue(MapTileModel tile, List<TraitPreferance> prefs)
	{
		float val = 0;
		foreach (TraitPreferance traitPreferance in prefs)
		{
			if (tile.HasTrait(traitPreferance.Trait))
				val += traitPreferance.Preference;
		}
		return val;
	}



	private void BuildAdjacencies()
	{
		Dictionary<Int2, SettlementInstance> adjacencies = new Dictionary<Int2, SettlementInstance>();

		foreach (KeyValuePair<Int2, SettlementInstance> settlement in settlements.ToList().RandomEnumerate())
		{
			HashSet<Int2> settlementsHit = new HashSet<Int2>();
			settlementsHit.Add(settlement.Key);
			BuildRoadsFromSettlement(settlement.Key, settlementsHit);

			adjacencies[settlement.Key] = new SettlementInstance(settlement.Key, settlement.Value.Culture, settlement.Value.SettlementType);
			adjacencies[settlement.Key].AdjacentSettlements = settlementsHit.ToList();
		}

		BuildKingdoms(adjacencies);
	}

	private void BuildKingdoms(Dictionary<Int2, SettlementInstance> adjacencies)
	{
		while (adjacencies.Count > 0)
		{
			Dictionary<Int2, SettlementInstance> Kingdom = new Dictionary<Int2, SettlementInstance>();
			int desiredKingdomSize = (int)Math.Max((Random.Range(.3f, 1f) * Random.Range(.3f, 1f) * 18), 1);
			var start = adjacencies.First();
			Kingdom[start.Key] = start.Value;
			for (int s = 1; s < desiredKingdomSize; s++)
			{
				foreach (var settlement in Kingdom)
				{
					Int2 adj = Kingdom[settlement.Key].GetAdjacent(adjacencies.Keys.ToList(), Kingdom.Keys.ToList(), settlements);
					if (adj != null)
					{
						Kingdom[adj] = adjacencies[adj];
						break;
					}
				}
			}
			Map.Map.Get(start.Key).TextEntry.Capitol = true;
			string kingdomName = DetermineKingdomName(start.Key, Kingdom);
			foreach (Int2 settlement in Kingdom.Keys)
			{
				adjacencies.Remove(settlement);
				Map.Map.Get(settlement).TextEntry.KingdomName = kingdomName;
				Map.Map.Get(settlement).TextEntry.KingdomHeraldry = Map.Map.Get(start.Key).TextEntry.SettlementHeraldry;
			}
		}
	}

	private string DetermineKingdomName(Int2 Capitol, Dictionary<Int2, SettlementInstance> Kingdom)
	{
		List<string> constraints = new List<string>();
		int kingdomSize = Kingdom.Count;
		if(kingdomSize == 1)
			constraints.Add("One");
		else if(kingdomSize <= 3)
			constraints.Add("Small");
		else if(kingdomSize <= 5)
			constraints.Add("Medium");
		else constraints.Add("Large");

		return TextChunkParser.TextData[settlements[Capitol].SettlementType.KingdomNameChunk]
			.GetText(constraints, Map.Map.Get(Capitol).TextEntry.Text);
	}

	private void BuildRoadsFromSettlement(Int2 settlement, HashSet<Int2> settlementsHit)
	{
		Int2 startTile = settlement;

		SortedDupList<Int2> frontierTiles = new SortedDupList<Int2>();
		frontierTiles.Insert(startTile, 1f);

		Map2D<float> distMap = new Map2D<float>(Map.Map.Width, Map.Map.Height);

		while (frontierTiles.Count > 0)
		{
			distMap.Set(frontierTiles.TopValue(), frontierTiles.TopKey());
			float currDifficulty = frontierTiles.TopKey();
			Int2 currTile = frontierTiles.Pop();
			foreach (var tile in Map.Map.GetAdjacentPoints(currTile))
			{
				if (!distMap.Get(tile).Equals(0))
					continue;

				if (Map.Map.Get(tile).HasTrait(MapTileModel.TileTraits.Settled))
				{
					Int2 sett = tile;
					bool alreadyHit = false;
					foreach (Int2 hit in settlementsHit)
					{
						if (hit.Equals(sett))
							alreadyHit = true;
					}
					if (!alreadyHit)
					{
						settlementsHit.Add(tile);
						BuildRoadsFromSettlement(settlement, settlementsHit);
						return;
					}
				}

				float difficulty = Map.Map.Get(tile).Difficulty;
				if (currDifficulty - difficulty > 0)
				{
					frontierTiles.Insert(tile, currDifficulty - difficulty);
					distMap.Set(tile, currDifficulty - difficulty);
				}
			}
		}
	}

	private Dictionary<Int2, List<Int2>> FindShippingRoutesFromPort(Int2 port, Dictionary<Int2, List<Int2>> portPaths)
	{
		Int2 startTile = port;

		SortedDupList<Int2> frontierTiles = new SortedDupList<Int2>();
		frontierTiles.Insert(startTile, 2f);

		Map2D<float> distMap = new Map2D<float>(Map.Map.Width, Map.Map.Height);

		while (frontierTiles.Count > 0)
		{
			distMap.Set(frontierTiles.TopValue(), frontierTiles.TopKey());
			float currDifficulty = frontierTiles.TopKey();
			Int2 currTile = frontierTiles.Pop();
			foreach (var tile in Map.Map.GetAdjacentPoints(currTile))
			{
				if (!Map.Map.Get(tile).HasTrait(MapTileModel.TileTraits.Ocean) || !distMap.Get(tile).Equals(0))
					continue;

				if (Map.Map.Get(tile).HasTrait(MapTileModel.TileTraits.Port))
				{
					Int2 sett = tile;
					bool alreadyHit = false;
					foreach (Int2 hit in portPaths.Keys)
					{
						if (hit.Equals(sett))
							alreadyHit = true;
					}
					if (!alreadyHit)
					{
						portPaths[tile] = BuildPathBackFromTile(tile, distMap, new List<Int2>());
						portPaths[tile].Reverse();
						portPaths = FindShippingRoutesFromPort(port, portPaths);
						return portPaths;
					}
				}

				float difficulty = Map.Map.Get(tile).Difficulty;
				if (currDifficulty - difficulty > 0)
				{
					frontierTiles.Insert(tile, currDifficulty - difficulty);
					distMap.Set(tile, currDifficulty - difficulty);
				}
			}
		}
		return portPaths;
	}

	private List<Int2> BuildPathBackFromTile(Int2 tile, Map2D<float> distMap, List<Int2> path)
	{
		path.Add(tile);
		Int2 maxTile = tile;
		foreach (var t in distMap.GetAdjacentPoints(tile))
		{
			if (distMap.Get(t) > distMap.Get(maxTile))
				maxTile = t;
		}

		if (!maxTile.Equals(tile))
			path = BuildPathBackFromTile(maxTile, distMap, path);
		path.Add(maxTile);
		return path;
	}
}
