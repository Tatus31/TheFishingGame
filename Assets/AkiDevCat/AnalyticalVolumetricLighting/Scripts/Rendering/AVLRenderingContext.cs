using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AkiDevCat.AVL.Rendering
{
    public class AVLRenderingContext : IRenderingContext
    {
        public ScriptableRenderer Renderer { get; internal set; }

        public AVLFeatureSettings Settings { get; internal set; }

#pragma warning disable CS0618 // Typ lub składowa jest przestarzała
        public RenderTargetIdentifier CameraColorTarget => Renderer.cameraColorTarget;
#pragma warning restore CS0618 // Typ lub składowa jest przestarzała

#pragma warning disable CS0618 // Typ lub składowa jest przestarzała
        public RenderTargetIdentifier CameraDepthTarget => Renderer.cameraDepthTarget;
#pragma warning restore CS0618 // Typ lub składowa jest przestarzała

        public GraphicsBuffer GlobalLightBuffer { get;  set; }
        
        public uint GlobalLightBufferSize { get; set; }
        
        public uint GlobalMaskBufferSize { get; set; }

        public GraphicsBuffer GlobalMaskBuffer { get; set; }
        
        public GraphicsBuffer MaskIndexBuffer { get; set; }
        
        public GraphicsBuffer MaskInverseIndexBuffer { get; set; }

        public GraphicsBuffer LightIndexBuffer { get; set; }
        
        public GraphicsBuffer LightClusterBuffer { get; set; }
        
        public List<LightData> GlobalLightList { get; set; }
        
        public List<MaskData> GlobalMaskList { get; set; }

        public AVLRenderingContext(AVLFeatureSettings settings)
        {
            Settings = settings;
        }
        
        public void Cleanup() // ToDo remove?
        {
            
        }
    }
}