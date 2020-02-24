using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

public class CustomGlowSystem
{
    public Texture2DArray Texture2DArray;
    static CustomGlowSystem m_Instance; // singleton
    static public CustomGlowSystem instance {
        get {
            if (m_Instance == null)
                m_Instance = new CustomGlowSystem();
            return m_Instance;
        }
    }

    internal HashSet<CustomGlowObj> m_GlowObjs = new HashSet<CustomGlowObj>();

    public void Add(CustomGlowObj o)
    {
        Remove(o);
        m_GlowObjs.Add(o);
        //Debug.Log("added effect " + o.gameObject.name);
    }

    public void Remove(CustomGlowObj o)
    {
        m_GlowObjs.Remove(o);
        //Debug.Log("removed effect " + o.gameObject.name);
    }
}

[ExecuteInEditMode]
public class CustomGlowRenderer : MonoBehaviour
{
    private CommandBuffer m_GlowBuffer;
    private Dictionary<Camera, CommandBuffer> m_Cameras = new Dictionary<Camera, CommandBuffer>();
    public Material Material;
    public Texture2D Texture2D;
    public RenderTexture RenderTexture;

    private void Cleanup()
    {
        foreach (var cam in m_Cameras)
        {
            if (cam.Key)
                cam.Key.RemoveCommandBuffer(CameraEvent.BeforeLighting, cam.Value);
        }

        m_Cameras.Clear();
    }

    public void OnDisable()
    {
        Cleanup();
    }

    public void OnEnable()
    {
        Cleanup();
    }

    public void OnWillRenderObject()
    {
        var render = gameObject.activeInHierarchy && enabled;
        if (!render)
        {
            Cleanup();
            return;
        }

        var cam = Camera.current;
        if (!cam)
            return;

        if (m_Cameras.ContainsKey(cam))
            return;

        // create new command buffer
        m_GlowBuffer = new CommandBuffer();
        m_GlowBuffer.name = "Glow map buffer";
        m_Cameras[cam] = m_GlowBuffer;

        var glowSystem = CustomGlowSystem.instance;

//        // create render texture for glow map
//        int tempID = Shader.PropertyToID("_Temp1");
//        m_GlowBuffer.GetTemporaryRT(tempID, -1, -1, 24, FilterMode.Bilinear);
//        m_GlowBuffer.SetRenderTarget(tempID);
//        m_GlowBuffer.ClearRenderTarget(true, true, Color.black); // clear before drawing to it each frame!!
//
//        // draw all glow objects to it
//        foreach(CustomGlowObj o in glowSystem.m_GlowObjs)
//        {
//            Renderer r = o.GetComponent<Renderer>();
//            Material glowMat = o.glowMaterial;
//            if(r && glowMat)
//                m_GlowBuffer.DrawRenderer(r, glowMat);
//        }
//
//        // set render texture as globally accessable 'glow map' texture
//        m_GlowBuffer.SetGlobalTexture("_GlowMap", tempID);


        // create render texture for glow map
        int tempID = Shader.PropertyToID("_Temp1");
        m_GlowBuffer.GetTemporaryRT(tempID, -1, -1, 24, FilterMode.Bilinear);
        m_GlowBuffer.SetRenderTarget(tempID);

        RenderTexture tempTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.Default);
        RenderTargetIdentifier identifier = tempTexture;
        List<Texture2D> t = new List<Texture2D>();
        foreach (CustomGlowObj o in glowSystem.m_GlowObjs)
        {
            m_GlowBuffer.ClearRenderTarget(true, true, Color.black);
            Renderer r = o.GetComponent<Renderer>();
            Material glowMat = o.glowMaterial;
            if (r && glowMat)
                m_GlowBuffer.DrawRenderer(r, glowMat);

            m_GlowBuffer.Blit(tempID, identifier);

//            Texture2D myTexture2D = new Texture2D(tempTexture.width, tempTexture.height);
//            RenderTexture.active = tempTexture;
//            myTexture2D.ReadPixels(new Rect(0, 0, tempTexture.width, tempTexture.height), 0, 0);
//            myTexture2D.Apply();

            Texture2D = toTexture2D(tempTexture);
            t.Add(Texture2D);
            RenderTexture = tempTexture;

//            m_GlowBuffer.SetGlobalTexture("_GlowMap", tempID);
        }

        // set texture2d array to shader



        // add this command buffer to the pipeline
        cam.AddCommandBuffer(CameraEvent.BeforeLighting, m_GlowBuffer);
        CreateTextureArray(t.ToArray());
    }

    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }
    private void CreateTextureArray(Texture2D[] ordinaryTextures)
    {
        // Create Texture2DArray
        Texture2DArray texture2DArray = new
            Texture2DArray(ordinaryTextures[0].width,
                ordinaryTextures[0].height, ordinaryTextures.Length,
                TextureFormat.RGBA32, true, false);
        // Apply settings
        texture2DArray.filterMode = FilterMode.Bilinear;
        texture2DArray.wrapMode = TextureWrapMode.Repeat;
        // Loop through ordinary textures and copy pixels to the
        // Texture2DArray
        for (int i = 0; i < ordinaryTextures.Length; i++)
        {
            texture2DArray.SetPixels(ordinaryTextures[i].GetPixels(0),
                i, 0);
        }

        // Apply our changes
        texture2DArray.Apply();
        // Set the texture to a material
        // Set the texture to a material
        Material.SetTexture("_MainTex", texture2DArray);
    }
}