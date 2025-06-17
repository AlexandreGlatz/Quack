using System;
using Avalonia.Media.Fonts;

public sealed class CustomFontCollection : EmbeddedFontCollection
{
    public CustomFontCollection() : base(
        new Uri("fonts:Inconsolata", UriKind.Absolute),
        new Uri(@"avares://Quack/Assets/Fonts", UriKind.Absolute))
    {}
}