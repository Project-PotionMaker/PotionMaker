// 툴에서 자동으로 생성하는 소스 파일입니다. 수정하지 마세요!
using System.Collections.Generic;
using System.IO;
using System.Text;

public class StructureData
{
    ///<summary>TID</summary>
    public readonly int TID;

    ///<summary>이름</summary>
    public readonly string Name;

    ///<summary>가로 길이</summary>
    public readonly int Width;

    ///<summary>세로 길이</summary>
    public readonly int Length;

    ///<summary>구조물 타입</summary>
    public readonly EStructureType StructureType;

    ///<summary>구조물 TID</summary>
    public readonly int TypeTID;

    ///<summary>구역 타입</summary>
    public readonly EAreaType AreaType;

    ///<summary>특수 구조물 타입</summary>
    public readonly ESpecialStructureType SpecialStructureType;

    public StructureData(BinaryReader reader)
    {
        TID = reader.ReadInt32();
        int name = reader.ReadInt32();
        Name = Encoding.UTF8.GetString(reader.ReadBytes(name));
        Width = reader.ReadInt32();
        Length = reader.ReadInt32();
        StructureType = (EStructureType)reader.ReadInt32();
        TypeTID = reader.ReadInt32();
        AreaType = (EAreaType)reader.ReadInt32();
        SpecialStructureType = (ESpecialStructureType)reader.ReadInt32();
    }
}
