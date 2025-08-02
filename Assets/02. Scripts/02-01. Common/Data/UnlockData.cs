// 툴에서 자동으로 생성하는 소스 파일입니다. 수정하지 마세요!
using System.Collections.Generic;
using System.IO;
using System.Text;

public class UnlockData
{
    ///<summary>TID</summary>
    public readonly int TID;

    ///<summary>티어</summary>
    public readonly int Tier;

    ///<summary>해금 타입</summary>
    public readonly EUnlockType UnlockType;

    ///<summary>대상 TID들</summary>
    public readonly string TargetTIDs;

    public UnlockData(BinaryReader reader)
    {
        TID = reader.ReadInt32();
        Tier = reader.ReadInt32();
        UnlockType = (EUnlockType)reader.ReadInt32();
        int targettids = reader.ReadInt32();
        TargetTIDs = Encoding.UTF8.GetString(reader.ReadBytes(targettids));
    }
}
