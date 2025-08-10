using System.Collections.Generic;



public class ShopInfoRepository
{
    public void Save(ShopInfo shopInfo)
    {

    }

    public List<ShopInfo> Load()
    {
        return new List<ShopInfo>()
        {
            new ShopInfo("테스트 포션상점1", new Currency(2000), new Reputation(2.5f), 3),
            new ShopInfo("테스트 포션상점2", new Currency(10), new Reputation(1.7f), 5),
            new ShopInfo("테스트 포션상점3", new Currency(50000), new Reputation(4.6f), 10)
        };
    }
}
