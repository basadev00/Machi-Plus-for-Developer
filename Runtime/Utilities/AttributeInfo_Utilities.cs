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
     * 関数名： GetBldgID
     * 概要： 建物情報（ビルディングID取得）
     */
    public static string GetBldgID(GameObject obj)
    {
        PLATEAUCityObjectGroup cityObjGroup = obj.transform.GetComponent<PLATEAUCityObjectGroup>();
        try
        {
            var cityObjectList = cityObjGroup.CityObjects;

            // 複数の PLATEAU 地物が結合されて1つのゲームオブジェクトになっている場合があるため、
            // 1つのゲームオブジェクトから取得できる cityObjects の数は1つまたは複数になっています。
            var rootCityObjects = cityObjectList.rootCityObjects;
            var attributesSb = new StringBuilder();

            foreach (var cityObjParam in rootCityObjects)
            {
                // 属性情報を見やすい形式の文字列にします。
                attributesSb.Append(cityObjParam.DebugString());
                attributesSb.Append("\n\n");
            }

            var firstPrimaryObj = cityObjGroup.PrimaryCityObjects.FirstOrDefault();
            var attributesMap = firstPrimaryObj.AttributesMap;

            // BuildingIDの取得
            // BuildingIDは、PLATEAUの属性情報では "uro:buildingIDAttribute"というキーの中で、キーバリュー辞書が入れ子になっている中の "uro:buildingID" というキーで取得できます。
            if (attributesMap.TryGetValue("uro:buildingIDAttribute", out var buildingAttr)) // 入れ子キーの外側です
            {
                // 属性のキーバリューペア(Attributes)で入れ子になっているものを取得します。
                // キー uro:buildingIDAttribute の中に入れ子で0個以上のキーバリューペアがあり、
                // そのキー uro:buildingID からバリュー（BuildingID）を取得します。
                if (buildingAttr.AttributesMapValue.TryGetValue("uro:buildingID", out var addressAttr1)) // 入れ子キーの内側です
                {
                    buildingID = addressAttr1.StringValue;
                    //Debug.Log("buildingID=" + buildingID);
                }
                
             }
            else if (attributesMap.TryGetValue("建物ID", out var addressAttr2)) // 入れ子キーの外側です
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
