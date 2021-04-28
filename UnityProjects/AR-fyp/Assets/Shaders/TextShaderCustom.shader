//sourced publically from http://wiki.unity3d.com/index.php?title=3DText
//Author: Eric Haines (Eric5h5)

//edited to include stencil masking so will work correctly with portals

Shader "GUI/TextShaderCustom" {
	Properties{
		_MainTex("Font Texture", 2D) = "white" {}
		_Color("Text Color", Color) = (1,1,1,1)

		_StencilReferenceID("Stencil ID Reference", Float) = 1
		[Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp("Stencil Comparison", Float) = 3
		[Enum(UnityEngine.Rendering.StencilOp)] _StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255
	}

		SubShader{
			Tags 
			{ 
				"RenderType" = "Transparent"
				"Queue" = "Transparent"  
			}

			LOD 200

			Lighting Off Cull Back ZWrite Off Fog { Mode Off }
			Blend SrcAlpha OneMinusSrcAlpha
			Pass {
				Color[_Color]
				Stencil
				{
					Ref[_StencilReferenceID]
					Comp[_StencilComp]	// equal
					Pass[_StencilOp]	// keep
					ReadMask[_StencilReadMask]
					WriteMask[_StencilWriteMask]
				}
				SetTexture[_MainTex] {
					combine primary, texture * primary
				}
								
			}
		}
}