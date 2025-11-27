using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace Helpers
{
    public class FpsCounter : MonoBehaviour
    {
        [BoxGroup("Position")][SerializeField] private float positionX;

        [BoxGroup("Position")][SerializeField] private float positionY;

        [BoxGroup("Size")][SerializeField] private int FontSize;

        private float fps;
        private float avgFps;

        private int sampleCount;

        private IEnumerator Start()
        {
            GUI.depth = 2;
            while (true)
            {
                fps = 1f / Time.unscaledDeltaTime;

                ++sampleCount;
                avgFps += (fps - avgFps) / sampleCount;

                yield return new WaitForSeconds(0.1f);
            }
        }

        private void OnGUI()
        {
            var textStyle = new GUIStyle { fontSize = FontSize, fontStyle = FontStyle.Bold };
            var position = new Rect(positionX, positionY, 100, 25);
            var text = $"FPS: {Mathf.Round(fps)} \nAVG: {Mathf.Round(avgFps)}";

            GUI.Label(position, text, textStyle);
        }
    }
}
