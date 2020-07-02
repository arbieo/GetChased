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
		
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _Colour;

			sampler2D _DisplacementX;
			sampler2D _DisplacementY;
			float  _Magnitude;

			struct vin
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : POSITION;
			};

			// Vertex function 
			v2f vert (vin v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			// Fragment function
			half4 frag (v2f i) : COLOR
			{
				/*half4 displaceX = tex2D(_DisplacementX, i.uvgrab);
				half4 displaceY = tex2D(_DisplacementY, i.uvgrab);
				half diplx = (displaceX.a-0.5)*2*_Magnitude;
				half diply = (displaceY.a-0.5)*2*_Magnitude;

				i.uvgrab.xy += half2(diplx, diply);

				fixed4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
				return col;*/
				return fixed4(0,0,0,0);
			}
		
			ENDCG
		} 
	}
}