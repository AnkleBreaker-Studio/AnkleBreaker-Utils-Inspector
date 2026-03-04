using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    [CustomPropertyDrawer(typeof(TableListAttribute))]
    public class TableListDrawer : PropertyDrawer
    {
        private const float RowHeight = 20f;
        private const float HeaderHeight = 22f;
        private const float Padding = 2f;
        private int _currentPage = 0;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!property.isArray)
            {
                EditorGUI.LabelField(position, label.text, "[TableList] requires array or List<>");
                return;
            }

            TableListAttribute attr = (TableListAttribute)attribute;
            float y = position.y;

            // Header with foldout + size
            Rect headerRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(headerRect, property.isExpanded, $"{label.text} [{property.arraySize}]", true);
            y += EditorGUIUtility.singleLineHeight + Padding;

            if (!property.isExpanded) return;

            if (property.arraySize == 0)
            {
                Rect emptyRect = new Rect(position.x + 16f, y, position.width - 16f, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(emptyRect, "(empty)", EditorStyles.centeredGreyMiniLabel);
                y += EditorGUIUtility.singleLineHeight;
            }
            else
            {
                // Get column names from first element
                SerializedProperty firstElement = property.GetArrayElementAtIndex(0);
                List<string> columns = new List<string>();
                SerializedProperty it = firstElement.Copy();
                int depth = it.depth;
                if (it.NextVisible(true))
                {
                    do
                    {
                        if (it.depth <= depth) break;
                        columns.Add(it.name);
                    } while (it.NextVisible(false));
                }

                if (columns.Count == 0)
                {
                    // Simple type array - just draw normally
                    for (int i = 0; i < property.arraySize; i++)
                    {
                        Rect rowRect = new Rect(position.x + 16f, y, position.width - 16f, RowHeight);
                        EditorGUI.PropertyField(rowRect, property.GetArrayElementAtIndex(i), GUIContent.none);
                        y += RowHeight + Padding;
                    }
                }
                else
                {
                    // Paging
                    int startIdx = 0;
                    int endIdx = property.arraySize;
                    if (attr.ShowPaging && property.arraySize > attr.MaxItemsPerPage)
                    {
                        int totalPages = Mathf.CeilToInt((float)property.arraySize / attr.MaxItemsPerPage);
                        _currentPage = Mathf.Clamp(_currentPage, 0, totalPages - 1);
                        startIdx = _currentPage * attr.MaxItemsPerPage;
                        endIdx = Mathf.Min(startIdx + attr.MaxItemsPerPage, property.arraySize);
                    }

                    // Column headers
                    float colWidth = (position.width - 36f) / columns.Count;
                    Rect headerBg = new Rect(position.x + 16f, y, position.width - 16f, HeaderHeight);
                    EditorGUI.DrawRect(headerBg, new Color(0.2f, 0.2f, 0.2f, 0.3f));

                    for (int c = 0; c < columns.Count; c++)
                    {
                        Rect colRect = new Rect(position.x + 18f + c * colWidth, y, colWidth, HeaderHeight);
                        EditorGUI.LabelField(colRect, ObjectNames.NicifyVariableName(columns[c]), EditorStyles.boldLabel);
                    }
                    y += HeaderHeight;

                    // Rows
                    for (int i = startIdx; i < endIdx; i++)
                    {
                        SerializedProperty element = property.GetArrayElementAtIndex(i);

                        if (i % 2 == 0)
                        {
                            Rect rowBg = new Rect(position.x + 16f, y, position.width - 16f, RowHeight);
                            EditorGUI.DrawRect(rowBg, new Color(0f, 0f, 0f, 0.08f));
                        }

                        SerializedProperty prop = element.Copy();
                        int d = prop.depth;
                        int col = 0;
                        if (prop.NextVisible(true))
                        {
                            do
                            {
                                if (prop.depth <= d) break;
                                if (col < columns.Count)
                                {
                                    Rect cellRect = new Rect(position.x + 18f + col * colWidth, y, colWidth - 4f, RowHeight);
                                    EditorGUI.PropertyField(cellRect, prop, GUIContent.none);
                                    col++;
                                }
                            } while (prop.NextVisible(false));
                        }
                        y += RowHeight + Padding;
                    }

                    // Paging controls
                    if (attr.ShowPaging && property.arraySize > attr.MaxItemsPerPage)
                    {
                        int totalPages = Mathf.CeilToInt((float)property.arraySize / attr.MaxItemsPerPage);
                        Rect pagingRect = new Rect(position.x + 16f, y, position.width - 16f, EditorGUIUtility.singleLineHeight);

                        float btnW = 40f;
                        Rect prevBtn = new Rect(pagingRect.x, pagingRect.y, btnW, pagingRect.height);
                        Rect pageLabel = new Rect(pagingRect.x + btnW, pagingRect.y, pagingRect.width - btnW * 2, pagingRect.height);
                        Rect nextBtn = new Rect(pagingRect.xMax - btnW, pagingRect.y, btnW, pagingRect.height);

                        if (GUI.Button(prevBtn, "<") && _currentPage > 0) _currentPage--;
                        EditorGUI.LabelField(pageLabel, $"Page {_currentPage + 1}/{totalPages}", EditorStyles.centeredGreyMiniLabel);
                        if (GUI.Button(nextBtn, ">") && _currentPage < totalPages - 1) _currentPage++;
                    }
                }
            }

            // Add/Remove buttons
            y += Padding;
            Rect buttonsRect = new Rect(position.x + 16f, y, position.width - 16f, EditorGUIUtility.singleLineHeight);
            float halfW = buttonsRect.width / 2f;
            if (GUI.Button(new Rect(buttonsRect.x, buttonsRect.y, halfW - 2f, buttonsRect.height), "+"))
                property.InsertArrayElementAtIndex(property.arraySize);
            if (GUI.Button(new Rect(buttonsRect.x + halfW + 2f, buttonsRect.y, halfW - 2f, buttonsRect.height), "-") && property.arraySize > 0)
                property.DeleteArrayElementAtIndex(property.arraySize - 1);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isArray)
                return EditorGUIUtility.singleLineHeight;

            float height = EditorGUIUtility.singleLineHeight + Padding;

            if (!property.isExpanded) return height;

            TableListAttribute attr = (TableListAttribute)attribute;

            if (property.arraySize == 0)
            {
                height += EditorGUIUtility.singleLineHeight;
            }
            else
            {
                int displayCount = property.arraySize;
                if (attr.ShowPaging && property.arraySize > attr.MaxItemsPerPage)
                    displayCount = Mathf.Min(attr.MaxItemsPerPage, property.arraySize);

                height += HeaderHeight;
                height += displayCount * (RowHeight + Padding);

                if (attr.ShowPaging && property.arraySize > attr.MaxItemsPerPage)
                    height += EditorGUIUtility.singleLineHeight + Padding;
            }

            height += EditorGUIUtility.singleLineHeight + Padding * 2;
            return height;
        }
    }
}
