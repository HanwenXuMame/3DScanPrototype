int _TextureType;
int _TextureWidth;
int _TextureHeight;

float4 tex2DNVX(sampler2D texture2d, float2 UV)
{
    // YYYY
    // YYYY
    // YYYY
    // YYYY
    // UUVV
    // UUVV

    float twoThirds = 1.0 / 1.5;
    float oneThird = 0.5 / 1.5;

    float2 YSectionSizeRelative = float2(1.0, twoThirds);

    float2 USectionStartPosRelative = float2(0.0, twoThirds);
    float2 USectionSizeRelative = float2(0.5, oneThird);

    float2 VSectionStartPosRelative = float2(0.5, twoThirds);
    float2 VSectionSizeRelative = float2(0.5, oneThird);

    float Y = tex2D(texture2d, UV * YSectionSizeRelative).a;
    float U = tex2D(texture2d, USectionStartPosRelative + UV * USectionSizeRelative).a;
    float V = tex2D(texture2d, VSectionStartPosRelative + UV * VSectionSizeRelative).a;

    float R = 1.164 * (Y - 16.0 / 256.0) + 1.596 * (V - 128.0 / 256.0);
    float G = 1.164 * (Y - 16.0 / 256.0) - 0.813 * (V - 128.0 / 256.0) - 0.391 * (U - 128.0 / 256.0);
    float B = 1.164 * (Y - 16.0 / 256.0) + 2.018 * (U - 128.0 / 256.0);

    return float4(R, G, B, 1.0);
}

float4 tex2DNV1221(sampler2D texture2d, float2 UV, bool UVNotVU)
{
    // YYYY
    // YYYY
    // YYYY
    // YYYY
    // UVUV for NV12  // VUVU for NV21
    // UVUV for NV12  // VUVU for NV21

    // _TextureWidth    // e.g. 2048
    // _TextureHeight   // e.g. 3072

    float2 nv12TextureSize = float2(_TextureWidth, _TextureHeight);
    int2 rgbTextureSize = int2(_TextureWidth, _TextureHeight * 2.0 / 3.0);

    int2 targetPixelPosAbsolute = int2(UV.x * rgbTextureSize.x, UV.y * rgbTextureSize.y);
    if (targetPixelPosAbsolute.x >= rgbTextureSize.x)
        targetPixelPosAbsolute.x -= 1;
    if (targetPixelPosAbsolute.y >= rgbTextureSize.y)
        targetPixelPosAbsolute.y -= 1;

    float2 pixelSizeRelative = float2(1.0, 1.0) / nv12TextureSize;
    float2 pixelHalfSizeRelative = pixelSizeRelative / 2.0;

    int2 YSectionSizeAbsolute = rgbTextureSize;

    int2 UVSectionStartPosAbsolute = int2(0, rgbTextureSize.y);

    int2 YPosAbsolute = targetPixelPosAbsolute;
    int2 UVPosAbsoluteOffset = targetPixelPosAbsolute / 2.0;    // shrinks UV to half of its range, which is a half of UV section
    UVPosAbsoluteOffset *= int2(2, 1);                        // stretches the half of UV section to whole UV section

    int2 UPosAbsolute = UVSectionStartPosAbsolute + UVPosAbsoluteOffset;
    int2 VPosAbsolute = UPosAbsolute + int2(1, 0);

    float Y = tex2D(texture2d, YPosAbsolute / nv12TextureSize + pixelHalfSizeRelative).a;
    float U = tex2D(texture2d, UPosAbsolute / nv12TextureSize + pixelHalfSizeRelative).a;
    float V = tex2D(texture2d, VPosAbsolute / nv12TextureSize + pixelHalfSizeRelative).a;
    if (!UVNotVU)
    {
        float temp = V;
        V = U;
        U = temp;
    }

    float R = 1.164 * (Y - 16.0 / 256.0) + 1.596 * (V - 128.0 / 256.0);
    float G = 1.164 * (Y - 16.0 / 256.0) - 0.813 * (V - 128.0 / 256.0) - 0.391 * (U - 128.0 / 256.0);
    float B = 1.164 * (Y - 16.0 / 256.0) + 2.018 * (U - 128.0 / 256.0);

    return float4(R, G, B, 1.0);
}

float4 sampleMvxTex(sampler2D texture2d, float2 UV)
{
    float4 clr;

    if (_TextureType == 0) //NVX
        clr = tex2DNVX(texture2d, UV);
    else if (_TextureType == 5) //NV12
        clr = tex2DNV1221(texture2d, UV, true);
    else if (_TextureType == 6) //NV21
        clr = tex2DNV1221(texture2d, UV, false);
    else //RGB, ETC2, DXT1, ASTC
        clr = tex2D(texture2d, UV);

    //RGB, ASTC, DTX1, ETC2 does not need colorspace fix
    if (_TextureType != 1 && _TextureType != 2 && _TextureType != 3 && _TextureType != 4)
    {
        //LinearSpace fix
#ifndef UNITY_COLORSPACE_GAMMA
        clr.rgb = GammaToLinearSpace(clr.rgb);
#endif
    }

    return clr;
}