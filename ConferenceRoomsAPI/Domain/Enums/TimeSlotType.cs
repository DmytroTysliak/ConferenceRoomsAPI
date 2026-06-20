namespace ConferenceRoomsAPI.Domain.Enums
{
    public enum TimeSlotType
    {
        // 06:00 – 09:00 = -10% discount
        Morning,

        // 09:00 – 12:00 and 14:00 – 18:00 = base rate
        Standard,

        // 12:00 – 14:00 → +15% surcharge
        Peak,

        // 18:00 – 23:00 = -20% discount
        Evening
    }
}
