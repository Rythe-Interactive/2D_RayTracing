using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomRenderPipeline : RenderPipeline
{
    CameraRenderer camRenderer = new CameraRenderer();

    public CustomRenderPipeline()
    {

    }

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        for(int i = 0; i < cameras.Length; ++i)
        {
            camRenderer.Render(context, cameras[i]);
        }
    }
}
