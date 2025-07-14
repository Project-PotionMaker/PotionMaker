using System.Collections.Generic;
using System;
using UnityEngine;

public class TableInfo : ScriptableObject
{
    // 실제 사용하는 테이블 이름.
    public string TableName;
    // 구글 시트에서 사용하는 이름. (_C, _S, _X 등이 붙어 있음)
    public string SheetName;
    // 클라이언트에서 사용하는 파일인지.
    public bool IsUseClient = true;
    // 서버에서 사용하는 파일인지.
    public bool IsUseServer = true;
    // 지정어에 해당하는 파일인지..
    public bool IsUseAppointWord = false;
    // 추출할 파일인지.
    public bool IsApply = false;
    // 테이블 데이터.
    public System.Data.DataTable Table;
    // 헤더 정보.
    public List<Header> Headers = new List<Header>();
    // 리스트 헤더 정보.
    public List<Header> ListHeaders = new List<Header>();
}

public class Header
{
    // 변수타입.
    public string VariableType = string.Empty;
    // 변수설명.
    public string Description = string.Empty;
    // 변수명.
    public string VariableName = string.Empty;
    // 리스트 이름 (리스트일 경우에만).
    public string ListName = string.Empty;
}

public class EnumInfo : ScriptableObject
{
    // 실제 사용하는 Enum 이름.
    public string EnumName;
    // 구글 시트에서 사용하는 이름. (_M 등이 붙어 있음)
    public string SheetName;
    // Max 멤버를 추가할 것인지.
    public bool IsAddMax = false;
    // Enum 멤버 정보.
    public List<EnumMember> EnumMembers = new List<EnumMember>();
}

[Serializable]
public class EnumMember
{
    // Enum 멤버값.
    public int Value;
    // Enum 멤버명.
    public string MemberName;
    // Enum 멤버 설명.
    public string Description;
}

public static class StringExtensions
{
    public static string ToSnakeCase(this string str)
    {
        string text = str;

        if (text == null)
        {
            return null;
        }

        text = text.Replace("TID", "Tid");
        text = text.Replace("UI", "Ui");
        text = text.Replace("ID", "Id");
        text = text.Replace("PVP", "Pvp");

        char beforeC = '0';
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < text.Length; ++i)
        {
            char c = text[i];
            if (char.IsUpper(c) == true && char.IsUpper(beforeC) == false)
            {
                if (i != 0)
                {
                    sb.Append('_');
                }
            }

            sb.Append(char.ToLowerInvariant(c));
            beforeC = c;
        }
        return sb.ToString();
    }
}