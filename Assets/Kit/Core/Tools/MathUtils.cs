using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
{
    public static float Sin(float x) 
    {
        float sinn;
        if (x < -3.14159265f)
            x += 6.28318531f;
        else
        if (x > 3.14159265f)
            x -= 6.28318531f;

        if (x < 0)
        {
            sinn = 1.27323954f * x + 0.405284735f * x * x;

            if (sinn < 0)
                sinn = 0.225f * (sinn * -sinn - sinn) + sinn;
            else
                sinn = 0.225f * (sinn * sinn - sinn) + sinn;
            return sinn;
        }
        else
        {
            sinn = 1.27323954f * x - 0.405284735f * x * x;

            if (sinn < 0)
                sinn = 0.225f * (sinn * -sinn - sinn) + sinn;
            else
                sinn = 0.225f * (sinn * sinn - sinn) + sinn;
            return sinn;
        }
    }
    public static float Cos(float x) 
    {
        return Sin(x + 1.5707963f);
    }
    public static int DivisorSignedMod(int dividend, int divisor)
    {
        return new int();
    }
    public static float Map(float value, float low, float high, float mappedLow, float mappdHigh)
    {
        float num = (float)(((double)value - (double)low) / ((double)high - (double)low));
        return mappedLow + num * (mappdHigh - mappedLow);
    }
    public static float MapClamped(float value, float lowerLimit, float upperLimit, float mappedLowerLimit, float mappedUpperLimit)
    {
        float num = Mathf.Clamp01((float)(((double)value - (double)lowerLimit) / ((double)upperLimit - (double)lowerLimit)));
        return mappedLowerLimit + num * (mappedUpperLimit - mappedLowerLimit);
    }
    public static float MapClampedTo01(float value, float lowLimit, float highLimit)
    {
        return Mathf.Clamp01((float)(((double)value - (double)lowLimit) / ((double)highLimit - (double)lowLimit)));
    }
    public static Vector3 ProjectToZPlane(Ray ray, float zplane = 0.0f)
    {
        float num1 = zplane - ray.origin.z;
        float num2 = !Mathf.Approximately(ray.direction.z, 0.0f) ? num1 / ray.direction.z : 1f;
        return ray.origin + ray.direction * num2;
    }
    internal static Vector2 MapClamped(float value, float fromLow, Vector2 toLow, int fromHi, Vector2 toHi)
    {
        float num = Mathf.Clamp01((float)(((double)value - (double)fromLow) / ((double)fromHi - (double)fromLow)));
        return toLow + num * (toHi + toLow);
    }
    public static float MultiNoise(float x, int octaves, float octaveDecay)
    {
        float num1 = 0.0f;
        for (int index = 0; index < octaves; ++index)
        {
            float num2 = (float)(1 << index);
            num1 += (Mathf.PerlinNoise(x * num2, 0.0f) - 0.5f) * Mathf.Pow(1f / octaveDecay, (float)index);
        }
        return num1;
    }
    internal static Vector2 MakeNormalFromDirection(Vector2 startPoint, Vector2 endPoint)
    {
        return MathUtils.MakeNormalFromDirection((endPoint - startPoint).normalized);
    }
    internal static Vector2 MakeNormalFromDirection(Vector2 dir)
    {
        return new Vector2(-dir.y, dir.x);
    }
    internal static Vector2 MakeDirectionFromNormal(Vector2 normal)
    {
        return new Vector2(normal.y, -normal.x);
    }
    internal static float SignedAngle(Vector3 vector1, Vector3 vector2)
    {
        float num = Vector3.Angle(vector1, vector2);
        return (double)Vector3.Cross(vector1, vector2).z < 0.0 ? -num : num;
    }
    internal static bool LinesIntersection(Vector2 point1, Vector2 dir1, Vector2 point2,
      Vector2 dir2, out Vector2 result)
    {
        float num1 = (float)((double)point1.x * (double)dir1.y - (double)point1.y * (double)dir1.x);
        float num2 = (float)((double)point2.x * (double)dir2.y - (double)point2.y * (double)dir2.x);
        float a = (float)((double)dir1.x * (double)dir2.y - (double)dir1.y * (double)dir2.x);
        if (Mathf.Approximately(a, 0.0f))
        {
            result = new Vector2();
            return false;
        }
        result = new Vector2((float)((double)num2 * (double)dir1.x - (double)num1 * (double)dir2.x), (float)((double)num2 * (double)dir1.y - (double)num1 * (double)dir2.y)) / a;
        return true;
    }
    public static Vector2 CatmullRomSpline(Vector2 p0, Vector2 p1, Vector2 p2,
      Vector2 p3, float t)
    {
        Vector2 vector2_1 = -p0 + 3f * p1 - 3f * p2 + p3;
        Vector2 vector2_2 = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector2 vector2_3 = -p0 + p2;
        return 0.5f * (2f * p1 + (vector2_3 + (vector2_2 + vector2_1 * t) * t) * t);
    }
    public static Vector2 CatmullRomSpline(List<Vector2> points, float t)
    {
        int b = Mathf.FloorToInt(t);
        int index1 = Mathf.Max(0, b - 1);
        int index2 = Mathf.Min(points.Count - 1, b);
        int index3 = Mathf.Min(points.Count - 1, b + 1);
        int index4 = Mathf.Min(points.Count - 1, b + 2);
        return MathUtils.CatmullRomSpline(points[index1], points[index2], points[index3], points[index4], t - (float)b);
    }
    public static List<Vector2> EquidistantPointsOnSpline(List<Vector2> points, float distance, int maxCount)
    {
        return MathUtils.PointsOnSplineAtGivenDistances(points, new float[1] { distance }, maxCount);
    }
    public static List<Vector2> PointsOnSplineAtGivenDistances(List<Vector2> points,
      float[] distances, int maxCount)
    {
        List<Vector2> vector2List = new List<Vector2>();
        if (points.Count == 0)
            return vector2List;
        vector2List.Add(points[0]);
        float num1 = 0.0f;
        float num2 = 0.0f;
        Vector2 a = points[0];
        while (vector2List.Count < maxCount)
        {
            num2 += 0.01f;
            if ((double)num2 <= (double)(points.Count - 1))
            {
                Vector2 b = MathUtils.CatmullRomSpline(points, num2);
                float num3 = num1 + Vector2.Distance(a, b);
                float num4 = distances[(vector2List.Count - 1) % distances.Length];
                if ((double)num3 > (double)num4)
                {
                    vector2List.Add(Vector2.Lerp(a, b, MathUtils.MapClampedTo01(num4, num1, num3)));
                    num3 -= num4;
                }
                a = b;
                num1 = num3;
            }
            else
                break;
        }
        return vector2List;
    }
    public static Vector2 CatmullRomSplineDerivative(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        Vector2 vector2_1 = -p0 + 3f * p1 - 3f * p2 + p3;
        Vector2 vector2_2 = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        return 0.5f * (-p0 + p2 + (2f * vector2_2 + 3f * vector2_1 * t) * t);
    }
    public static void FixWindingOrder(List<Vector2> points)
    {
        int count = points.Count;
        float num = float.MinValue;
        int index1 = -1;
        for (int index2 = 0; index2 < count; ++index2)
        {
            float x = points[index2].x;
            if ((double)x > (double)num)
            {
                index1 = index2;
                num = x;
            }
        }
        if (index1 < 0)
            return;
        Vector2 vector2_1 = points[(index1 + count - 1) % count];
        Vector2 vector2_2 = points[index1];
        Vector2 vector2_3 = points[(index1 + 1) % count];
        if ((double)Vector3.Cross((Vector3)(vector2_2 - vector2_1), (Vector3)(vector2_3 - vector2_2)).z <= 0.0)
            return;
        points.Reverse();
    }
}
