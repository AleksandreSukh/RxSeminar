using System;
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
            მაუსის_ივენთებზე();
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
}
