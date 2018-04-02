using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ALCS.KeyboardAndMouseLogger;
using ALCS.KeyboardAndMouseLogger.Keyboard;
using ALCS.KeyboardAndMouseLogger.Mouse;

namespace RxUsageSample
{
    class Program
    {
        static void Main(string[] args)
        {

            //=======================================================================================
            var interval = Observable.Interval(TimeSpan.FromMilliseconds(1000));
            interval.Dump();
            var ათეულები = interval.Select(i => (i / 10) % 10);

            ათეულები.Dump();

            var ათეულები_მარტო = ათეულები.DistinctUntilChanged();

            ათეულები_მარტო.Dump();


            //მაუსის_ივენთებზე();

            //Window();

            System.Threading.Thread.Sleep(-1);
        }


        private static void Window()
        {
            var mainSequence = Observable.Interval(TimeSpan.FromSeconds(1));
            var seqWindowed = mainSequence.Window(() => Observable.Interval(TimeSpan.FromSeconds(6)));

            seqWindowed.Subscribe(თვითონვინდოუ =>
            {
                Console.WriteLine($"\nNew window created at: {DateTime.Now}\n");
                თვითონვინდოუ.Subscribe(x => { Console.WriteLine("In window : {0}", x); });
            });
        }

        private static void მაუსის_ივენთებზე()
        {
            var mouseTrack = new MouseSpeedometer(new KeyAndMouseLogger(new MouseLogger(), new KeyLogger(
                new List<IKeyParser>()
                {
                    new KeysToShortcutsParser()
                })));

            //ეს დავიკიდოთ
            //mouseTrack.MouseMovementCaptured += (object sender, MouseMovementEventArgs e) =>
            //{
            //    Console.WriteLine(
            //        $"{Environment.NewLine}Event date:{e.Date} Length:{e.Length} Speed_PX/MS:{e.SpeedPxMs}{Environment.NewLine}");
            //};


            ////ეს არის ივენთის მამაპაპური ჰენდლერი
            //mouseTrack.MouseSpeedUpdated += (object sender, MouseMovementEventArgs e) =>
            //{
            //    Console.WriteLine(new string('=', Convert.ToInt32(e.SpeedPxMs * 10)));
            //    //$"{Environment.NewLine}Event date:{e.Date} Length:{e.Length} Speed_PX/MS:{e.SpeedPxMs}{Environment.NewLine}");
            //};

            //ეს კი ულტრათანამედროვე და პროგრესული ობზერვებლი
            //Observable: 
            var toObservable = Observable
                .FromEventPattern<EventHandler<MouseMovementEventArgs>, MouseMovementEventArgs>
                (h => mouseTrack.MouseSpeedUpdated += h, h => mouseTrack.MouseSpeedUpdated -= h);

            //სპიდომეტრი დავმალეთ
            //toObservable.Subscribe(
            //    o => Console.WriteLine(new string('=', Convert.ToInt32(o.EventArgs.SpeedPxMs * 10))));

            var სიჩქარეები = toObservable.Select(o => o.EventArgs.SpeedPxMs);

            var მარტო_ჩქარები = სიჩქარეები.Where(o => o > 1);
            var უფრო_ჩქარები = მარტო_ჩქარები.Where(o => o > 2);
            var იმენა_ჩქარები = უფრო_ჩქარები.Where(o => o > 3);
            var ვაბშე_ჩქარები = იმენა_ჩქარები.Where(o => o > 4);

            მარტო_ჩქარები.Subscribe(c => Console.WriteLine("Nela!"));
            უფრო_ჩქარები.Subscribe(c => Console.WriteLine("Azri araaq!"));
            იმენა_ჩქარები.Subscribe(c => Console.WriteLine("Cudi gzaa!"));
            ვაბშე_ჩქარები.Subscribe(c => Console.WriteLine("Mewyineba boz.."));

            mouseTrack.Start();
        }
    }

    public static class LinqPadLike
    {
        public static void DumpLatest<T>(this IObservable<T> observable)
        {
            observable.Subscribe(o => Console.WriteLine(o));
        }

        static ConcurrentDictionary<object, List<object>> CurrentlyBeingDumped = new ConcurrentDictionary<object, List<object>>();

        public static void Dump<T>(this IObservable<T> observable)
        {
            var contains = CurrentlyBeingDumped.ContainsKey(observable);
            var current = CurrentlyBeingDumped.GetOrAdd(observable, o => new List<object>());
            if (!contains)
            {
                observable.Subscribe(o =>
                {
                    current.Add(o);
                    var id = observable.ToString().Split('.').Last();
                    Console.WriteLine($"//>>>>>>>>>>>>>>>>>>>>>>>>:{id}");
                    current.ForEach(c => Console.WriteLine(c));
                    Console.WriteLine($"//<<<<<<<<<<<<<<<<<<<<<<<<:{id}");
                });
            }
        }
    }

}
