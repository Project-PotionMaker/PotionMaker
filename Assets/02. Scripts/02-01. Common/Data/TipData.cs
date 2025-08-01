// 툴에서 자동으로 생성하는 소스 파일입니다. 수정하지 마세요!
using System.Collections.Generic;
using System.IO;
using System.Text;

public class TipData
{
    ///<summary>TID</summary>
    public readonly int TID;

    ///<summary>내용</summary>
    public readonly string Description;

    ///<summary>내용 TID</summary>
    public readonly int Description_LocalizationTID;

    public TipData(BinaryReader reader)
    {
        TID = reader.ReadInt32();
        int description = reader.ReadInt32();
        Description = Encoding.UTF8.GetString(reader.ReadBytes(description));
        Description_LocalizationTID = reader.ReadInt32();
    }
}
