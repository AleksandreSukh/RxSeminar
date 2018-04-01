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

            toObservable.Subscribe(
                o => Console.WriteLine(new string('=', Convert.ToInt32(o.EventArgs.SpeedPxMs * 10))));




            mouseTrack.Start();

        }
    }
}
