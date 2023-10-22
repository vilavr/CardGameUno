using Newtonsoft.Json.Linq;

namespace MenuSystem;
    public class GeneralPrompt<T>
    {
        public string Question { get; }
        public List<T> AllowedValues { get; }

        public GeneralPrompt(string question, List<T> allowedValues)
        {
            Question = question;
            AllowedValues = allowedValues;
        }

        public T GetUserInput()
        {
            T? userInput; 
            while (true)
            {
                Console.WriteLine(Question);
                try
                {
                    userInput = (T)Convert.ChangeType(Console.ReadLine()!, typeof(T));

                    if (AllowedValues.Contains(userInput))
                    {
                        break; // exit the loop as we've got a valid input.
                    }
                    else
                    {
                        Console.WriteLine($"Invalid selection. Please choose from the following: {string.Join(", ", AllowedValues)}");
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("Invalid format. Please try again.");
                }
                catch (InvalidCastException)
                {
                    Console.WriteLine("This type of input is not supported.");
                }
            }

            return userInput;
        }

        public void UpdateSetting(string jsonKey, T value, string filePath)
        {
            string jsonData = File.ReadAllText(filePath);
            JObject jsonObject = JObject.Parse(jsonData);
            string[] keyParts = jsonKey.Split(':');

            JToken currentToken = jsonObject;
            for (int i = 0; i < keyParts.Length; i++)
            {
                if (currentToken == null)
                {
                    throw new NullReferenceException($"Missing element at {string.Join(':', keyParts, 0, i)}");
                }

                JToken? nextToken = currentToken[keyParts[i]];

                if (nextToken == null)
                {
                    throw new KeyNotFoundException($"Key not found: {string.Join(':', keyParts, 0, i + 1)}");
                }
                else if (nextToken is JObject)
                {
                    currentToken = nextToken;
                }
                else if (i == keyParts.Length - 1)
                {
                    JToken valueToReplace = JToken.FromObject(value!);
                    currentToken[keyParts[i]]?.Replace(valueToReplace);
                }
                else
                {
                    currentToken = nextToken;
                }
            }
            jsonData = jsonObject.ToString();
            File.WriteAllText(filePath, jsonData);
            Console.WriteLine("Settings have been successfully updated!");
        }
    }
