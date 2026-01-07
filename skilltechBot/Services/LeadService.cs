using SkillTechNavigator.Models;
using System.Text.Json;

namespace SkillTechNavigator.Services
{
    public interface ILeadService
    {
        Task<bool> CaptureLeadAsync(LeadCapture lead);
        Task<List<LeadCapture>> GetLeadsAsync();
    }

    public class LeadService : ILeadService
    {
        private readonly ILogger<LeadService> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly string _leadsFilePath;

        public LeadService(ILogger<LeadService> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
            _leadsFilePath = Path.Combine(env.WebRootPath, "data", "leads.json");
            
            // Ensure data directory exists
            var dataDir = Path.Combine(env.WebRootPath, "data");
            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }
        }

        public async Task<bool> CaptureLeadAsync(LeadCapture lead)
        {
            try
            {
                var leads = await GetLeadsAsync();
                
                // Check for duplicate email
                if (leads.Any(l => l.Email.Equals(lead.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    _logger.LogInformation($"Lead already exists for email: {lead.Email}");
                    // Update existing lead's interest
                    var existingLead = leads.First(l => l.Email.Equals(lead.Email, StringComparison.OrdinalIgnoreCase));
                    existingLead.Interest = lead.Interest;
                    existingLead.CourseId = lead.CourseId;
                    existingLead.CapturedAt = DateTime.UtcNow;
                }
                else
                {
                    leads.Add(lead);
                }

                var json = JsonSerializer.Serialize(leads, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
                
                await File.WriteAllTextAsync(_leadsFilePath, json);
                
                _logger.LogInformation($"Lead captured successfully: {lead.Email} - {lead.Interest}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to capture lead");
                return false;
            }
        }

        public async Task<List<LeadCapture>> GetLeadsAsync()
        {
            try
            {
                if (!File.Exists(_leadsFilePath))
                {
                    return new List<LeadCapture>();
                }

                var json = await File.ReadAllTextAsync(_leadsFilePath);
                return JsonSerializer.Deserialize<List<LeadCapture>>(json) ?? new List<LeadCapture>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read leads file");
                return new List<LeadCapture>();
            }
        }
    }
}
