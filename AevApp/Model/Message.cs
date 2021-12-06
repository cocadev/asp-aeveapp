using System;
using GalaSoft.MvvmLight;
using Humanizer;

namespace AevApp.Model
{
    public class Message : ObservableObject
    {
        private string _text;

        public string Text
        {
            get => _text;
            set {
                _text = value;
                RaisePropertyChanged(() => Text);
            }
        }

        private DateTime _messageDateTime;

        public DateTime MessageDateTime
        {
            get => _messageDateTime;
            set
            {
                _messageDateTime = value;
                RaisePropertyChanged(() => MessageDateTime);
            }
        }

        public string MessageTimeDisplay => MessageDateTime.Humanize();

        private bool _isIncoming;

        public bool IsIncoming
        {
            get => _isIncoming;
            set
            {
                _isIncoming = value;
                RaisePropertyChanged(() => IsIncoming);
            }
        }
        
    }
}

