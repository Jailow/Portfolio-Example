using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
namespace CaveMiner.Editor
{
    public class BlockCreator : EditorWindow
    {
        private static Vector2 _windowMinSize = new Vector2 { x = 375, y = 200 };
        private Transform _canvasTr;
        private Object _spritesFolderObject;
        private GameObject _mainFile;

        [MenuItem("Helpers/Block Creator", priority = 1)]
        private static void ShowBlockCreatorWindow()
        {
            var window = EditorWindow.GetWindow(typeof(BlockCreator));
            window.minSize = _windowMinSize;
            window.maxSize = _windowMinSize;
            window.titleContent = new GUIContent
            {
                text = "Block Creator",
            };
        }

        private void OnGUI()
        {
            EditorGUIUtility.labelWidth = 75;

            Rect rect = new Rect(0, 2.5f, _windowMinSize.x, 25);

            _spritesFolderObject = EditorGUI.ObjectField(rect, "Sprites Folder", _spritesFolderObject, typeof(Object), false);

            rect.y += 30;

            _mainFile = (GameObject)EditorGUI.ObjectField(rect, "Main File", _mainFile, typeof(GameObject), false);

            rect.y += 30;

            _canvasTr = (Transform)EditorGUI.ObjectField(rect, "Canvas", _canvasTr, typeof(Transform), true);

            rect.y += 30;

            if (GUI.Button(rect, "Make Block"))
            {
                var mainObject = Instantiate(_mainFile, _canvasTr);
                mainObject.transform.localScale = Vector3.one * 1.1f;

                var rectTr = mainObject.AddComponent<RectTransform>();
                rectTr.anchorMin = Vector2.zero;
                rectTr.anchorMax = Vector2.one;
                rectTr.offsetMin = new Vector2(0, -1025);
                rectTr.offsetMax = new Vector2(0, -1025);

                string folderPath = AssetDatabase.GetAssetPath(_spritesFolderObject);
                Sprite[] sprites = GetSpritesInFolder(folderPath);

                Debug.Log($"Textures count: {sprites.Length}");

                var spriteRenderers = mainObject.GetComponentsInChildren<SpriteRenderer>();

                for(int i = 0; i < spriteRenderers.Length; i++)
                {
                    spriteRenderers[i].sprite = sprites[i];
                }
            }
        }

        private Sprite[] GetSpritesInFolder(string folderPath)
        {
            string[] assetGuids = AssetDatabase.FindAssets("t:Sprite", new[] { folderPath });

            Sprite[] sprites = new Sprite[assetGuids.Length];

            for (int i = 0; i < assetGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[i]);
                sprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            }

            return sprites;
        }
    }
}
#endif