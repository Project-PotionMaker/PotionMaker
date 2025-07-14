using UnityEngine.UIElements;
using System;
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
public static class SheetConnect
{
    // OAuth 클라이언트 ID.
    public const string CLIENT_ID = "836414527196-6v51b29h80deh9nbi1mseq1r8l7l9qj9.apps.googleusercontent.com";
    // OAuth 클라이언트 시크릿.
    public const string CLIENT_SECRET = "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";
    //
    private static string[] scopes = new string[] { SheetsService.Scope.Spreadsheets };

    public static SheetsService Connect(string applicationName)
    {
        var pass = new ClientSecrets();
        pass.ClientId = CLIENT_ID;
        pass.ClientSecret = CLIENT_SECRET;

        var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(pass, scopes, "OAuth client", CancellationToken.None).Result;

        var service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = applicationName,
            ApiKey = "AIzaSyB9RicImZ0KnmAFvFIpjMLC2yu6lX1VPgQ"
        });

        return service;
    }
}
