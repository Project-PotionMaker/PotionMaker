// 툴에서 자동으로 생성하는 소스 파일입니다. 수정하지 마세요!
using System.Collections.Generic;
using System.IO;
using System.Text;

public class StorageData
{
    ///<summary>TID</summary>
    public readonly int TID;

    ///<summary>이름</summary>
    public readonly string Name;

    ///<summary>이름 TID</summary>
    public readonly int Name_LocalizationTID;

    ///<summary>기구 설명 TID</summary>
    public readonly int Description_LocalizationTID;

    ///<summary>힌트 TID</summary>
    public readonly int Hint_LocalizationTID;

    ///<summary>재료 타입</summary>
    public readonly EIngredientType IngredientType;

    public StorageData(BinaryReader reader)
    {
        TID = reader.ReadInt32();
        int name = reader.ReadInt32();
        Name = Encoding.UTF8.GetString(reader.ReadBytes(name));
        Name_LocalizationTID = reader.ReadInt32();
        Description_LocalizationTID = reader.ReadInt32();
        Hint_LocalizationTID = reader.ReadInt32();
        IngredientType = (EIngredientType)reader.ReadInt32();
    }
}
