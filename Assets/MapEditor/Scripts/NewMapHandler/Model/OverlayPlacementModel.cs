using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class OverlayParser
{
	public static Dictionary<string, OverlayPlacementModel> OverlayData = new Dictionary<string, OverlayPlacementModel>();

	public static void LoadOverlays()
	{
		OverlayData = ParserHelpers.ParseTypes<OverlayPlacementModel>("overlays");
	}
}

[Serializable]
public class TextureModel
{
	public string Path;
	private Texture2D Texture { get; set; }

	public Texture2D GetTexture()
	{
		if (Texture != null)
			return Texture;
		string fullPath = System.IO.Path.Combine(Application.streamingAssetsPath, Path);
		if (File.Exists(fullPath))
		{
			Byte[] file = File.ReadAllBytes(fullPath);
			Texture = new Texture2D(2, 2);
			Texture.LoadImage(file);
		}
		else Debug.LogWarning("Failed to find image at path " + fullPath);

		return Texture;
	}
}

public class OverlayPlacementModel : ParsableData
{
	public string GroundTrait;
	public string OverlayLayer;
	public TextureModel NoSides;
	public TextureModel OneSide;
	public TextureModel TwoAdjSides;
	public TextureModel TwoOppSides;
	public TextureModel ThreeSides;
	public TextureModel FourSides;
}
