using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WhatToDo
{
	class Program
	{
		static void Main(string[] args)
		{
 			var random = new Random(Environment.TickCount);
			var activity = args.Any() && args[0].ToLower() == "--activity";

			var suggestions = activity ? GetSuggestions("WhatToDo.ActivityGameWordList.txt") : GetSuggestions(GetActivityType(args));

			bool hasMoreSuggestion;
			ConsoleKeyInfo keyInfo;
			do
			{
				if (activity)
				{
					var game = GetActivityGameWord(suggestions, random);
					Console.WriteLine(game);
				}
				else
				{
					SuggestSomething(suggestions, random);
				}

				hasMoreSuggestion = suggestions.Count > 0;
				if (hasMoreSuggestion)
				{
					Console.WriteLine("Do you want to do something else?");
				}
				keyInfo = Console.ReadKey(true);
				Console.WriteLine();
			}
			while (hasMoreSuggestion);

			Console.WriteLine("I have no more suggestion, sorry. :(");
			Console.ReadKey(true);
		}

		private static void SuggestSomething(IList<string> suggestions, Random random)
		{
			var next = random.Next(suggestions.Count);
			Console.WriteLine(suggestions[next]);
			suggestions.RemoveAt(next);
		}

		private static ActivityType GetActivityType(string[] args)
		{
			const string indoor = "-indoor";
			const string outdoor = "-outdoor";

			if (args == null || args.Length == 0 ||
				(args.Contains(indoor) && args.Contains(outdoor)))
			{
				return ActivityType.All;
			}
			if (args.Contains(indoor))
			{
				return ActivityType.Indoor;
			}
			if (args.Contains(outdoor))
			{
				return ActivityType.Outdoor;
			}
			return ActivityType.All;
		}

		private static string GetActivityGameWord(IList<string> activityGameWords, Random random)
		{
			var nextType = random.Next((int)ActivityGameTask.Talk + 1);
			var next = random.Next(activityGameWords.Count);
			activityGameWords.RemoveAt(next);
			return $"{(ActivityGameTask)nextType} - {activityGameWords[next]}";
		}

		private static IList<string> GetSuggestions(ActivityType activityType)
		{
			var result = new List<string>();

			if (activityType == ActivityType.All || activityType == ActivityType.Indoor)
			{
				result.AddRange(GetSuggestions("WhatToDo.IndoorActivities.txt"));
			}
			if (activityType == ActivityType.All || activityType == ActivityType.Outdoor)
			{
				result.AddRange(GetSuggestions("WhatToDo.OutdoorActivities.txt"));
			}

			return result.Distinct().ToList();
		}

		private static IList<string> GetSuggestions(string resourceName)
		{
			var assembly = Assembly.GetExecutingAssembly();
			using (var toDoStream = assembly.GetManifestResourceStream(resourceName))
			{
				using (var streamReader = new StreamReader(toDoStream, Encoding.UTF8))
				{
					var fileContent = streamReader.ReadToEnd();
					return fileContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
				}
			}
		}
	}
}
