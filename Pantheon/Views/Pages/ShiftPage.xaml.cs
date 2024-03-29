﻿using System;
using Styx;
using System.Windows.Controls;
using System.Windows.Input;
using Pantheon.ViewModels.Pages;
using Uranus;

namespace Pantheon.Views.Pages;

/// <summary>
/// Interaction logic for ShiftPage.xaml
/// </summary>
public partial class ShiftPage
{
    public ShiftPageVM VM { get; set; }
    public Helios Helios { get; set; }
    public Charon Charon { get; set; }

    public ShiftPage(Helios helios, Charon charon)
    {
        Helios = helios;
        Charon = charon;
        VM = ShiftPageVM.CreateEmpty(helios, charon);
        InitializeComponent();
        DataContext = VM;
    }

    private async void ShiftPage_OnInitialized(object? sender, EventArgs e)
    {
        VM = await ShiftPageVM.CreateAsync(Helios, Charon);
        DataContext = VM;
    }

    private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        // Fixes issue when clicking cut/copy/paste in context menu
        if (textBox.SelectionLength == 0)
            textBox.SelectAll();
    }

    private void TextBox_LostMouseCapture(object sender, MouseEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        // If user highlights some text, don't override it
        if (textBox.SelectionLength == 0)
            textBox.SelectAll();

        // further clicks will not select all
        textBox.LostMouseCapture -= TextBox_LostMouseCapture;
    }

    private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        // once we've left the TextBox, return the select all behavior
        textBox.LostMouseCapture += TextBox_LostMouseCapture;
    }
}