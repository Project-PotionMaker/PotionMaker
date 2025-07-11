using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System;

public partial class DataExporterEditor : EditorWindow
{
    private List<string> Logs = new List<string>();

    private ListView LogView;
    private ListView TableView;
    private ListView EnumView;

    [MenuItem("PotionMakerSheet/DataExporter %#t")]
    public static void Init()
    {
        DataExporterEditor window = GetWindow(typeof(DataExporterEditor)) as DataExporterEditor;
        window.minSize = new Vector2(800, 600);
        window.Show();
    }

    private void OnEnable()
    {
        #region Areas

        var root = rootVisualElement;
        root.name = "Root";
        root.style.flexDirection = FlexDirection.Row;
        root.style.marginTop = 5;
        root.style.marginBottom = 5;

        var actionArea = new VisualElement();
        actionArea.name = "ActionArea";
        actionArea.style.width = 180;
        root.Add(actionArea);

        var enumArea = new VisualElement();
        enumArea.name = "EnumArea";
        enumArea.style.width = 200;
        root.Add(enumArea);

        var tableArea = new VisualElement();
        tableArea.name = "TableArea";
        tableArea.style.width = 200;
        tableArea.style.marginLeft = 3;
        root.Add(tableArea);

        var logArea = new VisualElement();
        logArea.name = "LogArea";
        logArea.StretchToParentSize();
        logArea.style.marginLeft = 586;
        logArea.style.marginRight = 5;
        root.Add(logArea);

        #endregion

        #region Action Area

        var importButton = new Button(OnClickImport);
        importButton.name = "ImportButton";
        importButton.text = "Import";
        actionArea.Add(importButton);

        var importDesciption = new Label();
        importDesciption.name = "ImportDesciption";
        importDesciption.style.fontSize = 10;
        importDesciption.style.color = Color.gray;
        importDesciption.style.unityTextAlign = TextAnchor.MiddleLeft;
        importDesciption.style.marginBottom = 15;
        importDesciption.text = "  WAK_ZA_DataTable.sheet 로드\n  WAK_ZA_Enum.sheet 로드";
        actionArea.Add(importDesciption);

        var appointWord = new TextField();
        appointWord.name = "AppointWord";
        appointWord.value = AppointWord;
        appointWord.RegisterValueChangedCallback(OnValueChangedAppointWord);
        actionArea.Add(appointWord);

        var appointWordDesciption = new Label();
        appointWordDesciption.name = "AppointWordDesciption";
        appointWordDesciption.style.fontSize = 10;
        appointWordDesciption.style.color = Color.gray;
        appointWordDesciption.style.unityTextAlign = TextAnchor.MiddleLeft;
        appointWordDesciption.style.marginBottom = 15;
        appointWordDesciption.text = "  sheet명 뒤에 붙일 지정어 [선택]\n  (C, S, V 제외)";
        actionArea.Add(appointWordDesciption);


        var clientToggle = new Toggle();
        clientToggle.name = "ClientToggle";
        clientToggle.text = "클라이언트 추출";
        clientToggle.value = IsApplyClient;
        clientToggle.RegisterValueChangedCallback(OnToggleValueChangedClient);
        actionArea.Add(clientToggle);

        var binaryToggle = new Toggle();
        binaryToggle.name = "BinaryToggle";
        binaryToggle.text = "바이너리만 추출";
        binaryToggle.value = IsBinaryOnly;
        binaryToggle.RegisterValueChangedCallback(OnToggleValueChangedBinary);
        actionArea.Add(binaryToggle);

        var clientDesciption = new Label();
        clientDesciption.name = "ClientDesciption";
        clientDesciption.style.fontSize = 10;
        clientDesciption.style.color = Color.gray;
        clientDesciption.style.marginBottom = 15;
        clientDesciption.text = "  스크립트 파일 추출 경로\n    : Scripts/DataTable/*.cs\n  바이너리 파일 추출 경로\n    : Resources/DataTable/*.bytes";
        actionArea.Add(clientDesciption);

        var exportButton = new Button(OnClickExport);
        exportButton.name = "ExportButton";
        exportButton.text = "Export";
        actionArea.Add(exportButton);
        #endregion

        #region Table Area

        Func<VisualElement> makeItem_table = () => new Toggle();
        Action<VisualElement, int> bindItem_table = (e, i) =>
        {
            Toggle toggle = e as Toggle;

            toggle.labelElement.style.unityTextAlign = TextAnchor.LowerCenter;
            if (TableInfos[i].IsUseAppointWord == true)
            {
                toggle.label = AppointWord;
                toggle.labelElement.style.color = new Color(0.8f, 0.9f, 0.9f);
            }
            else if (TableInfos[i].IsUseClient == true && TableInfos[i].IsUseServer == false)
            {
                toggle.label = "C";
                toggle.labelElement.style.color = new Color(0, 0.5f, 0.5f);
            }
            else if (TableInfos[i].IsUseClient == false && TableInfos[i].IsUseServer == true)
            {
                toggle.label = "S";
                toggle.labelElement.style.color = new Color(0.576f, 0.439f, 0.858f);
            }
            else
            {
                toggle.label = " ";
            }

            toggle.labelElement.style.minWidth = 11;
            toggle.labelElement.style.maxWidth = 11;
            toggle.text = TableInfos[i].TableName;
            toggle.value = TableInfos[i].IsApply;
            toggle.RegisterValueChangedCallback(x => TableInfos[i].IsApply = x.newValue);
        };

        var tableBox = new Box();
        tableBox.name = "TableArea";
        tableBox.StretchToParentSize();
        tableBox.style.marginTop = 70;
        tableBox.style.borderTopLeftRadius = 5;
        tableBox.style.borderTopRightRadius = 5;
        tableBox.style.borderBottomLeftRadius = 5;
        tableBox.style.borderBottomRightRadius = 5;
        tableBox.style.backgroundColor = new Color(0.35f, 0.35f, 0.35f);
        tableArea.Add(tableBox);

        var tableView = new ListView();
        tableView.itemsSource = TableInfos;
        tableView.fixedItemHeight = 16;
        tableView.makeItem = makeItem_table;
        tableView.bindItem = bindItem_table;
        tableView.selectionType = SelectionType.Single;
        tableView.onItemsChosen += OnItemChosenTable;
        tableView.style.flexGrow = 1.0f;
        tableBox.Add(tableView);
        TableView = tableView;

        var tableButtonBox = new ListView();
        tableButtonBox.name = "TableButtonArea";
        tableButtonBox.StretchToParentSize();
        tableButtonBox.style.marginTop = 20;
        tableButtonBox.style.height = 45;
        tableButtonBox.style.borderTopLeftRadius = 5;
        tableButtonBox.style.borderTopRightRadius = 5;
        tableButtonBox.style.borderBottomLeftRadius = 5;
        tableButtonBox.style.borderBottomRightRadius = 5;
        tableArea.Add(tableButtonBox);

        var selectAllButton = new Button(OnClickSelectAll);
        selectAllButton.name = "SelectAllButton";
        selectAllButton.text = "Select All";
        tableButtonBox.hierarchy.Add(selectAllButton);
        #endregion

        #region Log Area

        Func<VisualElement> makeItem_log = () => new Label();
        Action<VisualElement, int> bindItem_log = (e, i) =>
        {
            Label label = e as Label;
            label.text = Logs[i];
            if (ExporterEditor.IsLogType(Logs[i], ExporterEditor.LogType.ERROR))
            {
                label.style.color = Color.red;
                label.style.width = 1000;
            }
            else if (ExporterEditor.IsLogType(Logs[i], ExporterEditor.LogType.NOTICE))
            {
                label.style.color = Color.yellow;
                label.style.width = 1000;
            }
            else
            {
                label.style.color = Color.white;
            }
        };

        var logBox = new Box();
        logBox.name = "LogArea";
        logBox.StretchToParentSize();
        logBox.style.marginTop = 20;
        logBox.style.borderTopLeftRadius = 5;
        logBox.style.borderTopRightRadius = 5;
        logBox.style.borderBottomLeftRadius = 5;
        logBox.style.borderBottomRightRadius = 5;
        logBox.style.backgroundColor = new Color(0.35f, 0.35f, 0.35f);
        logArea.Add(logBox);

        var logView = new ListView();
        logView.itemsSource = Logs;
        logView.fixedItemHeight = 16;
        logView.makeItem = makeItem_log;
        logView.bindItem = bindItem_log;
        logView.selectionType = SelectionType.Multiple;
        logView.style.flexGrow = 1.0f;
        logBox.Add(logView);
        LogView = logView;

        var logClearButton = new Button(OnClickLogClear);
        logClearButton.name = "LogClearButton";
        logClearButton.text = "Log Clear";
        logClearButton.style.alignSelf = Align.FlexEnd;
        logArea.Add(logClearButton);

        #endregion
    }

