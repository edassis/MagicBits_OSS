using UnityEngine;

namespace MagicBits_OSS.Shared.Scripts
{
    public static class RectTransformExtensions
    {
        public static Rect GetWorldRect(this RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            return new Rect(corners[0], corners[2] - corners[0]);
        }
    }
}
