using System.Drawing;
using UnityEngine;
using TMPro;

public class RainbowText : MonoBehaviour
{
    [Tooltip("How fast the rainbow scrolls across the text (higher = faster)")]
    public float scrollSpeed = 1f;

    [Tooltip("How spread out the rainbow colors are across the text (higher = more colors visible at once)")]
    public float colorSpread = 0.01f;

    private TMP_Text textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        textMesh.ForceMeshUpdate(); // Ensure the text mesh is up to date

        TMP_TextInfo textInfo = textMesh.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            // Skip invisible characters (like spaces)
            if (!charInfo.isVisible) continue;

            // Use the character's horiztonal position to offset the hue, then shift everything over time using scrollSpeed
            float hue = Mathf.Repeat((charInfo.origin * colorSpread) - (Time.unscaledTime * scrollSpeed), 1f);
            UnityEngine.Color color = UnityEngine.Color.HSVToRGB(hue, 1f, 1f);

            // Each character has 4 vertices (corners of the quad)
            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Color32[] vertexColors = textInfo.meshInfo[meshIndex].colors32;
            vertexColors[vertexIndex + 0] = color;
            vertexColors[vertexIndex + 1] = color;
            vertexColors[vertexIndex + 2] = color;
            vertexColors[vertexIndex + 3] = color;
        }

        // Apply the color changes to the actual mesh
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.colors32 = textInfo.meshInfo[i].colors32;
            textMesh.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
    }
}
