using System;
using System.Collections.Generic;
using UnityEngine;

public static class UnityTools
{
    private static readonly Dictionary<int, UnityTools.SpriteData> SpriteGeometries = new Dictionary<int, SpriteData>();

    public static T GetComponentOrDie<T>(GameObject gameObject) where T : Component
    {
 
        T component = gameObject.GetComponent<T>();
        if ((UnityEngine.Object)component == (UnityEngine.Object)null)
            throw new UnityException("No component of type " + typeof(T).FullName + " in game object " + gameObject.name);
        return component;
    }

    public static UnityTools.SpriteData GetSpriteData(Sprite sprite)
    {

        UnityTools.SpriteData spriteData;
        if (!UnityTools.SpriteGeometries.TryGetValue(sprite.GetInstanceID(), out spriteData))
        {
            spriteData = new UnityTools.SpriteData(sprite);
            UnityTools.SpriteGeometries[sprite.GetInstanceID()] = spriteData;
        }
        return spriteData;
    }

    public static void ClearSpriteData()
    {
        UnityTools.SpriteGeometries.Clear();
    }

    public static T EnsureSingleton<T>(ref T instanceHolder) where T : MonoBehaviour
    {
        if (object.ReferenceEquals((object)instanceHolder, (object)null))
        {
            System.Type type = typeof(T);
            instanceHolder = UnityEngine.Object.FindFirstObjectByType<T>();
            if ((UnityEngine.Object)instanceHolder == (UnityEngine.Object)null)
            {
                GameObject gameObject1 = new GameObject(type.Name);
                gameObject1.hideFlags = HideFlags.HideAndDontSave;
                GameObject gameObject2 = gameObject1;
                UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object)gameObject2);
                instanceHolder = gameObject2.AddComponent<T>();
            }
        }
        return instanceHolder;
    }


    static UnityTools()
    {
    }


    public class SpriteData
    {
        private static readonly Vector2[] Empty;           
        private static readonly ushort[] EmptyIndexes;     

        public readonly Vector2[] Vertices;                
        public readonly Vector2[] UV;                      
        public readonly ushort[] Triangles;                
        public readonly Vector2 UVOffset;                  
        public readonly Vector2 UVScale;                   

        private Vector2[] _convexHullLeft;                 
        private Vector2[] _convexHullRight;                

        public Vector2[] ConvexHullLeft
        {
            get
            {
                if (this._convexHullLeft == null)
                    this.ComputeConvexHull();
                return this._convexHullLeft;
            }
        }

        public Vector2[] ConvexHullRight
        {
  
            get
            {
                if (this._convexHullRight == null)
                    this.ComputeConvexHull();
                return this._convexHullRight;
            }
        }

        public SpriteData(Sprite sprite)
        {
            this.Vertices = sprite.vertices;
            this.UV = sprite.uv;
            this.Triangles = sprite.triangles;
            if (this.Vertices.Length > 0 && this.Vertices.Length == this.UV.Length)
            {
                Vector2 vector2_1 = this.Vertices[0];
                for (int index = 1; index < this.Vertices.Length; ++index)
                {
                    Vector2 vector2_2 = this.Vertices[index];
                    if (!Mathf.Approximately(vector2_1.x, vector2_2.x) && !Mathf.Approximately(vector2_1.y, vector2_2.y))
                    {
                        Vector2 vector2_3 = this.UV[0];
                        Vector2 vector2_4 = this.UV[index];
                        this.UVScale = new Vector2((float)(((double)vector2_4.x - (double)vector2_3.x) / ((double)vector2_2.x - (double)vector2_1.x)), (float)(((double)vector2_4.y - (double)vector2_3.y) / ((double)vector2_2.y - (double)vector2_1.y)));
                        this.UVOffset = new Vector2(vector2_3.x - this.UVScale.x * vector2_1.x, vector2_3.y - this.UVScale.y * vector2_1.y);
                        break;
                    }
                }
            }
            else
            {
                UnityEngine.Debug.LogError((object)("Sprite " + sprite.name + " has different vertex and uv counts: " + (object)this.Vertices.Length + "/" + (object)this.UV.Length));
                this.Vertices = UnityTools.SpriteData.Empty;
                this.UV = UnityTools.SpriteData.Empty;
                this.Triangles = UnityTools.SpriteData.EmptyIndexes;
            }
        }

        private void ComputeConvexHull()
        {
            List<Vector2> vector2List1 = new List<Vector2>((IEnumerable<Vector2>)this.Vertices);
            vector2List1.Sort(new Comparison<Vector2>(this.CompareVertices));
            List<Vector2> vector2List2 = new List<Vector2>();
            for (int index = this.Vertices.Length - 1; index >= 0; --index)
            {
                while (vector2List2.Count > 1 && this.IsInside(vector2List2[vector2List2.Count - 2], vector2List2[vector2List2.Count - 1], vector2List1[index]))
                    vector2List2.RemoveAt(vector2List2.Count - 1);
                vector2List2.Add(vector2List1[index]);
            }
            List<Vector2> vector2List3 = new List<Vector2>();
            for (int index = 0; index < this.Vertices.Length; ++index)
            {
                while (vector2List3.Count > 1 && this.IsInside(vector2List3[vector2List3.Count - 2], vector2List3[vector2List3.Count - 1], vector2List1[index]))
                    vector2List3.RemoveAt(vector2List3.Count - 1);
                vector2List3.Add(vector2List1[index]);
            }
            this._convexHullLeft = vector2List2.ToArray();
            vector2List3.Reverse();
            this._convexHullRight = vector2List3.ToArray();
        }

        private bool IsInside(Vector2 v1, Vector2 v2, Vector2 v3)
        {
 
            return ((double)v1.x - (double)v3.x) * ((double)v2.y - (double)v3.y) <= ((double)v1.y - (double)v3.y) * ((double)v2.x - (double)v3.x);
        }

        private int CompareVertices(Vector2 v1, Vector2 v2)
        {
    
            int num = v1.y.CompareTo(v2.y);
            return num != 0 ? num : v1.x.CompareTo(v2.x);
        }

        static SpriteData()
        {
        }
    }
}
