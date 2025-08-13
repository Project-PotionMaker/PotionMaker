// 툴에서 자동으로 생성하는 소스 파일입니다. 수정하지 마세요!
using System.Collections.Generic;
using System.IO;
using System.Text;

public class XboxData
{
    ///<summary>TID</summary>
    public readonly int TID;

    ///<summary>키 이름</summary>
    public readonly string KeyName;

    ///<summary>유니티 경로</summary>
    public readonly string UnityInputPath;

    public XboxData(BinaryReader reader)
    {
        TID = reader.ReadInt32();
        int keyname = reader.ReadInt32();
        KeyName = Encoding.UTF8.GetString(reader.ReadBytes(keyname));
        int unityinputpath = reader.ReadInt32();
        UnityInputPath = Encoding.UTF8.GetString(reader.ReadBytes(unityinputpath));
    }
}
