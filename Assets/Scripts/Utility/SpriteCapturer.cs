using UnityEngine;
using System.IO;

public class SpriteCapturer : MonoBehaviour
{
    public Camera cam;
    public int baseResolution = 512; // Square base
    public float padding = 1.1f;

    public GameObject objToCapture;

    [ContextMenu("Capture Image")]
    public void Capture()
    {
        // --- 1. Compute bounds ---
        Bounds b = CalculateBounds(objToCapture);

        float width = b.size.x;
        float height = b.size.y;

        // --- 2. Determine aspect ratio for texture ---
        float aspect = width / height;

        int texW = baseResolution;
        int texH = baseResolution;

        if (aspect > 1f)
        {
            // Wide item (example: horizontal axes, bows)
            texW = Mathf.RoundToInt(baseResolution * aspect);
        }
        else if (aspect < 1f)
        {
            // Tall item (example: spears, staves)
            texH = Mathf.RoundToInt(baseResolution / aspect);
        }

        // --- 3. Create RenderTexture dynamically ---
        RenderTexture rt = new RenderTexture(texW, texH, 24, RenderTextureFormat.Default);
        rt.useMipMap = false;
        rt.autoGenerateMips = false;
        cam.targetTexture = rt;

        // --- 4. Center camera ---
        cam.transform.position = new Vector3(b.center.x, b.center.y, cam.transform.position.z);

        // --- 5. Adjust orthographic size to fit bounding box ---
        float size = Mathf.Max(b.extents.x / cam.aspect, b.extents.y);
        cam.orthographicSize = size * padding;

        // --- 6. Render screenshot ---
        RenderTexture.active = rt;
        cam.Render();

        Texture2D tex = new Texture2D(texW, texH, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, texW, texH), 0, 0);
        tex.Apply();

        byte[] png = tex.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/Sprites/ItemSprites/" + objToCapture.name.ToString() + "_Sprite.png", png);

        RenderTexture.active = null;
        cam.targetTexture = null;

        Debug.Log("Saved: " + objToCapture.name.ToString() + "_Sprite.png" + " (" + texW + "x" + texH + ")");
    }

    private Bounds CalculateBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        Bounds b = renderers[0].bounds;
        foreach (var r in renderers)
            b.Encapsulate(r.bounds);
        return b;
    }
}

