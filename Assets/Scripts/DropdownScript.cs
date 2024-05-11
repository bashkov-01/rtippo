using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownScript : MonoBehaviour
{
    /// <summary>
    /// Переменная для ссылки на компонент Dropdown в редакторе Unity
    /// </summary>
    public Dropdown dropdown;
    /// <summary>
    /// SelectedOption хранит значение выбранной опции.
    /// </summary>
    public static int SelectedOption { get; private set; }

    void Start()
    {
        AddOption("1");
        AddOption("2");
        AddOption("3");

        dropdown.SetValueWithoutNotify(0);
        SelectedOption = 1;

        dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(dropdown);
        });
        Debug.Log(DropdownScript.SelectedOption);
    }

    /// <summary>
    /// Метод AppOption для добавления новой опции в выпадающий список.
    /// </summary>
    /// <param name="option">Текст новой опции</param>
    public void AddOption(string option)
    {
        if (dropdown != null)
        {
            dropdown.options.Add(new Dropdown.OptionData(option));
        }
        else
        {
            Debug.LogError("Dropdown reference not set in DropdownScript.");
        }
    }

    /// <summary>
    /// Метод DropdownValueChanged вызываемый при изменении значения выпадающего списка.
    /// Обновляет значение выбранной опции.
    /// </summary>
    /// <param name="change">Измененный выпадающий список</param>
    void DropdownValueChanged(Dropdown change)
    {
        SelectedOption = dropdown.value + 1;
    }
}
