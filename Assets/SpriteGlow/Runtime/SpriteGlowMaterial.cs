using System.Collections.Generic;
using UnityEngine;

namespace SpriteGlow.Runtime
{
    public class SpriteGlowMaterial : Material
    {
        public Texture SpriteTexture
        {
            get { return mainTexture; }
            set { mainTexture = value; }
        }

        public bool DrawOutside
        {
            get { return IsKeywordEnabled(outsideMaterialKeyword); }
            set { throw new System.NotImplementedException(); }
        }

        public bool InstancingEnabled
        {
            get { return enableInstancing; }
            set { enableInstancing = value; }
        }

        private const string outlineShaderName = "Sprites/Outline";
        private const string outsideMaterialKeyword = "SPRITE_OUTLINE_OUTSIDE";

        private static readonly Shader outlineShader = Shader.Find(outlineShaderName);
        private static readonly List<SpriteGlowMaterial> sharedMaterials = new List<SpriteGlowMaterial>();

        public SpriteGlowMaterial (Texture spriteTexture, bool drawOutside = false, bool instancingEnabled = false)
            : base(outlineShader)
        {
            if (!outlineShader) Debug.LogError("`{outlineShaderName}` shader not found. Make sure the shader is included to the build.");

            mainTexture = spriteTexture;
            if (drawOutside) EnableKeyword(outsideMaterialKeyword);
            if (instancingEnabled) enableInstancing = true;
        }

        public static Material GetSharedFor (SpriteGlowEffect spriteGlow)
        {
            for (int i = 0; i < sharedMaterials.Count; i++)
            {
                if (sharedMaterials[i].SpriteTexture == spriteGlow.Renderer.sprite.texture &&
                    sharedMaterials[i].DrawOutside == spriteGlow.DrawOutside &&
                    sharedMaterials[i].InstancingEnabled == spriteGlow.EnableInstancing)
                    return sharedMaterials[i];
            }

            var material = new SpriteGlowMaterial(spriteGlow.Renderer.sprite.texture, spriteGlow.DrawOutside, spriteGlow.EnableInstancing);
            material.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor | HideFlags.NotEditable;
            sharedMaterials.Add(material);

            return material;
        }
    }
}
