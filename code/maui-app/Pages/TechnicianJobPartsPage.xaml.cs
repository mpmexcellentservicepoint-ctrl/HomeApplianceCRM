using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace maui_app.Pages
{
    public partial class TechnicianJobPartsPage : ContentPage
    {
        public ObservableCollection<JobPartDto> JobParts { get; set; } = new();
        public ObservableCollection<PartDto> AvailableParts { get; set; } = new();
        public int JobId { get; set; }
        private readonly HttpClient _http;

        public TechnicianJobPartsPage(int jobId)
        {
            InitializeComponent();
            JobId = jobId;
            _http = new HttpClient { BaseAddress = new System.Uri("https://localhost:5001/") };
            BindingContext = this;
            LoadJobParts();
            LoadAvailableParts();
        }

        private async void LoadJobParts()
        {
            var parts = await _http.GetFromJsonAsync<JobPartDto[]>($"api/jobparts/job/{JobId}");
            JobParts.Clear();
            if (parts != null)
                foreach (var p in parts) JobParts.Add(p);
        }

        private async void LoadAvailableParts()
        {
            var parts = await _http.GetFromJsonAsync<PartDto[]>($"api/parts");
            AvailableParts.Clear();
            if (parts != null)
                foreach (var p in parts) AvailableParts.Add(p);
        }

        private async void RequestPart_Clicked(object sender, System.EventArgs e)
        {
            if (AvailableParts.Count == 0) return;
            var part = AvailableParts[0]; // For demo, pick first
            var req = new { PartId = part.Id, JobId = JobId, Quantity = 1 };
            await _http.PostAsJsonAsync("api/parts/request", req);
            LoadJobParts();
        }

        private async void MarkUsed_Clicked(object sender, System.EventArgs e)
        {
            if (JobParts.Count == 0) return;
            var jobPart = JobParts[0]; // For demo, pick first
            await _http.PostAsJsonAsync("api/jobparts/markused", jobPart);
        }

        private async void ReturnPart_Clicked(object sender, System.EventArgs e)
        {
            if (JobParts.Count == 0) return;
            var jobPart = JobParts[0]; // For demo, pick first
            await _http.PostAsJsonAsync("api/jobparts/return", jobPart);
            LoadJobParts();
        }
    }

    public class JobPartDto
    {
        public int Id { get; set; }
        public int PartId { get; set; }
        public string PartName { get; set; }
        public int Quantity { get; set; }
    }
    public class PartDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StockQty { get; set; }
    }
}
