using System;
using Godot;
using GodotGOAPAI.Source.EventSystem;

namespace GodotGOAPAI.Source.UI;

public partial class GatherResourcesPopup : PanelContainer
{
    [Export] private Button _acceptButton;
    [Export] private LineEdit _logAmountField;
    [Export] private LineEdit _stoneAmountField;
    private bool _isButtonConnected = false;

    public override void _Ready()
    {
        if (_acceptButton == null)
            throw new ArgumentNullException(nameof(_acceptButton));
        if (_logAmountField == null)
            throw new ArgumentNullException(nameof(_logAmountField));
        if (_stoneAmountField == null)
            throw new ArgumentNullException(nameof(_stoneAmountField));

        _isButtonConnected = true;
        _acceptButton.Pressed += OnButtonPressed;
    }
    
    private void OnButtonPressed()
    {
        _isButtonConnected = false;
        _acceptButton.Pressed -= OnButtonPressed;
        int.TryParse(_logAmountField.Text, out var logAmount);
        int.TryParse(_stoneAmountField.Text, out var stoneAmount);
        EventBus.Instance.SendEvent(new GatherResourcesEvent(logAmount, stoneAmount));
        GetParent().RemoveChild(this);
        QueueFree();
    }

    public override void _ExitTree()
    {
        if(_isButtonConnected)
            _acceptButton.Pressed -= OnButtonPressed;
    }
}