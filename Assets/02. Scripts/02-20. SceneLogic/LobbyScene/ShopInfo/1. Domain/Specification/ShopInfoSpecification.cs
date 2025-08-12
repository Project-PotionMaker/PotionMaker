using System;
using System.IO;
using System.Text.RegularExpressions;

public class ShopInfoSpecification
{
    private static readonly Regex ShopNameRegex =
    new Regex(@"^[0-9가-힣a-zA-Z\s]{2,15}$", RegexOptions.Compiled);

    public string ErrorMessage { get; private set; }

    public bool IsSatisfied(string shopName, Currency currency, Reputation reputation, int day)
    {
        if (string.IsNullOrEmpty(shopName))
        {
            ErrorMessage = "포션 상점명은 비어있을 수 없습니다.";
            return false;
        }

        if (!ShopNameRegex.IsMatch(shopName))
        {
            ErrorMessage = "포션 상점명은 2자 이상 15자 이하의 한글 또는 영문이어야 합니다.";
            return false;
        }

        if (day < 0)
        {
            ErrorMessage = "진행일 수는 0보다 커야 합니다.";
            return false;
        }
        return true;
    }
}
