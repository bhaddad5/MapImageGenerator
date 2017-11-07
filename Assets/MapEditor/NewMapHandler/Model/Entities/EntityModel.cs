public class EntityModel
{
	public enum PlacementType
	{
		TopEdge,
		BottomEdge,
		LeftEdge,
		RightEdge,
		TopLeftCorner,
		BottomLeftCorner,
		TopRightCorner,
		BottomRightCorner,
		Center,
		Scattered
	}

	public PlacementType Placement;
}