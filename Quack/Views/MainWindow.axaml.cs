using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using System.Reflection.Metadata;
using System;
using Avalonia.VisualTree;
namespace Quack.Views;

public partial class MainWindow : Window
{

    private TextBox _activeTextBox;
    private List<TextBox> _textBoxes = new List<TextBox>();

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void OpenFileButton_Clicked(object sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Text File",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            await using var stream = await files[0].OpenReadAsync();
            using var streamReader = new StreamReader(stream);

            var fileContent = await streamReader.ReadToEndAsync();
            
            TextBox box = OpenTextEditor(files[0].Name);
            box.SelectAll();
            _activeTextBox = box;
            _textBoxes.Add(box);

            _activeTextBox.Text = fileContent;
        }
    }

    private async void SaveFileButton_Clicked(object sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel == null)
            return;

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save file Text"
        });

        if (file is not null)
        {
            await using var stream = await file.OpenWriteAsync();
            using var streamWriter = new StreamWriter(stream);

            await streamWriter.WriteLineAsync(_activeTextBox.Text);
        }
    }

    private void NewFileButton_Clicked(object sender, RoutedEventArgs e)
    {
        TextBox box = OpenTextEditor("New code");
        _activeTextBox = box;
        _activeTextBox.SelectAll();
        _textBoxes.Add(box);
    }

    private TextBox OpenTextEditor(String tabHeader)
    {
        TabItem tab = new TabItem
        {
            Header = tabHeader,
            BorderBrush = new SolidColorBrush(Colors.White),
            BorderThickness = new Thickness(1),
            Margin = new Thickness(3.4, 0)
        };

        TextBox editor = new TextBox
        {
            Margin = new Thickness(-9, -2),
            CornerRadius = new CornerRadius(0),
            AcceptsReturn = true,
            AcceptsTab = true,
            TextWrapping = TextWrapping.NoWrap,
            FontFamily = "Inconsolata"
        };

        editors.Items.Add(tab);
        tab.Content = editor;

        return editor;
    }
}