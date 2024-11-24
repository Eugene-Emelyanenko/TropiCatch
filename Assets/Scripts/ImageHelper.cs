using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ImageHelper
{
    public static Texture2D ResizeTexture(Texture2D originalTexture, int maxSize)
    {
        if (originalTexture != null)
        {
            int newWidth = originalTexture.width;
            int newHeight = originalTexture.height;
            if (originalTexture.width > originalTexture.height)
            {
                newWidth = maxSize;
                newHeight = (int)((float)originalTexture.height / originalTexture.width * maxSize);
            }
            else
            {
                newHeight = maxSize;
                newWidth = (int)((float)originalTexture.width / originalTexture.height * maxSize);
            }
            Texture2D resizedTexture = ResizeTextureToMax(originalTexture, newWidth, newHeight);
            return resizedTexture;
        }
        return originalTexture;
    }
    private static Texture2D ResizeTextureToMax(Texture2D sourceTexture, int maxWidth, int maxHeight)
    {
        RenderTexture rt = RenderTexture.GetTemporary(maxWidth, maxHeight);
        RenderTexture.active = rt;

        Graphics.Blit(sourceTexture, rt);
        Texture2D resultTexture = new Texture2D(maxWidth, maxHeight);

        resultTexture.ReadPixels(new Rect(0, 0, maxWidth, maxHeight), 0, 0);
        resultTexture.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);

        return resultTexture;
    }
    
    public static string GenerateUniqueName() => "image_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
}
