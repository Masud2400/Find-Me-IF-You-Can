using UnityEngine;

public static class GetIndex
{	
	private static int GRID_SIZE = 30;

    public static Vector2Int GetGridIndex(Vector3 position)
	{
		RectTransform targetImage = AssetManager.Instance.TargetImage;
		
		float width = targetImage.rect.width;
		float height = targetImage.rect.height;
		
		float startX = (-width / 2f) + (GRID_SIZE / 2f);
		float startY = (height / 2f) - (GRID_SIZE / 2f);
		
		int x = Mathf.RoundToInt((position.x - startX) / GRID_SIZE);
		int y = Mathf.RoundToInt((startY - position.y) / GRID_SIZE);

		return new Vector2Int(x, y);
	}
}
