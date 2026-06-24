using ConferenceRoomsAPI.Domain.Entities;
using ConferenceRoomsAPI.Domain.Enums;

namespace ConferenceRoomsAPI.Infrastructure.Services
{
    public class PricingService
    {
        private const decimal MorningDiscount = 0.90m; // -10%
        private const decimal StandartRate = 1.00m; // base price
        private const decimal PeakSurchange = 1.15m; // +15%
        private const decimal EveningDiscount = 0.80m; // -20%



        public decimal CalculateRoomCost(Room room, DateTime start,  DateTime end)
        {
            decimal totalCost = 0;
            var current = start;

            while(current < end)
            {
                var slotEnd = GetNextSlotBoundary(current, end);
                var hours = (decimal)(slotEnd - current).TotalHours;
                var multiplier = GetPriceMultiplier(current);

                totalCost += room.HourlyPrice * hours * multiplier;
                current = slotEnd;
            }

            return Math.Round(totalCost, 2);
        }

        public static DateTime GetNextSlotBoundary(DateTime current, DateTime bookingEnd)
        {
            //All tariff slot limits during the day
            var boundaries = new[] { 6, 9, 12, 14, 18, 23 };

            var nextBoundaryHour = boundaries.FirstOrDefault(h => h > current.Hour);

            if (nextBoundaryHour == 0)
                return bookingEnd;

            var boundary = current.Date.AddHours(nextBoundaryHour);
            return boundary < bookingEnd ? boundary : bookingEnd;
        }

        public decimal GetPriceMultiplier(DateTime time)
        {
            return GetTimeSlot(time) switch
            {
                TimeSlotType.Morning => MorningDiscount,
                TimeSlotType.Standard => StandartRate,
                TimeSlotType.Peak => PeakSurchange,
                TimeSlotType.Evening => EveningDiscount,
                _ => StandartRate
            };
        }

        public TimeSlotType GetTimeSlot(DateTime time)
        {
            var hour = time.Hour;

            return hour switch
            {
                >= 6 and < 9 => TimeSlotType.Morning,
                >= 12 and < 14 => TimeSlotType.Peak,      // Peak check before Standard
                >= 9 and < 18 => TimeSlotType.Standard,
                >= 18 and < 23 => TimeSlotType.Evening,
                _ => throw new InvalidOperationException(
                    $"Booking time {time:HH:mm} is outside working hours (06:00–23:00)")
            };
        }

        public decimal CalculateTotalCost(Room room, DateTime start, DateTime end, IEnumerable<Service> services)
        {
            var roomCost = CalculateRoomCost(room, start, end);
            var serviceCost = services.Sum(s => s.Price);

            return roomCost + serviceCost;
        }

    }
}
