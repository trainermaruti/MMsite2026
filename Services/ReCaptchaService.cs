using System.Text.Json;

namespace MarutiTrainingPortal.Services
{
    public class ReCaptchaService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;
        private readonly string _siteKey;
        private readonly string _verifyUrl;

        public ReCaptchaService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            
            // Try environment variables first (for deployment), then appsettings
            _secretKey = Environment.GetEnvironmentVariable("RECAPTCHA_SECRET") 
                        ?? _configuration["ReCaptcha:SecretKey"] 
                        ?? throw new InvalidOperationException("ReCaptcha SecretKey not configured");
            
            _siteKey = Environment.GetEnvironmentVariable("RECAPTCHA_SITEKEY") 
                      ?? _configuration["ReCaptcha:SiteKey"] 
                      ?? throw new InvalidOperationException("ReCaptcha SiteKey not configured");
            
            _verifyUrl = _configuration["ReCaptcha:VerifyUrl"] ?? "https://www.google.com/recaptcha/api/siteverify";
        }

        public string GetSiteKey() => _siteKey;

        public async Task<(bool IsValid, string ErrorMessage)> VerifyAsync(string responseToken, string? remoteIp = null)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(responseToken))
            {
                return (false, "Please complete the reCAPTCHA verification.");
            }

            try
            {
                // Build request parameters
                var requestParams = new Dictionary<string, string>
                {
                    { "secret", _secretKey },
                    { "response", responseToken }
                };

                if (!string.IsNullOrWhiteSpace(remoteIp))
                {
                    requestParams.Add("remoteip", remoteIp);
                }

                // Send verification request to Google
                var response = await _httpClient.PostAsync(_verifyUrl, new FormUrlEncodedContent(requestParams));
                
                if (!response.IsSuccessStatusCode)
                {
                    return (false, "reCAPTCHA verification service unavailable. Please try again later.");
                }

                // Parse response
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ReCaptchaResponse>(jsonResponse);

                if (result == null)
                {
                    return (false, "Invalid reCAPTCHA response format.");
                }

                // Check if verification was successful
                if (!result.success)
                {
                    // Handle specific error codes
                    if (result.error_codes != null && result.error_codes.Length > 0)
                    {
                        var errorCode = result.error_codes[0];
                        return (false, GetErrorMessage(errorCode));
                    }

                    return (false, "reCAPTCHA verification failed. Please try again.");
                }

                return (true, string.Empty);
            }
            catch (HttpRequestException ex)
            {
                // Network error
                Console.WriteLine($"reCAPTCHA verification network error: {ex.Message}");
                return (false, "Unable to verify reCAPTCHA. Please check your internet connection and try again.");
            }
            catch (Exception ex)
            {
                // General error
                Console.WriteLine($"reCAPTCHA verification error: {ex.Message}");
                return (false, "reCAPTCHA verification encountered an error. Please try again.");
            }
        }

        private string GetErrorMessage(string errorCode)
        {
            return errorCode switch
            {
                "missing-input-secret" => "reCAPTCHA configuration error (missing secret).",
                "invalid-input-secret" => "reCAPTCHA configuration error (invalid secret).",
                "missing-input-response" => "Please complete the reCAPTCHA verification.",
                "invalid-input-response" => "Invalid reCAPTCHA response. Please try again.",
                "bad-request" => "reCAPTCHA request was malformed.",
                "timeout-or-duplicate" => "reCAPTCHA has expired or was already used. Please try again.",
                _ => $"reCAPTCHA verification failed ({errorCode}). Please try again."
            };
        }

        // Response model for Google reCAPTCHA API
        private class ReCaptchaResponse
        {
            public bool success { get; set; }
            public string? challenge_ts { get; set; }
            public string? hostname { get; set; }
            public string[]? error_codes { get; set; }
        }
    }
}
