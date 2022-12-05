Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _EmisionPower ("_Emision", Range(0, 1)) = 0.5
        _MainTex ("Correct Tex", 2D) = "white" {}
        _SecondTex ("Wrong Tex", 2D) = "white" {}
        _NormalMap ("NormalMap", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _MinUV ("Min Y", Range(0, 1)) = 0 
        _RoundPower("Round Power", Range(-1, 1)) = 0.36
        _ClipAlphaOffset("_ClipAlphaOffset", Range(-1, 1)) = 0.0
        _TexBlendPower("_TexBlendPower", Range(0, 1)) = 0.0
        
        _ColorTex ("Render Texture", 2D) = "white" {}
        
        [Toggle(USE_VERTEX_COLOR)] _UseVertexColor("_UseVertexColor", Float) = 0
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 200
        ZWrite Off
        //CULL OFF
        //        Cull Off //  HERE IS WHERE YOU PUT CULL OFF
        

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma shader_feature USE_VERTEX_COLOR

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _SecondTex;
        sampler2D _NormalMap;
        sampler2D _ColorTex;

        struct Input
        {
            float4 color : COLOR;
            float2 uv_MainTex : TEXCOORD0;
            float2 uv_SecondTex : TEXCOORD1;
            float2 uv_Radius2 : TEXCOORD2;
        };

        half _Glossiness;
        half _Metallic;
        half _MinUV;
        fixed4 _Color;
        float _EmisionPower;
        float _RoundPower;
        float _ClipAlphaOffset;
        float _TexBlendPower;

        


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
            return fixed4(1, 1, 1, 1);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            clip(IN.uv_MainTex.y - (_MinUV + _ClipAlphaOffset));
            //float power = 10;
            //float c = IN.uv_MainTex.x - floor(IN.uv_MainTex.x * power) / power;

            // 0.52 => 5.2 => 5 / 10 => 0.5. 0.52 - 0.5 = 0.02

            //power / 1000 * 3 = 
            //clip(c - 0.03);
            float4 img1 = tex2D (_MainTex, IN.uv_MainTex.xy - float2(0, _MinUV));
            float4 img2 = tex2D (_SecondTex, IN.uv_SecondTex.xy - float2(0, _MinUV));
            float4 finalImg = lerp(img1, img2, 1 - _TexBlendPower);
#ifdef USE_VERTEX_COLOR
            o.Albedo = finalImg.rgb * IN.color;
#else
            o.Albedo = img1.rgb * _Color;
#endif
            
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Normal = tex2D (_NormalMap, IN.uv_MainTex.xy - float2(0, _MinUV));;
            o.Smoothness = _Glossiness;
            o.Alpha = img1.a;
            o.Emission = IN.color * _EmisionPower;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
