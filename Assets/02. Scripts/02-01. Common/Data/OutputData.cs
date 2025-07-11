// 툴에서 자동으로 생성하는 소스 파일입니다. 수정하지 마세요!
using System.Collections.Generic;
using System.IO;
using System.Text;

public class OutputData
{
    ///<summary>TID</summary>
    public readonly int TID;

    ///<summary>이름</summary>
    public readonly string Name;

    ///<summary>색상코드</summary>
    public readonly string ColorCode;

    ///<summary>재료1 TID</summary>
    private readonly int Ingredient1TID;

    ///<summary>재료2 TID</summary>
    private readonly int Ingredient2TID;

    ///<summary>IngredientTID 리스트</summary>
    public readonly List<int> IngredientTIDList = new List<int>();
    public OutputData(BinaryReader reader)
    {
        TID = reader.ReadInt32();
        int name = reader.ReadInt32();
        Name = Encoding.UTF8.GetString(reader.ReadBytes(name));
        int colorcode = reader.ReadInt32();
        ColorCode = Encoding.UTF8.GetString(reader.ReadBytes(colorcode));
        Ingredient1TID = reader.ReadInt32();
        Ingredient2TID = reader.ReadInt32();

        LinkTable();
    }

    public void LinkTable()
    {
        IngredientTIDList.Add(Ingredient1TID);
        IngredientTIDList.Add(Ingredient2TID);
    }
}
