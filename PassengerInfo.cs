namespace kt5work;

public class PassengerInfo
{
    public string PassengerName { get; set; } = "";
    public string FlightNumber { get; set; } = "";
    public string DepartureTime { get; set; } = "";
    public string TicketClass { get; set; } = "";
    public PassengerInfo() { }

    public PassengerInfo(string fullName, string flightNumber)
    {
        var parts = fullName.Trim().Split(' ');
        PassengerName = parts.Length >= 2 ? $"{parts[0]} {parts[1]}" : fullName;
        FlightNumber = flightNumber;
    }
    public PassengerInfo(string passengerName, string flightNumber, string departureTime, string ticketClass)
    {
        PassengerName = passengerName;
        FlightNumber = flightNumber;
        DepartureTime = departureTime;
        TicketClass = ticketClass;
    }

    public override string ToString()
    {
        if (!string.IsNullOrEmpty(DepartureTime) || !string.IsNullOrEmpty(TicketClass))
            return $"Пассажир: {PassengerName}, Рейс: {FlightNumber}, Вылет: {DepartureTime}, Класс: {TicketClass}";
        return $"Пассажир: {PassengerName}, Рейс: {FlightNumber}";
    }
}
