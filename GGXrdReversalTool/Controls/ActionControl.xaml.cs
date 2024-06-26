﻿using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using GGXrdReversalTool.Commands;
using GGXrdReversalTool.Library.Logging;
using GGXrdReversalTool.Library.Models.Inputs;
using GGXrdReversalTool.Library.Scenarios.Action;
using GGXrdReversalTool.Library.Scenarios.Action.Implementations;
using Microsoft.Win32;

namespace GGXrdReversalTool.Controls;

public sealed partial class ActionControl
{
    public ActionControl()
    {
        InitializeComponent();
    }

    private string _rawInputText = string.Empty;
    public string RawInputText
    {
        get => _rawInputText;
        set
        {
            if (value == _rawInputText) return;
            _rawInputText = value;
            OnPropertyChanged();
            CreateScenario();
        }
    }

    public RelayCommand ImportCommand => new(Import);
    private void Import()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Reversal Tool Replay Slot file (*.ggrs)|*.ggrs"
        };
        var dialogResult = openFileDialog.ShowDialog();

        if (!dialogResult.HasValue || !dialogResult.Value) return;
        
        using var streamReader = new StreamReader(openFileDialog.FileName);

        var content = streamReader.ReadToEnd();

        var slotInput = new SlotInput(content);


        if (slotInput.IsValid)
        {
            RawInputText = slotInput.CondensedInputListText.Aggregate((a, b) => $"{a},{b}");
        }
        else
        {
            MessageBox.Show("Failed to import inputs!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public RelayCommand ExportCommand => new(Export, CanExport);

    private void Export()
    {
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Reversal Tool Replay Slot file (*.ggrs)|*.ggrs"
        };

        var dialogResult = saveFileDialog.ShowDialog();

        if (!dialogResult.HasValue || !dialogResult.Value) return;

        var condensedInputText = new SlotInput(RawInputText).CondensedInputListText.Aggregate((a, b) => $"{a},{b}");

        try
        {
            using var streamWriter = new StreamWriter(saveFileDialog.FileName);
            streamWriter.Write(condensedInputText);

            MessageBox.Show("Inputs Exported!");
        }
        catch (Exception e)
        {
            LogManager.Instance.WriteException(e);

            MessageBox.Show("Failed to export inputs!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private bool CanExport()
    {
        return ScenarioAction is { Input.IsValid: true };
    }

    #region InsertPresetInputCommand

    public RelayCommand<string> InsertPresetInputCommand => new(InsertPresetInput, CanInsertPresetInput);

    

    private void InsertPresetInput(string input)
    {
        RawInputText = RawInputText +
                       $"{(!RawInputText.EndsWith(",") && !string.IsNullOrWhiteSpace(RawInputText)  ? "," : "")}" +
                       input;
    }

    private bool CanInsertPresetInput(string input)
    {
        return IsEnabled;
    }

    #endregion
    
    public IScenarioAction? ScenarioAction
    {
        get => (IScenarioAction?)GetValue(ScenarioActionProperty);
        set => SetValue(ScenarioActionProperty, value);
    }

    public static readonly DependencyProperty ScenarioActionProperty =
        DependencyProperty.Register(nameof(ScenarioAction), typeof(IScenarioAction), typeof(ActionControl),
            new PropertyMetadata(default(IScenarioAction?)));


    private void CreateScenario()
    {
        ScenarioAction = new PlayReversalAction
        {
            Input = new SlotInput(RawInputText)
        };
    }

    //TODO Remove
    private bool IsLegacyFile(string content)
    {
        Regex regex = new Regex("[0-9a-fA-F]+");

        return content.Split(",").All(x => regex.IsMatch(x));
    }
}