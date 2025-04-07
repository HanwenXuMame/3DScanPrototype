Shader "Custom/CombinedShader"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _LumosityCullThreshold("Luminosity Cull Threshold", Range(0,256)) = 1
        _MatBrightnessBoost("Brightness Adjust", Range(0,5)) = 1
        _MatShadowValue("Shadow Opacity", Range(-1, 1)) = 0
        _Id("Id", Range(0,256)) = 0
        _CullMode("Cull Mode", Float) = 2
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Overlay" }
        Pass
        {
            Cull [_CullMode]
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 5.0
            #include "UnityCG.cginc"
            #include "SampleMvxTexture.cginc" // Include for sampleMvxTex

            // Struct definitions
            struct POLTEX
            {
                float3 vertex;
                float u;    // Depth
                float v;    // Depth index
                uint col;   // Current rendering frame number
            };

            struct DEPTH
            {
                float depth_value;      // Depth value at this point
                int pol_index;          // Index value to the poltex
                int depth_frame;        // Current rendering frame No
            };

            // Variables from properties
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _LumosityCullThreshold;
            float _MatBrightnessBoost;
            float _MatShadowValue;
            float _Id;
            float _CullMode = 1;

            // Variables from C# script
            uniform RWStructuredBuffer<POLTEX> _RefinedPoltexData : register(u1);
            uniform RWStructuredBuffer<DEPTH> _DepthData : register(u2);
            uniform RWStructuredBuffer<int> _LastPoltexIndex : register(u3);
            uniform RWStructuredBuffer<POLTEX> _AllPoltexData : register(u4);

            float4x4 _VxCamera : register(u5);
            uint _Resolution : register(u6);

            float3 _CamPos : register(u7);
            float3 _ViewAspectRatio : register(u8);
            float3 _ViewOffset : register(u9);
            int _ViewOccluding : register(u10);
            int _ViewDepthPostProcessMode : register(u11);
            float _ViewClipRadius : register(u12);
            float _GlobalShadowValue : register(u13);
            int _PoltexHeadRoom : register(u14);
            float _GlobalBrightnessValue : register(u15);
            float _DepthThreshold : register(u16);
            float _CamPosThreshold : register(u17);
            int _CaptureAllData : register(u19);

            struct vertIn
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float3 normal : NORMAL; // Add normal input from the mesh
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 screenPos : TEXCOORD3;
                float3 normal : NORMAL; // Add normal to pass to the fragment shader
            };

            // Vertex function
            v2f vert(vertIn v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); // Use texture mapping logic from MeshTextured
                o.color = v.color * _Color;
                o.screenPos = mul(_VxCamera, mul(unity_ObjectToWorld, v.vertex));
                o.normal = v.normal;
                return o;
            }

            // Fragment function
            float4 frag(v2f i) : SV_Target
            {
                // Use the texture sampling logic from MeshTextured
                return sampleMvxTex(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}