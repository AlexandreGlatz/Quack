using System;
using Avalonia.Media.Fonts;

public sealed class FontCollection : EmbeddedFontCollection
{
    public FontCollection() : base(
        new Uri("fonts:Inconsolata", UriKind.Absolute),
        new Uri("avares://GoogleFonts/Assets/Fonts#Inconsolata-Regular", UriKind.Absolute))
    {}
}