using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class DataReaderEditor : EditorWindow
{
    private readonly Color LIGHT_COLOR = Color.white * 0.3f;
    private readonly Color DARK_COLOR = Color.white * 0.1f;

    private MultiColumnHeaderState HeaderState;
    private MultiColumnHeader Header;
    private MultiColumnHeaderState.Column[] Columns;

    private List<List<string>> HistoryData = new List<List<string>>();
    private int HeaderSize;

    private Vector2 ScrollPos;

    public static void Show(TableInfo tableInfo)
    {
        DataReaderEditor window = GetWindow(typeof(DataReaderEditor)) as DataReaderEditor;
        window.minSize = new Vector2(800, 600);
        window.LoadHistory(tableInfo);
        window.Initialize();
        window.ShowUtility();
    }

    private void LoadHistory(TableInfo tableInfo)
    {
        HistoryData.Clear();
        HeaderSize = 0;

        TextAsset asset = EditorGUIUtility.Load($"DataTable/{tableInfo.TableName}Data.txt") as TextAsset;
        string[] strarr = asset.text.Split('\n');
        for (int i = 0; i < strarr.Length; ++i)
        {
            if (strarr[i].Length == 0)
            {
                continue;
            }

            strarr[i] = strarr[i].Replace("\r", string.Empty);
            if (strarr[i].EndsWith("\t") == true)
            {
                strarr[i] = strarr[i].Remove(strarr[i].Length - 1, 1);
            }

            string[] strarr2 = strarr[i].Split('\t');

            if (HeaderSize == 0)
            {
                HeaderSize = strarr2.Length;
            }

            List<string> stringList = new List<string>();
            HistoryData.Add(stringList);

            for (int ii = 0; ii < strarr2.Length; ++ii)
            {
                stringList.Add(strarr2[ii]);
            }
        }
    }

    private void Initialize()
    {
        Columns = new MultiColumnHeaderState.Column[HeaderSize];

        for (int i = 0; i < Columns.Length; ++i)
        {
            Columns[i] = new MultiColumnHeaderState.Column()
            {
                allowToggleVisibility = false,
                autoResize = true,
                canSort = false,
                sortingArrowAlignment = TextAlignment.Center,
                headerContent = new GUIContent("col" + (i+1)),
                headerTextAlignment = TextAlignment.Center,
            };
        }
         
        HeaderState = new MultiColumnHeaderState(Columns);
        Header = new MultiColumnHeader(HeaderState);
        Header.visibleColumnsChanged += (multiColumnHeader) => multiColumnHeader.ResizeToFit();
    }

    private void OnGUI()
    {
        if (Header == null)
        {
            return;
        }

        GUILayout.FlexibleSpace();

        Rect windowRect = GUILayoutUtility.GetLastRect();
        windowRect.width = position.width;
        windowRect.height = position.height;

        Rect viewRect = new Rect(windowRect)
        {
            xMax = this.Columns.Sum((column) => column.width),
            yMax = HistoryData.Count * EditorGUIUtility.singleLineHeight
        };

        Rect columnRectPrototype = new Rect(windowRect)
        {
            width = viewRect.xMax,
            height = EditorGUIUtility.singleLineHeight,
        };

        Rect positionalRectAreaOfScrollView = GUILayoutUtility.GetRect(0, float.MaxValue, 0, float.MaxValue);
        ScrollPos = GUI.BeginScrollView(positionalRectAreaOfScrollView, ScrollPos, viewRect, false, false);

        Header.OnGUI(columnRectPrototype, 0.0f);

        for (int i = 0; i < HistoryData.Count; ++i)
        {
            Rect rowRect = new Rect(columnRectPrototype);
            rowRect.y += EditorGUIUtility.singleLineHeight * (i + 1);
            EditorGUI.DrawRect(rowRect, (i % 2 == 0) ? DARK_COLOR : LIGHT_COLOR);

            for (int ii = 0; ii < HeaderSize; ++ii)
            {
                if (Header.IsColumnVisible(ii) == true)
                {
                    int width = HistoryData[i][ii].Length * 12;
                    if (Header.GetColumn(ii).width < width)
                    {
                        Header.GetColumn(ii).width = width;
                    }

                    int visibleColumnIndex = Header.GetVisibleColumnIndex(ii);

                    Rect columnRect = Header.GetColumnRect(visibleColumnIndex);
                    columnRect.y = rowRect.y;

                    GUIStyle nameFieldGUIStyle = new GUIStyle(GUI.skin.label)
                    {
                        padding = new RectOffset(left: 10, right: 10, top: 2, bottom: 2),
                        alignment = TextAnchor.MiddleCenter
                    };

                    EditorGUI.LabelField(
                        position: Header.GetCellRect(visibleColumnIndex, columnRect),
                        label: new GUIContent(HistoryData[i][ii]),
                        style: nameFieldGUIStyle
                    );
                }
            }
        }

        GUI.EndScrollView(true);
    }
}
