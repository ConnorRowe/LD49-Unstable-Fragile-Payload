using Godot;
using System;

namespace Unstable
{
    public class SettingsScreen : InfoScreen
    {
        private HSlider masterSlider;
        private HSlider musicSlider;
        private HSlider sFXSlider;
        private CheckButton fullscreenCheck;

        private int masterBusId;
        private int musicBusId;
        private int sFXBusId;

        public override void _Ready()
        {
            base._Ready();

            masterSlider = GetNode<HSlider>("VBoxContainer/MasterSlider");
            musicSlider = GetNode<HSlider>("VBoxContainer/MusicSlider");
            sFXSlider = GetNode<HSlider>("VBoxContainer/SFXSlider");
            fullscreenCheck = GetNode<CheckButton>("VBoxContainer/FullscreenCheck");

            masterBusId = AudioServer.GetBusIndex("Master");
            musicBusId = AudioServer.GetBusIndex("Music");
            sFXBusId = AudioServer.GetBusIndex("SFX");

            masterSlider.Value = GetBusVolumeDb(masterBusId);
            musicSlider.Value = GetBusVolumeDb(musicBusId);
            sFXSlider.Value = GetBusVolumeDb(sFXBusId);
            fullscreenCheck.Pressed = OS.WindowFullscreen;

            masterSlider.Connect("value_changed", this, nameof(MasterVolChanged));
            musicSlider.Connect("value_changed", this, nameof(MusicVolChanged));
            sFXSlider.Connect("value_changed", this, nameof(SFXVolChanged));
            fullscreenCheck.Connect("toggled", this, nameof(FullscreenToggled));
        }

        private float GetBusVolumeDb(int busId)
        {
            return AudioServer.GetBusVolumeDb(busId);
        }

        private void SetBusVolumeDb(int busId, float vol)
        {
            AudioServer.SetBusVolumeDb(busId, vol);
        }

        private void MasterVolChanged(float vol)
        {
            SetBusVolumeDb(masterBusId, vol);
        }

        private void MusicVolChanged(float vol)
        {
            SetBusVolumeDb(musicBusId, vol);
        }

        private void SFXVolChanged(float vol)
        {
            SetBusVolumeDb(sFXBusId, vol);
        }

        private void FullscreenToggled(bool toggle)
        {
            OS.WindowFullscreen = toggle;
        }
    }
}