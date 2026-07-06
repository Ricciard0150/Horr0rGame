Shader "Hidden/ScreenRecorder/ScreenCaptureBlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off ZWrite Off ZTest Always
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Flip Y coordinate (replaces the Vector2(1, -1) scale from Graphics.Blit)
                float2 uv = float2(i.uv.x, 1.0 - i.uv.y);
                
                fixed4 col = tex2D(_MainTex, uv);
                
                // Color space correction for Linear Color Space projects
                // ScreenCapture.CaptureScreenshotAsTexture() returns gamma-encoded RGB data
                // In Linear projects, we need to convert Gamma->Linear to match the RenderTexture format
                // In Gamma projects, no conversion is needed (data is already in gamma space)
                
                #if !defined(UNITY_COLORSPACE_GAMMA)
                    col.rgb = GammaToLinearSpace(col.rgb);
                #endif
                
                return col;
            }
            ENDCG
        }
    }
}
