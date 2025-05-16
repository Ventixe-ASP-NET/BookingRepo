using Infrastructure.Business.Dto;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Business.Managers
{
    public class EventServiceClient
    {
        private readonly HttpClient _http;

        public EventServiceClient(HttpClient http)
        {
            _http = http;
            _http.BaseAddress = new Uri("https://ventixe-event-rest-api-cxeqehfrcqcvdkck.swedencentral-01.azurewebsites.net/"); // <-- Ändra till rätt URL
        }

        public async Task<EventDto?> GetEventByIdAsync(Guid id)
        {
            try
            {
                var response = await _http.GetFromJsonAsync<EventDto>($"api/Event/{id}");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EventServiceClient error: {ex.Message}");
                return null;
            }
        }
    }
}
