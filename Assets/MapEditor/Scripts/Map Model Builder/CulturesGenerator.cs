using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CulturesGenerator
{
	protected MapModel Map;

	public MapModel GenerateMap(MapModel map, WorldModel world)
	{
		Map = map;
		foreach (CulturePlacementModel culture in world.Cultures)
		{
			BuildCulture(culture);
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

			SettlementTypeModel SettlementType = SettlementTypeParser.SettlementsData[settlementPlacementInfo.SettlementType];

			Map.Map.Get(pos).Entities.Add(SettlementType.Entity);
			Map.Map.Get(pos).Traits.Add("Settled");
			Map.Map.Get(pos).SetMaxHeight(0);
			Map.Map.Get(pos).TextEntry = new SettlementTextModel()
			{
				Text = TextChunkParser.TextData[SettlementType.NameChunk].GetText(),
				SettlementDescription = culture.CultureName + SettlementType.SettlementTypeName,
				BackgroundTexture = culture.HeraldryBackgrounds[Random.Range(0, culture.HeraldryBackgrounds.Count)],
				ForegroundTexture = culture.HeraldryForegrounds[Random.Range(0, culture.HeraldryForegrounds.Count)],
				OverlayTexture = culture.HeraldryOverlayImage,
			};

			Debug.Log(Map.Map.Get(pos).TextEntry.Text);
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
		{
			foreach (TraitPreferance traitPreferance in settlementType.TraitPreferences)
			{
				if (tile.HasTrait(traitPreferance.Trait))
					val += traitPreferance.Preference;
			}
		}
		return val;
	}
}
