// 툴에서 자동으로 생성하는 소스 파일입니다. 수정하지 마세요!
using System.Collections.Generic;
using System.IO;
using System.Text;

public class InputData
{
    ///<summary>TID</summary>
    public readonly int TID;

    ///<summary>이름</summary>
    public readonly string Name;

    ///<summary>출력 타입</summary>
    public readonly EInputType InputType;

    ///<summary>입력물 TID</summary>
    public readonly int InputTID;

    public InputData(BinaryReader reader)
    {
        TID = reader.ReadInt32();
        int name = reader.ReadInt32();
        Name = Encoding.UTF8.GetString(reader.ReadBytes(name));
        InputType = (EInputType)reader.ReadInt32();
        InputTID = reader.ReadInt32();
    }
}
