namespace FilmDiziTakip
{
    public partial class App : Application
    {
        // Giriş yapan kullanıcının ID'si burada tutulur
        public static int GirisYapanKullaniciId { get; set; }

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell(); // ya da giriş sayfan neyse
        }
    }
}
