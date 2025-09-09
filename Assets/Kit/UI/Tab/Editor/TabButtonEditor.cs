using System;
using UnityEngine;
using UnityEditor;

namespace GameLogic.UI.Custom
{
	[CustomEditor(typeof(TabButton), true)]
	[CanEditMultipleObjects]
	public class TabButtonEditor : Editor
	{
		#region SerializedProperty

		private TabButton tab;

		SerializedProperty button;
		SerializedProperty animated;
		SerializedProperty buttonPressedScale;
		SerializedProperty buttonPressedEase;
		SerializedProperty buttonPressedDuration;
		SerializedProperty scaleRect;
		SerializedProperty targetOnRect;

		SerializedProperty selectedSprImage;
		SerializedProperty selectedSprite;
		SerializedProperty unselectedSprite;

		SerializedProperty selectedColorImage;
		SerializedProperty selectedColor;
		SerializedProperty unselectedColor;

		SerializedProperty notiObject;
		SerializedProperty notiText;

		SerializedProperty lockObject;
		SerializedProperty lockText;

		SerializedProperty buttonText;

		SerializedProperty colorEnable;
		SerializedProperty spriteEnable;
		SerializedProperty notiEnable;
		SerializedProperty lockEnable;
		#endregion

		#region private
		private bool _colorActive;
		private bool _spriteActive;
		private bool _notiActive;
		private bool _lockActive;
		#endregion

		GUIStyle _cacheBoldtext = null;
		GUIStyle Boldtext
		{
			get
			{
				if (_cacheBoldtext == null)
				{
					_cacheBoldtext = new GUIStyle(GUI.skin.label);
					_cacheBoldtext.fontStyle = FontStyle.Bold;
				}
				return _cacheBoldtext;
			}
		}

		GUIStyle _cacheDescriptiontext = null;
		GUIStyle Descriptiontext
		{
			get
			{
				if (_cacheDescriptiontext == null)
				{
					_cacheDescriptiontext = new GUIStyle(GUI.skin.label);
					_cacheDescriptiontext.fontSize = 10;
					_cacheDescriptiontext.alignment = TextAnchor.MiddleLeft;
					_cacheDescriptiontext.normal.textColor = Color.white;
				}
				return _cacheDescriptiontext;
			}
		}

		protected virtual void OnEnable()
		{
			tab = target as TabButton;

            // Basic Settings.
            button = serializedObject.FindProperty("button");
            animated = serializedObject.FindProperty("animated");
            buttonPressedScale = serializedObject.FindProperty("buttonPressedScale");
            buttonPressedEase = serializedObject.FindProperty("buttonPressedEase");
            buttonPressedDuration = serializedObject.FindProperty("buttonPressedDuration");
            scaleRect = serializedObject.FindProperty("scaleRect");
            targetOnRect = serializedObject.FindProperty("targetOnRect");
            
			selectedSprImage = serializedObject.FindProperty("changeSprImg");
			selectedSprite = serializedObject.FindProperty("selectedSprite");
			unselectedSprite = serializedObject.FindProperty("unselectedSprite");
			selectedColorImage = serializedObject.FindProperty("changeColorImg");
			selectedColor = serializedObject.FindProperty("selectedColor");
			unselectedColor = serializedObject.FindProperty("unselectedColor");
			notiObject = serializedObject.FindProperty("notiObject");
			notiText = serializedObject.FindProperty("notiText");
			lockObject = serializedObject.FindProperty("lockObject");
			lockText = serializedObject.FindProperty("lockText");

			buttonText = serializedObject.FindProperty("buttonText");

			// Control Settings.
			colorEnable = serializedObject.FindProperty("colorEnable");
			spriteEnable = serializedObject.FindProperty("spriteEnable");
			notiEnable = serializedObject.FindProperty("notiEnable");
			lockEnable = serializedObject.FindProperty("lockEnable");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			HeaderInformation();

			ShowGroup("Basic Settings", BasicSettings);
			EditorGUILayout.Separator();

			ShowGroup("Button Color Settings", ShowColorSettings);
			EditorGUILayout.Separator();

			ShowGroup("Button Sprite Settings", ShowSpriteSettings);
			EditorGUILayout.Separator();

			ShowGroup("Button Noti Settings", ShowNotiSettings);
			EditorGUILayout.Separator();

			ShowGroup("Button Lock Settings", ShowLockSettings);
			EditorGUILayout.Separator();

			serializedObject.ApplyModifiedProperties();
		}
		void BasicSettings()
		{
			EditorGUILayout.PropertyField(button);
			EditorGUILayout.PropertyField(animated);
		
			if (animated.boolValue)
			{
				EditorGUILayout.PropertyField(buttonPressedScale);
				EditorGUILayout.PropertyField(buttonPressedEase);
				EditorGUILayout.PropertyField(buttonPressedDuration);
				EditorGUILayout.PropertyField(scaleRect);
			}
			EditorGUILayout.PropertyField(targetOnRect);
			EditorGUILayout.PropertyField(buttonText);
		}
		void ShowColorSettings()
		{
			_colorActive = colorEnable.boolValue;
			_colorActive = GUILayout.Toggle(_colorActive, _colorActive ? "Color Able" : "Color Disable", "button");
			colorEnable.boolValue = _colorActive;

			if (tab.colorEnable)
			{
				EditorGUILayout.PropertyField(selectedColorImage, new GUIContent("Change Color Image", "바꿀려고 하는 이미지를 넣어줍니다, 이미지가 없는 경우 해당 컴포넌트 이미지를 가져옵니다"));
				EditorGUILayout.PropertyField(selectedColor);
				EditorGUILayout.PropertyField(unselectedColor);
			}
		}

