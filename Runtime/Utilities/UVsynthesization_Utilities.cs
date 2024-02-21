using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UVsynthesization_Utilities
{
    public static Texture2D UVSynthesis(Texture2D uvmap, string surfaceimagepath, KeyValuePair<string, LoadBuilding> entry)
    {
        // ��������摜��Texture2D�Ƃ��ēǂݍ���
        Texture2D compositeImage = LoadTexture(surfaceimagepath);

        // UV�W�J�}�摜�ɉ摜����������
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

    // �p�X����Texture2D��ǂݍ��ރ��\�b�h
    public static Texture2D LoadTexture(string path)
    {
        // �p�X����o�C�g�z���ǂݍ���
        byte[] bytes = System.IO.File.ReadAllBytes(path);
        // �o�C�g�z�񂩂�Texture2D���쐬����
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        return texture;
    }

    // Texture2D���p�X�ɕۑ����郁�\�b�h
    public static void SaveTexture(Texture2D texture, string path)
    {
        // Texture2D����PNG�`���̃o�C�g�z����쐬����
        byte[] bytes = texture.EncodeToPNG();
        // �o�C�g�z����p�X�ɏ�������
        System.IO.File.WriteAllBytes(path, bytes);
    }

    // UV�W�J�}�摜�ɉ摜���������郁�\�b�h
    static void CompositeImage(Texture2D uvMap, Texture2D compositeImage, int faceIndex, Vector2Int[] facePixels)
    {
        // UV�W�J�}�摜�̕��ƍ������擾����
        int uvMapWidth = uvMap.width;
        int uvMapHeight = uvMap.height;
        // ��������摜�̕��ƍ������擾����
        int compositeImageWidth = compositeImage.width;
        int compositeImageHeight = compositeImage.height;
        // �ʂ̒��_�̃s�N�Z�����W����ʂ̕��ƍ������v�Z����
        int faceWidth = facePixels[1].x - facePixels[0].x;
        int faceHeight = facePixels[3].y - facePixels[0].y;
        // �ʂ̃s�N�Z���ɍ�������摜�̐F��ݒ肷��
        for (int x = 0; x < faceWidth; x++)
        {
            for (int y = 0; y < faceHeight; y++)
            {
                // �ʂ̃s�N�Z�����W���v�Z����
                int faceX = facePixels[0].x + x;
                int faceY = facePixels[0].y + y;
                // ��������摜�̃s�N�Z�����W���v�Z����
                int compositeX = Mathf.RoundToInt((float)x / faceWidth * compositeImageWidth);
                int compositeY = Mathf.RoundToInt((float)y / faceHeight * compositeImageHeight);
                // ��������摜�̐F���擾����
                Color compositeColor = compositeImage.GetPixel(compositeX, compositeY);
                // UV�W�J�}�摜�̃s�N�Z���ɐF��ݒ肷��
                uvMap.SetPixel(faceX, faceY, compositeColor);
            }
        }
        // UV�W�J�}�摜�̕ύX�𔽉f������
        uvMap.Apply();
    }
}
