using System;
using System.Collections.Generic;
using System.Threading;

namespace BukovynaAirport
{
    class Flight
    {
        public string Number { get; }

        public Flight(string number)
        {
            Number = number;
        }
    }

    class GroundUnit
    {
        public string Id { get; }
        public bool IsFree { get; private set; }
        public string AssignedTo { get; private set; }

        public GroundUnit(string id)
        {
            Id = id;
            IsFree = true;
        }

        public void Assign(string flightNumber)
        {
            IsFree = false;
            AssignedTo = flightNumber;
        }

        public void Release()
        {
            IsFree = true;
            AssignedTo = null;
        }
    }

    class Journal
    {
        private int minutes;

        public Journal(int startHour, int startMinute)
        {
            minutes = startHour * 60 + startMinute;
        }

        public void Write(string source, string message)
        {
            int hour = minutes / 60;
            int minute = minutes % 60;
            Console.WriteLine("[" + hour.ToString("00") + ":" + minute.ToString("00") + "] [" + source + "] " + message);
            minutes += 1;
        }
    }

    class GroundEquipmentPool
    {
        private readonly List<GroundUnit> units = new List<GroundUnit>();
        private readonly int capacity;
        private readonly Journal journal;

        public GroundEquipmentPool(int capacity, Journal journal)
        {
            this.capacity = capacity;
            this.journal = journal;
        }

        public GroundUnit Acquire(Flight flight)
        {
            foreach (GroundUnit unit in units)
            {
                if (!unit.IsFree)
                    continue;

                unit.Assign(flight.Number);
                journal.Write("Пул", unit.Id + " видано рейсу " + flight.Number + " із уже підготовлених");
                return unit;
            }

            if (units.Count >= capacity)
            {
                journal.Write("Пул", "Рейс " + flight.Number + " чекає: вільних одиниць немає, нові не створюються");
                return null;
            }

            journal.Write("Пул", "Підготовка нової одиниці наземного обладнання");
            Thread.Sleep(400);
            GroundUnit created = new GroundUnit("EQ-" + (units.Count + 1));
            units.Add(created);
            created.Assign(flight.Number);
            journal.Write("Пул", created.Id + " підготовлено і видано рейсу " + flight.Number);
            return created;
        }

        public void Release(GroundUnit unit, Flight flight)
        {
            unit.Release();
            journal.Write("Пул", unit.Id + " повернено після рейсу " + flight.Number);
        }
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Situation3();
        }

        static void Situation3()
        {
            Journal journal = new Journal(8, 0);
            GroundEquipmentPool pool = new GroundEquipmentPool(2, journal);
            Flight ps101 = new Flight("PS101");
            Flight ps102 = new Flight("PS102");
            Flight ps103 = new Flight("PS103");

            GroundUnit first = pool.Acquire(ps101);
            pool.Acquire(ps102);
            pool.Acquire(ps103);
            pool.Release(first, ps101);
            pool.Acquire(ps103);
        }
    }
}
