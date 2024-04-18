using UnityEngine;

namespace Cosmos
{
    public static class RectTransformExts
    {
		public static void SetSizeDeltaWidth(this RectTransform @this, float width)
		{
			Vector2 size = @this.sizeDelta;
			size.x = width;
			@this.sizeDelta = size;
		}
		public static void SetSizeDeltaHeight(this RectTransform @this, float height)
		{
			Vector2 size = @this.sizeDelta;
			size.y = height;
			@this.sizeDelta = size;
		}
		public static void SetAnchoredPositionX(this RectTransform @this, float x)
		{
			Vector3 temp = @this.anchoredPosition;
			temp.x = x;
			@this.anchoredPosition = temp;
		}
		public static void SetAnchoredPositionY(this RectTransform @this, float y)
		{
			Vector3 temp = @this.anchoredPosition;
			temp.y = y;
			@this.anchoredPosition = temp;
		}
		public static void SetAnchoredPositionZ(this RectTransform @this, float z)
		{
			Vector3 temp = @this.anchoredPosition;
			temp.z = z;
			@this.anchoredPosition = temp;
		}
	}
}
