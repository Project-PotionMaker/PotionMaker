using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class ShopInfoRepository
{
    private const string SaveFileName = "ShopInfoSaveData.json";
    public void Save(int slotIndex, ShopInfo shopInfo)
    {
        // 1. 기존 데이터 스팀 클라우드에서 로드 
        // List<ShopInfo> shopInfoList = Load();
        // shopInfoList[slotIndex] = shopInfo;

        // 2. 전체 리스트를 다시 JSON으로 직렬화하여 저장
        //string updatedJson = JsonConvert.SerializeObject(ConvertToDTOList(shopInfoList));
        //File.WriteAllText(SaveFileName, updatedJson);
    }

    public List<ShopInfo> Load()
    {
        // 1. 스팀 클라우드에서 Json파일을 로드 
        // List<ShopInfoDTO> loadedShopInfoDtoList = ??
        // 2. List<ShopInfoDTO> -> List<ShopInfo>로 변환하여 반환
        // return ConvertToShopInfoList(loadedShopInfoDtoList);


        return new List<ShopInfo>()
        {
            new ShopInfo("테스트 포션상점1", new Currency(2000), new Reputation(2.5f), 
            new Sales(10000), 3),
            new ShopInfo("테스트 포션상점2", new Currency(10), new Reputation(1.7f),
            new Sales(17000), 5),
            new ShopInfo("테스트 포션상점3", new Currency(50000), new Reputation(4.6f),
            new Sales(53500), 10)
        };
    }

    private List<ShopInfoDTO> ConvertToDTOList(List<ShopInfo> shopInfoList)
    {
        if (shopInfoList == null)
        {
            return new List<ShopInfoDTO>();
        }
        return shopInfoList.Select(shopInfo => shopInfo.ToDTO()).ToList();
    }

    private List<ShopInfo> ConvertToShopInfoList(List<ShopInfoDTO> shopInfoDtoList)
    {
        if (shopInfoDtoList == null)
        {
            return new List<ShopInfo>();
        }
        return shopInfoDtoList.Select(dto => dto != null ? new ShopInfo(dto) : null).ToList();
    }

}
