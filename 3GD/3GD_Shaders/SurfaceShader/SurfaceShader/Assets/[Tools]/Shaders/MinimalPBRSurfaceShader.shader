Shader "Custom/MinimalPBRSurfaceShader" 
{
	Properties 
    {
        // J'ai gardé les noms unity pour facilement passer du shader Standard a ce shader custom.
        // [NoScaleOffset] Pour éviter d'avoir l'interface de tiling.
        [NoScaleOffset] _MainTex("Albedo (RGB) Alpha (A)", 2D) = "white"
        // [Normal] Pour dire a Unity d'afficher un message d'avertissement si la texture branchée n'est pas une normal map.
        [NoScaleOffset][Normal] _BumpMap("Normal map ", 2D) = "bump"
        [NoScaleOffset] _EmissionMap("Emission (RGB)", 2D) = "black"
        [NoScaleOffset] _MetallicGlossMap("Metallic (R) smoothness (A)", 2D) = "black"
        [NoScaleOffset] _OcclusionMap("Ambiant Occlusion (R)", 2D) = "white"
	}
	SubShader 
    {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
        #pragma vertex vert
		#pragma surface surf Standard fullforwardshadows

		#pragma target 3.0

		struct Input 
        {
            // attention les uv avec cette syntaxe sont connus d'Unity et automatiquement bindé
			float2 uv_MainTex; 
		};

        uniform sampler2D _MainTex;
        uniform sampler2D _BumpMap;
        uniform sampler2D _EmissionMap;
        uniform sampler2D _MetallicGlossMap;
        uniform sampler2D _OcclusionMap;

		UNITY_INSTANCING_BUFFER_START(Props)
            // a creuser.
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            // ici pas de code pour initialiser uv_MainTex c'est automagique.
        }

		void surf (Input IN, inout SurfaceOutputStandard o) 
        {
            const float4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
            const float4 bumpMap = tex2D(_BumpMap, IN.uv_MainTex);
            const float4 emissionMap = tex2D(_EmissionMap, IN.uv_MainTex);
            const float4 metallicGlossMap = tex2D(_MetallicGlossMap, IN.uv_MainTex);
            const float4 occlusionMap = tex2D(_OcclusionMap, IN.uv_MainTex);

            o.Albedo = mainTex.rgb;
            o.Emission = emissionMap.rgb;
            o.Metallic = metallicGlossMap.r;
            o.Smoothness = metallicGlossMap.a;
            o.Normal = UnpackNormal(bumpMap);
            o.Alpha = mainTex.a;
            o.Occlusion = occlusionMap.r;
		}
		ENDCG
	}

	FallBack "Diffuse"
}
