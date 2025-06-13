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
using Avalonia.Controls.ApplicationLifetimes;
namespace Quack.Views;

public partial class MainWindow : Window
{

    struct CODEFILE
    {
        public TextBox textBox;
        public TabItem tab;
        public string fileName;
        public string filePath;

        CODEFILE(TextBox box, TabItem tabItem, string name, string path)
        {
            textBox = box; tab = tabItem; fileName = name; filePath = path; 
        }
    }
    private CODEFILE _activeCodeFile;
    private List<CODEFILE> _codefile = new List<CODEFILE>();

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

            CODEFILE codeFile = OpenTextEditor(files[0].Name, files[0].Path.ToString());
            // codeFile.textBox.SelectAll();
            _activeCodeFile = codeFile;
            _codefile.Add(codeFile);

            _activeCodeFile.textBox.Text = fileContent;
        }
    }

    private async void SaveFileButton_Clicked(object sender, RoutedEventArgs e)
    {
        var file = await StorageProvider.TryGetFileFromPathAsync(_activeCodeFile.filePath);
        if (file != null)
        {
            await using var stream = await file.OpenWriteAsync();
            using var streamWriter = new StreamWriter(stream);

            await streamWriter.WriteLineAsync(_activeCodeFile.textBox.Text);
        }
        else
        {
            SaveAsButton_Clicked(sender, e);
        }
    }

    private async void SaveAsButton_Clicked(object sender, RoutedEventArgs e)
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

            await streamWriter.WriteLineAsync(_activeCodeFile.textBox.Text);
        }
    }

    private void NewFileButton_Clicked(object sender, RoutedEventArgs e)
    {
        CODEFILE cfile = OpenTextEditor("New code", "");
        cfile.textBox.SelectAll();
        _activeCodeFile = cfile;
        _codefile.Add(cfile);
    }

    private CODEFILE OpenTextEditor(String tabHeader, String path)
    {
        CODEFILE codeFile = new CODEFILE();
        codeFile.fileName = tabHeader;
        codeFile.filePath = path.Substring(7);
        TabItem tab = new TabItem
        {
            Header = tabHeader,
            BorderBrush = new SolidColorBrush(Colors.White),
            BorderThickness = new Thickness(1, 1, 1, 0),
            Margin = new Thickness(3.4, 0),
        };
        tab.PointerReleased += delegate { OpenTab(codeFile); };

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

        codeFile.tab = tab;
        codeFile.textBox = editor;

        return codeFile;
    }

    private async void ExitButton_Clicked(object sender, RoutedEventArgs e)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            lifetime.Shutdown();
        }
    }

    private void OpenTab(CODEFILE cfile)
    {
        cfile.tab.Background = new SolidColorBrush(Color.FromRgb(48, 48, 48));
        _activeCodeFile = cfile;
    }
}