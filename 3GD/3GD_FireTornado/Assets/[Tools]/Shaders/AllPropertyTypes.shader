Shader "Custom/AllPropertyTypes"
{
	Properties
	{
        // visit https://docs.unity3d.com/Manual/SL-Properties.html   https://docs.unity3d.com/ScriptReference/MaterialPropertyDrawer.html
        
        // basic property :
        
		_Tex0 ("Texture0", 2D) = "white" {}
        _Cube0 ("Cube0", CUBE) = "white" {}
        _Vector0 ("Vector0", Vector) = (1, 1, 1, 1)
        _Float0("Float", Float) = 1
        _Color0("Color", Color) = (1, 1, 1, 1)
        
        
        [Header(A separation bold text)] // draw texture in bold. Handy to separate shader section.
        
        // same type but different inspector 
        
        // do not show tiling/offset fields (most of the time useless)
        [NoScaleOffset] _Tex1 ("Texture0", 2D) = "black" {}
        
        [Header(Float)]
        
        // using enum. You declare a list of pair label/value.
        [Enum(Custom0,0, Custom1,1)] _Float1("Float with enum", Float) = 1
        [Enum(Cos45,0.707106, PI,3.141592, TheUltimateQuestionOfLife, 42)] _Float2("Float with enum 2", Float) = 42 
        
        [Toggle] _Toggle0("Toggle type", Float) = 0.0
        
        // using range 
        // very specific and legacy notation : it should use an attribute instead.
        _FloatRange("Float with slider", Range(0, 1)) = 1 
        
        [Space(50)] // force a vertical separator.
        [Header(Special)] 
        
        // keyword enum : you can activate or not a custom compilation if there is the proper multi_compile
        [KeywordEnum(Disable, Enable)] CUSTOM_COMPILATION("CUSTOM_COMPILATION keyword", Float) = 0
        
        // you can use property to set render mode (on our exemple setting the culling mode
        [Enum(None,0, Front,1, Back,2)] _CullProperty ("Culling ", Float) = 0.0
	}
    
	SubShader
	{
		Tags 
        { 
            "RenderType" = "Opaque" 
        }

		Pass
		{
            Blend One OneMinusSrcAlpha
            Cull [_CullProperty] 
            Lighting Off 
            ZWrite On
            ZTest LEqual
            
			CGPROGRAM
            #pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
            
            #pragma multi_compile DEFAULT_COMPILATION CUSTOM_COMPILATION

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			float4 frag (v2f IN) : COLOR
			{
				return float4(1, 1, 1, 1);
			}
			ENDCG
		}
	}
}
