using System;
using System.Collections.Generic;

namespace BukovynaAirport_Task1
{
    public static class Logger
    {
        public static void Log(string source, string message)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [{source}] {message}");
        }
    }

    public sealed class FlightControlCenter
    {
        private static readonly Lazy<FlightControlCenter> _instance =
            new Lazy<FlightControlCenter>(() => new FlightControlCenter());

        public Guid Id { get; } = Guid.NewGuid();
        private readonly Dictionary<int, string> _runwayOccupancy = new Dictionary<int, string>();
        private readonly object _lock = new object();

        private FlightControlCenter()
        {
            Logger.Log("FlightControlCenter", "Центр управління польотами ініціалізовано.");
        }

        public static FlightControlCenter Instance => _instance.Value;

        public bool TryReserveRunway(int runwayNumber, string flightNumber, string operationType)
        {
            lock (_lock)
            {
                if (_runwayOccupancy.TryGetValue(runwayNumber, out string currentFlight))
                {
                    Logger.Log("FlightControlCenter", $"Відмова «Смуга {runwayNumber} зайнята рейсом {currentFlight}»");
                    return false;
                }

                _runwayOccupancy[runwayNumber] = flightNumber;
                Logger.Log("FlightControlCenter", $"Смугу {runwayNumber} заброньовано під {operationType} рейсу {flightNumber}.");
                return true;
            }
        }
    }

    public class ArrivalsModule
    {
        public FlightControlCenter ControlCenter => FlightControlCenter.Instance;

        public void RequestLanding(int runwayNumber, string flightNumber)
        {
            Logger.Log("ArrivalsModule", $"Модуль прильотів отримує центр і бронює смугу {runwayNumber} під посадку рейсу {flightNumber}.");
            ControlCenter.TryReserveRunway(runwayNumber, flightNumber, "посадку");
        }
    }

    public class DeparturesModule
    {
        public FlightControlCenter ControlCenter => FlightControlCenter.Instance;

        public void RequestTakeoff(int runwayNumber, string flightNumber)
        {
            Logger.Log("DeparturesModule", $"Модуль вильотів отримує центр і намагається забронювати смугу {runwayNumber} під зліт рейсу {flightNumber}.");
            ControlCenter.TryReserveRunway(runwayNumber, flightNumber, "зліт");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            ArrivalsModule arrivals = new ArrivalsModule();
            DeparturesModule departures = new DeparturesModule();

            arrivals.RequestLanding(1, "PS-101");

            departures.RequestTakeoff(1, "PS-202");

            Logger.Log("Runner", $"Ідентифікатор центру з модуля прильотів: {arrivals.ControlCenter.Id}");
            Logger.Log("Runner", $"Ідентифікатор центру з модуля вильотів:  {departures.ControlCenter.Id}");
        }
    }
}