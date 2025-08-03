// 툴에서 자동으로 생성하는 소스 파일입니다. 수정하지 마세요!
using System.Collections.Generic;
using System.IO;
using System.Text;

public class ProductData
{
    ///<summary>TID</summary>
    public readonly int TID;

    ///<summary>이름</summary>
    public readonly string Name;

    ///<summary>상품 타입</summary>
    public readonly EProductType ProductType;

    ///<summary>이름 TID</summary>
    public readonly int Name_LocalizationTID;

    ///<summary>설명 TID</summary>
    public readonly int Description_LocalizationTID;

    ///<summary>가격</summary>
    public readonly int Price;

    ///<summary>타겟 TID</summary>
    public readonly int TargetTID;

    ///<summary>임시설명</summary>
    public readonly string Description;

    public ProductData(BinaryReader reader)
    {
        TID = reader.ReadInt32();
        int name = reader.ReadInt32();
        Name = Encoding.UTF8.GetString(reader.ReadBytes(name));
        ProductType = (EProductType)reader.ReadInt32();
        Name_LocalizationTID = reader.ReadInt32();
        Description_LocalizationTID = reader.ReadInt32();
        Price = reader.ReadInt32();
        TargetTID = reader.ReadInt32();
        int description = reader.ReadInt32();
        Description = Encoding.UTF8.GetString(reader.ReadBytes(description));
    }
}
