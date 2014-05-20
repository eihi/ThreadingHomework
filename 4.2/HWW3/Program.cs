using System;
using System.Threading;

// Gebaseerd op werk van Mark Fluitsma

namespace HWW3
{
    // Parameters die je kunt veranderen om queue te testen

    // Als alles lijkt te werken, moet je NBOXES maar eens op 1000 zetten en TICK op 10
    
    class CONST
    {
        public const int NPRODUCERS = 6;
        public const int NCONSUMERS = 6;
        public const int TICK = 1000; // de basis-tijdeenheid in miliseconden
        public const int NBOXES = 10; // aantal dozen dat een producent maakt en een consument gebruikt
        public const int BUFLEN = 5;  // de lengte van de queue
    }

    // Klasses Consumer en Producer; volgens mij in orde
    abstract class Worker
    {
        protected Queue q;
        protected String name;

        public Worker()
        {
            Thread workerThread = new Thread(work);
            workerThread.Start();
        }
        protected int random(int n)
        {
            return new Random().Next() % n;
        }
        public abstract void handleBox(int i);
        
        private void work()
        {
            for (int i = 1; i <= CONST.NBOXES; i++)
            {
                handleBox(i);
                Thread.Sleep(random(CONST.TICK));
            }
        }
    }

    class Producer : Worker
    {
        public Producer(String name, Queue q)
        {
            this.name = name;
            this.q = q;
        }

        public override void handleBox(int i)
        {
            Box box = new Box(name, i);
            q.Put(name, box);
        }

    }

    class Consumer : Worker
    {
        public Consumer(String name, Queue q)
        {
            this.name = name;
            this.q = q;
        }
        public override void handleBox(int i)
        {
            Box box = q.Get(name);
        }
    }

    // Klasse Box: eenvoudig doosje. Niks meer aan doen.
    // Bevat alleen de naam van de producent en een volgnummer
    class Box
    {
        private string name;
        private int number;

        public Box(string name, int number)
        {
            this.name = name;
            this.number = number;
        }

        public string Id
        {
            get
            {
                return name + " - " + number;
            }
        }
    }

    // De ronde wachtrij
    // Die moet je dus thread-safe maken en voorzien van semaforen...
    class Queue
    {
        private Box[] buffer = new Box[CONST.BUFLEN];

        // Twee indexen en de lengte bijhouden.
        // Redundant, maar lekker makkelijk!
        private int getpos, putpos;
        private int count;
        private Object queueLock = new Object();

        public Queue()
        {
            getpos = 0;
            putpos = 0;
            count = 0;
        }

        public Box Get(string consumername)
        {
            lock (queueLock)
            {
                Box box = buffer[getpos];
                getpos = (getpos + 1) % CONST.BUFLEN;
                count--;
                Console.WriteLine(consumername + ": gets " + box.Id);
                return box;
            }
        }

        public void Put(string producername, Box box)
        {
            lock (queueLock)
            {
                Console.WriteLine(producername + ": puts " + box.Id);
                buffer[putpos] = box;
                putpos = (putpos + 1) % CONST.BUFLEN;
                count++;
            }
        }
    }

    // Test it...
    class Program
    {
        static void Main(string[] args)
        {
            Queue q = new Queue();

            for (int i = 1; i <= CONST.NPRODUCERS; i++)
                new Producer("P" + i, q);

            for (int i = 1; i <= CONST.NCONSUMERS; i++)
                new Consumer("C" + i, q);

            Console.ReadLine();
        }
    }
}
