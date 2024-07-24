namespace SfPopUpAnComboTest
{
    public partial class MainPage : ContentPage
    {
        GetCardInformation _getCardPage;
        public MainPage(GetCardInformation getCardPage)
        {
            InitializeComponent();
            _getCardPage = getCardPage; 
        }

        private async void ButtonClicked(object sender, EventArgs e)
        {
            //GetCardInformation getCardPage = new(true);
            var cardToPayWith = await _getCardPage.GetCardInfoResult();
        }
    }
}
