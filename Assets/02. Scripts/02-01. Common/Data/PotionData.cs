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

    ///<summary>티어</summary>
    public readonly int Tier;

    ///<summary>능력 설명</summary>
    public readonly string AbilityDescription;

    ///<summary>향 설명</summary>
    public readonly string FlavorDescription;

    ///<summary>특징 설명</summary>
    public readonly string FeatureDescription;

    ///<summary>재료1TID</summary>
    private readonly int Ingredient1TID;

    ///<summary>재료2TID</summary>
    private readonly int Ingredient2TID;

    ///<summary>재료3TID</summary>
    private readonly int Ingredient3TID;

    ///<summary>레시피 코드</summary>
    public readonly string RecipeCode;

    ///<summary>IngredientTID 리스트</summary>
    public readonly List<int> IngredientTIDList = new List<int>();
    public PotionData(BinaryReader reader)
    {
        TID = reader.ReadInt32();
        int name = reader.ReadInt32();
        Name = Encoding.UTF8.GetString(reader.ReadBytes(name));
        Tier = reader.ReadInt32();
        int abilitydescription = reader.ReadInt32();
        AbilityDescription = Encoding.UTF8.GetString(reader.ReadBytes(abilitydescription));
        int flavordescription = reader.ReadInt32();
        FlavorDescription = Encoding.UTF8.GetString(reader.ReadBytes(flavordescription));
        int featuredescription = reader.ReadInt32();
        FeatureDescription = Encoding.UTF8.GetString(reader.ReadBytes(featuredescription));
        Ingredient1TID = reader.ReadInt32();
        Ingredient2TID = reader.ReadInt32();
        Ingredient3TID = reader.ReadInt32();
        int recipecode = reader.ReadInt32();
        RecipeCode = Encoding.UTF8.GetString(reader.ReadBytes(recipecode));

        LinkTable();
    }

    public void LinkTable()
    {
        IngredientTIDList.Add(Ingredient1TID);
        IngredientTIDList.Add(Ingredient2TID);
        IngredientTIDList.Add(Ingredient3TID);
    }
}
