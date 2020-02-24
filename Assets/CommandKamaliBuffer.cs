using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CommandKamaliBuffer : CommandBuffer
{
    public void GetTemporaryRT(
        int nameID,
        int width,
        int height,
        int depthBuffer,
        FilterMode filter)
    {
        this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, RenderTextureFormat.Default);
    }
}
