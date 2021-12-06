using System;
using System.Windows.Input;
using AevApp.Model;
using AevApp.Model.Enum;
using GalaSoft.MvvmLight.Command;
using Xamarin.Forms;

namespace AevApp.Controls
{
    public class CameraView : Xamarin.Forms.View
    {
        public static BindableProperty CameraProperty = BindableProperty.Create(
            "Camera",
            typeof(CameraOptions),
            typeof(CameraView),
            CameraOptions.Rear);

        public static BindableProperty NumberOfPhotoExpectedProperty = BindableProperty.Create(
            "Camera",
            typeof(int),
            typeof(CameraView),
            1);

        public static BindableProperty IsTakingPhotoProperty = BindableProperty.Create(
            "IsTakingPhoto",
            typeof(bool),
            typeof(CameraView),
            false);

        /// <summary>
        /// Enable/Disable capturing a photo on tapping the screen
        /// </summary>
        public static BindableProperty CaptureOnTapProperty = BindableProperty.Create(
            "CaptureOnTap",
            typeof(bool),
            typeof(CameraView),
            false);

        /// <summary>
        /// A trigger to take a photo. For example, an external button that updates the value, which will trigger the property change, and
        /// call the takePhoto in the custom renderer
        /// </summary>
        public static BindableProperty TakePhotoTriggerProperty = BindableProperty.Create(
            "TakePhotoTrigger",
            typeof(int),
            typeof(CameraView),
            0);

        public static BindableProperty FlashSwitcherProperty = BindableProperty.Create(
            "FlashSwitcher",
            typeof(bool),
            typeof(CameraView),
            false);

        public static readonly BindableProperty OnPhotoCommandProperty =
            BindableProperty.Create<CameraView, ICommand>(x => x.OnPhotoCommand, null);

        public CameraOptions Camera
        {
            get => (CameraOptions)GetValue(CameraProperty);
            set => SetValue(CameraProperty, value);
        }

        public int NumberOfPhotoExpected
        {
            get => (int)GetValue(NumberOfPhotoExpectedProperty);
            set => SetValue(NumberOfPhotoExpectedProperty, value);
        }

        public bool IsTakingPhoto
        {
            get => (bool)GetValue(IsTakingPhotoProperty);
            set => SetValue(IsTakingPhotoProperty, value);
        }

        public int TakePhotoTrigger
        {
            get => (int)GetValue(TakePhotoTriggerProperty);
            set => SetValue(TakePhotoTriggerProperty, value);
        }

        public bool FlashSwitcher
        {
            get => (bool)GetValue(FlashSwitcherProperty);
            set => SetValue(FlashSwitcherProperty, value);
        }

        public bool CaptureOnTap
        {
            get => (bool)GetValue(CaptureOnTapProperty);
            set => SetValue(CaptureOnTapProperty, value);
        }

        public ICommand OnPhotoCommand
        {
            get => (ICommand)GetValue(OnPhotoCommandProperty);
            set => SetValue(OnPhotoCommandProperty, value);
        }

        public void PhotoTaken(Photo photo)
        {
            OnPhotoCommand.Execute(photo);
        }
    }
}
