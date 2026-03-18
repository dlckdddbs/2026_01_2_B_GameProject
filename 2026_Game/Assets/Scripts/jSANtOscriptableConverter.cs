#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using NUnit.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;



public class jSANtOscriptableConverter : EditorWindow
{
    private string jsonFillePaht = "";          //JSON 파일 경로 문자열 값
    private string outputFolor = "Assets/ScriptableObjects/items";      //출력 SO파일 경로 값
    private bool createDatabase = true;         //데이터 베이스 활용 여부 체그값 

    [MenuItem("Tols/JSON to Scriptable Objects")]

    public static void ShoWindow()
    {
        GetWindow<jSANtOscriptableConverter>("JSON to Scriptable Objects");
    }

    private void OnGUI()
    {
        GUILayout.Label("JSON to Scriptable object Converter", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if(GUILayout.Button("Select JSON File")){
            jsonFillePaht = EditorUtility.OpenFilePanel("Select JSON File ", "", "json");
        }

        EditorGUILayout.LabelField("Selected : ", jsonFillePaht);
        EditorGUILayout.Space();

        outputFolor = EditorGUILayout.TextField("Output Foloder : ", outputFolor);
        createDatabase = EditorGUILayout.Toggle("Create Database Asset", createDatabase);
        EditorGUILayout.Space();

        if(GUILayout.Button("Convert to Scriptable Objects"))
        {
            if (string.IsNullOrEmpty(jsonFillePaht))
            {
                EditorUtility.DisplayDialog("Error", "Pease Select a JSON file first", "OK");
                return;
            }
            ConvertJsonToScriptableObjects();
        }

    }

    private void ConvertJsonToScriptableObjects()           //JSON 파일 Scriptable Objects 파일로 변환 시켜주는 함수
    {
        if (!Directory.Exists(outputFolor))                 //폴더 위치를 확인하고 없으면 생성한다.
        {
            Directory.CreateDirectory(outputFolor);
        }


        //json 파일 읽기
        string jsonText = File.ReadAllText(jsonFillePaht);      //json v파일을 읽는다
        try
        {
            List<ItemData> itemDataList = JsonConvert.DeserializeObject<List<ItemData>>(jsonText);

            List<ItemSo> createdItems = new List<ItemSo>();             //itemSO 리스트 생성

            //각 아이템을 데이터 스크립터블 오브젝트로 변환
            foreach (ItemData itemData in itemDataList) 
            {
                ItemSo itemso = ScriptableObject.CreateInstance<ItemSo>();          //itemSo파일을 생성


                itemso.id = itemData.id;
                itemso.Itemname = itemData.Itemname;
                itemso.nameEng = itemData.nameEng;
                itemso.description = itemData.description;

                //열거형 변환
                if(System.Enum.TryParse(itemData.ItemTypeString, out ItemType parsedType))
                {
                    itemso.itemType = parsedType;
                }
                else
                {
                    Debug.LogWarning($"아이템 {itemData.Itemname}의 유효하지 않은 타입 : {itemData.ItemTypeString}");
                }


                itemso.price = itemData.id;
                itemso.power = itemData.power;
                itemso.level = itemData.level;
                itemso.isStackable = itemData.isStackable;

                //아이콘 로드 (경로가 있는 경우)

                if (!string.IsNullOrEmpty(itemData.iconPath))                       //아이콘 경로가 있는지 확인한다.
                {
                    itemso.icon = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Resources/{itemData.iconPath}.png");
                    
                    if(itemso.icon == null)
                    {
                        Debug.LogWarning($"아이템 {itemData.nameEng} 의 아이콘을 찾을 수 없습니다 : {itemData.iconPath}");
                    }
                }

                //스크립터블 오브젝트 저장 - ID를 4자리 숫자로 포맷팅
                string assetPath = $"{outputFolor}/Item_{itemData.id.ToString("D4")}_{itemData.nameEng}.asset";
                AssetDatabase.CreateAsset(itemso,assetPath);

                //에셋 이름 지정
                itemso.name = $"Item_{itemData.id.ToString("D4")}+{itemData.nameEng}";
                createdItems.Add(itemso);

                EditorUtility.SetDirty(itemso);

            }
            //데이터 베이스
            if (createDatabase && createdItems.Count > 0)
            {
                ItemDataBaseSo dataBase = ScriptableObject.CreateInstance<ItemDataBaseSo>();        //생성
                dataBase.items = createdItems;

                AssetDatabase.CreateAsset(dataBase, $"{outputFolor}/ItemDatabase.asset");
                EditorUtility.SetDirty(dataBase);

            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Sucess", $"Created {createdItems.Count} scriptable objects!", "OK");
        }
        catch(System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Falied to Convert Json : {e.Message}", "OK");
            Debug.LogError($"JSON 변환 오류 : {e}");
        }


    }

   



}

#endif
