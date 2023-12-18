using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SFB;

public class RenderCamera : MonoBehaviour
{
    public Manager manager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SavePixelMap()
    {
        if (manager.selectedScreen != null)
        {
            string path = StandaloneFileBrowser.SaveFilePanel("Save File", "", "", "");
            //se è stata annullata l'azione di salvare un file, esci senza fare niente.
            if (path.Length == 0)
                return;
            SaveRenderTextureToFile(GetComponent<Camera>().targetTexture, path, SaveTextureFileFormat.PNG);
        }
    }

    public enum SaveTextureFileFormat
    {
        EXR, JPG, PNG, TGA
    };

    static public void SaveTexture2DToFile(Texture2D tex, string filePath, SaveTextureFileFormat fileFormat, int jpgQuality = 95)
    {
        switch (fileFormat)
        {
            case SaveTextureFileFormat.EXR:
                System.IO.File.WriteAllBytes(filePath + ".exr", tex.EncodeToEXR());
                break;
            case SaveTextureFileFormat.JPG:
                System.IO.File.WriteAllBytes(filePath + ".jpg", tex.EncodeToJPG(jpgQuality));
                break;
            case SaveTextureFileFormat.PNG:
                System.IO.File.WriteAllBytes(filePath + ".png", tex.EncodeToPNG());
                break;
            case SaveTextureFileFormat.TGA:
                System.IO.File.WriteAllBytes(filePath + ".tga", tex.EncodeToTGA());
                break;
        }
    }

    static public void SaveRenderTextureToFile(RenderTexture renderTexture, string filePath, SaveTextureFileFormat fileFormat = SaveTextureFileFormat.PNG, int jpgQuality = 95)
    {
        Texture2D tex;
        if (fileFormat != SaveTextureFileFormat.EXR)
            tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false, false);
        else
            tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBAFloat, false, true);
        var oldRt = RenderTexture.active;
        RenderTexture.active = renderTexture;
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();
        RenderTexture.active = oldRt;
        SaveTexture2DToFile(tex, filePath, fileFormat, jpgQuality);
        if (Application.isPlaying)
            Object.Destroy(tex);
        else
            Object.DestroyImmediate(tex);

    }

    // Update is called once per frame
    void Update()
    {
        if (manager.selectedScreen != null)
        {
            Screen screen = manager.selectedScreen.GetComponent<Screen>();
            float centerX = manager.selectedScreen.transform.position.x-.25f;
            float centerY = manager.selectedScreen.transform.position.y+.25f;
            centerX += screen.size.x / 2.0f;
            centerY -= screen.size.y / 2.0f;
            transform.position = new Vector3(centerX, centerY, -10);
            if (GetComponent<Camera>().targetTexture != null)
            {
                GetComponent<Camera>().targetTexture.Release();
            }
            GetComponent<Camera>().orthographicSize = .5f * screen.size.y;
            GetComponent<Camera>().targetTexture = new RenderTexture((int)screen.resolution.x, (int) screen.resolution.y,24);
        }
    }
}
