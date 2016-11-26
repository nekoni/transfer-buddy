using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransferWise.Client;
using TransferBuddy.Worker.Repositories;

namespace TransferBuddy.Worker
{
    /// <summary>
    /// The program class which bootstrap the applicaiton.
    /// </summary>
    public class Program
    {
        static int rsi14 = 14;

        static int sma90 = 90;

        static int sma4 = 4;
    
        static string[] currencies = new string [] { "EUR", "USD", "GBP", "SEK", "AUD", "CHF", "DKK", "JPY", "CZK", "HKD", "NOK", "PLN", "NZD", "RUB" };

        static TransferWiseClient client = new TransferWiseClient("https://api.transferwise.com");

        /// <summary>
        /// The main entry point.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
             Console.ReadKey();
            // Task.Run(async () =>
            // {
            //     await InitializeDatabase();
            // }).Wait();

            Task.Run(async () =>
            {
                await FixDatabase();
            }).Wait();


            // Task.Run(async () => 
            // {
            //     await MonitorAlert();
            // }).Wait();
        }

        static async Task MonitorAlert()
        {
            Dictionary<string, List<Rate>> cache = new Dictionary<string, List<Rate>>();
            DateTime day = DateTime.Now;

            while (true)
            {
                for (int i = 0; i < currencies.Length; i++)
                {
                    for (int j = 0; j < currencies.Length; j++)
                    {
                        var source = currencies[i];
                        var target = currencies[j];

                        if (source == target)
                        {
                            continue;
                        }

                        Console.WriteLine($"{source} {target}");
                        var key = $"{source}_{target}";
                        if (!cache.ContainsKey(key))
                        {
                            var startDate = DateTime.Now;
                            var endDate = DateTime.Now.AddDays(-90);

                            var rates = (await client.GetRatesAsync(source, target, startDate, endDate)).ToList();
                            cache.Add(key, rates);
                        }

                        var lastRate = (await client.GetRatesAsync(source, target, DateTime.Now, DateTime.Now)).FirstOrDefault();
                        if (lastRate != null)
                        {
                            var rates = cache[key];
                            rates.RemoveAt(rates.Count - 1);

                            rates.Add(lastRate);
                        }

                        var updatedRates = cache[key];
                        var rate90 = GetRate(updatedRates, sma90);
                        var rate4 = GetRate(updatedRates, sma4);
                        var rateRsi = GetRsi(updatedRates, rsi14);

                        TriggerAlerts(source, target, rate90, rate4, rateRsi);

                        if (DateTime.Now.DayOfYear != day.DayOfYear)
                        {
                            var rep90 = new RateRepository("sma90", source, target);
                            await rep90.Add(rate90);

                            var rep4 = new RateRepository("sma4", source, target);
                            await rep4.Add(rate4);
                    
                            var rep14 = new RateRepository("rsi14", source, target);
                            await rep14.Add(rateRsi);

                            day = DateTime.Now;
                        }
                    }
                }

                System.Threading.Thread.Sleep(60000);
            }
        }

        private static void TriggerAlerts(string source, string target, Models.Rate rate90, Models.Rate rate4, Models.Rate rateRsi)
        {
            // TODO
        }

        static async Task FixDatabase()
        {
            var startDate = DateTime.Now.AddDays(-190);
            var endDate = DateTime.Now;

            for (int i = 0; i < currencies.Length; i++)
            {
                for (int j = 0; j < currencies.Length; j++)
                {
                    var source = currencies[i];
                    var target = currencies[j];

                    if (source == target)
                    {
                        continue;
                    }

                    Console.WriteLine($"{source} {target}");
                    var rates = (await client.GetRatesAsync(source, target, startDate, endDate)).ToList();

                    var rep90 = new RateRepository("sma90", source, target);
                    foreach (var rate in PatchRate(GetRate(rates, sma90, 0).ToList()))
                    {
                        await rep90.Add(rate);
                    }

                    var rep4 = new RateRepository("sma4", source, target);
                    foreach (var rate in PatchRate(GetRate(rates, sma4, 0).ToList()))
                    {
                        await rep4.Add(rate);
                    }

                    var rep14 = new RateRepository("rsi14", source, target);
                    foreach (var rate in PatchRate(GetRsi(rates, rsi14, 0).ToList()))
                    {
                        await rep14.Add(rate);
                    }
                }
            }
        }

        static List<Models.Rate> PatchRate(List<Models.Rate> rates)
        {
            var missingDays = 35;
            var missingRates = new List<Models.Rate>();
            var missingDate = DateTime.Parse("0116-10-22T00:00:00+0000");
            for (int i = 0; i < missingDays; i++)
            {
                rates[i].Date = missingDate;
                missingDate = missingDate.AddDays(1);
                missingRates.Add(rates[i]);
            }

            return missingRates;
        }

