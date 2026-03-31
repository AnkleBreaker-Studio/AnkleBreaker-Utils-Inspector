using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    /// <summary>
    /// Odin-style PropertyDrawer for AB_SerializedDictionary.
    ///
    /// Staging values live in memory (not in the serialized arrays) so they
    /// never interfere with the Dictionary serialization round-trip.
    /// They are only committed to the arrays when the user clicks "Add".
    ///
    /// Pagination: displays PageSize entries at a time with Prev/Next controls.
    /// </summary>
    [CustomPropertyDrawer(typeof(AB_SerializedDictionary<,>), true)]
    public class AB_SerializedDictionaryPropertyDrawer : PropertyDrawer
    {
        // ── Layout ──────────────────────────────────────────────────
        private const float HeaderH       = 22f;
        private const float ColHeaderH    = 18f;
        private const float RemoveBtnW    = 22f;
        private const float Pad           = 2f;
        private const float BoxPad        = 4f;
        private const float PlusBtnW      = 22f;
        private const float ItemsLabelW   = 60f;
        private const float SepH          = 1f;
        private const float StagingLabelW = 46f;
        private const float AddBtnH       = 20f;
        private const float RowH          = 18f;
        private const float PaginationH   = 22f;
        private const int   PageSize      = 20;

        // ── Colors ──────────────────────────────────────────────────
        static Color C_HeaderBg(bool d)  => d ? new Color(0.21f, 0.21f, 0.21f) : new Color(0.72f, 0.72f, 0.72f);
        static Color C_BodyBg(bool d)    => d ? new Color(0.24f, 0.24f, 0.24f) : new Color(0.84f, 0.84f, 0.84f);
        static Color C_Sep(bool d)       => d ? new Color(0.14f, 0.14f, 0.14f) : new Color(0.55f, 0.55f, 0.55f);
        static Color C_RowEven(bool d)   => d ? new Color(0.25f, 0.25f, 0.25f, 0.3f) : new Color(0.80f, 0.80f, 0.80f, 0.3f);
        static Color C_RowOdd(bool d)    => d ? new Color(0.28f, 0.28f, 0.28f, 0.3f) : new Color(0.76f, 0.76f, 0.76f, 0.3f);
        static Color C_ColHeader(bool d) => d ? new Color(0.19f, 0.19f, 0.19f) : new Color(0.68f, 0.68f, 0.68f);
        static readonly Color C_DupeKey  = new Color(0.9f, 0.5f, 0.1f, 0.35f);

        // ── Staging data (lives in memory only, never serialized) ───
        private class StagingEntry
        {
            public SerializedPropertyType keyType;
            public SerializedPropertyType valType;
            public object keyValue;
            public object valValue;
        }

        private static readonly Dictionary<string, StagingEntry> s_Staging = new();

        // ── Pagination state (in memory) ────────────────────────────
        private static readonly Dictionary<string, int> s_CurrentPage = new();

        static string PK(SerializedProperty p) =>
            p.propertyPath + "##" + p.serializedObject.targetObject.GetInstanceID();

        static bool HasStaging(SerializedProperty p) => s_Staging.ContainsKey(PK(p));

        static StagingEntry GetStaging(SerializedProperty p) =>
            s_Staging.TryGetValue(PK(p), out var e) ? e : null;

        static void ClearStaging(SerializedProperty p) => s_Staging.Remove(PK(p));

        static int GetPage(SerializedProperty p)
        {
            s_CurrentPage.TryGetValue(PK(p), out int page);
            return page;
        }

        static void SetPage(SerializedProperty p, int page) => s_CurrentPage[PK(p)] = page;

        // ─────────────────────────────────────────────────────────────
        //  CREATE STAGING from array type info
        // ─────────────────────────────────────────────────────────────
        static StagingEntry CreateStaging(SerializedProperty keys, SerializedProperty values)
        {
            // We need to detect the element type. Temporarily insert + read + remove.
            bool wasEmptyK = keys.arraySize == 0;
            bool wasEmptyV = values.arraySize == 0;

            if (wasEmptyK) keys.InsertArrayElementAtIndex(0);
            if (wasEmptyV) values.InsertArrayElementAtIndex(0);

            // Force apply so we can read types
            keys.serializedObject.ApplyModifiedPropertiesWithoutUndo();

            var kProp = keys.GetArrayElementAtIndex(wasEmptyK ? 0 : keys.arraySize - 1);
            var vProp = values.GetArrayElementAtIndex(wasEmptyV ? 0 : values.arraySize - 1);

            var entry = new StagingEntry
            {
                keyType  = kProp.propertyType,
                valType  = vProp.propertyType,
                keyValue = DefaultFor(kProp.propertyType),
                valValue = DefaultFor(vProp.propertyType)
            };

            // Remove temp elements
            if (wasEmptyK) { keys.DeleteArrayElementAtIndex(0); }
            if (wasEmptyV) { values.DeleteArrayElementAtIndex(0); }

            if (wasEmptyK || wasEmptyV)
                keys.serializedObject.ApplyModifiedPropertiesWithoutUndo();

            return entry;
        }

        static object DefaultFor(SerializedPropertyType t) => t switch
        {
            SerializedPropertyType.Integer         => 0,
            SerializedPropertyType.Boolean         => false,
            SerializedPropertyType.Float           => 0f,
            SerializedPropertyType.String          => "",
            SerializedPropertyType.Enum            => 0,
            SerializedPropertyType.Color           => (object)Color.white,
            SerializedPropertyType.Vector2         => (object)Vector2.zero,
            SerializedPropertyType.Vector3         => (object)Vector3.zero,
            SerializedPropertyType.Vector4         => (object)Vector4.zero,
            SerializedPropertyType.Rect            => (object)Rect.zero,
            SerializedPropertyType.ObjectReference => null,
            _                                      => null
        };

        // ─────────────────────────────────────────────────────────────
        //  PAGINATION HELPERS
        // ─────────────────────────────────────────────────────────────
        static int TotalPages(int count) => count <= 0 ? 1 : Mathf.CeilToInt((float)count / PageSize);

        static int ClampPage(int page, int count)
        {
            int maxPage = TotalPages(count) - 1;
            return Mathf.Clamp(page, 0, maxPage);
        }

        static void GetPageRange(int page, int count, out int start, out int end)
        {
            start = page * PageSize;
            end   = Mathf.Min(start + PageSize, count);
        }

        bool NeedsPagination(int count) => count > PageSize;

        // ─────────────────────────────────────────────────────────────
        //  HEIGHT
        // ─────────────────────────────────────────────────────────────
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return HeaderH;

            var keys   = property.FindPropertyRelative("m_Keys");
            var values = property.FindPropertyRelative("m_Values");
            if (keys == null || values == null) return HeaderH;

            float h = HeaderH;
            int count = keys.arraySize;

            // Staging area
            if (HasStaging(property))
                h += Pad + RowH + Pad + RowH + Pad + AddBtnH + Pad;

            // Separator + column headers
            h += SepH + Pad + ColHeaderH;

            // Paginated rows only
            int page = ClampPage(GetPage(property), count);
            GetPageRange(page, count, out int start, out int end);

            for (int i = start; i < end; i++)
            {
                float kH = EditorGUI.GetPropertyHeight(keys.GetArrayElementAtIndex(i), true);
                float vH = EditorGUI.GetPropertyHeight(values.GetArrayElementAtIndex(i), true);
                h += Mathf.Max(kH, vH) + Pad;
            }

            // Pagination bar
            if (NeedsPagination(count))
                h += Pad + PaginationH;

            h += BoxPad;
            return h;
        }

        // ─────────────────────────────────────────────────────────────
        //  GUI
        // ─────────────────────────────────────────────────────────────
        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            var keys   = property.FindPropertyRelative("m_Keys");
            var values = property.FindPropertyRelative("m_Values");
            if (keys == null || values == null)
            {
                EditorGUI.LabelField(pos, label.text, "Error: m_Keys / m_Values not found");
                return;
            }

            bool dark  = EditorGUIUtility.isProSkin;
            int  count = keys.arraySize;

            // ═══════════════════════════════════════════════════════════
            //  HEADER
            // ═══════════════════════════════════════════════════════════
            Rect hdr = new Rect(pos.x, pos.y, pos.width, HeaderH);
            EditorGUI.DrawRect(hdr, C_HeaderBg(dark));

            // Foldout
            Color tc = dark ? new Color(0.85f, 0.85f, 0.85f) : new Color(0.15f, 0.15f, 0.15f);
            GUIStyle fs = new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold };
            fs.normal.textColor = fs.onNormal.textColor = fs.focused.textColor =
                fs.onFocused.textColor = fs.active.textColor = fs.onActive.textColor = tc;

            float foldW = pos.width - ItemsLabelW - PlusBtnW - Pad * 3;
            property.isExpanded = EditorGUI.Foldout(
                new Rect(pos.x + 4, pos.y + 1, foldW, HeaderH - 2),
                property.isExpanded, label.text, true, fs);

            // "N Items"
            GUIStyle itemsStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleRight,
                normal = { textColor = dark ? new Color(0.55f, 0.55f, 0.55f) : new Color(0.4f, 0.4f, 0.4f) }
            };
            EditorGUI.LabelField(
                new Rect(pos.xMax - PlusBtnW - Pad - ItemsLabelW, pos.y, ItemsLabelW, HeaderH),
                $"{count} Items", itemsStyle);

            // "+" button
            if (GUI.Button(new Rect(pos.xMax - PlusBtnW - 2, pos.y + 2, PlusBtnW, HeaderH - 4), "+"))
            {
                property.isExpanded = true;
                if (!HasStaging(property))
                {
                    var staging = CreateStaging(keys, values);
                    s_Staging[PK(property)] = staging;
                }
            }

            if (!property.isExpanded)
            {
                ClearStaging(property);
                return;
            }

            // ═══════════════════════════════════════════════════════════
            //  BODY
            // ═══════════════════════════════════════════════════════════
            float bodyTop = pos.y + HeaderH;
            float bodyH   = pos.height - HeaderH;
            EditorGUI.DrawRect(new Rect(pos.x, bodyTop, pos.width, bodyH), C_BodyBg(dark));

            float y = bodyTop;

            // ── STAGING AREA ─────────────────────────────────────────
            var stg = GetStaging(property);
            if (stg != null)
            {
                y += Pad;

                GUIStyle stageLbl = new GUIStyle(EditorStyles.label)
                {
                    normal = { textColor = new Color(0.45f, 0.65f, 0.85f) }
                };

                float fieldX = pos.x + BoxPad + StagingLabelW + Pad;
                float fieldW = pos.width - BoxPad * 2 - StagingLabelW - Pad;

                // Key field
                EditorGUI.LabelField(new Rect(pos.x + BoxPad, y, StagingLabelW, RowH), "Key", stageLbl);
                stg.keyValue = DrawValueField(new Rect(fieldX, y, fieldW, RowH), stg.keyType, stg.keyValue);
                y += RowH + Pad;

                // Value field
                EditorGUI.LabelField(new Rect(pos.x + BoxPad, y, StagingLabelW, RowH), "Value", stageLbl);
                stg.valValue = DrawValueField(new Rect(fieldX, y, fieldW, RowH), stg.valType, stg.valValue);
                y += RowH + Pad;

                // Check duplicate key
                string stagingKeyStr = ValueToString(stg.keyType, stg.keyValue);
                bool isDuplicate = false;
                for (int i = 0; i < count; i++)
                {
                    if (PropStr(keys.GetArrayElementAtIndex(i)) == stagingKeyStr)
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                // Add button or duplicate warning
                Rect addRect = new Rect(pos.x + BoxPad, y, pos.width - BoxPad * 2, AddBtnH);
                if (isDuplicate)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    GUI.Button(addRect, "An item with the same key already exists");
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    if (GUI.Button(addRect, "Add"))
                    {
                        // Commit staging: insert into arrays
                        int idx = keys.arraySize;
                        keys.InsertArrayElementAtIndex(idx);
                        values.InsertArrayElementAtIndex(idx);
                        SetPropertyValue(keys.GetArrayElementAtIndex(idx), stg.keyType, stg.keyValue);
                        SetPropertyValue(values.GetArrayElementAtIndex(idx), stg.valType, stg.valValue);
                        ClearStaging(property);

                        // Jump to last page so the new element is visible
                        int newCount = keys.arraySize;
                        SetPage(property, TotalPages(newCount) - 1);
                    }
                }
                y += AddBtnH + Pad;
            }

            // ── SEPARATOR ────────────────────────────────────────────
            EditorGUI.DrawRect(new Rect(pos.x + 1, y, pos.width - 2, SepH), C_Sep(dark));
            y += SepH + Pad;

            // ── COLUMN HEADERS ───────────────────────────────────────
            // Refresh count (might have changed after Add)
            count = keys.arraySize;

            Rect colR = new Rect(pos.x, y, pos.width, ColHeaderH);
            EditorGUI.DrawRect(colR, C_ColHeader(dark));

            float usableW = pos.width - BoxPad * 2 - RemoveBtnW - Pad;
            float keyColW = usableW * 0.4f;
            float valColW = usableW * 0.6f;

            GUIStyle colStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = dark ? new Color(0.6f, 0.6f, 0.6f) : new Color(0.35f, 0.35f, 0.35f) }
            };
            EditorGUI.LabelField(new Rect(pos.x + BoxPad, y, keyColW, ColHeaderH), "Key", colStyle);
            EditorGUI.LabelField(new Rect(pos.x + BoxPad + keyColW, y, valColW, ColHeaderH), "Value", colStyle);
            y += ColHeaderH;

            // ── TABLE ROWS (paginated) ───────────────────────────────
            int removeIdx = -1;
            HashSet<int> dupes = DetectDuplicates(keys, count);

            int page = ClampPage(GetPage(property), count);
            SetPage(property, page); // store clamped value
            GetPageRange(page, count, out int startIdx, out int endIdx);

            for (int i = startIdx; i < endIdx; i++)
            {
                var kp = keys.GetArrayElementAtIndex(i);
                var vp = values.GetArrayElementAtIndex(i);
                float kH = EditorGUI.GetPropertyHeight(kp, true);
                float vH = EditorGUI.GetPropertyHeight(vp, true);
                float rH = Mathf.Max(kH, vH);

                Rect rowBg = new Rect(pos.x + 1, y, pos.width - 2, rH + Pad);
                if (dupes.Contains(i))
                    EditorGUI.DrawRect(rowBg, C_DupeKey);
                else
                    EditorGUI.DrawRect(rowBg, i % 2 == 0 ? C_RowEven(dark) : C_RowOdd(dark));

                Rect kR  = new Rect(pos.x + BoxPad, y + 1, keyColW - Pad, kH);
                Rect vR  = new Rect(pos.x + BoxPad + keyColW, y + 1, valColW - Pad, vH);
                Rect rmR = new Rect(pos.x + BoxPad + keyColW + valColW + Pad, y + 1,
                    RemoveBtnW, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(kR, kp, GUIContent.none, true);
                EditorGUI.PropertyField(vR, vp, GUIContent.none, true);

                if (GUI.Button(rmR, "×"))
                    removeIdx = i;

                y += rH + Pad;
            }

            if (removeIdx >= 0)
            {
                keys.DeleteArrayElementAtIndex(removeIdx);
                values.DeleteArrayElementAtIndex(removeIdx);
                // Re-clamp page after removal
                int newCount = keys.arraySize;
                SetPage(property, ClampPage(page, newCount));
            }

            // ── PAGINATION BAR ───────────────────────────────────────
            if (NeedsPagination(count))
            {
                y += Pad;
                int totalPages = TotalPages(count);

                float barX = pos.x + BoxPad;
                float barW = pos.width - BoxPad * 2;
                float btnW = 50f;
                float labelW = barW - btnW * 2 - Pad * 2;

                // Prev button
                EditorGUI.BeginDisabledGroup(page <= 0);
                if (GUI.Button(new Rect(barX, y, btnW, PaginationH), "◀ Prev"))
                    SetPage(property, page - 1);
                EditorGUI.EndDisabledGroup();

                // Page label "Page X / Y  (items start-end of total)"
                GUIStyle pageStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = dark ? new Color(0.7f, 0.7f, 0.7f) : new Color(0.3f, 0.3f, 0.3f) }
                };
                string pageLabel = $"Page {page + 1} / {totalPages}   ({startIdx + 1}–{endIdx} of {count})";
                EditorGUI.LabelField(new Rect(barX + btnW + Pad, y, labelW, PaginationH), pageLabel, pageStyle);

                // Next button
                EditorGUI.BeginDisabledGroup(page >= totalPages - 1);
                if (GUI.Button(new Rect(barX + barW - btnW, y, btnW, PaginationH), "Next ▶"))
                    SetPage(property, page + 1);
                EditorGUI.EndDisabledGroup();
            }
        }

        // ─────────────────────────────────────────────────────────────
        //  DRAW A VALUE FIELD (in-memory, not SerializedProperty)
        // ─────────────────────────────────────────────────────────────
        static object DrawValueField(Rect rect, SerializedPropertyType type, object value)
        {
            switch (type)
            {
                case SerializedPropertyType.String:
                    return EditorGUI.TextField(rect, (string)(value ?? ""));

                case SerializedPropertyType.Integer:
                    return EditorGUI.IntField(rect, value is int i ? i : 0);

                case SerializedPropertyType.Float:
                    return EditorGUI.FloatField(rect, value is float f ? f : 0f);

                case SerializedPropertyType.Boolean:
                    return EditorGUI.Toggle(rect, value is bool b && b);

                case SerializedPropertyType.Enum:
                    // We can't easily draw an enum popup without the actual type.
                    // Fall back to int field for the enum index.
                    return EditorGUI.IntField(rect, value is int ei ? ei : 0);

                case SerializedPropertyType.Color:
                    return EditorGUI.ColorField(rect, value is Color c ? c : Color.white);

                case SerializedPropertyType.Vector2:
                    return EditorGUI.Vector2Field(rect, GUIContent.none, value is Vector2 v2 ? v2 : Vector2.zero);

                case SerializedPropertyType.Vector3:
                    return EditorGUI.Vector3Field(rect, GUIContent.none, value is Vector3 v3 ? v3 : Vector3.zero);

                case SerializedPropertyType.Vector4:
                    return EditorGUI.Vector4Field(rect, GUIContent.none, value is Vector4 v4 ? v4 : Vector4.zero);

                case SerializedPropertyType.ObjectReference:
                    return EditorGUI.ObjectField(rect, value as Object, typeof(Object), true);

                case SerializedPropertyType.Rect:
                    return EditorGUI.RectField(rect, value is Rect r ? r : Rect.zero);

                default:
                    EditorGUI.LabelField(rect, $"(unsupported type: {type})");
                    return value;
            }
        }

        // ─────────────────────────────────────────────────────────────
        //  SET SERIALIZED PROPERTY FROM IN-MEMORY VALUE
        // ─────────────────────────────────────────────────────────────
        static void SetPropertyValue(SerializedProperty prop, SerializedPropertyType type, object value)
        {
            switch (type)
            {
                case SerializedPropertyType.String:          prop.stringValue = (string)(value ?? ""); break;
                case SerializedPropertyType.Integer:         prop.intValue = value is int i ? i : 0; break;
                case SerializedPropertyType.Float:           prop.floatValue = value is float f ? f : 0f; break;
                case SerializedPropertyType.Boolean:         prop.boolValue = value is bool b && b; break;
                case SerializedPropertyType.Enum:            prop.enumValueIndex = value is int ei ? ei : 0; break;
                case SerializedPropertyType.Color:           prop.colorValue = value is Color c ? c : Color.white; break;
                case SerializedPropertyType.Vector2:         prop.vector2Value = value is Vector2 v2 ? v2 : Vector2.zero; break;
                case SerializedPropertyType.Vector3:         prop.vector3Value = value is Vector3 v3 ? v3 : Vector3.zero; break;
                case SerializedPropertyType.Vector4:         prop.vector4Value = value is Vector4 v4 ? v4 : Vector4.zero; break;
                case SerializedPropertyType.ObjectReference: prop.objectReferenceValue = value as Object; break;
                case SerializedPropertyType.Rect:            prop.rectValue = value is Rect r ? r : Rect.zero; break;
            }
        }

        // ─────────────────────────────────────────────────────────────
        //  VALUE → STRING (for duplicate detection in staging)
        // ─────────────────────────────────────────────────────────────
        static string ValueToString(SerializedPropertyType type, object value) => type switch
        {
            SerializedPropertyType.Integer         => (value is int i ? i : 0).ToString(),
            SerializedPropertyType.Boolean         => (value is bool b && b).ToString(),
            SerializedPropertyType.Float           => (value is float f ? f : 0f).ToString("R"),
            SerializedPropertyType.String          => (string)(value ?? ""),
            SerializedPropertyType.Enum            => (value is int ei ? ei : 0).ToString(),
            SerializedPropertyType.ObjectReference => value is Object o && o != null
                ? o.GetInstanceID().ToString() : "null",
            SerializedPropertyType.Vector2         => (value is Vector2 v2 ? v2 : Vector2.zero).ToString(),
            SerializedPropertyType.Vector3         => (value is Vector3 v3 ? v3 : Vector3.zero).ToString(),
            _                                      => value?.ToString() ?? "null"
        };

        // ─────────────────────────────────────────────────────────────
        //  DUPLICATE DETECTION (committed entries)
        // ─────────────────────────────────────────────────────────────
        static HashSet<int> DetectDuplicates(SerializedProperty keys, int count)
        {
            var dupes = new HashSet<int>();
            if (count < 2) return dupes;
            var seen = new Dictionary<string, int>();
            for (int i = 0; i < count; i++)
            {
                string s = PropStr(keys.GetArrayElementAtIndex(i));
                if (seen.TryGetValue(s, out int prev)) { dupes.Add(prev); dupes.Add(i); }
                else seen[s] = i;
            }
            return dupes;
        }

        static string PropStr(SerializedProperty p) => p.propertyType switch
        {
            SerializedPropertyType.Integer         => p.intValue.ToString(),
            SerializedPropertyType.Boolean         => p.boolValue.ToString(),
            SerializedPropertyType.Float           => p.floatValue.ToString("R"),
            SerializedPropertyType.String          => p.stringValue ?? "",
            SerializedPropertyType.Enum            => p.enumValueIndex.ToString(),
            SerializedPropertyType.ObjectReference => p.objectReferenceValue != null
                ? p.objectReferenceValue.GetInstanceID().ToString() : "null",
            SerializedPropertyType.Vector2         => p.vector2Value.ToString(),
            SerializedPropertyType.Vector3         => p.vector3Value.ToString(),
            _                                      => p.propertyPath
        };
    }
}