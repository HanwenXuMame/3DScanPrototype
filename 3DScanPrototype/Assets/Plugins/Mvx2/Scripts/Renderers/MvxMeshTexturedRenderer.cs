﻿using System;
using UnityEngine;

namespace MVXUnity
{
    [AddComponentMenu("Mvx2/Data Processors/Mesh Textured Renderer")]
    public class MvxMeshTexturedRenderer : MvxMeshRenderer
    {
        #region data

        protected override void CreateMaterialInstances()
        {
            base.CreateMaterialInstances();
            if (materialInstances == null || materialInstances.Length == 0)
                return;

            foreach (Material materialInstance in materialInstances)
                materialInstance.SetTexture(TEXTURE_SHADER_PROPERTY_NAME, null);
        }

        #endregion

        #region process frame

        public static bool SupportsFrameRendering(Mvx2API.Frame frame)
        {
            return frame.StreamContainsDataLayer(Mvx2API.BasicDataLayersGuids.VERTEX_POSITIONS_DATA_LAYER, false)
                && frame.StreamContainsDataLayer(Mvx2API.BasicDataLayersGuids.VERTEX_INDICES_DATA_LAYER, false)
                && frame.StreamContainsDataLayer(Mvx2API.BasicDataLayersGuids.VERTEX_UVS_DATA_LAYER, false)
                && FrameContainsTexture(frame);
        }

        private static bool FrameContainsTexture(Mvx2API.Frame frame)
        {
            return frame.StreamContainsDataLayer(Mvx2API.BasicDataLayersGuids.ASTC_TEXTURE_DATA_LAYER, false)
                || frame.StreamContainsDataLayer(Mvx2API.BasicDataLayersGuids.DXT1_TEXTURE_DATA_LAYER, false)
                || frame.StreamContainsDataLayer(Mvx2API.BasicDataLayersGuids.ETC2_TEXTURE_DATA_LAYER, false)
                || frame.StreamContainsDataLayer(Mvx2API.BasicDataLayersGuids.NVX_TEXTURE_DATA_LAYER, false)
                || frame.StreamContainsDataLayer(Mvx2API.BasicDataLayersGuids.NV12_TEXTURE_DATA_LAYER, false)
                || frame.StreamContainsDataLayer(Mvx2API.BasicDataLayersGuids.NV21_TEXTURE_DATA_LAYER, false)
                || frame.StreamContainsDataLayer(Mvx2API.BasicDataLayersGuids.RGB_TEXTURE_DATA_LAYER, false);
        }

        protected override bool CanProcessFrame(Mvx2API.Frame frame)
        {
            return SupportsFrameRendering(frame);
        }

        protected override void ProcessNextFrame(MVCommon.SharedRef<Mvx2API.Frame> frame)
        {
            base.ProcessNextFrame(frame);
            CollectTextureDataFromFrame(frame.sharedObj);
        }

        protected override bool IgnoreColors()
        {
            return true;
        }

        #endregion

        #region texture

        private static readonly string TEXTURE_SHADER_PROPERTY_NAME = "_MainTex";
        private static readonly string TEXTURE_TYPE_SHADER_PROPERTY_NAME = "_TextureType";
        private static readonly string TEXTURE_WIDTH_PROPERTY_NAME = "_TextureWidth";
        private static readonly string TEXTURE_HEIGHT_PROPERTY_NAME = "_TextureHeight";
        private static readonly string USE_NORMALS_PROPERTY_NAME = "_UseNormals";

        private static readonly int TEXTURE_SHADER_PROPERTY_ID = Shader.PropertyToID(TEXTURE_SHADER_PROPERTY_NAME);
        private static readonly int TEXTURE_TYPE_SHADER_PROPERTY_ID = Shader.PropertyToID(TEXTURE_TYPE_SHADER_PROPERTY_NAME);
        private static readonly int TEXTURE_WIDTH_PROPERTY_ID = Shader.PropertyToID(TEXTURE_WIDTH_PROPERTY_NAME);
        private static readonly int TEXTURE_HEIGHT_PROPERTY_ID = Shader.PropertyToID(TEXTURE_HEIGHT_PROPERTY_NAME);
        private static readonly int USE_NORMALS_PROPERTY_ID = Shader.PropertyToID(USE_NORMALS_PROPERTY_NAME);

        private enum TextureTypeCodes
        {
            TTC_ASTC = 4,
            TTC_DXT1 = 3,
            TTC_ETC2 = 2,
            TTC_NVX = 0,
            TTC_RGB = 1,
            TTC_NV12 = 5,
            TTC_NV21 = 6
        };

        // an array of textures - they are switched between updates to improve performance -> textures double-buffering
        private Texture2D[] m_textures = new Texture2D[2];
        private int m_activeTextureIndex = -1;

