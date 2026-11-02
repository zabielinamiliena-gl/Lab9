using System;
using System.Collections.Generic;
using System.Threading;

namespace BukovynaAirport_Task2
{
    public static class Logger
    {
        public static void Log(string source, string message)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [{source}] {message}");
        }
    }

    public class SeatLayout
    {
        public string AircraftModel { get; set; }
        public List<string> BlockedSeats { get; set; }

        public SeatLayout(string aircraftModel, List<string> blockedSeats = null)
        {
            AircraftModel = aircraftModel;
            BlockedSeats = blockedSeats ?? new List<string>();
        }

        public SeatLayout DeepClone()
        {
            return new SeatLayout(AircraftModel, new List<string>(BlockedSeats));
        }

        public override string ToString()
        {
            string blocked = BlockedSeats.Count > 0 ? string.Join(", ", BlockedSeats) : "немає";
            return $"Модель: {AircraftModel} | Заблоковані місця: [{blocked}]";
        }
    }

    public class Flight : ICloneable
    {
        public string FlightNumber { get; set; }
        public string Route { get; set; }
        public DateTime FlightDate { get; set; }
        public string AircraftType { get; set; }
        public SeatLayout Layout { get; set; }
        public List<string> Passengers { get; set; }
        public List<string> CateringMenu { get; set; }

        public Flight(string route, string aircraftType, SeatLayout layout, List<string> initialMenu)
        {
            Route = route;
            AircraftType = aircraftType;
            Layout = layout;
            CateringMenu = initialMenu ?? new List<string>();
            Passengers = new List<string>();

            Logger.Log("Flight", $"[Еталон {route}] Початок дорогого завантаження конфігурації літака...");
            Thread.Sleep(1200); 
            Logger.Log("Flight", $"[Еталон {route}] Конфігурація успішно завантажена.");
        }

        private Flight() { }

        public object Clone()
        {
            return new Flight
            {
                FlightNumber = this.FlightNumber,
                Route = this.Route,
                FlightDate = this.FlightDate,
                AircraftType = this.AircraftType,
                Layout = this.Layout?.DeepClone(),
                Passengers = new List<string>(this.Passengers),
                CateringMenu = new List<string>(this.CateringMenu)
            };
        }

        public void DisplayInfo()
        {
            Console.WriteLine($"  * Рейс: {FlightNumber ?? "ЕТАЛОН"} | Маршрут: {Route} | Дата: {(FlightDate == default ? "не вказана" : FlightDate.ToString("yyyy-MM-dd"))}");
            Console.WriteLine($"    Тип літака: {AircraftType}");
            Console.WriteLine($"    Схема салону: {Layout}");
            Console.WriteLine($"    Пасажири: {(Passengers.Count > 0 ? string.Join(", ", Passengers) : "відсутні")}");
            Console.WriteLine($"    Харчування: {(CateringMenu.Count > 0 ? string.Join(", ", CateringMenu) : "відсутнє")}\n");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            SeatLayout layout = new SeatLayout("Boeing 737-800");
            List<string> menu = new List<string> { "Стандартний сендвіч", "Кава", "Вода" };

            Flight prototype = new Flight("Чернівці–Варшава", "Boeing 737-800", layout, menu);

            Logger.Log("Runner", "Клонування 3 рейсів на основі еталону (без повторного завантаження)...");

            Flight flight1 = (Flight)prototype.Clone();
            flight1.FlightNumber = "PS-101";
            flight1.FlightDate = DateTime.Today.AddDays(1);

            Flight flight2 = (Flight)prototype.Clone();
            flight2.FlightNumber = "PS-102";
            flight2.FlightDate = DateTime.Today.AddDays(2);

            Flight flight3 = (Flight)prototype.Clone();
            flight3.FlightNumber = "PS-103";
            flight3.FlightDate = DateTime.Today.AddDays(3);

            Logger.Log("Runner", "Модифікація рейсу PS-101 (зміна меню, додавання пасажира, блокування місця 12A)...");
            flight1.CateringMenu.Add("Десерт Тірамісу");
            flight1.Passengers.Add("Олександр Бойко");
            flight1.Layout.BlockedSeats.Add("12A");

            Console.WriteLine("\n--- ПЕРЕВІРКА НЕЗАЛЕЖНОСТІ ОБ'ЄКТІВ (ПОРІВНЯННЯ) ---\n");
            Console.WriteLine("[ЕТАЛОННИЙ РЕЙС]:");
            prototype.DisplayInfo();

            Console.WriteLine("[ЗМІНЕНИЙ РЕЙС PS-101]:");
            flight1.DisplayInfo();
        }
    }
}