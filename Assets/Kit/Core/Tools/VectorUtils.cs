using UnityEngine;

/// <summary>
/// VECTOR 값 계산을 도와주는 정적 클래스
/// </summary>
public static class VectorUtils
{
    public static Vector2 ConvertToCanvas(Vector3 world, RectTransform canvas)
    {
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(world);
        return new Vector2(((viewportPosition.x * canvas.sizeDelta.x) - (canvas.sizeDelta.x * 0.5f)), ((viewportPosition.y * canvas.sizeDelta.y) - (canvas.sizeDelta.y * 0.5f)));
    }

    public static Vector2 ConvertToCanvas(Vector3 world, Vector2 canvas)
    {
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(world);        
        return new Vector2(((viewportPosition.x * canvas.x) - (canvas.x * 0.5f)), ((viewportPosition.y * canvas.y) - (canvas.y * 0.5f))); 
    }

    public static Vector2 ConvertToCanvas(Vector3 world, Vector2 canvas, float offsetY = 0f)
    {
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(world);
        Vector2 result = new Vector2(((viewportPosition.x * canvas.x) - (canvas.x * 0.5f)), ((viewportPosition.y * canvas.y) - (canvas.y * 0.5f)));        
        result = new Vector2(result.x, result.y + offsetY / Camera.main.orthographicSize);
        return result;
    }

    public static Vector2 SwitchToRectTransform(RectTransform from, RectTransform to)
    {
        Vector2 localPoint;
        Vector2 fromPivotDerivedOffset = new Vector2(from.rect.width * from.pivot.x + from.rect.xMin, from.rect.height * from.pivot.y + from.rect.yMin);
        Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, from.position);
        screenP += fromPivotDerivedOffset;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(to, screenP, null, out localPoint);
        Vector2 pivotDerivedOffset = new Vector2(to.rect.width * to.pivot.x + to.rect.xMin, to.rect.height * to.pivot.y + to.rect.yMin);
        return to.anchoredPosition + localPoint - pivotDerivedOffset;
    }
    /// <summary>
    /// 스크린 좌표를 로컬 좌표계로 변환해주는 메소드
    /// </summary>
    /// <param name="from">지정할 UI Rect</param>
    /// <param name="inputPos">변환할 좌표</param>
    /// <param name="camera">UI Camera</param>
    /// <returns></returns>
    public static Vector3 ConvertToLocalPos(RectTransform from, Vector3 inputPos, Camera camera)
    {
        Vector2 localPoint = Vector3.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(from, inputPos, camera, out localPoint);

        return localPoint;
    }
    /// <summary>
    /// 스크린 좌표를 월드 좌표계로 변환해주는 메소드
    /// </summary>
    /// <param name="from">지정할 UI Rect</param>
    /// <param name="inputPos">변환할 좌표</param>
    /// <param name="camera">UI Camera</param>
    /// <returns></returns>
    public static Vector3 ConvertToWorldPos(RectTransform from, Vector3 inputPos, Camera camera)
    {
        Vector3 worldPoint = Vector3.zero;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(from, inputPos, camera, out worldPoint);

        return worldPoint;
    }
}