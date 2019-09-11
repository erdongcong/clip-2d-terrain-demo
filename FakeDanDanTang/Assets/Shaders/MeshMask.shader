Shader "FakeDanDanTang/MeshMask"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
			ColorMask 0
			ZWrite Off
			Stencil {
				Ref 2
				Comp always
		        Pass replace
			}
        }
    }
}
