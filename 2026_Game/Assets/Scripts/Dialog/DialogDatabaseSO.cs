using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogDatabaseSO", menuName = "Dialog/DialogDatabaseSO")]
public class DialogDatabaseSO : ScriptableObject
{
    public List<DialgSO> dialogs = new List<DialgSO>();

    public Dictionary<int, DialgSO> dialogById;         //Ä³½ÌÀ» À§ÇÑ µñ¼Å³Ê¸® »ç¿ë

    public void Initailize()
    {
        dialogById = new Dictionary<int, DialgSO>();

        foreach (var dialog in dialogs)
        {
            if (dialog != null)
            {
                dialogById[dialog.id] = dialog;
            }
        }
    }

    public DialgSO GetDialongById(int id)
    {
        if (dialogById == null) Initailize();

        if(dialogById.TryGetValue(id,out DialgSO dialog))
        {
            return dialog;
        }

        return null;
    }
}
