Shader "Mvx2/MeshTextured" {
  Properties{
    _MainTex("Main Texture", 2D) = "white" {}
  }

    SubShader{
    Pass{
      Cull Off

      CGPROGRAM
      #pragma target 2.0
      #pragma vertex vert
      #pragma fragment frag

      #include "UnityCG.cginc"

      uniform sampler2D _MainTex;
      float4 _MainTex_ST;

      #include "SampleMvxTexture.cginc"

      struct v2f {
        float4  pos : SV_POSITION;
        float2  uv : TEXCOORD0;
      };

      v2f vert(appdata_base v)
      {
        v2f o;

        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

        return o;
      }
      
      float4 frag(v2f i) : COLOR
      {
        return sampleMvxTex(_MainTex, i.uv);
      }
        ENDCG
    }

    // shadow caster rendering pass
    // using macros from UnityCG.cginc
    Pass
    {
        Tags {"LightMode"="ShadowCaster"}

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma multi_compile_shadowcaster
        #include "UnityCG.cginc"

        struct v2f { 
            V2F_SHADOW_CASTER;
        };

        v2f vert(appdata_base v)
        {
            v2f o;
            TRANSFER_SHADOW_CASTER(o)
            return o;
        }

        float4 frag(v2f i) : SV_Target
        {
            SHADOW_CASTER_FRAGMENT(i)
        }
        ENDCG
    }
  }

}
