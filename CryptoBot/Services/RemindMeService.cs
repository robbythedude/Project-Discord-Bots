using CryptoBot.Models;
using CryptoBot.Modules;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CryptoBot.Services
{
    public class RemindMeService
    {
        private static readonly string timeRegex = $"^(?!0)[0-9]+((year)|(month)|(day)|(hour)|(minute)|(second))";

        public static string InputIntoDB(string[] stringArray, DateTime currentTime, string user)
        {
            string time = stringArray[0];
            string message = stringArray[1];

            if (Regex.IsMatch(time, timeRegex))
            {
                DateTime newtime = GetNewTime(time, currentTime);
                 var reminder = new Reminder()
                {
                    User = user,
                    Message = message,
                    TimeToTrigger = newtime.ToString()
                };

                List<Reminder> reminders = ReadTextFile();
                reminders.Add(reminder);
                reminders = SortList(reminders);
                WriteToTextFile(reminders);
            }
            return "The time is not a valid format. Please use this format 12hours, 10minutes, 4seconds";
        }

        private static DateTime GetNewTime(string time, DateTime currentTime)
        {
            string numberRegex = $"[0-9]+";
            int months = 0;
            int hours = 0;
            int minutes = 0;
            int seconds = 0;

            int number = Int32.Parse(Regex.Match(time, numberRegex).ToString());


            if (time.Contains("month"))
            {
                months = number;
            }
            else if (time.Contains("hour")){
                hours = number;
            }
            else if (time.Contains("minute")){
                minutes = number;
            }
            else if (time.Contains("second"))
            {
                seconds = number;
            }
            DateTime newTime = currentTime.AddHours(hours).AddMinutes(minutes);

            return newTime;
        }

        private static string GetTextFile()
        {
            string AssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            string Directory = System.IO.Path.GetDirectoryName(AssemblyPath);
            string fileName = "RemindMe.txt";
            return Path.Combine(Directory, fileName);
        }

        private static void WriteToTextFile(List<Reminder> reminders)
        {
            string path = GetTextFile();
            try
            {
                using (var file = new StreamWriter(path))
                {
                    foreach (Reminder reminder in reminders){
                        string line = $"{reminder.TimeToTrigger}|{reminder.User}|{reminder.Message}";
                        file.WriteLine(line);
                    }
                }
            }
            catch(Exception e)
            {
                throw e;
            }

        }

        public static List<Reminder> ReadTextFile()
        {
            string path = GetTextFile();

            var reminders = new List<Reminder>();
            try
            {
                using (var file = new StreamReader(path))
                {
                    while (file.Peek() >= 0)
                    {
                        string line = file.ReadLine();

                        string[] parsedLine = line.Split("|");

                        if (parsedLine.Length != 3)
                        {
                            throw new Exception("There is something wrong with the text file");
                        }
                        var reminder = new Reminder()
                        {
                            TimeToTrigger = parsedLine[0],
                            User = parsedLine[1],
                            Message = parsedLine[2]
                        };
                        reminders.Add(reminder);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return reminders;
        }

        public static Reminder GetFirstReminder()
        {
            string path = GetTextFile();

            string firstLine = File.ReadLines(path).First();
            Reminder reminder = null;
            if (!String.IsNullOrEmpty(firstLine))
            {
                string[] parsedLine = firstLine.Split("|");

                 reminder = new Reminder()
                {
                    TimeToTrigger = parsedLine[0],
                    User = parsedLine[1],
                    Message = parsedLine[2]
                };
            }
            return reminder;
        }

        public static void DeleteFirstReminder()
        {
            string path = GetTextFile();

            var lines = File.ReadLines(path);
            File.WriteAllLines(path, lines.Skip(1).ToArray());
        }

        public static void SortTextFile()
        {
            List<Reminder> reminders = ReadTextFile();
            reminders = SortList(reminders);
            WriteToTextFile(reminders);
        }

        private static List<Reminder> SortList(List<Reminder> reminders)
        {
            reminders.Sort((x, y) => DateTime.Compare(Convert.ToDateTime(x.TimeToTrigger), Convert.ToDateTime(x.TimeToTrigger)));

            return reminders;
        }

        public static string CheckTime(SocketGuild guild)
        {
            string message = "";
            Reminder reminder = GetFirstReminder();
            if (reminder != null)
            {
                DateTime currentTime = DateTime.UtcNow;

                DateTime savedTime = Convert.ToDateTime(reminder.TimeToTrigger);

                if (savedTime <= currentTime)
                {
                    DeleteFirstReminder();

                    SocketGuildUser user = guild.Users.FirstOrDefault(u => u.Username == reminder.User);
                    message = $"{user.Mention} you are being reminded about the message: {reminder.Message}";
                }
            }
            return message;
        }
    }
}
