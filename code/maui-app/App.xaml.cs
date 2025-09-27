using Microsoft.Maui.Controls;

namespace maui_app
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new Pages.TechnicianJobListPage("demo-tech-id"));
        }
    }
}
