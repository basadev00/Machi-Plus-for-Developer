using PLATEAU.CityInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

public static class AttributeInfo_Utilities
{
    private const int CharacterLimit = 15000;
    private static string buildingID = string.Empty;

    /*
     * �֐����F GetBldgID
     * �T�v�F �������i�r���f�B���OID�擾�j
     */
    public static string GetBldgID(GameObject obj)
    {
        PLATEAUCityObjectGroup cityObjGroup = obj.transform.GetComponent<PLATEAUCityObjectGroup>();
        try
        {
            var cityObjectList = cityObjGroup.CityObjects;

            // ������ PLATEAU �n�������������1�̃Q�[���I�u�W�F�N�g�ɂȂ��Ă���ꍇ�����邽�߁A
            // 1�̃Q�[���I�u�W�F�N�g����擾�ł��� cityObjects �̐���1�܂��͕����ɂȂ��Ă��܂��B
            var rootCityObjects = cityObjectList.rootCityObjects;
            var attributesSb = new StringBuilder();

            foreach (var cityObjParam in rootCityObjects)
            {
                // �����������₷���`���̕�����ɂ��܂��B
                attributesSb.Append(cityObjParam.DebugString());
                attributesSb.Append("\n\n");
            }

            var firstPrimaryObj = cityObjGroup.PrimaryCityObjects.FirstOrDefault();
            var attributesMap = firstPrimaryObj.AttributesMap;

            // BuildingID�̎擾
            // BuildingID�́APLATEAU�̑������ł� "uro:buildingIDAttribute"�Ƃ����L�[�̒��ŁA�L�[�o�����[����������q�ɂȂ��Ă��钆�� "uro:buildingID" �Ƃ����L�[�Ŏ擾�ł��܂��B
            if (attributesMap.TryGetValue("uro:buildingIDAttribute", out var buildingAttr)) // ����q�L�[�̊O���ł�
            {
                // �����̃L�[�o�����[�y�A(Attributes)�œ���q�ɂȂ��Ă�����̂��擾���܂��B
                // �L�[ uro:buildingIDAttribute �̒��ɓ���q��0�ȏ�̃L�[�o�����[�y�A������A
                // ���̃L�[ uro:buildingID ����o�����[�iBuildingID�j���擾���܂��B
                if (buildingAttr.AttributesMapValue.TryGetValue("uro:buildingID", out var addressAttr1)) // ����q�L�[�̓����ł�
                {
                    buildingID = addressAttr1.StringValue;
                    //Debug.Log("buildingID=" + buildingID);
                }
                
             }
            else if (attributesMap.TryGetValue("����ID", out var addressAttr2)) // ����q�L�[�̊O���ł�
            {
                buildingID = addressAttr2.StringValue;
            }

            return (buildingID);
        }
        catch
        {
            return "";
        }
    }



}
