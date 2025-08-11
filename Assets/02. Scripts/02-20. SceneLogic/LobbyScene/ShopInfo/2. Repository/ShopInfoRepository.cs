using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ShopInfoRepository
{
    private const string SaveFileName = "ShopInfoSaveData.json";

    private List<GridSaveData> _gridSaveDataList = new List<GridSaveData>()
    {
        new GridSaveData(new Vector3Int(-5, 0, 4), 10000),
        new GridSaveData(new Vector3Int(-5, 0, 4), 10000), // 절구
        new GridSaveData(new Vector3Int(-3, 0, 4), 10003), // 가열 냄비
        new GridSaveData(new Vector3Int(-1, 0, 0), 10012), // 픽업테이블
        new GridSaveData(new Vector3Int(-1, 0, 1), 10012), // 픽업테이블
        new GridSaveData(new Vector3Int(-1, 0, 2), 10012), // 픽업테이블
        new GridSaveData(new Vector3Int(0, 0, 0), 10012), // 픽업테이블
        new GridSaveData(new Vector3Int(0, 0, 4), 10013), // 쓰레기통
        new GridSaveData(new Vector3Int(-5, 0, 0), 10014), // 계산기
        new GridSaveData(new Vector3Int(-1, 0, -5), 10015), // 허름한 의자
        new GridSaveData(new Vector3Int(0, 0, -5), 10016), // 푹신한 의자
        new GridSaveData(new Vector3Int(0, 0, 2), 10006), // 병입기
        new GridSaveData(new Vector3Int(4, 0, 2), 10017, 10006), // 식물 상자
        new GridSaveData(new Vector3Int(4, 0, 2), 10017, 10007), // 식물 상자
    };

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
            new Sales(10000), _gridSaveDataList, 3),
            new ShopInfo("테스트 포션상점2", new Currency(10), new Reputation(1.7f),
            new Sales(17000), _gridSaveDataList, 5),
            new ShopInfo("테스트 포션상점3", new Currency(50000), new Reputation(4.6f),
            new Sales(53500), _gridSaveDataList, 10)
        };
    }

    private void MakeShopInfo()
    {

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
