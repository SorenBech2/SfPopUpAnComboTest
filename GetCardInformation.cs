using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls.Shapes;
using Syncfusion.Maui.Inputs;
using Syncfusion.Maui.Popup;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SfPopUpAnComboTest
{
    public partial class GetCardInformation : ObservableObject
    {
        private string? selectedExpirationMonth;
        private string? selectedExpirationYear;
        private readonly Entry cardHolderNameEntry;
        private SfMaskedEntry cardNumberEntry;
        private readonly Entry securityCodeEntry;
        private readonly SfPopup GetCardInfoPopUp;
        private readonly Button addCardButton;
        private readonly CheckBox acceptTermsCheckBox;
        private readonly TapGestureRecognizer acceptTermsTextTapped;
        private bool saveCard = false;
        private bool addCardButtonWasPressed = false;

        [ObservableProperty]
        private ObservableCollection<DateInformation> expirationMonths = [];
        [ObservableProperty]
        private ObservableCollection<DateInformation> expirationYears = [];

        public GetCardInformation()
        {
            bool payWithCard = true;
            // Builds the expiration month list
            for (int i = 1; i <= 12; i++)
            {
                string month;
                if (i < 10)
                    month = $"0{i}";
                else
                    month = i.ToString();

                DateInformation dateInformation = new()
                {
                    Days = string.Empty,
                    Months = month,
                    Years = string.Empty
                };
                ExpirationMonths.Add(dateInformation);
            }

            // Builds the expiration year list
            DateTime year = DateTime.Now;
            for (int i = 0; i < 6; i++)
            {
                DateInformation dateInformation = new()
                {
                    Days = string.Empty,
                    Months = string.Empty,
                    Years = year.Year.ToString()
                };
                ExpirationYears.Add(dateInformation);
                year = year.AddYears(1);
            }

            double popupHeight;
            if (payWithCard)
                popupHeight = 450;
            else
                popupHeight = 410;

            GetCardInfoPopUp = new()
            {
                ShowHeader = true,
                HeaderHeight = 45,
                ShowFooter = false,
                MinimumHeightRequest = popupHeight,
                WidthRequest = 355,
                StaysOpen = true,
                ShowCloseButton = true,
                PopupStyle = new PopupStyle()
                {
                    CornerRadius = 7,
                    HasShadow = true
                },
                StartY = (int)(DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density / 5)
            };
            GetCardInfoPopUp.PopupStyle.SetAppThemeColor(PopupStyle.PopupBackgroundProperty, Colors.WhiteSmoke, Microsoft.Maui.Graphics.Color.FromArgb("#FF6E6E6E"));
            GetCardInfoPopUp.Closed += GetCardInfoPopUp_Closed;

            DataTemplate headerTemplate = new(() =>
            {
                Label headerContent = new() 
                {
                    Text = "Add Credit or Debit Card",
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 18,
                    Padding = new Thickness(15, 0, 0, 5),
                    VerticalTextAlignment = TextAlignment.End
                };
                return headerContent;
            });
            GetCardInfoPopUp.HeaderTemplate = headerTemplate;

            Grid getCardInfoGrid = new()
            {
                RowDefinitions = {
                    new RowDefinition() { Height = new GridLength(15) },
                    new RowDefinition() { Height = new GridLength(30) },
                    new RowDefinition() { Height = new GridLength(7) },
                    new RowDefinition() { Height = new GridLength(40) },
                    new RowDefinition() { Height = new GridLength(30) },
                    new RowDefinition() { Height = new GridLength(7) },
                    new RowDefinition() { Height = new GridLength(40) },
                    new RowDefinition() { Height = new GridLength(30) },
                    new RowDefinition() { Height = new GridLength(7) }
                },
                ColumnDefinitions = {
                    new ColumnDefinition() { Width = new GridLength(20) },
                    new ColumnDefinition() { Width = new GridLength(65) },
                    new ColumnDefinition() { Width = new GridLength(110) },
                    new ColumnDefinition() { Width = new GridLength(20) },
                    new ColumnDefinition() { Width = new GridLength(140) }
                },
                Margin = new Thickness(5, 0, 0, 0),

            };
            Border gridBorder = new()
            {
                HeightRequest = 230, WidthRequest = 325,
                Margin = new Thickness(0, 10, 0, 0),
                Padding = new Thickness(5, 5, 0, 0)
            };
            gridBorder.Content = getCardInfoGrid;

            // Name of card holder
            getCardInfoGrid.AddWithSpan(new Label
            {
                Text = "Card Holder Name",
                FontSize = 13,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.End,
            }, 0, 0, 1, 4);

            cardHolderNameEntry = new()
            {
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                HeightRequest = 40,
                Keyboard = Keyboard.Default,
                Margin = new Thickness(0, 0, 0, -17),
                Placeholder = "Name As Printed On Card",
                PlaceholderColor = Colors.DarkGray,
            };
            cardHolderNameEntry.TextChanged += EntryTextChanged;
            getCardInfoGrid.AddWithSpan(cardHolderNameEntry, 1, 1, 1, 4);
            getCardInfoGrid.AddWithSpan(new Line()
            {
                X2 = 295,
                VerticalOptions = LayoutOptions.End
            }, 2, 0, 1, 5);

            // Card number
            getCardInfoGrid.AddWithSpan(new Label()
            {
                Text = "CardNumber",
                FontSize = 13,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.End
            }, 3, 0, 1, 4);
            cardNumberEntry = new()
            {
                Background = Colors.Transparent,
                ClearButtonVisibility = ClearButtonVisibility.WhileEditing,
                FontAttributes = FontAttributes.Bold,
                FontFamily = "Mulish",
                FontSize = 16,
                Keyboard = Keyboard.Numeric,
                Margin = new Thickness(0, 0, 0, -20),
                Mask = "0000 0000 0000 0000",
                MaskType = MaskedEntryMaskType.Simple,
                Placeholder = "XXXX XXXX XXXX XXXX",
                PlaceholderColor = Colors.DarkGray,
                PromptChar = 'X',
                HidePromptOnLeave = true,
                ShowBorder = false,
                ValueMaskFormat = MaskedEntryMaskFormat.ExcludePromptAndLiterals,
            };
            cardNumberEntry.SetAppThemeColor(SfMaskedEntry.TextColorProperty, Color.FromArgb("#FF003366"), Colors.WhiteSmoke);
            cardNumberEntry.ValueChanged += CardNumberEntry_ValueChanged;
            cardNumberEntry.Focused += CardNumberEntry_Focused;
            getCardInfoGrid.AddWithSpan(cardNumberEntry, 4, 1, 1, 4);

            getCardInfoGrid.AddWithSpan(new Label()
            {
                Text = "Valid Until",
                FontSize = 13,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.End
            }, 6, 0, 1, 4);

            SfComboBox expiryMonthDropDown = new()
            {
                Background = Colors.Transparent,
                DisplayMemberPath = "Months",
                FontSize= 16,
                IsClearButtonVisible = false,
                ItemsSource = ExpirationMonths,
                MaxDropDownHeight = 150,
                Margin = new Thickness(5, 15, 0, 0),
                Placeholder = "MM",
                Stroke = Colors.Transparent,
                TextMemberPath = "Months",
                DropDownItemFontSize = 16,
                WidthRequest = 70, HeightRequest = 40,
                //DropDownPlacement = Syncfusion.Maui.Core.DropDownPlacement.Auto
            };
            expiryMonthDropDown.SelectionChanged += ExpirationMonthsEntry_SelectionChanged;
            getCardInfoGrid.Add(expiryMonthDropDown, 1, 7);

            SfComboBox expiryYearDropDown = new()
            {
                Background = Colors.Transparent,
                DisplayMemberPath = "Years",
                DropDownItemFontSize = 16,
                FontSize = 16,
                IsClearButtonVisible = false,
                ItemsSource = ExpirationYears,
                MaxDropDownHeight = 150,
                Margin = new Thickness(-5, 15, 0, 0),
                Placeholder = "YYYY",
                Stroke = Colors.Transparent,
                TextMemberPath = "Years",
                WidthRequest = 85, HeightRequest = 40
            };
            expiryYearDropDown.SelectionChanged += ExpirationYearsEntry_SelectionChanged;
            getCardInfoGrid.Add(expiryYearDropDown, 2, 7);

            getCardInfoGrid.AddWithSpan(new Label()
            {
                Text = "Security Code",
                FontSize = 13,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.End
            }, 6, 3, 1, 2);
            securityCodeEntry = new()
            {
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                HeightRequest = 40,
                WidthRequest = 50,
                HorizontalOptions= LayoutOptions.Start,
                Keyboard = Keyboard.Numeric,
                Margin = new Thickness(0, 20, 0, 0),
                Placeholder = "000",
                PlaceholderColor = Colors.DarkGray,
            };
            getCardInfoGrid.Add(securityCodeEntry, 4, 7);

            // Save card for future use check-box
            CheckBox saveCardCheckBox = new() 
            {
                HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(15,0,0,0)
            };
            saveCardCheckBox.CheckedChanged += SaveCardCheckBox_CheckedChanged;
            Label saveCardText = new()
            {
                Text = "Save Card For Future Use",
                FontAttributes = FontAttributes.Bold,
                VerticalOptions= LayoutOptions.Center
            };
            saveCardText.SetAppThemeColor(Label.TextColorProperty, Color.FromArgb("#FF003366"), Colors.WhiteSmoke);
            HorizontalStackLayout saveCardLayout = new() { saveCardCheckBox, saveCardText };

            // Accept Terms & Conditions check-box label/button
            acceptTermsCheckBox = new()
            {
                HorizontalOptions= LayoutOptions.Start, 
                VerticalOptions= LayoutOptions.Center,
            };
            acceptTermsCheckBox.CheckedChanged += AcceptTermsCheckBox_CheckedChanged;
            Label acceptTermsText = new()
            {
                Text = "Accept Terms And Conditions",
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center,
                TextDecorations = TextDecorations.Underline
            };
            acceptTermsText.SetAppThemeColor(Label.TextColorProperty, Color.FromArgb("#FF003366"), Colors.WhiteSmoke);
            acceptTermsTextTapped = new() { NumberOfTapsRequired = 1 };
            acceptTermsTextTapped.Tapped += AcceptTermsText_Tapped;
            acceptTermsText.GestureRecognizers.Add(acceptTermsTextTapped);
            HorizontalStackLayout acceptTermsLayout = new() { acceptTermsCheckBox, acceptTermsText };
            acceptTermsLayout.Margin = new Thickness(15, 0, 0, 10);

            // Add card button
            addCardButton = new()
            {
                HorizontalOptions= LayoutOptions.Center,
                IsEnabled = false,
                WidthRequest = 315
            };
            addCardButton.Pressed += AddCardButton_Pressed;

            if (payWithCard)
            {
                saveCardCheckBox.IsChecked = false;
                saveCardCheckBox.IsVisible = true;
                saveCardText.IsVisible = true;
                addCardButton.Text = "Use Card To Pay";
            }
            else
            {
                saveCardCheckBox.IsChecked = true;
                saveCardCheckBox.IsVisible = false;
                saveCardText.IsVisible = false;
                addCardButton.Text = "Add Card";
            }

            GetCardInfoPopUp.ContentTemplate = new DataTemplate(() =>
            {
                return new VerticalStackLayout() { gridBorder, saveCardLayout, acceptTermsLayout, addCardButton };
            });
        }

        public async Task<(Card?, bool)> GetCardInfoResult()
        {
            await GetCardInfoPopUp.ShowAsync();

            if (!addCardButtonWasPressed)
                securityCodeEntry.Text = string.Empty;   // nNsures the class returns null if closewindow has been pushed.

            if (string.IsNullOrEmpty(cardNumberEntry.Text) || string.IsNullOrEmpty(cardHolderNameEntry.Text) || string.IsNullOrEmpty(securityCodeEntry.Text)
                    || string.IsNullOrEmpty(selectedExpirationMonth) || string.IsNullOrEmpty(selectedExpirationYear))
                return (null, saveCard);
            else
                return (new Card()
                {
                    CardNumber = cardNumberEntry.Text.Replace(" ", ""),
                    CardholderName = cardHolderNameEntry.Text,
                    Cvv = securityCodeEntry.Text,
                    ExpiryDate = $"{selectedExpirationMonth}{selectedExpirationYear[^2..]}"
                },
                saveCard);
        }

        private void GetCardInfoPopUp_Closed(object? sender, EventArgs e)
        {
            // Disposes of eventhandlers
            try
            {
                if (GetCardInfoPopUp != null)
                {
                    GetCardInfoPopUp.Closed -= GetCardInfoPopUp_Closed;
                    GetCardInfoPopUp.Dismiss();
                }
                if (cardHolderNameEntry != null)
                    cardHolderNameEntry.TextChanged -= EntryTextChanged;
                if (cardNumberEntry != null)
                {
                    cardNumberEntry.ValueChanged -= CardNumberEntry_ValueChanged;
                    cardNumberEntry.Focused -= CardNumberEntry_Focused;
                }
                if (securityCodeEntry != null)
                    securityCodeEntry.TextChanged -= EntryTextChanged;
                if (acceptTermsCheckBox != null)
                    acceptTermsCheckBox.CheckedChanged -= AcceptTermsCheckBox_CheckedChanged;
                if (acceptTermsTextTapped != null)
                    acceptTermsTextTapped.Tapped -= AcceptTermsText_Tapped;
                if (addCardButton != null)
                    addCardButton.Pressed -= AddCardButton_Pressed;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetCardInformation.GetCardInfoPopUp_Closed failed. disposing of event handlers failed with exception {ex.Message}.");
            }
        }

        private void CardNumberEntry_Focused(object? sender, FocusEventArgs e)
        {
            if (cardNumberEntry != null && string.IsNullOrEmpty(cardNumberEntry.Text))
            {
                cardNumberEntry.Dispatcher.DispatchDelayed(TimeSpan.FromSeconds(0.01), () =>
                {
#if IOS
                    cardNumberEntry.CursorPosition = 1;
#else
                    cardNumberEntry.CursorPosition = 0;
#endif
                    // Open keyboard
                    cardNumberEntry.Focus();
                });
            }
        }

        private async void CardNumberEntry_ValueChanged(object? sender, Syncfusion.Maui.Inputs.MaskedEntryValueChangedEventArgs e)
        {
            if (e.IsMaskCompleted)
            {
                TextChangedEventArgs eT = new("", "");
                EntryTextChanged(sender, eT);
            }
        }

        private void EntryTextChanged(object? sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue != null)
            {
                if (!string.IsNullOrEmpty(cardHolderNameEntry.Text)
                    && !string.IsNullOrEmpty(cardNumberEntry.Text)
                    && !string.IsNullOrEmpty(selectedExpirationMonth)
                    && !string.IsNullOrEmpty(selectedExpirationYear)
                    && !string.IsNullOrEmpty(securityCodeEntry.Text)
                    && acceptTermsCheckBox.IsChecked)
                    addCardButton.IsEnabled = true;
                else
                    addCardButton.IsEnabled = false;
            }
        }

        private void ExpirationMonthsEntry_SelectionChanged(object? sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null)
            {
                if (e.AddedItems[0] is DateInformation temp)
                    selectedExpirationMonth = temp.Months;
            }
            TextChangedEventArgs eT = new("", "");
            EntryTextChanged(sender, eT);
        }

        private void ExpirationYearsEntry_SelectionChanged(object? sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null)
            {
                if (e.AddedItems[0] is DateInformation temp)
                    selectedExpirationYear = temp.Years;
            }
            TextChangedEventArgs eT = new("", "");
            EntryTextChanged(sender, eT);
        }

        private void SaveCardCheckBox_CheckedChanged(object? sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                addCardButton.Text = "Save And Use Card To Pay";
                saveCard = true;
            }
            else
            {
                addCardButton.Text = "Use Card To Pay";
                saveCard = false;
            }
        }

        private void AcceptTermsCheckBox_CheckedChanged(object? sender, CheckedChangedEventArgs e)
        {
            TextChangedEventArgs eT = new("", "");
            EntryTextChanged(sender, eT);
        }

        private void AcceptTermsText_Tapped(object? sender, TappedEventArgs e)
        {
            // Do something
        }

        private async void AddCardButton_Pressed(object? sender, EventArgs e)
        {
            var delay = new DelayResult(5);
            var waitABit = await delay.GetResult();
            addCardButtonWasPressed = true;
            GetCardInfoPopUp.IsOpen = false;
        }
    }
}