Shader "Custom/SphereDisplacement"
{
    Properties
    {
        _Color ("Tint", Color) = (1,1,1,1)
        _Strength ("Strength", Range(-2,2)) = 0
    }
    SubShader
    {

        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque"}
		ZWrite On Lighting Off Cull Off Fog { Mode Off } Blend One Zero

		GrabPass { "_GrabTexture" }

        Pass 
		{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

            sampler2D _GrabTexture;

            half _Strength;
            fixed4 _Color;

			struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata_base v) {
                v2f o;
                // use UnityObjectToClipPos from UnityCG.cginc to calculate 
                // the clip-space of the vertex
                o.pos = UnityObjectToClipPos(v.vertex);
                // use ComputeGrabScreenPos function from UnityCG.cginc
                // to get the correct texture coordinate
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float4 grabPos = ComputeGrabScreenPos(i.pos);
                //grabPos.xy = fixed2(0.5,0.5) + fixed2(pow(i.grabPos.x - 0.5, _Strength), pow(i.grabPos.y - 0.5, _Strength));
                half4 bgcolor = tex2Dproj(_GrabTexture, grabPos);
                return bgcolor;
            }

            ENDCG
        }
        
    }
}