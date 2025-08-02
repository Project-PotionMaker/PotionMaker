// 툴에서 자동으로 생성하는 소스 파일입니다. 수정하지 마세요!
using System.Collections.Generic;
using System.IO;
using System.Text;

public class PotionData
{
    ///<summary>TID</summary>
    public readonly int TID;

    ///<summary>포션 이름</summary>
    public readonly string Name;

    ///<summary>이름 TID</summary>
    public readonly int Name_TID;

    ///<summary>능력 설명</summary>
    public readonly int Ability_LocalizationTID;

    ///<summary>향 설명</summary>
    public readonly int Flavor_LocalizationTID;

    ///<summary>특징 설명</summary>
    public readonly int Feature_LocalizationTID;

    ///<summary>티어</summary>
    public readonly int Tier;

    ///<summary>재료1TID</summary>
    private readonly int Ingredient1TID;

    ///<summary>재료2TID</summary>
    private readonly int Ingredient2TID;

    ///<summary>레시피 코드</summary>
    public readonly string RecipeCode;

    ///<summary>가격</summary>
    public readonly int Price;

    ///<summary>IngredientTID 리스트</summary>
    public readonly List<int> IngredientTIDList = new List<int>();
    public PotionData(BinaryReader reader)
    {
        TID = reader.ReadInt32();
        int name = reader.ReadInt32();
        Name = Encoding.UTF8.GetString(reader.ReadBytes(name));
        Name_TID = reader.ReadInt32();
        Ability_LocalizationTID = reader.ReadInt32();
        Flavor_LocalizationTID = reader.ReadInt32();
        Feature_LocalizationTID = reader.ReadInt32();
        Tier = reader.ReadInt32();
        Ingredient1TID = reader.ReadInt32();
        Ingredient2TID = reader.ReadInt32();
        int recipecode = reader.ReadInt32();
        RecipeCode = Encoding.UTF8.GetString(reader.ReadBytes(recipecode));
        Price = reader.ReadInt32();

        LinkTable();
    }

    public void LinkTable()
    {
        IngredientTIDList.Add(Ingredient1TID);
        IngredientTIDList.Add(Ingredient2TID);
    }
}
