using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace APIClient
{
    class Program
    {
        static HttpClient client = new HttpClient();
        // string[] urls =  { "https://localhost:44379/" };
        static async Task Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("Hello World!");

            // await AsyncAwait();
            //   await TaskWhenAll();
            //   await SemaphoreSlim(); 
            await ParallelForeach();
            Console.WriteLine("Time elapsed " + stopwatch.Elapsed);
            Console.Read();
        }

        static async Task AsyncAwait()
        {
            Console.WriteLine("AsyncAwait");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44379/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                for (int i = 0; i < 5000; i++)
                {

                    HttpResponseMessage response = await client.GetAsync("WeatherForecast");
                    if (response.IsSuccessStatusCode)
                    {
                        // Get the URI of the created resource.  
                        Console.WriteLine("AsyncAwait success");
                    }
                }
            }
        }

        static async Task TaskWhenAll()
        {
            Console.WriteLine("TaskWhenAll");
            var urls = new[] { "https://localhost:44379/WeatherForecast" };
            var maxThreads = 5000;
            //   var q = new ConcurrentQueue<string>(urls);
            var tasks = new List<Task>();
            for (int n = 0; n < maxThreads; n++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    //     while (q.TryDequeue(out string url))
                    {
                        var html = await client.GetStringAsync("https://localhost:44379/WeatherForecast");
                        // Console.WriteLine($"retrieved {html.Length} characters from {url}");
                    }
                }));
            }
            await Task.WhenAll(tasks);
        }

        static async Task SemaphoreSlim()
        {
            Console.WriteLine("SemaphoreSlim");
            var urls = new[] { "https://localhost:44379/WeatherForecast" };
            var allTasks = new List<Task>();
            var throttler = new SemaphoreSlim(initialCount: 5000);
            //   foreach (var url in urls)
            {
                await throttler.WaitAsync();
                allTasks.Add(
                    Task.Run(async () =>
                    {
                        try
                        {
                            var html = await client.GetStringAsync("https://localhost:44379/WeatherForecast");
                            // Console.WriteLine($"retrieved {html.Length} characters from {url}");
                        }
                        finally
                        {
                            throttler.Release();
                        }
                    }));
            }
            await Task.WhenAll(allTasks);

        }

        static async Task ParallelForeach()
        {

            Console.WriteLine("ParallelForeach");
            List<string> urls = new List<string>();
            for (int i = 0; i < 5000; i++)
            {
                urls.Add("https://localhost:44379/WeatherForecast");
            }

            var options = new ParallelOptions() { MaxDegreeOfParallelism = 5000 };
            Parallel.ForEach(urls, options, url =>
            {
                var html = client.GetStringAsync(url).Result;
                // Console.WriteLine($"retrieved {html.Length} characters from {url}");
            });
        }
    }
}
