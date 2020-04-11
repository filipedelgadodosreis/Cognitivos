using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.AnomalyDetector;
using Microsoft.Azure.CognitiveServices.AnomalyDetector.Models;
namespace AnaliseAnomalia
{
    class Program
    {
        // Necessário criar duas variáveis de ambiente para funcione corretamente.
        //setx ANOMALY_DETECTOR_KEY 2263789d652847e2a590b20e4f5459b4
        //setx ANOMALY_DETECTOR_ENDPOINT https://15netanomalia.cognitiveservices.azure.com/
        static void Main(string[] args)
        {
            //This sample assumes you have created an environment variable for your key and endpoint
            string endpoint = Environment.GetEnvironmentVariable("ANOMALY_DETECTOR_ENDPOINT");
            string key = Environment.GetEnvironmentVariable("ANOMALY_DETECTOR_KEY");
            string datapath = "request-data.csv";

            IAnomalyDetectorClient client = createClient(endpoint, key); //Anomaly Detector client

            Request request = GetSeriesFromFile(datapath); // The request payload with points from the data file

            EntireDetectSampleAsync(client, request).Wait(); // Async method for batch anomaly detection
            LastDetectSampleAsync(client, request).Wait(); // Async method for analyzing the latest data point in the set

            Console.WriteLine("\nPress ENTER to exit.");
            Console.ReadLine();
        }

        /// <summary>
        /// Método responsável por autenticar o cliente
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        static IAnomalyDetectorClient createClient(string endpoint, string key)
        {
            IAnomalyDetectorClient client = new AnomalyDetectorClient(new ApiKeyServiceClientCredentials(key))
            {
                Endpoint = endpoint
            };
            return client;
        }

        /// <summary>
        /// Método responsavel por ler os dados da série temporal
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static Request GetSeriesFromFile(string path)
        {
            List<Point> list = File.ReadAllLines(path, Encoding.UTF8)
                .Where(e => e.Trim().Length != 0)
                .Select(e => e.Split(','))
                .Where(e => e.Length == 2)
                .Select(e => new Point(DateTime.Parse(e[0]), double.Parse(e[1]))).ToList();

            return new Request(list, Granularity.Daily);
        }

        /// <summary>
        /// Detectar anomalias no conjunto de dados inteiro
        /// </summary>
        /// <param name="client"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        static async Task EntireDetectSampleAsync(IAnomalyDetectorClient client, Request request)
        {
            Console.WriteLine("Detecting anomalies in the entire time series.");

            EntireDetectResponse result = await client.EntireDetectAsync(request).ConfigureAwait(false);

            if (result.IsAnomaly.Contains(true))
            {
                Console.WriteLine("An anomaly was detected at index:");
                for (int i = 0; i < request.Series.Count; ++i)
                {
                    if (result.IsAnomaly[i])
                    {
                        Console.Write(i);
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine(" No anomalies detected in the series.");
            }
        }

        /// <summary>
        /// Detectar o status de anomalias
        /// </summary>
        /// <param name="client"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        static async Task LastDetectSampleAsync(IAnomalyDetectorClient client, Request request)
        {

            Console.WriteLine("Detecting the anomaly status of the latest point in the series.");
            LastDetectResponse result = await client.LastDetectAsync(request).ConfigureAwait(false);

            if (result.IsAnomaly)
            {
                Console.WriteLine("The latest point was detected as an anomaly.");
            }
            else
            {
                Console.WriteLine("The latest point was not detected as an anomaly.");
            }
        }
    }
}
