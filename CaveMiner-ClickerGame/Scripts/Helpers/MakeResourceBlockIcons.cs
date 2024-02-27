using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

#if UNITY_EDITOR
namespace CaveMiner.Editor
{
    public class MakeResourceBlockIcons : EditorWindow
    {
        private static Vector2 _windowMinSize = new Vector2 { x = 375, y = 200 };
        private static BlockData _blockData;

        [MenuItem("Helpers/MakeResourceBlockIcons", priority = 1)]
        private static void CommonChestCheck()
        {
            var gameData = Resources.Load<GameData>("Resources/");

            var window = EditorWindow.GetWindow(typeof(MakeResourceBlockIcons));
            window.minSize = _windowMinSize;
            window.maxSize = _windowMinSize;
            window.titleContent = new GUIContent
            {
                text = "Block Icons Maker",
            };
        }

        private void OnGUI()
        {
            EditorGUIUtility.labelWidth = 75;

            EditorGUILayout.BeginVertical();

            Rect rect = new Rect(0, 2.5f, _windowMinSize.x, 25);
            _blockData = (BlockData)EditorGUI.ObjectField(rect, "BlockData", _blockData, typeof(BlockData), false);
            rect.y += _windowMinSize.y - 25;
            if(GUI.Button(rect, "Make Icon") && _blockData != null)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                var meshRenderer = cube.GetComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = new Material(_blockData.BreakBlockParticleMaterial);
                meshRenderer.sharedMaterial.shader = Shader.Find("Unlit/Texture");

                Camera camera = new GameObject().AddComponent<Camera>();
                camera.gameObject.AddComponent<CanvasRenderer>();
                camera.clearFlags = CameraClearFlags.Nothing;
                camera.orthographic = true;
                camera.orthographicSize = 0.85f;
                camera.transform.position = new Vector3(5f, 4.95f, 5f);
                camera.transform.localEulerAngles = new Vector3(35f, -135f, 0f);

                Rect cameraRect = new Rect(0, 0, 256, 256);
                RenderTexture renderTexture = new RenderTexture(256, 256, GraphicsFormat.R8G8B8A8_UNorm, GraphicsFormat.D32_SFloat_S8_UInt);
                Texture2D screenShot = new Texture2D(256, 256, TextureFormat.ARGB32, false);
                screenShot.alphaIsTransparency = true;
                camera.targetTexture = renderTexture;
                camera.Render();
                RenderTexture.active = renderTexture;

                screenShot.ReadPixels(cameraRect, 0, 0);
                screenShot.Apply();
                camera.targetTexture = null;
                RenderTexture.active = null;

                DestroyImmediate(renderTexture);

                int count = 0;
                while (true)
                {
                    if (!File.Exists($"Assets/Resources/Screenshots/{_blockData.name}_{count}.png"))
                    {
                        File.WriteAllBytes($"Assets/Resources/Screenshots/{_blockData.name}_{count}.png", screenShot.EncodeToPNG());
                        break;
                    }

                    count++;
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                DestroyImmediate(camera.gameObject);
                DestroyImmediate(cube.gameObject);
            }

            EditorGUILayout.EndVertical();
        }
    }
}
#endif