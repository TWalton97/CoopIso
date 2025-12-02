using UnityEngine;
using System.IO;

public class SpriteCapturer : MonoBehaviour
{
    public Camera cam;
    public int resolution = 512;
    public float padding = 1f;
    public GameObject objToCapture;

    [Range(0f, 1f)]
    public float alphaCutoff = 0.01f;    // Pixels with alpha <= this are trimmed

    [ContextMenu("Capture Image")]
    public void Capture()
    {
        // --- Compute bounds ---
        Bounds b = CalculateBounds(objToCapture);

        RenderTexture rt = new RenderTexture(resolution, resolution, 24, RenderTextureFormat.ARGB32);
        cam.targetTexture = rt;

        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0);

        Vector3 camPos = cam.transform.position;
        cam.transform.position = new Vector3(b.center.x, b.center.y, camPos.z);

        float objectHalfSize = Mathf.Max(b.extents.x, b.extents.y);
        cam.orthographicSize = objectHalfSize * padding;

        RenderTexture.active = rt;
        cam.Render();

        // Read pixels
        Texture2D tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
        tex.Apply();

        // ---- TRIM THE BORDER ----
        Texture2D trimmed = TrimTexture(tex, alphaCutoff);

        // Save
        string folder = Application.dataPath + "/Sprites/ItemSprites/";
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string path = folder + objToCapture.name + "_Sprite.png";
        File.WriteAllBytes(path, trimmed.EncodeToPNG());

        RenderTexture.active = null;
        cam.targetTexture = null;

        Debug.Log("Saved TRIMMED image to: " + path);
    }

    private Bounds CalculateBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        Bounds b = renderers[0].bounds;

        foreach (var r in renderers)
            b.Encapsulate(r.bounds);

        return b;
    }

    private Texture2D TrimTexture(Texture2D source, float alphaThreshold)
    {
        int w = source.width;
        int h = source.height;

        Color32[] pixels = source.GetPixels32();

        int minX = w, maxX = 0, minY = h, maxY = 0;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                Color32 c = pixels[y * w + x];
                if (c.a > alphaThreshold * 255)
                {
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
        }

        int newW = maxX - minX + 1;
        int newH = maxY - minY + 1;

        Texture2D trimmed = new Texture2D(newW, newH, TextureFormat.RGBA32, false);

        for (int y = 0; y < newH; y++)
        {
            for (int x = 0; x < newW; x++)
            {
                trimmed.SetPixel(x, y, source.GetPixel(minX + x, minY + y));
            }
        }

        trimmed.Apply();
        return trimmed;
    }
}

