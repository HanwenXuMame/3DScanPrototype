// Upgrade NOTE: replaced '_LightMatrix0' with 'unity_WorldToLight'

Shader "Mvx2/MeshTexturedLit" {
  Properties{
    _MainTex("Main Texture", 2D) = "white" {}

    //[ToggleOff] _UseNormals("Use Normals", Float) = 1.0

    _ShadowStrength("Shadow Strength", Range(0, 1)) = 1.0
    _AmbientStrength("Shadow Ambient Strength", Range(0, 1)) = 1.0
    _DiffuseStrength("Diffuse Strength", Range(0, 1)) = 1.0
  }

    SubShader {
    Pass {
      Tags { "LightMode" = "ForwardBase" } // pass for ambient light and first directional light source without cookie

      Cull Off

      CGPROGRAM
      #pragma target 3.0
      #pragma vertex vert
      #pragma fragment frag
      #pragma multi_compile_fwdbase

      #include "UnityCG.cginc"
      #include "AutoLight.cginc"

      uniform float4 _LightColor0; //From UnityCG

      uniform sampler2D _MainTex;
      float4 _MainTex_ST;

      float _DiffuseStrength;
      float _ShadowStrength;
      float _AmbientStrength;

      int _UseNormals;

      #include "SampleMvxTexture.cginc"

      struct v2f {
        float4 pos : SV_POSITION;
        float3 normal : NORMAL;
        float2 uv : TEXCOORD0;
        float4 posWorld : TEXCOORD1;
        
        LIGHTING_COORDS(2, 3)
      };

      v2f vert(appdata_base v)
      {
        v2f o;

        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

        o.normal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz); //Calculate the normal
        o.posWorld = mul(unity_ObjectToWorld, v.vertex); //Calculate the world position for our point
        
        TRANSFER_SHADOW(o)
        return o;
      }
      
      float4 frag(v2f i) : COLOR
      {
        //Sample color texture
        float4 clr = sampleMvxTex(_MainTex, i.uv);


        //https://docs.unity3d.com/Manual/SL-VertexFragmentShaderExamples.html
        //https://en.wikibooks.org/wiki/Cg_Programming/Unity/Cookies
        //https://catlikecoding.com/unity/tutorials/rendering/part-5/

        float3 normalDirection = normalize(i.normal);
        float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

        float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.rgb; //Ambient component
        float3 diffuseReflection = 1;

        if (_UseNormals == 1) {
            diffuseReflection = _LightColor0.rgb * max(0.0, dot(normalDirection, lightDirection)); //Diffuse component
            ambientLighting = ShadeSH9(half4(normalDirection, 1));
        }
        else {
            diffuseReflection = _LightColor0.rgb;
            ambientLighting = ShadeSH9(half4(0, 1, 0, 1));
        }
        
        //shadow distance fade
        float viewZ = dot(_WorldSpaceCameraPos - i.posWorld, UNITY_MATRIX_V[2].xyz);
        float shadowFadeDistance = UnityComputeShadowFadeDistance(i.posWorld, viewZ);
        float shadowFade = UnityComputeShadowFade(shadowFadeDistance);
        float shadowAttenuation = saturate(SHADOW_ATTENUATION(i) + shadowFade);

        float3 diffuseShadow;

        if (_DiffuseStrength != 0) {
            //diffuseShadow = lerp(_LightColor0.rgb, i.diff, _DiffuseStrength);
            diffuseShadow = diffuseReflection / _DiffuseStrength;
        }
        else {
            diffuseShadow = _LightColor0.rgb;
        }

        fixed3 lighting = (ambientLighting * _AmbientStrength + lerp(_LightColor0.rgb, shadowAttenuation * diffuseShadow, _ShadowStrength));

        return float4(lighting * clr.xyz, 1);
      }
        ENDCG
    }

    Pass {
      Tags { "LightMode" = "ForwardAdd" } // pass for additional light sources

      Blend One One // additive blending 
      Fog { Color(0,0,0,0) } // in additive pass fog should be black

      CGPROGRAM
      #pragma target 3.0
      #pragma vertex vert
      #pragma fragment frag
      #pragma multi_compile_lightpass

      #include "UnityCG.cginc"
      #include "AutoLight.cginc"

      uniform float4 _LightColor0; // color of light source (from "Lighting.cginc")
#if defined (DIRECTIONAL_COOKIE) || defined (SPOT)
      //uniform sampler2D _LightTexture0; // cookie alpha texture map (from Autolight.cginc)
#elif defined (POINT_COOKIE)
      //uniform samplerCUBE _LightTexture0; // cookie alpha texture map (from Autolight.cginc)
#endif

      float _DiffuseStrength;
      float _ShadowStrength;
      float _AmbientStrength;

      int _UseNormals;

      struct v2f 
      {
        float4 pos : SV_POSITION;
        float3 normal : NORMAL;
        float4 posWorld : TEXCOORD0;
        float4 posLight : TEXCOORD1;
        
        LIGHTING_COORDS(2, 3)
      };

      v2f vert(appdata_base v)
      {
        v2f o;

        o.pos = UnityObjectToClipPos(v.vertex);
        o.normal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
        o.posWorld = mul(unity_ObjectToWorld, v.vertex); //Calculate the world position for our point
#if defined(DIRECTIONAL)
        o.posLight = 1;
#else
        o.posLight = mul(unity_WorldToLight, o.posWorld);
#endif

        TRANSFER_SHADOW(o)

        return o;
      }

      float4 frag(v2f i) : COLOR
      {
        float3 normalDirection = normalize(i.normal);

        float3 viewDirection = normalize(_WorldSpaceCameraPos - i.posWorld.xyz);
        float3 lightDirection;
        float diffuseAttenuation = 1.0; // by default no attenuation with distance

#if defined (DIRECTIONAL) || defined (DIRECTIONAL_COOKIE)
        lightDirection = normalize(_WorldSpaceLightPos0.xyz);
#elif defined (POINT_NOATT)
        lightDirection = normalize(
            _WorldSpaceLightPos0 - i.posWorld.xyz);
#elif defined(POINT)||defined(POINT_COOKIE)||defined(SPOT)
        float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
        float distance = length(vertexToLightSource);
        diffuseAttenuation = 1.0 / distance; // linear attenuation 
        lightDirection = normalize(vertexToLightSource);
#endif


        //Lighting starts here
        
        float3 diffuseReflection = 1;

        if (_UseNormals == 1) {
            diffuseReflection = _LightColor0.rgb * max(0.0, dot(normalDirection, lightDirection)); //Diffuse component
        }
        else {
            diffuseReflection = _LightColor0.rgb;
        }

        //shadow distance fade
        float viewZ = dot(_WorldSpaceCameraPos - i.posWorld, UNITY_MATRIX_V[2].xyz);
        float shadowFadeDistance = UnityComputeShadowFadeDistance(i.posWorld, viewZ);
        float shadowFade = UnityComputeShadowFade(shadowFadeDistance);
        float shadowAttenuation = saturate(SHADOW_ATTENUATION(i) + shadowFade);

        float3 diffuseShadow;

        if (_DiffuseStrength != 0) {
            //diffuseShadow = lerp(_LightColor0.rgb, i.diff, _DiffuseStrength);
            diffuseShadow = diffuseReflection / _DiffuseStrength;
        }
        else {
            diffuseShadow = _LightColor0.rgb;
        }

        fixed3 lighting = (lerp(_LightColor0.rgb, shadowAttenuation * diffuseShadow, _ShadowStrength));

        //Lighting ends here

        UNITY_LIGHT_ATTENUATION(cookieAttenuation, 0, i.posWorld.xyz);

        return float4(cookieAttenuation * diffuseAttenuation * lighting, 1.0);
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
