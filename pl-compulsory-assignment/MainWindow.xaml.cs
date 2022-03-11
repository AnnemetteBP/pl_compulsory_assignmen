using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace pl_compulsory_assignment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> times = new List<string>();
        Stopwatch stopWatch = new Stopwatch();
    
        public MainWindow()
        {
            InitializeComponent();
            resultList.Items.Clear();
        }

        private void buttonClick(object sender, RoutedEventArgs e)
        {
            stopWatch.Restart();
            long start = long.Parse(startValue.Text);
            long end = long.Parse(endValue.Text);
            var orderedPrimesSeq = PrimeGenerator.GetPrimesSequential(start, end);
            PrimeGenerator.measureTime(times, stopWatch);
            resultList.Items.Add($"Number of primes: {orderedPrimesSeq.Count} in range {start} to {end}. \n The time for running this operation in sequential was: {stopWatch.Elapsed}");
        }

        private async void buttonClickTwo(object sender, RoutedEventArgs e)
        {
            long start = long.Parse(startValue.Text);
            long end = long.Parse(endValue.Text);
            List<long> orderedPrimesSeq = new List<long>();

            await Task.Factory.StartNew(() => {
                stopWatch.Restart();
                var result = PrimeGenerator.GetPrimesParallel(start, end);
                string theString = "";
                bool stillWaiting = true;
                while (stillWaiting)
                {
                    if (PrimeGenerator.tasks.All(t => t.IsCompleted))
                    {
                        PrimeGenerator.measureTime(times, stopWatch);
                        stillWaiting = false;
                    }
                }
                theString = $"Number of primes: {result.Count} in range {start} to {end}. \n The time for running this operation in parallel was: {stopWatch.Elapsed}";
                return theString;
            }, TaskCreationOptions.LongRunning).ContinueWith(t => {
                resultList.Items.Add(t.Result);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
