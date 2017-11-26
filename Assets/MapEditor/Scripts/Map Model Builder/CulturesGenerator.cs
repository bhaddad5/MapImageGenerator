﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CulturesGenerator
{
	protected MapModel Map;
	private List<KeyValuePair<Int2, CultureModel>> settlements = new List<KeyValuePair<Int2, CultureModel>>();

	public MapModel GenerateMap(MapModel map, WorldModel world)
	{
		Map = map;
		foreach (CulturePlacementModel culture in world.Cultures)
		{
			BuildCulture(culture);
		}

		BuildRoads();
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
			settlements.Add(new KeyValuePair<Int2, CultureModel>(pos, culture));

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

						break;
					}
				}
			}

			Map.Map.Get(pos).Traits.Add(MapTileModel.TileTraits.Settled.ToString());
			Map.Map.Get(pos).SetMaxHeight(0);

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
				BackgroundTexture = culture.HeraldryBackgrounds[Random.Range(0, culture.HeraldryBackgrounds.Count)],
				ForegroundTexture = culture.HeraldryForegrounds[Random.Range(0, culture.HeraldryForegrounds.Count)],
				OverlayTexture = culture.HeraldryOverlayImage,
				BackgroundColor1 = ColorOptionsParser.ColorOptions[culture.HeraldryBackgroundColorSource].GetRandColor(),
				BackgroundColor2 = ColorOptionsParser.ColorOptions[culture.HeraldryBackgroundColorSource].GetRandColor(),
				ForegroundColor = ColorOptionsParser.ColorOptions[culture.HeraldryForegroundColorSource].GetRandColor(),
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

	private void BuildRoads()
	{
		foreach (KeyValuePair<Int2, CultureModel> settlement in settlements)
		{
			HashSet<Int2> settlementsHit = new HashSet<Int2>();
			settlementsHit.Add(settlement.Key);
			BuildRoadsFromSettlement(settlement.Key, settlementsHit);

			Debug.Log(Map.Map.Get(settlement.Key).TextEntry.Text + ", " + settlementsHit.Count);
		}
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
}
