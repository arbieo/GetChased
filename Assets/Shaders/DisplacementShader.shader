Shader "Custom/DisplacementShader" 
{
	Properties 
	{
		_DisplacementX ("Displacement Map X", 2D) = "alpha" {}
		_DisplacementY ("Displacement Map Y", 2D) = "alpha" {}
		_Magnitude ("Magnitude", Range(0,1)) = 0.05
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

			sampler2D _MainTex;
			fixed4 _Colour;

			sampler2D _DisplacementX;
			sampler2D _DisplacementY;
			float  _Magnitude;

			struct vin_vct
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f_vct
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;

				float4 uvgrab : TEXCOORD1;
			};

			// Vertex function 
			v2f_vct vert (vin_vct v)
			{
				v2f_vct o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;

				o.texcoord = v.texcoord;

				o.uvgrab = ComputeGrabScreenPos(o.vertex);
				return o;
			}

			// Fragment function
			half4 frag (v2f_vct i) : COLOR
			{
				half4 displaceX = tex2D(_DisplacementX, i.uvgrab);
				half4 displaceY = tex2D(_DisplacementY, i.uvgrab);
				half diplx = (displaceX.a-0.5)*2*_Magnitude;
				half diply = (displaceY.a-0.5)*2*_Magnitude;

				i.uvgrab.xy += half2(diplx, diply);

				fixed4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
				return col;
			}
		
			ENDCG
		} 
	}
}