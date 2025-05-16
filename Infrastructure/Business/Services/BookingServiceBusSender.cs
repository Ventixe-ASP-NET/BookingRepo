using Azure.Messaging.ServiceBus;
using Infrastructure.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Business.Service
{
    public class BookingServiceBusSender
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;

        public BookingServiceBusSender(string connectionString, string queueName)
        {
            _client = new ServiceBusClient(connectionString);
            _sender = _client.CreateSender(queueName);
        }

        public async Task SendBookingCreatedAsync(int bookingId, Guid eventId, List<BookedTicketEntity> tickets)
        {
            var messageBody = JsonSerializer.Serialize(new
            {
                BookingId = bookingId,
                EventId = eventId,
                Tickets = tickets, // serialiserar hela listan
                CreatedAt = DateTime.UtcNow
            });

            var message = new ServiceBusMessage(messageBody);
            await _sender.SendMessageAsync(message);
        }
    }
}
