using System;
using System.Collections.Generic;

namespace BukovynaAirport
{
    enum AircraftType
    {
        Regional,
        NarrowBody,
        WideBody
    }

    class CrewMember
    {
        public string Name { get; private set; }
        public string Role { get; private set; }

        private CrewMember(string name, string role)
        {
            Name = name;
            Role = role;
        }

        public static CrewMember Create(string name, string role)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Ім'я члена екіпажу не може бути порожнім.");
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Роль члена екіпажу не може бути порожньою.");
            return new CrewMember(name, role);
        }
    }

    class Passenger
    {
        public string Name { get; private set; }
        public string Seat { get; private set; }

        private Passenger(string name, string seat)
        {
            Name = name;
            Seat = seat;
        }

        public static Passenger Create(string name, string seat)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Ім'я пасажира не може бути порожнім.");
            if (string.IsNullOrWhiteSpace(seat))
                throw new ArgumentException("Місце пасажира не може бути порожнім.");
            return new Passenger(name, seat);
        }
    }

    class Catering
    {
        public string Menu { get; private set; }
        public int Portions { get; private set; }

        private Catering(string menu, int portions)
        {
            Menu = menu;
            Portions = portions;
        }

        public static Catering Create(string menu, int portions)
        {
            if (string.IsNullOrWhiteSpace(menu))
                throw new ArgumentException("Меню бортового харчування не може бути порожнім.");
            if (portions <= 0)
                throw new ArgumentException("Порцій харчування має бути більше нуля.");
            return new Catering(menu, portions);
        }
    }

    class Flight
    {
        private readonly List<CrewMember> crew;
        private readonly List<Passenger> passengers;

        public string Number { get; private set; }
        public string Route { get; private set; }
        public DateTime Date { get; private set; }
        public AircraftType AircraftType { get; private set; }
        public string CabinScheme { get; private set; }
        public IReadOnlyCollection<CrewMember> Crew => crew.AsReadOnly();
        public IReadOnlyCollection<Passenger> Passengers => passengers.AsReadOnly();
        public Catering Catering { get; private set; }
        public int AssignedUnits { get; private set; }

        private Flight(string number, string route, DateTime date, AircraftType aircraftType, string cabinScheme, List<CrewMember> crew, List<Passenger> passengers, Catering catering)
        {
            Number = number;
            Route = route;
            Date = date;
            AircraftType = aircraftType;
            CabinScheme = cabinScheme;
            this.crew = new List<CrewMember>(crew);
            this.passengers = new List<Passenger>(passengers);
            Catering = catering;
        }

        public static Flight Create(string number, string route, DateTime date, AircraftType aircraftType, string cabinScheme, IEnumerable<CrewMember> crew, IEnumerable<Passenger> passengers, Catering catering)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Номер рейсу не може бути порожнім.");
            if (string.IsNullOrWhiteSpace(route))
                throw new ArgumentException("Напрямок рейсу не може бути порожнім.");
            if (string.IsNullOrWhiteSpace(cabinScheme))
                throw new ArgumentException("Схема салону не може бути порожньою.");
            if (crew == null || passengers == null || catering == null)
                throw new ArgumentException("Екіпаж, пасажири і харчування мають бути задані.");

            List<CrewMember> crewCopy = new List<CrewMember>(crew);
            List<Passenger> passengerCopy = new List<Passenger>(passengers);
            if (crewCopy.Count == 0)
                throw new ArgumentException("Рейс не можна створити без екіпажу.");
            if (catering.Portions < passengerCopy.Count)
                throw new ArgumentException("Порцій харчування менше, ніж пасажирів.");

            return new Flight(number, route, date, aircraftType, cabinScheme, crewCopy, passengerCopy, catering);
        }

        public bool CanDepart()
        {
            return crew.Count > 0 && passengers.Count > 0 && Catering.Portions >= passengers.Count && AssignedUnits > 0;
        }

        public void AssignGroundUnits(int units)
        {
            if (units <= 0)
                throw new ArgumentException("Кількість обладнання має бути більшою за нуль.");
            if (AssignedUnits > 0)
                throw new ArgumentException("Обладнання для рейсу " + Number + " уже виділене.");
            AssignedUnits = units;
        }

        public void ClearGroundUnits()
        {
            AssignedUnits = 0;
        }
    }

    class Runway
    {
        public string Number { get; private set; }
        public bool IsFree { get; private set; }
        public string OccupiedBy { get; private set; }

        private Runway(string number)
        {
            Number = number;
            IsFree = true;
        }

        public static Runway Create(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Номер смуги не може бути порожнім.");
            return new Runway(number);
        }

        public void Occupy(string flightNumber)
        {
            if (string.IsNullOrWhiteSpace(flightNumber))
                throw new ArgumentException("Не вказано рейс для зайняття смуги.");
            if (!IsFree)
                throw new ArgumentException("Смуга " + Number + " зайнята рейсом " + OccupiedBy + ".");
            IsFree = false;
            OccupiedBy = flightNumber;
        }

        public void Release(string flightNumber)
        {
            if (IsFree || OccupiedBy != flightNumber)
                throw new ArgumentException("Смуга " + Number + " не зайнята рейсом " + flightNumber + ".");
            IsFree = true;
            OccupiedBy = null;
        }
    }

    class JetBridge
    {
        public string Id { get; private set; }
        public string ConnectedFlight { get; private set; }
        public bool IsFree => ConnectedFlight == null;

        private JetBridge(string id)
        {
            Id = id;
        }

        public static JetBridge Create(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Ідентифікатор телетрапа не може бути порожнім.");
            return new JetBridge(id);
        }

        public void Connect(string flightNumber)
        {
            if (string.IsNullOrWhiteSpace(flightNumber))
                throw new ArgumentException("Не вказано рейс для телетрапа.");
            if (!IsFree)
                throw new ArgumentException("Телетрап " + Id + " уже під'єднаний до рейсу " + ConnectedFlight + ".");
            ConnectedFlight = flightNumber;
        }

        public void Disconnect()
        {
            if (IsFree)
                throw new ArgumentException("Телетрап " + Id + " не під'єднаний.");
            ConnectedFlight = null;
        }
    }

    class GroundEquipmentPool
    {
        public int Capacity { get; private set; }
        public int Available { get; private set; }

        private GroundEquipmentPool(int capacity)
        {
            Capacity = capacity;
            Available = capacity;
        }

        public static GroundEquipmentPool Create(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException("Кількість наземного обладнання має бути більшою за нуль.");
            return new GroundEquipmentPool(capacity);
        }

        public void Reserve(int units)
        {
            if (units <= 0)
                throw new ArgumentException("Треба виділити хоча б одну одиницю обладнання.");
            if (units > Available)
                throw new ArgumentException("Немає вільного наземного обладнання: потрібно " + units + ", вільно " + Available + ".");
            Available -= units;
        }

        public void Release(int units)
        {
            if (units <= 0 || Available + units > Capacity)
                throw new ArgumentException("Некоректне повернення обладнання.");
            Available += units;
        }
    }

    class ArrivalModule
    {
        private readonly Runway runway;
        private readonly JetBridge bridge;

        public ArrivalModule(Runway runway, JetBridge bridge)
        {
            this.runway = runway;
            this.bridge = bridge;
        }

        public string Receive(Flight flight)
        {
            if (runway.IsFree || runway.OccupiedBy != flight.Number)
                throw new ArgumentException("Рейс " + flight.Number + " не займає смугу, приліт неможливий.");
            bridge.Connect(flight.Number);
            runway.Release(flight.Number);
            return "Модуль прильотів: телетрап " + bridge.Id + " під'єднано до " + flight.Number + ", смуга " + runway.Number + " вільна.";
        }
    }

    class DepartureModule
    {
        private readonly Runway runway;
        private readonly JetBridge bridge;
        private readonly GroundEquipmentPool pool;

        public DepartureModule(Runway runway, JetBridge bridge, GroundEquipmentPool pool)
        {
            this.runway = runway;
            this.bridge = bridge;
            this.pool = pool;
        }

        public string Depart(Flight flight)
        {
            if (!flight.CanDepart())
                throw new ArgumentException("Рейс " + flight.Number + " не готовий до вильоту.");
            if (!runway.IsFree)
                throw new ArgumentException("Смуга зайнята, зліт неможливий.");
            if (bridge.ConnectedFlight != flight.Number)
                throw new ArgumentException("Телетрап не під'єднаний до рейсу " + flight.Number + ".");

            runway.Occupy(flight.Number);
            bridge.Disconnect();
            pool.Release(flight.AssignedUnits);
            flight.ClearGroundUnits();
            runway.Release(flight.Number);
            return "Модуль вильотів: рейс " + flight.Number + " вилетів, обладнання повернене.";
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Flight flight = Flight.Create(
                "PS101",
                "Чернівці-Київ",
                new DateTime(2026, 11, 2, 8, 30, 0),
                AircraftType.Regional,
                "2-2",
                new[] { CrewMember.Create("Іваненко", "капітан"), CrewMember.Create("Петренко", "другий пілот") },
                new[] { Passenger.Create("Коваленко", "1A") },
                Catering.Create("Стандарт", 1));

            Runway runway = Runway.Create("10");
            JetBridge bridge = JetBridge.Create("T1");
            GroundEquipmentPool pool = GroundEquipmentPool.Create(3);

            pool.Reserve(1);
            flight.AssignGroundUnits(1);
            runway.Occupy(flight.Number);

            Console.WriteLine(flight.Number + " " + flight.Route + ", салон " + flight.CabinScheme + ", екіпаж " + flight.Crew.Count);
            Console.WriteLine(new ArrivalModule(runway, bridge).Receive(flight));
            Console.WriteLine("Готовий до вильоту: " + flight.CanDepart());
            Console.WriteLine(new DepartureModule(runway, bridge, pool).Depart(flight));
            Console.WriteLine("Смуга вільна: " + runway.IsFree + ", обладнання вільно: " + pool.Available);
        }
    }
}
