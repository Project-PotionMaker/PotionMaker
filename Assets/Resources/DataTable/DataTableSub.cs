using System.IO;
using System.Collections;
using UnityEngine;
using System;

public partial class DataTable : POCOSingleton<DataTable>
{
    private BinaryReader Reader = null;

    private void GetBytes_FromResources(string name, Action<byte[]> callback)
    {
        TextAsset ta = Resources.Load("DataTable/" + name + "Data") as TextAsset;
        callback.Invoke(ta.bytes);
    }

    public byte[] GetBytes_ForEditor(string name)
    {
        return null;
    }

    public IEnumerator Load_Routine()
    {
        yield return LoadRoutine();

        Debug.Log("LoadRoutine End");
    }
}
