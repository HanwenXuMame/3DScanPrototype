Shader "Mvx2/PointCloud" {
    SubShader{
    Pass{
        LOD 200

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag

        struct VertexInput {
            float4 v : POSITION;
            float4 color: COLOR;
        };

        struct VertexOutput {
            float4 pos : SV_POSITION;
            float4 col : COLOR;
            float pointSize : PSIZE;
        };

        VertexOutput vert(VertexInput v) {

            VertexOutput o;
            o.pos = UnityObjectToClipPos(v.v);
            o.col = v.color;
            o.pointSize = 1;

            return o;
        }

        float4 frag(VertexOutput o) : COLOR{
            return o.col;
        }

        ENDCG
        }
    }

}
