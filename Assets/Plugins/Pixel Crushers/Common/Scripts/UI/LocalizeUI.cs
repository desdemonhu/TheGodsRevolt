﻿// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers
{

    [AddComponentMenu("")] // Use wrapper instead.
    public class LocalizeUI : MonoBehaviour
    {

        [Tooltip("Overrides the global text table.")]
        [SerializeField]
        private TextTable m_textTable;

        [Tooltip("(Optional) If assigned, use this instead of the UI element's text's value as the field lookup value.")]
        [SerializeField]
        private string m_fieldName = string.Empty;

        public TextTable textTable
        {
            get { return (m_textTable != null) ? m_textTable : ((UILocalizationManager.instance != null) ? UILocalizationManager.instance.textTable : GlobalTextTable.textTable); }
            set { m_textTable = value; }
        }

        public string fieldName
        {
            get { return string.IsNullOrEmpty(m_fieldName) ? null : m_fieldName; }
            set { m_fieldName = value; }
        }

        private bool m_started = false;
        protected bool started
        {
            get { return m_started; }
            private set { m_started = value; }
        }

        private List<string> m_fieldNames = new List<string>();
        protected List<string> fieldNames
        {
            get { return m_fieldNames; }
            set { m_fieldNames = value; }
        }

        private UnityEngine.UI.Text m_text = null;
        public UnityEngine.UI.Text text
        {
            get { return m_text; }
            set { m_text = value; }
        }

#if (UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1)
        private UnityEngine.Object m_dropdown = null;
        public UnityEngine.Object dropdown
#else
        private UnityEngine.UI.Dropdown m_dropdown = null;
        public UnityEngine.UI.Dropdown dropdown
#endif
        {
            get { return m_dropdown; }
            set { m_dropdown = value; }
        }

        protected virtual void Start()
        {
            started = true;
            UpdateText();
        }

        protected virtual void OnEnable()
        {
            UpdateText();
        }

        public virtual void UpdateText()
        {
            if (!started) return;
            if (UILocalizationManager.instance == null) return;
            var textTable = this.textTable;
            var language = UILocalizationManager.instance.currentLanguage;
            if (string.IsNullOrEmpty(language)) return;

            // Skip if no language set:
            if (textTable == null)
            {
                Debug.LogWarning("No localized text table is assigned to " + name + " or a UI Localized Manager component.", this);
                return;
            }

            if (!textTable.HasLanguage(language))
            {
                Debug.LogWarning("Text table " + textTable.name + " does not have a language '" + language + "'.", textTable);
                return;
            }

            // Make sure we have a Text or Dropdown:
            if (text == null && dropdown == null)
            {
                text = GetComponent<UnityEngine.UI.Text>();
#if !(UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1)
                dropdown = GetComponent<UnityEngine.UI.Dropdown>();
#endif
                if (text == null && dropdown == null)
                {
                    Debug.LogWarning("Localize UI didn't find a Text or Dropdown component on " + name + ".", this);
                    return;
                }
            }

            // Get the original values to use as field lookups:
            if (string.IsNullOrEmpty(fieldName))
            {
                fieldName = (text != null) ? text.text : string.Empty;
            }
#if !(UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1)
            if ((fieldNames.Count == 0) && (dropdown != null)) 
            {
                dropdown.options.ForEach(opt => fieldNames.Add(opt.text));
            }
#endif

            // Localize Text:
            if (text != null)
            {
                if (!textTable.HasField(fieldName))
                {
                    Debug.LogWarning("Text table " + textTable.name + " does not have a field '" + fieldName + "'.", textTable);
                }
                else
                {
                    text.text = GetLocalizedText(fieldName);
                }
            }

#if !(UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1)
            // Localize Dropdown:
            if (dropdown != null)
            {
                for (int i = 0; i < dropdown.options.Count; i++)
                {
                    if (i < fieldNames.Count)
                    {
                        dropdown.options[i].text = GetLocalizedText(fieldNames[i]);
                    }
                }
                dropdown.captionText.text = GetLocalizedText(fieldNames[dropdown.value]);
            }
#endif
        }

        protected virtual string GetLocalizedText(string fieldName)
        {
            return (textTable != null && textTable.HasField(fieldName))
                ? textTable.GetFieldTextForLanguage(fieldName, GlobalTextTable.currentLanguage)
                : GlobalTextTable.Lookup(fieldName);
        }

        /// <summary>
        /// Sets the field name, which is the key to use in the text table.
        /// By default, the field name is the initial value of the Text component.
        /// </summary>
        /// <param name="fieldName">Field name.</param>
        public virtual void SetFieldName(string newFieldName = "")
        {
            if (text == null) text = GetComponent<UnityEngine.UI.Text>();
            fieldName = (string.IsNullOrEmpty(newFieldName) && text != null) ? text.text : newFieldName;
        }

        /// <summary>
        /// If changing the Dropdown options, call this afterward to update the localization.
        /// </summary>
        public virtual void UpdateDropdownOptions()
        {
            fieldNames.Clear();
            UpdateText();
        }
    }
}