    #region ActionArea

    private void OnClickImport()
    {
        Import();
    }

    private void OnClickExport()
    {
        Export();
    }

    private void OnValueChangedServerPath(ChangeEvent<string> e)
    {
        ServerPath = e.newValue;
    }

    private void OnValueChangedAppointWord(ChangeEvent<string> e)
    {
        AppointWord = e.newValue;
    }

    private void OnToggleValueChangedClient(ChangeEvent<bool> e)
    {
        IsApplyClient = e.newValue;
    }

    private void OnToggleValueChangedServer(ChangeEvent<bool> e)
    {
        IsApplyServer = e.newValue;
    }

    private void OnToggleValueChangedBinary(ChangeEvent<bool> e)
    {
        IsBinaryOnly = e.newValue;
    }

    #endregion

    #region EnumArea

    private void RefreshEnumNames()
    {
        EnumView.Rebuild();
    }

    #endregion

    #region TableArea

    private void RefreshTableNames()
    {
        TableView.Rebuild();
    }

    private void OnItemChosenTable(object obj)
    {
        TableInfo tableInfo = obj as TableInfo;

        if (tableInfo.IsUseClient == true)
        {
            DataReaderEditor.Show(tableInfo);
        }
        else
        {
            LogNoticeMessage($"{tableInfo.TableName} 파일은 클라이언트에서 사용되지 않습니다.");
        }
    }