        private void CollectTextureDataFromFrame(Mvx2API.Frame frame)
        {
            if (materialInstances == null || materialInstances.Length == 0)
                return;

            int textureType;
            TextureFormat textureFormat;
            FilterMode filterMode;
            Mvx2API.FrameTextureExtractor.TextureType mvxTextureType;

            if (frame.StreamContainsDataLayer(Mvx2API.BasicDataLayersGuids.ASTC_TEXTURE_DATA_LAYER, false))
            {
                textureType = (int) TextureTypeCodes.TTC_ASTC;
                textureFormat = TextureFormat.ASTC_8x8;
                filterMode = FilterMode.Bilinear;
                mvxTextureType = Mvx2API.FrameTextureExtractor.TextureType.TT_ASTC;
            }
            else if (frame.StreamContainsDataLayer(Mvx2API.BasicDataLayersGuids.DXT1_TEXTURE_DATA_LAYER, false))
            {
                textureType = (int) TextureTypeCodes.TTC_DXT1;
                textureFormat = TextureFormat.DXT1;
                filterMode = FilterMode.Bilinear;
                mvxTextureType = Mvx2API.FrameTextureExtractor.TextureType.TT_DXT1;
            }
            else if (frame.StreamContainsDataLayer(Mvx2API.BasicDataLayersGuids.ETC2_TEXTURE_DATA_LAYER, false))
            {
                textureType = (int) TextureTypeCodes.TTC_ETC2;
                textureFormat = TextureFormat.ETC2_RGB;
                filterMode = FilterMode.Bilinear;
                mvxTextureType = Mvx2API.FrameTextureExtractor.TextureType.TT_ETC2;
            }
            else if (frame.StreamContainsDataLayer(Mvx2API.BasicDataLayersGuids.NVX_TEXTURE_DATA_LAYER, false))
            {
                textureType = (int) TextureTypeCodes.TTC_NVX;
                textureFormat = TextureFormat.Alpha8;
                filterMode = FilterMode.Bilinear;
                mvxTextureType = Mvx2API.FrameTextureExtractor.TextureType.TT_NVX;
            }
            else if (frame.StreamContainsDataLayer(Mvx2API.BasicDataLayersGuids.NV12_TEXTURE_DATA_LAYER, false))
            {
                textureType = (int) TextureTypeCodes.TTC_NV12;
                textureFormat = TextureFormat.Alpha8;
                filterMode = FilterMode.Point;
                mvxTextureType = Mvx2API.FrameTextureExtractor.TextureType.TT_NV12;
            }
            else if (frame.StreamContainsDataLayer(Mvx2API.BasicDataLayersGuids.NV21_TEXTURE_DATA_LAYER, false))
            {
                textureType = (int) TextureTypeCodes.TTC_NV21;
                textureFormat = TextureFormat.Alpha8;
                filterMode = FilterMode.Point;
                mvxTextureType = Mvx2API.FrameTextureExtractor.TextureType.TT_NV21;
            }
            else if (frame.StreamContainsDataLayer(Mvx2API.BasicDataLayersGuids.RGB_TEXTURE_DATA_LAYER, false))
            {
                textureType = (int) TextureTypeCodes.TTC_RGB;
                textureFormat = TextureFormat.RGB24;
                filterMode = FilterMode.Bilinear;
                mvxTextureType = Mvx2API.FrameTextureExtractor.TextureType.TT_RGB;
            }
            else
            {
                foreach (Material materialInstance in materialInstances)
                    materialInstance.SetTexture(TEXTURE_SHADER_PROPERTY_ID, null);
                return;
            }

            bool hasNormals = frame.StreamContainsDataLayer(Mvx2API.BasicDataLayersGuids.VERTEX_NORMALS_DATA_LAYER, false);            

            ushort textureWidth, textureHeight;
            Mvx2API.FrameTextureExtractor.GetTextureResolution(frame, mvxTextureType, out textureWidth, out textureHeight);
            UInt32 textureSizeInBytes = Mvx2API.FrameTextureExtractor.GetTextureDataSizeInBytes(frame, mvxTextureType);
            IntPtr textureData = Mvx2API.FrameTextureExtractor.GetTextureData(frame, mvxTextureType);

            m_activeTextureIndex = (m_activeTextureIndex + 1) % m_textures.Length;
            Texture2D newActiveTexture = m_textures[m_activeTextureIndex];
            EnsureTextureProperties(ref newActiveTexture, textureFormat, filterMode, textureWidth, textureHeight);
            m_textures[m_activeTextureIndex] = newActiveTexture;

            newActiveTexture.LoadRawTextureData(textureData, (Int32)textureSizeInBytes);
            newActiveTexture.Apply(false, false);

            foreach (Material materialInstance in materialInstances)
            {
                materialInstance.SetInt(TEXTURE_TYPE_SHADER_PROPERTY_ID, textureType);
                materialInstance.SetTexture(TEXTURE_SHADER_PROPERTY_ID, newActiveTexture);
                materialInstance.SetInt(TEXTURE_WIDTH_PROPERTY_ID, textureWidth);
                materialInstance.SetInt(TEXTURE_HEIGHT_PROPERTY_ID, textureHeight);

                materialInstance.SetInt(USE_NORMALS_PROPERTY_ID, hasNormals ? 1 : 0);
            }
        }

        private void EnsureTextureProperties(ref Texture2D texture, TextureFormat targetFormat, FilterMode filterMode, ushort targetWidth, ushort targetHeight)
        {
            if (texture == null
                || texture.format != targetFormat || texture.filterMode != filterMode
                || texture.width != targetWidth || texture.height != targetHeight)
            {
                texture = new Texture2D(targetWidth, targetHeight, targetFormat, false);
                texture.filterMode = filterMode;
            }
        }

        #endregion

        #region MonoBehaviour

        public override void Reset()
        {
            base.Reset();
#if UNITY_EDITOR
            Material defaultMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Plugins/Mvx2/Materials/MeshTextured.mat");
            if (defaultMaterial != null)
                materialTemplates = new Material[] { defaultMaterial };
#endif
        }

        #endregion
    }
}