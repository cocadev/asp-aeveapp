using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace AevApp.ViewModel
{
    public class SecurityTierItem : ObservableObject
    {
        public int Id { get; set; }

        public string CheckType { get; set; }

        private string _title;

        public string Title
        {
            get => _title;
            set
            {
                if(_title == value) return;
                _title = value;
                RaisePropertyChanged(()=> Title);
            }
        }

        private string _result;

        public string Result
        {
            get => _result;
            set
            {
                if (_result == value) return;
                _result = value;
                UpdateDisplay();
                RaisePropertyChanged(() => Result);
            }
        }

        public bool Manual { get; set; }

        private void UpdateDisplay()
        {
            if (DisplayStyle != null && DisplayStyle.Equals("Optional", StringComparison.InvariantCultureIgnoreCase))
            {
                if (Result.Equals("Pass", StringComparison.InvariantCultureIgnoreCase))
                {
                    BackgroundColor = "#80A2E4";
                    TextColor = "#FFFFFF";
                    BorderColor = "#80A2E4";
                }
                else if (Result.Equals("NotApplicable", StringComparison.InvariantCultureIgnoreCase))
                {
                    BackgroundColor = "#FFFFFF";
                    TextColor = "#80A2E4";
                    BorderColor = "#80A2E4";
                }
                else
                {
                    // Fail as the default
                    // Fail hasn't been defined for this style yet
                    BackgroundColor = "#FFFFFF";
                    TextColor = "#C81B4F";
                    BorderColor = "#FF7BA3";
                }
            }
            else
            {
                // Default as the default

                if (Result.Equals("Pass", StringComparison.InvariantCultureIgnoreCase))
                {
                    BackgroundColor = "#2AC57A";
                    TextColor = "#FFFFFF";
                    BorderColor = "#2AC57A";
                }
                else if (Result.Equals("NotApplicable", StringComparison.InvariantCultureIgnoreCase))
                {
                    BackgroundColor = "#FFFFFF";
                    TextColor = "#6495ED";
                    BorderColor = "#6495ED";
                }
                else
                {
                    // Fail as the default
                    BackgroundColor = "#FFFFFF";
                    TextColor = "#C81B4F";
                    BorderColor = "#FF7BA3";
                }
            }
        }

        private string _backgroundColor;
        public string BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                if (_backgroundColor == value) return;
                _backgroundColor = value;
                RaisePropertyChanged(() => BackgroundColor);
            }
        }

        private string _textColor;
        public string TextColor
        {
            get => _textColor;
            set
            {
                if (_textColor == value) return;
                _textColor = value;
                RaisePropertyChanged(() => TextColor);
            }
        }

        private string _borderColor;
        public string BorderColor
        {
            get => _borderColor;
            set
            {
                if (_borderColor == value) return;
                _borderColor = value;
                RaisePropertyChanged(() => BorderColor);
            }
        }
        
        public int SecurityCheckId { get; set; }

        public string DefaultResult { get; set; }

        public List<string> ValidResults { get; set; }

        public string DisplayStyle { get; set; }

        public void ToggleResult()
        {
            // Cycle through the valid results

            // If no result, or Result not in Valid result then start with the default result
            if (string.IsNullOrWhiteSpace(Result))
            {
                Result = DefaultResult;
            }

            var i = ValidResults.IndexOf(Result);

            if (i < 0)
            {
                Result = DefaultResult;
            }

            // Move to next result
            i++;

            // If moved past all current results then return to 0
            if (i >= ValidResults.Count)
            {
                i = 0;
            }

            Result = ValidResults[i];
        }
    }
}