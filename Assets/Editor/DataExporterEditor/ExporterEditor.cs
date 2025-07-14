using UnityEngine.UIElements;
using System;
using System.Linq;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Google.Apis.Sheets.v4;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;

public static class ExporterEditor
{
    public enum LogType
    {
        NONE = 0,
        NOTICE,
        ERROR
    }

    // OAuth 클라이언트 ID.
    public const string CLIENT_ID = "836414527196-6v51b29h80deh9nbi1mseq1r8l7l9qj9.apps.googleusercontent.com";
    // OAuth 클라이언트 시크릿.
    public const string CLIENT_SECRET = "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";

    public static SheetsService Connect()
    {
        var pass = new ClientSecrets();
        pass.ClientId = CLIENT_ID;
        pass.ClientSecret = CLIENT_SECRET;

        var scopes = new string[] { SheetsService.Scope.SpreadsheetsReadonly };
        var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(pass, scopes, "OAuth client", CancellationToken.None).Result;

        var service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "ExporterEditor"
        });

        return service;
    }

    public static string GetLowerString(string temp)
    {
        if (string.IsNullOrEmpty(temp) == true)
        {
            return string.Empty;
        }

        string remainder = temp.Substring(1);
        string first = temp.Replace(remainder, string.Empty);
        first = first.ToLower();

        return first + remainder;
    }

    public static void SelectAll(List<TableInfo> tableInfos, ListView listView)
    {
        bool notAll = tableInfos.Find((x) => x.IsApply == false) != null;

        for (int i = 0; i < tableInfos.Count; ++i)
        {
            tableInfos[i].IsApply = notAll;
        }

        listView.Rebuild();
    }

    public static void SelectAllStartWith(List<TableInfo> tableInfos, ListView listView, string[] startWithArray)
    {
        bool notAll = tableInfos.Find((x) => startWithArray.Any(y => x.TableName.StartsWith(y)) && x.IsApply == false) != null;

        for (int i = 0; i < tableInfos.Count; ++i)
        {
            var tableInfo = tableInfos[i];
            if (!startWithArray.Any(x => tableInfo.TableName.StartsWith(x)))
            {
                continue;
            }

            tableInfo.IsApply = notAll;
        }

        listView.Rebuild();
    }

    public static void SheetToDataTable(TableInfo tableInfo, IList<IList<object>> value, bool removeComment)
    {
        tableInfo.Table = new System.Data.DataTable();
        tableInfo.Table.TableName = tableInfo.TableName;

        // 첫번째 줄은 변수명.
        SetVariableColumns(tableInfo, value);

        // 모든 데이터 테이블에 넣기.
        for (int row = 0; row < value.Count; ++row)
        {
            if (value[row].Count != tableInfo.Table.Columns.Count)
            {
                UnityEngine.Debug.Log("error row : " + row);
                UnityEngine.Debug.Log("row count : " + value[row].Count);
            }
            else
            {
                DataRow dr = tableInfo.Table.NewRow();
                for (int col = 0; col < tableInfo.Table.Columns.Count; ++col)
                {
                    dr[col] = value[row][col].ToString();
                }
                tableInfo.Table.Rows.Add(dr);
            }
        }

        // 변수 타입이 comment인 컬럼 제거.
        if (removeComment == true)
        {
            IList<object> variableTypes = value[2];
            for (int i = variableTypes.Count - 1; i >= 0; --i)
            {
                if (variableTypes[i].ToString().ToLower().Equals("comment") == true)
                {
                    tableInfo.Table.Columns.RemoveAt(i);
                }
            }
        }
    }

    public static void SetVariableColumns(TableInfo tableInfo, IList<IList<object>> value)
    {
        IList<object> variableNames = value[0];
        for (int i = 0; i < variableNames.Count; ++i)
        {
            DataColumn dataColumn = new DataColumn(variableNames[i].ToString());
            tableInfo.Table.Columns.Add(dataColumn);
        }
    }

    public static void SetHeaders(TableInfo tableInfo)
    {
        List<Header> headers = new List<Header>();
        List<Header> listHeaders = new List<Header>();

        DataRow description = tableInfo.Table.Rows[1];
        DataRow variableType = tableInfo.Table.Rows[2];

        for (int col = 0; col < tableInfo.Table.Columns.Count; ++col)
        {
            Header header = new Header();
            header.VariableType = variableType[col].ToString();
            header.Description = description[col].ToString();
            header.VariableName = tableInfo.Table.Columns[col].ColumnName;

            if (Regex.IsMatch(header.VariableName, "[0-9]+$") == true)
            {
                header.ListName = Regex.Replace(header.VariableName, "[0-9]+$", string.Empty);
            }
            else if (Regex.IsMatch(header.VariableName, @"\D+[0-9]+\D+") == true)
            {
                header.ListName = Regex.Replace(header.VariableName, "[0-9]+", string.Empty);
            }

            if (header.ListName.Equals(string.Empty) == false)
            {
                if (listHeaders.Find(x => x.ListName.Equals(header.ListName) == true) == null)
                {
                    listHeaders.Add(header);
                }
            }

            headers.Add(header);
        }

        tableInfo.Headers = headers;
        tableInfo.ListHeaders = listHeaders;
    }

    public static TypeCode GetTypeCode(string typeName)
    {
        if (typeName.Equals("int") == true)
        {
            return TypeCode.Int32;
        }
        else if (typeName.Equals("long") == true)
        {
            return TypeCode.Int64;
        }
        else if (typeName.Equals("byte") == true)
        {
            return TypeCode.Byte;
        }
        else if (typeName.Equals("float") == true)
        {
            return TypeCode.Single;
        }
        else if (typeName.Equals("short") == true)
        {
            return TypeCode.Int16;
        }
        else if (typeName.Equals("bool") == true)
        {
            return TypeCode.Boolean;
        }
        else if (typeName.Equals("string") == true)
        {
            return TypeCode.String;
        }
        else if(typeName.Equals("char") == true)
        {
            return TypeCode.Char;
        }
        else
        {
            return TypeCode.Empty;
        }
    }

    public static byte[] GetBytes(TypeCode type, string value)
    {
        switch (type)
        {
            case TypeCode.Empty:
                {
                    return null;
                }
            case TypeCode.String:
                {
                    List<byte> string_all_bytes = new List<byte>();
                    byte[] string_bytes = Encoding.UTF8.GetBytes(value);
                    int lengthValue = string_bytes.Length;
                    int length_size = Marshal.SizeOf(lengthValue);
                    byte[] length_bytes = new byte[length_size];
                    IntPtr string_ptr = Marshal.AllocHGlobal(length_size);
                    Marshal.StructureToPtr(lengthValue, string_ptr, true);
                    Marshal.Copy(string_ptr, length_bytes, 0, length_size);
                    Marshal.FreeHGlobal(string_ptr);
                    string_all_bytes.AddRange(length_bytes);
                    string_all_bytes.AddRange(string_bytes);

                    return string_all_bytes.ToArray();
                }
            default:
                {
                    var convertedValue = Convert.ChangeType(value, type);
                    int size = Marshal.SizeOf(convertedValue);
                    if (type == TypeCode.Boolean)
                    {
                        size = 1;
                    }

                    byte[] bytes = new byte[size];

                    IntPtr ptr = Marshal.AllocHGlobal(size);
                    Marshal.StructureToPtr(convertedValue, ptr, true);
                    Marshal.Copy(ptr, bytes, 0, size);
                    Marshal.FreeHGlobal(ptr);

                    return bytes;
                }
        }
    }

    public static bool IsLogType(string log, LogType type)
    {
        return log.StartsWith(type.ToString());
    }

    public static void ClearLog(List<string> logs, ListView view)
    {
        if (logs != null) logs.Clear();

        if (view != null) view.Clear();
    }

    public static async void RefreshLog(ListView logView)
    {
        if (logView == null) return;

        logView.Rebuild();
        await Task.Delay(1);
        logView.ScrollToItem(logView.itemsSource.Count - 1);
    }

    public static void AddLog(LogType type, List<string> logs, string log, bool showDate = true)
    {
        if (logs == null) return;

        if (showDate == true)
        {
            switch (type)
            {
                case LogType.NONE: logs.Add($"{log}  ({DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")})"); break;
                default: logs.Add($"{type}: {log}  ({DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")})"); break;
            }
        }
        else
        {
            switch (type)
            {
                case LogType.NONE: logs.Add(log); ; break;
                default: logs.Add($"{type}: {log}"); break;
            }
        }
    }
}