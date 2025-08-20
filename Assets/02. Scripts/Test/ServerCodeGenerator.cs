using System.Text;
using System;

public static class ServerCodeGenerator
{
    // 사용할 문자 집합 (대문자 알파벳 + 숫자)
    private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    /// <summary>
    /// 서버 ID를 기반으로 5자리 랜덤 코드 생성
    /// </summary>
    public static string ToRoomCode(long serverId, int length = 5)
    {
        // serverId를 int로 줄여서 시드로 사용
        var seed = (int)(serverId & 0xFFFFFFFF);
        Random rng = new Random(seed);

        StringBuilder sb = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            int index = rng.Next(_chars.Length);
            sb.Append(_chars[index]);
        }
        return sb.ToString();
    }
}