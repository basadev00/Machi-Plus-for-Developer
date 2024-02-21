using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UVsynthesization_Utilities
{
    public static Texture2D UVSynthesis(Texture2D uvmap, string surfaceimagepath, KeyValuePair<string, LoadBuilding> entry)
    {
        // 合成する画像をTexture2Dとして読み込む
        Texture2D compositeImage = LoadTexture(surfaceimagepath);

        // UV展開図画像に画像を合成する
        Vector2Int[] facePixels = new Vector2Int[]
        {
            new Vector2Int(entry.Value.Vertex0_X, entry.Value.Vertex0_Y),
            new Vector2Int(entry.Value.Vertex1_X, entry.Value.Vertex1_Y),
            new Vector2Int(entry.Value.Vertex2_X, entry.Value.Vertex2_Y),
            new Vector2Int(entry.Value.Vertex3_X, entry.Value.Vertex3_Y)
        };
        CompositeImage(uvmap, compositeImage, entry.Value.surface_num, facePixels);

        return uvmap;
    }

    // パスからTexture2Dを読み込むメソッド
    public static Texture2D LoadTexture(string path)
    {
        // パスからバイト配列を読み込む
        byte[] bytes = System.IO.File.ReadAllBytes(path);
        // バイト配列からTexture2Dを作成する
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        return texture;
    }

    // Texture2Dをパスに保存するメソッド
    public static void SaveTexture(Texture2D texture, string path)
    {
        // Texture2DからPNG形式のバイト配列を作成する
        byte[] bytes = texture.EncodeToPNG();
        // バイト配列をパスに書き込む
        System.IO.File.WriteAllBytes(path, bytes);
    }

    // UV展開図画像に画像を合成するメソッド
    static void CompositeImage(Texture2D uvMap, Texture2D compositeImage, int faceIndex, Vector2Int[] facePixels)
    {
        // UV展開図画像の幅と高さを取得する
        int uvMapWidth = uvMap.width;
        int uvMapHeight = uvMap.height;
        // 合成する画像の幅と高さを取得する
        int compositeImageWidth = compositeImage.width;
        int compositeImageHeight = compositeImage.height;
        // 面の頂点のピクセル座標から面の幅と高さを計算する
        int faceWidth = facePixels[1].x - facePixels[0].x;
        int faceHeight = facePixels[3].y - facePixels[0].y;
        // 面のピクセルに合成する画像の色を設定する
        for (int x = 0; x < faceWidth; x++)
        {
            for (int y = 0; y < faceHeight; y++)
            {
                // 面のピクセル座標を計算する
                int faceX = facePixels[0].x + x;
                int faceY = facePixels[0].y + y;
                // 合成する画像のピクセル座標を計算する
                int compositeX = Mathf.RoundToInt((float)x / faceWidth * compositeImageWidth);
                int compositeY = Mathf.RoundToInt((float)y / faceHeight * compositeImageHeight);
                // 合成する画像の色を取得する
                Color compositeColor = compositeImage.GetPixel(compositeX, compositeY);
                // UV展開図画像のピクセルに色を設定する
                uvMap.SetPixel(faceX, faceY, compositeColor);
            }
        }
        // UV展開図画像の変更を反映させる
        uvMap.Apply();
    }
}
