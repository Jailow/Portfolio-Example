using UnityEngine;

namespace CaveMiner.Helpers
{
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField] private Color _textColor;
        [SerializeField] private int _fontSize;

        private float deltaTime = 0.0f;

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        private void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        private void OnGUI()
        {
            int w = Screen.width;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(30, 30, w, _fontSize);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = _fontSize;
            style.normal.textColor = _textColor;

            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(rect, text, style);
        }
#endif
    }
}