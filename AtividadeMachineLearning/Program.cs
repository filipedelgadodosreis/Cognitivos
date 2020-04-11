using System;
using System.Globalization;
using Azure.AI.TextAnalytics;

namespace AnaliseTexto
{
    class Program
    {
        private static readonly TextAnalyticsApiKeyCredential credentials = new TextAnalyticsApiKeyCredential("1a3a324605244b8b8b1752bc50bc7d59");
        private static readonly Uri endpoint = new Uri("https://15netanalisetexto.cognitiveservices.azure.com/");

        static void Main(string[] args)
        {
            var client = new TextAnalyticsClient(endpoint, credentials);

            SentimentAnalysisExample(client);
            LanguageDetectionExample(client);
            EntityRecognitionExample(client);
            EntityPIIExample(client);
            EntityLinkingExample(client);
            KeyPhraseExtractionExample(client);

            Console.ReadKey();
        }

        /// <summary>
        /// Método para analise de sentimento de determinada frase
        /// </summary>
        /// <param name="client"></param>
        static void SentimentAnalysisExample(TextAnalyticsClient client)
        {
            string inputText = "I had the best day of my life. I wish you were there with me.";
            DocumentSentiment documentSentiment = client.AnalyzeSentiment(inputText);
            Console.WriteLine($"Document sentiment: {documentSentiment.Sentiment}\n");

            var si = new StringInfo(inputText);
            foreach (var sentence in documentSentiment.Sentences)
            {
                Console.WriteLine($"\tSentence [length {sentence.GraphemeLength}]");
                Console.WriteLine($"\tText: \"{si.SubstringByTextElements(sentence.GraphemeOffset, sentence.GraphemeLength)}\"");
                Console.WriteLine($"\tSentence sentiment: {sentence.Sentiment}");
                Console.WriteLine($"\tPositive score: {sentence.ConfidenceScores.Positive:0.00}");
                Console.WriteLine($"\tNegative score: {sentence.ConfidenceScores.Negative:0.00}");
                Console.WriteLine($"\tNeutral score: {sentence.ConfidenceScores.Neutral:0.00}\n");
            }
        }

        /// <summary>
        /// Método utilizado para detecção de idiomas
        /// </summary>
        /// <param name="client"></param>
        static void LanguageDetectionExample(TextAnalyticsClient client)
        {
            //DetectedLanguage detectedLanguage = client.DetectLanguage("Ce document est rédigé en Français.");
            DetectedLanguage detectedLanguage = client.DetectLanguage("Documento escrito em portugues.");
            Console.WriteLine("Language:");
            Console.WriteLine($"\t{detectedLanguage.Name},\tISO-6391: {detectedLanguage.Iso6391Name}\n");
        }

        /// <summary>
        /// Método para vincular uma entidade
        /// </summary>
        /// <param name="client"></param>
        static void EntityRecognitionExample(TextAnalyticsClient client)
        {
            var response = client.RecognizeEntities("I had a wonderful trip to Rio de Janeiro last week.");
            Console.WriteLine("Named Entities:");
            foreach (var entity in response.Value)
            {
                Console.WriteLine($"\tText: {entity.Text},\tCategory: {entity.Category},\tSub-Category: {entity.SubCategory}");
                Console.WriteLine($"\t\tLength: {entity.GraphemeLength},\tScore: {entity.ConfidenceScore:F2}\n");
            }
        }

        /// <summary>
        /// Método para detectar informações pessoais
        /// </summary>
        /// <param name="client"></param>
        static void EntityPIIExample(TextAnalyticsClient client)
        {
            string inputText = "Insurance policy for SSN on file 123-12-1234 is here by approved.";
            var response = client.RecognizePiiEntities(inputText);
            Console.WriteLine("Personally Identifiable Information Entities:");
            foreach (var entity in response.Value)
            {
                Console.WriteLine($"\tText: {entity.Text},\tCategory: {entity.Category},\tSub-Category: {entity.SubCategory}");
                Console.WriteLine($"\t\tLength: {entity.GraphemeLength},\tScore: {entity.ConfidenceScore:F2}\n");
            }
        }

        /// <summary>
        /// Método para vinculação de entidades.
        /// </summary>
        /// <param name="client"></param>
        static void EntityLinkingExample(TextAnalyticsClient client)
        {
            var response = client.RecognizeLinkedEntities(
                "Microsoft was founded by Bill Gates and Paul Allen on April 4, 1975, " +
                "to develop and sell BASIC interpreters for the Altair 8800. " +
                "During his career at Microsoft, Gates held the positions of chairman, " +
                "chief executive officer, president and chief software architect, " +
                "while also being the largest individual shareholder until May 2014.");
            Console.WriteLine("Linked Entities:");
            foreach (var entity in response.Value)
            {
                Console.WriteLine($"\tName: {entity.Name},\tID: {entity.DataSourceEntityId},\tURL: {entity.Url}\tData Source: {entity.DataSource}");
                Console.WriteLine("\tMatches:");
                foreach (var match in entity.Matches)
                {
                    Console.WriteLine($"\t\tText: {match.Text}");
                    Console.WriteLine($"\t\tLength: {match.GraphemeLength},\tScore: {match.ConfidenceScore:F2}\n");
                }
            }
        }

        /// <summary>
        /// Método para extrair palavras chaves
        /// </summary>
        /// <param name="client"></param>
        static void KeyPhraseExtractionExample(TextAnalyticsClient client)
        {
            var response = client.ExtractKeyPhrases("My cat might need to see a veterinarian.");

            // Printing key phrases
            Console.WriteLine("Key phrases:");

            foreach (string keyphrase in response.Value)
            {
                Console.WriteLine($"\t{keyphrase}");
            }
        }
    }
}
