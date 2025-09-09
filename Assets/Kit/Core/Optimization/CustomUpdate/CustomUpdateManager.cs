using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.Core.Optimization
{
	[DisallowMultipleComponent]
	public sealed class CustomUpdateManager : MonoBehaviour
	{
		private static CustomUpdateManager m_instance;
		private static List<IUpdatable> m_list;
		private static bool m_isQuit;

		public static IReadOnlyList<IUpdatable> List => m_list;
		public static int Count => m_list.Count;

        private void Update()
        {
            for (var i = m_list.Count - 1; i >= 0; i--)
            {
                var obj = m_list[i];
                obj.OnUpdate();
            }
        }

        private void OnApplicationQuit()
		{
			m_isQuit = true;
		}

		public static void Initialize()
		{
			Initialize(0);
		}

		public static void Initialize(int capacity)
		{
			if (m_instance != null) return;

			var gameObject = new GameObject(nameof(CustomUpdateManager))
			{
				hideFlags = HideFlags.HideAndDontSave,
			};

			m_instance = gameObject.AddComponent<CustomUpdateManager>();
			m_list = new List<IUpdatable>(capacity);

			DontDestroyOnLoad(gameObject);
		}

		public static void Add(IUpdatable obj)
		{
			if (m_isQuit) return;
			Initialize();
			m_list.Add(obj);
		}

		public static void Remove(IUpdatable obj)
		{
			if (m_isQuit) return;
			Initialize();
			m_list.Remove(obj);
		}

		public static void Clear()
		{
			if (m_isQuit) return;
			Initialize();
			m_list.Clear();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void RuntimeInitializeOnLoadMethod()
		{
			if (m_instance != null)
			{
				Destroy(m_instance);
			}

			m_list?.Clear();

			m_instance = null;
			m_list = null;
			m_isQuit = false;
		}
	}
}

