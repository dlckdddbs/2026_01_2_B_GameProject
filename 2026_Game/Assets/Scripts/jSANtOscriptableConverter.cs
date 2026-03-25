#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using NUnit.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEditor.Callbacks;


public enum ConversionType
{
    items,
    Dialogs
}

[Serializable]
public class DialogRowData
{
    public int? id;     //int?는 Nullable<int>의 축약 표현, 선언하면 null값도 가질 수 있는 정수형
    public string characterName;
    public string text;
    public int? nextId;
    public string protraitPath;
    public string choiceText;
    public int? choiceNextId;
 }

public class jSANtOscriptableConverter : EditorWindow
{
    private string jsonFillePaht = "";          //JSON 파일 경로 문자열 값
    private string outputFolor = "Assets/ScriptableObjects";      //출력 SO파일 경로 값
    private bool createDatabase = true;         //데이터 베이스 활용 여부 체그값 
    private ConversionType conversionType = ConversionType.items;


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

        //변환 타입 선택
        conversionType = (ConversionType)EditorGUILayout.EnumPopup("Conversion Type : ", conversionType);

        //타입에 따라 기본 출력 폴더 설정
        if(conversionType==ConversionType.items && outputFolor == "Assets/ScriptableObjects")
        {
            outputFolor = "Assets/ScriptableObjects/Items";
        }
        else if (conversionType == ConversionType.Dialogs && outputFolor == "Assets/ScriptableObjects")
        {
            outputFolor = "Assets/ScriptableObjects/Dialogs";
        }


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

            switch (conversionType)
            {
                case ConversionType.items:
                    ConvertJsonToItemScriptableObjects();
                    break;
                case ConversionType.Dialogs:
                    ConvertJsonToDialogScriptableObjects();
                    break;
            }

        }

    }

    private void ConvertJsonToItemScriptableObjects()           //JSON 파일 Scriptable Objects 파일로 변환 시켜주는 함수
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

    //대화 JSON을 스크립터블 오브젝트로 변환
    private void ConvertJsonToDialogScriptableObjects()
    {
        if (!Directory.Exists(outputFolor))
        {
            Directory.CreateDirectory(outputFolor);
        }

        string JsonText = File.ReadAllText(jsonFillePaht);

        try
        {
            //JSON 파싱
            List<DialogRowData> rowDatalist = JsonConvert.DeserializeObject<List<DialogRowData>>(JsonText);

            //대화 데이터 재구성
            Dictionary<int, DialgSO> dialogMap = new Dictionary<int, DialgSO>();
            List<DialgSO> CreatDialogs = new List<DialgSO>();


            //1단계 대화 항목 생성
            foreach (var rowdata in rowDatalist)
            {
                if (!rowdata.id.HasValue)       //id없는 row는 스킵
                {
                    continue;
                }

                //id 있는 행을 대화로 처리
                DialgSO dialgSO = ScriptableObject.CreateInstance<DialgSO>();

                dialgSO.id = rowdata.id.Value;
                dialgSO.characterName = rowdata.characterName;
                dialgSO.text = rowdata.text;
                dialgSO.nextId = rowdata.nextId.HasValue ? rowdata.nextId.Value : -1;
                dialgSO.choices = new List<DialogChoiceSO>();
                //초상화 로드(경로가 있을 경우)
                if (!string.IsNullOrEmpty(rowdata.protraitPath))
                {
                    dialgSO.protrait = Resources.Load<Sprite>(rowdata.protraitPath);
                    if (dialgSO.protrait == null)
                    {
                        Debug.LogWarning($"대화 {rowdata.id}의 초상화를 찾을 수 없습니다.");
                    }

                }
                dialogMap[dialgSO.id] = dialgSO;
                CreatDialogs.Add(dialgSO);
            }
            //2단계
            foreach(var rowData in rowDatalist)
            {
                //id가 없고 choiceText 가 있는 행은 선택지로 처리
                if(!rowData.id.HasValue && !string.IsNullOrEmpty(rowData.choiceText) && rowData.choiceNextId.HasValue)
                {
                    //이전 행의 ID를 부모 ID로 사용(연속되는 선택지의 경우)
                    int parentId = -1;

                    //이 선택지 바로 위에 잇는 대화(id가 있는 항목)을 찾음
                    int currentIndex = rowDatalist.IndexOf(rowData);
                    for(int i =currentIndex-1; i >= 0; i--)
                    {
                        if (rowDatalist[i].id.HasValue)
                        {
                            parentId = rowDatalist[i].id.Value;
                            break;
                        }
                    }

                    //부모 ID를 찾지 못했거나 부모 ID가 -1인 경우 (첫번째 항복)
                    if (parentId == -1)
                    {
                        Debug.LogWarning($"선택지 {rowData.choiceText} 의 부모 대화를 찾을 수 없음");
                    }
                    if(dialogMap.TryGetValue(parentId, out DialgSO parentDialog))
                    {
                        DialogChoiceSO choiceSo = ScriptableObject.CreateInstance<DialogChoiceSO>();
                        choiceSo.text = rowData.choiceText;
                        choiceSo.nextId = rowData.choiceNextId.Value; // id.Value가 아니라 choiceNextId.Value로 변경

                        //선택지 에셋 저장
                        string choiceAssetPath = $"{outputFolor}/Choice_{parentId}_{parentDialog.choices.Count + 1}.aseet";
                        AssetDatabase.CreateAsset(choiceSo, choiceAssetPath);
                        EditorUtility.SetDirty(choiceSo);
                        parentDialog.choices.Add(choiceSo);
                    }
                    else
                    {
                        Debug.LogWarning($"선택지 {rowData.choiceText}를 연결할 대화 (ID : {parentId}를 찾을 수 없습니다)");
                    }

                }
            }

            //3단계 : 대화 스크립터블 오브젝트 저장
            foreach(var dialog in CreatDialogs)
            {
                string assetPath = $"{outputFolor}/Dialog {dialog.id.ToString("D4")}.asset";
                AssetDatabase.CreateAsset(dialog, assetPath);

                //에셋 이름 지정 
                dialog.name = $"Dialog_{dialog.id.ToString("D4")}";

                EditorUtility.SetDirty(dialog);
            }

            if(createDatabase && CreatDialogs.Count > 0)
            {
                DialogDatabaseSO database = ScriptableObject.CreateInstance<DialogDatabaseSO>();
                database.dialogs = CreatDialogs;

                AssetDatabase.CreateAsset(database, $"{outputFolor}.DialogDatabase.assets");
                EditorUtility.SetDirty(database);
                   
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Sucess", $"created {CreatDialogs.Count} dialog scriptable object!", "OK");

        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Faild to convert JSON :{e.Message}", "OK");
            Debug.Log($"JSON 변환 오류 : {e}");
        }
    }

   



}

#endif