        static async Task InitializeDatabase()
        {
            var startDate = DateTime.Now.AddDays(-365);
            var endDate = DateTime.Now;

            for (int i = 0; i < currencies.Length; i++)
            {
                for (int j = 0; j < currencies.Length; j++)
                {
                    var source = currencies[i];
                    var target = currencies[j];

                    if (source == target)
                    {
                        continue;
                    }

                    Console.WriteLine($"{source} {target}");
                    var rates = (await client.GetRatesAsync(source, target, startDate, endDate)).ToList();

                    var rep90 = new RateRepository("sma90", source, target);
                    foreach (var rate in GetRate(rates, sma90, 0).ToList())
                    {
                        await rep90.Add(rate);
                    }

                    var rep4 = new RateRepository("sma4", source, target);
                    foreach (var rate in GetRate(rates, sma4, 0).ToList())
                    {
                        await rep4.Add(rate);
                    }

                    var rep14 = new RateRepository("rsi14", source, target);
                    foreach (var rate in GetRsi(rates, rsi14, 0).ToList())
                    {
                        await rep14.Add(rate);
                    }
                }
            }
        }

        static Models.Rate GetRate(List<Rate> rates, int func)
        {
            var total = default(decimal);
            var date = default(DateTime);
            foreach (var rate in rates)
            {
                total+= rate.Value;
                date = rate.Time;
            }
            
            var result = total / func;
            return new Models.Rate { Date = date, Value = result };
        }

        static IEnumerable<Models.Rate> GetRate(List<Rate> rates, int func, int index)
        {
            if (rates.Count == index)
            {
                yield break;
            }

            var total = default(decimal);
            var date = default(DateTime);
            for (int i = index; i < func + index; i++)
            {
                total+= rates[index].Value;
                date = rates[index].Time;
            }
            
            var result = total / func;
            yield return new Models.Rate { Date = date, Value = result };

            foreach (var r in GetRate(rates, func, index + 1))
            {
                yield return r;
            }
        }

        static Models.Rate GetRsi(List<Rate> rates, int func)
        {
            var date = DateTime.Now;
            var rsi = new Rsi(func);
            foreach (var rate in rates)
            {
                rsi.AddValue(rate.Value);
                date = rate.Time;
            }
            
            return new Models.Rate { Date = date, Value = rsi.Last };
        }

        static IEnumerable<Models.Rate> GetRsi(List<Rate> rates, int func, int index)
        {
            if (rates.Count == index)
            {
                yield break;
            }

            var date = DateTime.Now;
            var rsi = new Rsi(func);
            for (int i = index; i < func + index; i++)
            {
                rsi.AddValue(rates[index].Value);
                date = rates[index].Time;
            }
            
            yield return new Models.Rate { Date = date, Value = rsi.Last };

            foreach (var r in GetRate(rates, func, index + 1))
            {
                yield return r;
            }
        }

        private class Rsi
        {
            int     _period;
            int     _valueCount;
            decimal _last;
            decimal _previousValue;
            decimal _averageGain;
            decimal _averageLoss;
        
            public Rsi(int period)
            {
                if (period <= 1 || 1000 < period)
                    throw new ArgumentOutOfRangeException("period");
        
                _period = period;
            }
        
            public bool AddValue(decimal value)
            {
                _last = 0;
        
                if (_previousValue <= 0)
                {
                    _previousValue = value;
                    return false;
                }
        
                if (_valueCount < _period - 1)
                {
                    if (_previousValue < value)
                        _averageGain += value - _previousValue;
                    else
                        _averageLoss += _previousValue - value;
                    _valueCount++;
                    _previousValue = value;
                    return false;
                }
        
                if (_valueCount == _period - 1)
                {
                    if (_previousValue < value)
                        _averageGain += value - _previousValue;
                    else
                        _averageLoss += _previousValue - value;
                    _valueCount++;
        
                    _averageGain   /= _period;
                    _averageLoss   /= _period;
                }
                else
                {
                    _valueCount++;
                    if (_previousValue < value)
                    {
                        _averageGain = (_averageGain * (_period - 1.0m) + (value - _previousValue))/_period;
                        _averageLoss = (_averageLoss * (_period - 1.0m))/_period;
                    }
                    else
                    {
                        _averageGain = (_averageGain * (_period - 1.0m))/_period;
                        _averageLoss = (_averageLoss * (_period - 1.0m) + (_previousValue - value))/_period;
                    }
                }
        
                if (_averageLoss != 0)
                    _last = 100.0m - 100.0m / (1.0m + _averageGain/_averageLoss);
                else
                    _last = 100;
        
                _previousValue = value;
        
                return true;
            }
        
            public decimal Last
            {
                get
                {
                    return _last;
                }
            }
        }
    }
}
