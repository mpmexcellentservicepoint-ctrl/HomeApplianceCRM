using Microsoft.Maui.Controls;

namespace maui_app.Pages
{
    public partial class TechnicianJobDetailPage : ContentPage
    {
        private int _jobId;
        public TechnicianJobDetailPage(dynamic job)
        {
            InitializeComponent();
            BindingContext = job;
            _jobId = job.Id;
        }

        private async void OnManagePartsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TechnicianJobPartsPage(_jobId));
        }
    }
}
