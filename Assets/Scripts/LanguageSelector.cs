using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using TMPro;

public class LanguageSelector : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    private bool isUpdating = false;

    IEnumerator Start()
    {
        yield return LocalizationSettings.InitializationOperation;

        UpdateDropdownList();

        LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;

        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
    }

    private void OnSelectedLocaleChanged(Locale newLocale)
    {
        UpdateDropdownList();
    }

    public void OnDropdownValueChanged(int index)
    {
        if (isUpdating) return;

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }

    private void UpdateDropdownList()
    {
        isUpdating = true; 

        var options = new List<string>();
        int selectedIndex = 0;

        var locales = LocalizationSettings.AvailableLocales.Locales;
        var currentLocale = LocalizationSettings.SelectedLocale;

        bool isHungarianActive = currentLocale.Identifier.Code.Contains("hu");

        for (int i = 0; i < locales.Count; ++i)
        {
            var locale = locales[i];
            string displayName = "";

           
            if (locale.Identifier.Code.Contains("en"))
            {
                if (isHungarianActive) displayName = "Angol";
                else displayName = "English";
            }
           
            else if (locale.Identifier.Code.Contains("hu"))
            {
                if (isHungarianActive) displayName = "Magyar";
                else displayName = "Hungarian";
            }
            else
            {
                displayName = locale.Identifier.CultureInfo.NativeName;
            }

            options.Add(displayName);

            if (currentLocale == locale)
            {
                selectedIndex = i;
            }
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.value = selectedIndex;

        isUpdating = false; 
    }
}