using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ShopInfoRepository
{
    private const string SaveFileName = "ShopInfoSaveData.json";

    private List<GridSaveData> _gridSaveDataList = new List<GridSaveData>()
    {
        new GridSaveData(new Vector3Int(-6, 0, 3), 10000), // 절구
        new GridSaveData(new Vector3Int(-5, 0, 3), 10000), // 절구
        new GridSaveData(new Vector3Int(-4, 0, 3), 10001), // 분쇄기
        new GridSaveData(new Vector3Int(-2, 0, 3), 10002), // 혼합기
        new GridSaveData(new Vector3Int(-1, 0, 3), 10003), // 가열 냄비
        new GridSaveData(new Vector3Int(-3, 0, 3), 10012), // 픽업테이블
        new GridSaveData(new Vector3Int(-5, 0, 0), 10012), // 픽업테이블
        new GridSaveData(new Vector3Int(-4, 0, 0), 10012), // 픽업테이블
        new GridSaveData(new Vector3Int(1, 0, 3), 10013), // 쓰레기통
        new GridSaveData(new Vector3Int(-2, 0, 0), 10006), // 병입기
        new GridSaveData(new Vector3Int(2, 0, 3), 10017, 10005), // 식물 상자
        new GridSaveData(new Vector3Int(3, 0, 3), 10017, 10006), // 식물 상자
        new GridSaveData(new Vector3Int(4, 0, 3), 10017, 10007), // 식물 상자
        new GridSaveData(new Vector3Int(5, 0, 3), 10018, 20002), // 동물 상자
        new GridSaveData(new Vector3Int(-2, 0, -4), 10015), // 허름한 의자
        new GridSaveData(new Vector3Int(-1, 0, -4), 10015), // 허름한 의자
        new GridSaveData(new Vector3Int(0, 0, -4), 10015), // 허름한 의자
    };

    public void Save(ShopInfo shopInfo)
    {
        int slotIndex = shopInfo.SlotIndex;
        if (slotIndex < 0 || 2 < slotIndex)
        {
            throw new ArgumentOutOfRangeException
                ($"잘못된 슬롯 인덱스({slotIndex})입니다. 0에서 2 사이의 값만 유효합니다.");
        }

        string savePath = Path.Combine(Application.persistentDataPath, SaveFileName);
        List<ShopInfoDTO> shopInfoDtoList;

        // 1. 기존 데이터 불러오기
        if (File.Exists(savePath))
        {
            string jsonData = File.ReadAllText(savePath);
            shopInfoDtoList = JsonConvert.DeserializeObject<List<ShopInfoDTO>>(jsonData);

            // 파일이 손상되었거나, 예전에 다른 크기로 저장했을 경우를 대비해 크기를 3으로 강제 보정
            if (shopInfoDtoList == null || shopInfoDtoList.Count != 3)
            {
                shopInfoDtoList = new List<ShopInfoDTO>(new ShopInfoDTO[3]);
            }
        }
        else
        {
            // 2. 파일이 없으면 3개의 null 슬롯을 가진 리스트로 새로 초기화
            shopInfoDtoList = new List<ShopInfoDTO>(new ShopInfoDTO[3]);
        }

        // 3. 지정된 슬롯 인덱스에 데이터 덮어쓰기
        shopInfoDtoList[slotIndex] = shopInfo?.ToDTO();

        // 4. JSON으로 변환하여 파일에 저장
        string updatedJson = JsonConvert.SerializeObject(shopInfoDtoList, Formatting.Indented);
        File.WriteAllText(savePath, updatedJson);

        Debug.Log($"슬롯 {slotIndex}에 데이터 저장 완료. 경로: {savePath}");
    }

    public List<ShopInfo> Load()
    {
        // 1. 스팀 클라우드에서 Json파일을 로드 
        // List<ShopInfoDTO> loadedShopInfoDtoList = ??
        // 2. List<ShopInfoDTO> -> List<ShopInfo>로 변환하여 반환
        // return ConvertToShopInfoList(loadedShopInfoDtoList);


        return new List<ShopInfo>()
        {
            new ShopInfo("테스트 포션상점1", 0, 3, 1, new Currency(2000), new Reputation(2.5f), 
            new Sales(10000), _gridSaveDataList),
            new ShopInfo("테스트 포션상점2", 1, 7, 2, new Currency(10000), new Reputation(4.7f),
            new Sales(17000), _gridSaveDataList),
            new ShopInfo("테스트 포션상점3", 2, 15, 3, new Currency(50000), new Reputation(3.4f),
            new Sales(53500), _gridSaveDataList)
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
