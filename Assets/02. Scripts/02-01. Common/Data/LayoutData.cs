// 툴에서 자동으로 생성하는 소스 파일입니다. 수정하지 마세요!
using System.Collections.Generic;
using System.IO;
using System.Text;

public class LayoutData
{
    ///<summary>TID</summary>
    public readonly int TID;

    ///<summary>레이아웃 이름</summary>
    public readonly string Name;

    ///<summary>티어</summary>
    public readonly int Tier;

    ///<summary>시작 임대료</summary>
    public readonly int InitialRentCost;

    ///<summary>임대료 증가값</summary>
    public readonly int RentIncrement;

    ///<summary>씬 이름</summary>
    public readonly string SceneName;

    ///<summary>상품 TID</summary>
    public readonly int ProductTID;

    public LayoutData(BinaryReader reader)
    {
        TID = reader.ReadInt32();
        int name = reader.ReadInt32();
        Name = Encoding.UTF8.GetString(reader.ReadBytes(name));
        Tier = reader.ReadInt32();
        InitialRentCost = reader.ReadInt32();
        RentIncrement = reader.ReadInt32();
        int scenename = reader.ReadInt32();
        SceneName = Encoding.UTF8.GetString(reader.ReadBytes(scenename));
        ProductTID = reader.ReadInt32();
    }
}
