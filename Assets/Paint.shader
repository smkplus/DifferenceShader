Shader "Unlit/SimpleInkPaint"
{
	Properties{
		_MainTex("MainTex", 2D) = "white" 
		_SecondTex("SecondTex",2D) = "white"
		_Brush("Brush", 2D) = "white"
		_BrushScale("BrushScale", FLOAT) = 0.1
		_ControlColor("ControlColor", VECTOR) = (0, 0, 0, 0)
		_PaintUV("Hit UV Position", VECTOR) = (0, 0, 0, 0) 
		_Save("Save",Int) = 1
	}

	SubShader{
			CGINCLUDE
				struct app_data {
					float4 vertex:POSITION;
					float4 uv:TEXCOORD0;
				};

				struct v2f {
					float4 pos:SV_POSITION;
					float4 uv:TEXCOORD0;
				};

				sampler2D _MainTex;
				sampler2D _SecondTex;
				sampler2D _Brush;
				float4 _PaintUV;
				float _BrushScale;
				float4 _ControlColor;
			ENDCG

			Pass{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				bool IsPaintRange(float2 mainUV, float2 paintUV, float brushScale) {
					return
						paintUV.x - brushScale < mainUV.x &&
						mainUV.x < paintUV.x + brushScale &&
						paintUV.y - brushScale < mainUV.y &&
						mainUV.y < paintUV.y + brushScale;
				}

				v2f vert(app_data i) {
					v2f o;
					o.pos = UnityObjectToClipPos(i.vertex);
					o.uv = i.uv;
					return o;
				}

				float4 frag(v2f i) : SV_TARGET{
					float h = _BrushScale;
					float4 mainColor = tex2D(_MainTex, i.uv.xy);
					float4 secondColor = tex2D(_SecondTex, i.uv.xy);
					float4 brushColor = float4(1, 1, 1, 1);
					
          
					if (IsPaintRange(i.uv, _PaintUV, h)) {
							float2 uv = (i.uv - _PaintUV) / h * 0.5 + 0.5;
							brushColor = tex2D(_Brush, uv.xy);
                            return abs(mainColor - (brushColor.a*_ControlColor));
					}
					return  abs(mainColor-secondColor);
				}
				ENDCG
			}
		}
}