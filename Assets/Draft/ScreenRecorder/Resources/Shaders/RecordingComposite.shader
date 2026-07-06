Shader "Hidden/ScreenRecorder/Composite"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        [NoScaleOffset] _OverlayTex ("Overlay", 2D) = "clear" {}
        _OverlayRect ("Overlay Rect (x,y,w,h)", Vector) = (0, 0, 1, 1)
        _OverlayOpacity ("Overlay Opacity", Range(0,1)) = 1.0
        _OverlayEnabled ("Overlay Enabled", Float) = 0
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
            sampler2D _OverlayTex;
            float4 _OverlayRect;
            float _OverlayOpacity;
            float _OverlayEnabled;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Sample main texture (game view)
                fixed4 color = tex2D(_MainTex, i.uv);
                
                // Apply overlay if enabled
                if (_OverlayEnabled > 0.5)
                {
                    // Calculate overlay UV based on rect (position & size)
                    float2 overlayPos = _OverlayRect.xy;
                    float2 overlaySize = _OverlayRect.zw;
                    float2 overlayUV = (i.uv - overlayPos) / overlaySize;
                    // _MainTex UV space has y=0 at video top (flipped by pre-blit).
                    // _OverlayTex is a Texture2D with y=0 at bottom, so flip to match.
                    overlayUV.y = 1.0 - overlayUV.y;
                    
                    // Check if within overlay bounds
                    if (overlayUV.x >= 0 && overlayUV.x <= 1 && overlayUV.y >= 0 && overlayUV.y <= 1)
                    {
                        fixed4 overlay = tex2D(_OverlayTex, overlayUV);
                        float alpha = overlay.a * _OverlayOpacity;
                        color.rgb = color.rgb * (1.0 - alpha) + overlay.rgb * alpha;
                    }
                }
                
                return color;
            }
            ENDCG
        }
    }
}
