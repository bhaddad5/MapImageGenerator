using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHeightGenerator
{
	Map2D<float> GenerateHeightMap(int width, int height);
}
