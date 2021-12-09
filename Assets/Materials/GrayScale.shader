Shader "Custom/GrayScale"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _MinimumHue;
        float _MaximumHue;
        float _MinimumLuminosity;
        float _MaximumLuminosity;
        float _GrayScaleSensitivity;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        float Epsilon = 1e-10;
 
        float3 RGBtoHCV(in float3 RGB)
        {
            // Based on work by Sam Hocevar and Emil Persson
            float4 P = (RGB.g < RGB.b) ? float4(RGB.bg, -1.0, 2.0/3.0) : float4(RGB.gb, 0.0, -1.0/3.0);
            float4 Q = (RGB.r < P.x) ? float4(P.xyw, RGB.r) : float4(RGB.r, P.yzx);
            float C = Q.x - min(Q.w, Q.y);
            float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
            return float3(H, C, Q.x);
        }

        float3 RGBtoHSL(in float3 RGB)
        {
            float3 HCV = RGBtoHCV(RGB);
            float L = HCV.z - HCV.y * 0.5;
            float S = HCV.y / (1 - abs(L * 2 - 1) + Epsilon);
            return float3(HCV.x, S, L);
        }
        
        float3 RGBtoHSV(in float3 RGB)
        {
            float3 HCV = RGBtoHCV(RGB);
            float S = HCV.y / (HCV.z + Epsilon);
            return float3(HCV.x, S, HCV.z);
        }

        float GetColorValueFromInt(in fixed4 pixel, in int i) {
            if(i == 0) {
                return pixel.r;
            } else if(i == 1) {
                return pixel.g;
            } else {
                return pixel.b;
            }
        }

        bool IsGrayScale(in fixed4 pixel) {
            for(int i = 0; i < 3; i++) {
                for(int j = 0; j < 3; j++) {
                    if(i == j) break;
                    if(abs(GetColorValueFromInt(pixel, i) - GetColorValueFromInt(pixel, j)) > _GrayScaleSensitivity/255.0f) return false;
                }
            }
            return true;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

            float3 cHSV = RGBtoHSL(c.rgb);

            bool highlight = false;
            
            if((cHSV.z >= _MinimumLuminosity/100.0f && cHSV.z <= _MaximumLuminosity/100.0f) && cHSV.y > _GrayScaleSensitivity) {
                if(_MinimumHue > _MaximumHue) {
                    if(cHSV.x >= _MinimumHue/360.0f || cHSV.x <= _MaximumHue/360.0f) {
                        highlight = true;
                    }
                } else {
                    if(cHSV.x >= _MinimumHue/360.0f && cHSV.x <= _MaximumHue/360.0f) {
                        highlight = true;
                    }
                }
            }
            float brightness = 1.7f;
            if(highlight) {
                c.r = c.r*brightness;
                c.g = c.g*brightness;
                c.b = c.b*brightness;

                o.Albedo = c.rgb;
            } else {
                o.Albedo = (c.r + c.g + c.b)/8;
            }
            
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
