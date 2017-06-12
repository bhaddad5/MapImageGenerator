using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionsTransparencyController : MonoBehaviour
{
	public MeshRenderer regionsMat;

	public void SetRegionsTransparency(float f)
	{
		var c = regionsMat.material.color;
		c.a = f;
		regionsMat.material.color = c;
	}
}