		void ShowSpriteSettings()
		{
			_spriteActive = spriteEnable.boolValue;
			_spriteActive = GUILayout.Toggle(_spriteActive, _spriteActive ? "Sprite Able" : "Sprite Disable", "button");
			spriteEnable.boolValue = _spriteActive;

			//EditorGUILayout.PropertyField(spriteEnable, new GUIContent("Sprite", "탭 버튼한테 스프라이트 줄래?"));

			if (tab.spriteEnable)
			{
				EditorGUILayout.PropertyField(selectedSprImage, new GUIContent("Change Sprite Image", "바꿀려고 하는 이미지를 넣어줍니다, 이미지가 없는 경우 해당 컴포넌트 이미지를 가져옵니다"));
				EditorGUILayout.PropertyField(selectedSprite);
				EditorGUILayout.PropertyField(unselectedSprite);
			}

		}

		void ShowLockSettings()
		{
			_lockActive = lockEnable.boolValue;
			_lockActive = GUILayout.Toggle(_lockActive, _lockActive ? "Lock Able" : "Lock Disable", "button");
			lockEnable.boolValue = _lockActive;

			//EditorGUILayout.PropertyField(lockEnable, new GUIContent("Lock", "탭 버튼한테 잠김처리 줄래?"));

			if (tab.lockEnable)
			{
				//EditorGUILayout.LabelField("Lock", "Setting");
				EditorGUILayout.HelpBox("Set LockObject and LockText", MessageType.None);

				EditorGUILayout.PropertyField(lockObject);
				EditorGUILayout.PropertyField(lockText);
			}

		}

		void ShowNotiSettings()
		{
			_notiActive = notiEnable.boolValue;
			_notiActive = GUILayout.Toggle(_notiActive, _notiActive ? "Noti Able" : "Noti Disable", "button");
			notiEnable.boolValue = _notiActive;

			//EditorGUILayout.PropertyField(notiEnable, new GUIContent("Noti", "탭 버튼한테 알림처리 줄래?"));

			if (tab.notiEnable)
			{
				//EditorGUILayout.LabelField("Notification", "Setting");
				EditorGUILayout.HelpBox("Set NotiObject and NotiText", MessageType.None);

				EditorGUILayout.PropertyField(notiObject);
				EditorGUILayout.PropertyField(notiText);
			}

		}

		void ShowGroup(string title, Action showGroupFunction)
		{
			EditorGUILayout.LabelField(title, Boldtext);
			EditorGUI.indentLevel++;
			showGroupFunction();
			EditorGUI.indentLevel--;
		}

		private void HeaderInformation()
		{
			GUILayout.BeginVertical("HelpBox");
			GUILayout.Label("Tab Button", new GUIStyle() { fontSize = 15, alignment = TextAnchor.MiddleCenter });
			GUILayout.Label("1. 상단의 Tabs Component 가 존재해야합니다.", Descriptiontext);
			GUILayout.Label("2. Code로 Enum 에서 TabType 설정해야합니다.", Descriptiontext);
			GUILayout.Label("Version: 1.0.1 Mong", new GUIStyle() { fontSize = 7, alignment = TextAnchor.MiddleCenter });
			GUILayout.EndVertical();
		}
	}
}

