// 툴에서 자동으로 생성하는 소스 파일입니다. 수정하지 마세요!
using System.Collections.Generic;
using System.IO;
using System.Text;

public class MachineData
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

    ///<summary>기구 코드</summary>
    public readonly char MachineCode;

    ///<summary>구역 타입</summary>
    public readonly EAreaType AreaType;

    ///<summary>최대 입력 개수</summary>
    public readonly int MaxInputCount;

    ///<summary>최대 진행도</summary>
    public readonly float MaxProgress;

    ///<summary>틱당 진행도 (속도)</summary>
    public readonly float ProgressPerTick;

    ///<summary>출력 개수</summary>
    public readonly int OutputAmount;

    ///<summary>상호작용 타입</summary>
    public readonly EInteractType InteractType;

    ///<summary>구조물TID</summary>
    public readonly int StructureTID;

    public MachineData(BinaryReader reader)
    {
        TID = reader.ReadInt32();
        int name = reader.ReadInt32();
        Name = Encoding.UTF8.GetString(reader.ReadBytes(name));
        Name_LocalizationTID = reader.ReadInt32();
        Description_LocalizationTID = reader.ReadInt32();
        Hint_LocalizationTID = reader.ReadInt32();
        MachineCode = reader.ReadChar();
        AreaType = (EAreaType)reader.ReadInt32();
        MaxInputCount = reader.ReadInt32();
        MaxProgress = reader.ReadSingle();
        ProgressPerTick = reader.ReadSingle();
        OutputAmount = reader.ReadInt32();
        InteractType = (EInteractType)reader.ReadInt32();
        StructureTID = reader.ReadInt32();
    }
}
