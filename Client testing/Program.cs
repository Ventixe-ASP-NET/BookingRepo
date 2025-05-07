using BookingsGrpcServer.Protos;
using Grpc.Core;
using Grpc.Net.Client;

class Program
{
    static async Task Main(string[] args)
    {
        // 👇 Denna URL måste matcha din gRPC-server, t.ex. när du kör lokalt:
        // Starta serverprojektet först!
        var channel = GrpcChannel.ForAddress("http://localhost:5000", new GrpcChannelOptions
        {
            Credentials = ChannelCredentials.Insecure
        });

        var client = new BookingService.BookingServiceClient(channel);

        var request = new AddBookingRequest
        {
            InvoiceId = 123000000,
            BookingName = "Brorsans gRPC-test4",
            CreatedAt = DateTime.UtcNow.ToString("o"),
            EventId = "E2516979-6637-458D-A698-5752AD205CCF"
        };

        var reply = await client.AddBookingAsync(request);

        Console.WriteLine($"Svar från servern:");
        Console.WriteLine($"Success: {reply.Success}");
        Console.WriteLine($"Message: {reply.Message}");

        Console.ReadLine();

        var request2 = new GetAllBookingsRequest();
        var response = await client.GetAllBookingsAsync(request2);

        foreach (var booking in response.Bookings)
        {
            Console.WriteLine($"Booking ID: {booking.Id} – {booking.BookingName} – {booking.EventName}");
        }
    }
}