    private void OnClickSelectAll()
    {
        ExporterEditor.SelectAll(TableInfos, TableView);
    }

    private void OnClickSelectDefenseAll()
    {
        ExporterEditor.SelectAllStartWith(TableInfos, TableView, new string[] { "DefenseBattle", "Buff" });
    }

    private void OnClickSelectHousingAll()
    {
        ExporterEditor.SelectAllStartWith(TableInfos, TableView, new string[] { "Housing" });
    }

    private void OnClickSelectShopAll()
    {
        ExporterEditor.SelectAllStartWith(TableInfos, TableView, new string[] { "Shop" });
    }
    #endregion

    #region LogArea

    private void OnClickLogClear()
    {
        ExporterEditor.ClearLog(Logs, LogView);
    }

    private void LogMessage(string log, bool showDate = true)
    {
        ExporterEditor.AddLog(ExporterEditor.LogType.NONE, Logs, log, showDate);
        ExporterEditor.RefreshLog(LogView);
    }

    private void LogNoticeMessage(string log, bool showDate = true)
    {
        ExporterEditor.AddLog(ExporterEditor.LogType.NOTICE, Logs, log, showDate);
        ExporterEditor.RefreshLog(LogView);
    }

    private void LogErrorMessage(string log, bool showDate = true)
    {
        ExporterEditor.AddLog(ExporterEditor.LogType.ERROR, Logs, log, showDate);
        ExporterEditor.RefreshLog(LogView);
    }

    #endregion
}
