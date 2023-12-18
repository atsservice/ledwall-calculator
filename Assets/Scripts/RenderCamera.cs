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
        if (manager.selectedScreen != null)
        {
            DownloadRenderTexture(GetComponent<Camera>().targetTexture, "pixelmap", SaveTextureFileFormat.PNG);
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


        byte[] textureBytes = tex.EncodeToPNG();

        if (!WebGLFileSaver.IsSavingSupported())
        {
            Debug.Log("cannot save file");
        }
        else
        {
            WebGLFileSaver.SaveFile(textureBytes, filename,"image/png");
        }

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
