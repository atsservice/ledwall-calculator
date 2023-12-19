using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SFB;
using System.Runtime.InteropServices;


public class RenderCamera : MonoBehaviour
{
    public Manager manager;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void SavePixelMap()
    {
        if (manager.screens.Count == 0)
        {
            return;
        }
        UpdateCamera();

        DownloadRenderTexture(GetComponent<Camera>().targetTexture, "pixelmap", SaveTextureFileFormat.PNG);

    }
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

    public enum SaveTextureFileFormat
    {
        EXR, JPG, PNG, TGA
    };

    static public void DownloadRenderTexture(RenderTexture renderTexture, string filename, SaveTextureFileFormat fileFormat = SaveTextureFileFormat.PNG, int jpgQuality = 95)
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


        if (Application.platform != RuntimePlatform.WebGLPlayer)
        {
            string path = StandaloneFileBrowser.SaveFilePanel("Save File", "", "", "png");
            SaveTexture2DToFile(tex, path, fileFormat, jpgQuality);
        }
        else if (!WebGLFileSaver.IsSavingSupported())
        {
            Debug.Log("cannot save file");
        }
        else
        {
            byte[] textureBytes = tex.EncodeToPNG();
            WebGLFileSaver.SaveFile(textureBytes, filename, "image/png");
        }

        if (Application.isPlaying)
            Object.Destroy(tex);
        else
            Object.DestroyImmediate(tex);

    }

    // Update is called once per frame
    void UpdateCamera()
    {
        Screen screen = manager.screens[0].GetComponent<Screen>();
        float centerX = screen.transform.position.x - screen.tileSize.x / 2.0f;
        float centerY = screen.transform.position.y + screen.tileSize.y / 2.0f;
        centerX += screen.size.x / 2.0f;
        centerY -= screen.size.y / 2.0f;
        transform.position = new Vector3(centerX, centerY, -10);
        if (GetComponent<Camera>().targetTexture != null)
        {
            GetComponent<Camera>().targetTexture.Release();
        }
        GetComponent<Camera>().orthographicSize = .5f * screen.size.y;
        GetComponent<Camera>().targetTexture = new RenderTexture((int)screen.resolution.x, (int)screen.resolution.y, 24);
    }

}
