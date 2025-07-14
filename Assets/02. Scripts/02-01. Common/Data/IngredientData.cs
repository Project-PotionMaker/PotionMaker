// 툴에서 자동으로 생성하는 소스 파일입니다. 수정하지 마세요!
using System.Collections.Generic;
using System.IO;
using System.Text;

public class IngredientData
{
    ///<summary>TID</summary>
    public readonly int TID;

    ///<summary>이름</summary>
    public readonly string Name;

    ///<summary>장소 설명</summary>
    public readonly string PlaceDescription;

    ///<summary>세부 설명</summary>
    public readonly string DetailDescription;

    ///<summary>사용 가능한 기구 TID</summary>
    public readonly int AvailableMachineTID;

    public IngredientData(BinaryReader reader)
    {
        TID = reader.ReadInt32();
        int name = reader.ReadInt32();
        Name = Encoding.UTF8.GetString(reader.ReadBytes(name));
        int placedescription = reader.ReadInt32();
        PlaceDescription = Encoding.UTF8.GetString(reader.ReadBytes(placedescription));
        int detaildescription = reader.ReadInt32();
        DetailDescription = Encoding.UTF8.GetString(reader.ReadBytes(detaildescription));
        AvailableMachineTID = reader.ReadInt32();
    }
}
