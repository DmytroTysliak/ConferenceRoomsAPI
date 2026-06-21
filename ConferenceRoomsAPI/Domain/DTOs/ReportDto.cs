namespace ConferenceRoomsAPI.Domain.DTOs
{
    public class ReportDto
    {
        public record RevenueReportResponse(DateTime From, DateTime To, List<RoomRevenueItem> Rooms, decimal GrandTotal);

        public record RoomRevenueItem(Guid RoomId, string RoomName, int TotalBookings, decimal TotalRevenue);

        public record RoomOccupacyResponse(DateTime From, DateTime To, List<RoomRevenueItem> Rooms);

        public record RoomOccupacyItem(Guid RoomId, string RoomName, double BookedHours, double AvalibleHours, double OccupacyPercent);

        public record PopularServicesReportResponse(List<ServicePopularItem> Srvices);

        public record ServicePopularItem(Guid ServiceId, string ServiceName, int TimesOrdered, decimal TotalRevenue);
    }
}
