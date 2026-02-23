using FilmDiziTakip.Modeller;
using System.Xml.Linq;
namespace FilmDiziTakip;


public partial class DirectorMainPage : ContentPage
{
    public DirectorMainPage()
    {
        InitializeComponent();
    }

    private async void BtnGeri_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new MainPage());
    }

    private async void BtnFilmTakibi_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FilmTakibiPage());
    }

    private async void BtnDiziTakibi_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DiziTakibiPage());
    }

    private async void BtnKullaniciTakibi_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new KullaniciTakibiPage());
    }

}