using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialgSO", menuName = "Dialog System/DialgSO")]
public class DialgSO : ScriptableObject
{
    public int id;
    public string characterName;
    public string text;
    public int nextId;
    public Sprite portrait;

    public List<DialogChoiceSO> choices = new List<DialogChoiceSO>();
    public Sprite protrait;

    public string portraitPath;



}
