namespace SfPopUpAnComboTest
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void ButtonClicked(object sender, EventArgs e)
        {
            (Card?, bool) cardToPayWith = new();

            GetCardInformation getCardPage = new(true);
            cardToPayWith = await getCardPage.GetCardInfoResult();
        }
    }
}
