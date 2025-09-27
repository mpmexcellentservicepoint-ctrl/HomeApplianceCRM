using System.Collections.ObjectModel;
using System.Net.Http.Json;
using Microsoft.Maui.Controls;

namespace maui_app.Pages
{
    public partial class TechnicianJobListPage : ContentPage
    {
        public ObservableCollection<JobDto> Jobs { get; set; } = new();
        public TechnicianJobListPage(string technicianId)
        {
            InitializeComponent();
            LoadJobs(technicianId);
        }
        private async void LoadJobs(string technicianId)
        {
            var http = new HttpClient();
            var jobs = await http.GetFromJsonAsync<List<JobDto>>("https://your-api-url/api/jobs");
            foreach (var job in jobs.Where(j => j.technicianId == technicianId))
                Jobs.Add(job);
            JobListView.ItemsSource = Jobs;
        }
        private async void OnJobSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is JobDto job)
            {
                await Navigation.PushAsync(new TechnicianJobDetailPage(job));
            }
        }
    }
    public class JobDto
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string SLAStatus { get; set; }
        public ProductDto Product { get; set; }
        public string TechnicianId { get; set; }
    }
    public class ProductDto
    {
        public string Name { get; set; }
    }
}
