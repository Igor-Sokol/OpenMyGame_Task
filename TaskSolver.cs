using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace TaskExam
{
    internal class TaskSolver
    {
        public static void Main(string[] args)
        {
            TestGenerateWordsFromWord();
            TestMaxLengthTwoChar();
            TestGetPreviousMaxDigital();
            TestSearchQueenOrHorse();
            TestCalculateMaxCoins();

            Console.WriteLine("All Test completed!");
        }


        /// задание 1) Слова из слова
        public static List<string> GenerateWordsFromWord(string word, List<string> wordDictionary)
        {
            List<string> reuslt = new List<string>();
            Dictionary<char, int> letters = new Dictionary<char, int>();

            foreach (var item in wordDictionary)
            {
                bool correct = true;

                ConfigureDictionary(letters, word);
                foreach (var letter in item)
                {
                    if (letters.ContainsKey(letter) && letters[letter] > 0)
                    {
                        letters[letter]--;
                    }
                    else
                    {
                        correct = false;
                        break;
                    }
                }

                if (correct)
                {
                    reuslt.Add(item);
                }
            }

            return reuslt.OrderBy(w => w).ToList();

            // Nested Methods
            static void ConfigureDictionary(Dictionary<char, int> dictionary, string word)
            {
                dictionary.Clear();

                foreach (var ch in word)
                {
                    if (dictionary.ContainsKey(ch))
                    {
                        dictionary[ch]++;
                    }
                    else 
                    {
                        dictionary.Add(ch, 1);
                    }
                }
            }
        }

        /// задание 2) Два уникальных символа
        public static int MaxLengthTwoChar(string word)
        {
            string currentWord = word;

            int maxLength = 0;
            foreach (var item in CreateUniquePairs(currentWord))
            {
                string tempWord = DeleteChar(currentWord, ch => ch != item.a && ch != item.b);

                if (!IsRepeatedInRowExist(tempWord, out _) && maxLength < tempWord.Length)
                {
                    maxLength = tempWord.Length;
                }
            }

            return maxLength;

            // Nested Methods
            static IEnumerable<(char a, char b)> CreateUniquePairs(IEnumerable<char> chars)
            {
                HashSet<char> uniqueChars = new HashSet<char>(chars);
                List<char> cachedChars = uniqueChars.ToList();

                for (int i = 0; i < cachedChars.Count; i++)
                {
                    for (int j = 1; j < cachedChars.Count - i; j++)
                    {
                        yield return (cachedChars[i], cachedChars[i + j]);
                    }
                }
            }

            static bool IsRepeatedInRowExist(string word, out char repeatedChar)
            {
                char previousChar = default;

                foreach (var letter in word)
                {
                    if (letter == previousChar)
                    {
                        repeatedChar = letter;
                        return true;
                    }

                    previousChar = letter;
                }

                repeatedChar = default;
                return false;
            }

            static string DeleteChar(string word, Predicate<char> predicate)
            {
                if (predicate is null)
                {
                    return word;
                }

                StringBuilder result = new StringBuilder(word);

                for (int i = 0; i < result.Length; i++)
                {
                    if (predicate(result[i]))
                    {
                        result.Remove(i, 1);
                        i--;
                    }
                }

                return result.ToString();
            }
        }

        /// задание 3) Предыдущее число
        public static long GetPreviousMaxDigital(long value)
        {
            if (value < 0 || value > 1_000_000_000_000)
            {
                return -1;
            }

            int[] numbers = LongToNumbers(value);

            for (int i = numbers.Length - 1; i >= 0; i--)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    if (numbers[i] < numbers[j])
                    {
                        (numbers[i], numbers[j]) = (numbers[j], numbers[i]);

                        Array.Sort(numbers, j + 1, numbers.Length - (j + 1), Comparer<int>.Create(new Comparison<int>((a, b) => b.CompareTo(a))));

                        if (numbers[0] == 0)
                        {
                            return -1;
                        }

                        return NumbersToLong(numbers);
                    }
                }
            }

            return -1;

            // Nested Methods
            static int[] LongToNumbers(long value)
            {
                if (value < 0)
                {
                    throw new ArgumentException("Value can not be less than 0.", nameof(value));
                }

                if (value == 0)
                {
                    return new int[] { 0 };
                }

                List<int> result = new List<int>();
                long currentValue = value;

                while (currentValue > 0)
                {
                    result.Insert(0, (int)(currentValue % 10L));
                    currentValue /= 10L;
                }

                return result.ToArray();
            }

            static long NumbersToLong(int[] numbers)
            {
                long result = 0;
                for (int i = 0; i < numbers.Length; i++)
                {
                    if (numbers[i] < 0 || numbers[i] > 9)
                    {
                        throw new ArgumentException("Number can not be less than 0 or more than 9.", nameof(numbers));
                    }

                    result *= 10;
                    result += numbers[i];
                }

                return result;
            }
        }

        /// задание 4) Конь и Королева
        public static List<int> SearchQueenOrHorse(char[][] gridMap)
        {
            Vector2 startPosition = FindChar(gridMap, 's');
            Vector2 exitPosition = FindChar(gridMap, 'e');

            if (exitPosition == new Vector2(-1, -1) || startPosition == new Vector2(-1, -1))
            {
                return new List<int> { -1, -1 };
            }

            List<int> result = new List<int>();

            Vector2[] horseDirections = new Vector2[]
            {
                new Vector2(1, 2), new Vector2(2, 1), // I
                new Vector2(-1, 2), new Vector2(-2, 1), // II
                new Vector2(-1, -2), new Vector2(-2, -1), // III
                new Vector2(1, -2), new Vector2(2, -1), // IV
            };
            result.Add(FindSteps(gridMap, startPosition, exitPosition, horseDirections, false));

            Vector2[] queenDirections = new Vector2[]
            {
                new Vector2(1, 1),
                new Vector2(1, 0),
                new Vector2(1, -1),
                new Vector2(0, -1),
                new Vector2(-1, 1),
                new Vector2(-1, 0),
                new Vector2(-1, -1),
                new Vector2(0, 1),
            };
            result.Add(FindSteps(gridMap, startPosition, exitPosition, queenDirections, true));

            return result;

            // Nested Methods
            static int FindSteps(char[][] grid, Vector2 startPosition, Vector2 exitPosition, Vector2[] moveDirections, bool unlimitedDistance)
            {
                int[][] tempGrid = GetMatrix(grid.GetLength(0), grid[0].Length, -1);

                List<Vector2> positions = new List<Vector2> { startPosition };
                tempGrid[(int)startPosition.Y][(int)startPosition.X] = 0;

                while (positions.Count > 0)
                {
                    Vector2 stayPosition = FindMinManhattanDistance(positions, exitPosition);
                    int currentStep = tempGrid[(int)stayPosition.Y][(int)stayPosition.X];
                    positions.Remove(stayPosition);

                    for (int j = 0; j < moveDirections.Length; j++)
                    {
                        Vector2 tempPosition = stayPosition + moveDirections[j];

                        while (IsValidPlace(grid, tempPosition))
                        {
                            int gridValue = tempGrid[(int)tempPosition.Y][(int)tempPosition.X];
                            if (gridValue >= currentStep || gridValue == -1)
                            {
                                if (tempPosition == exitPosition)
                                {
                                    return currentStep + 1;
                                }

                                tempGrid[(int)tempPosition.Y][(int)tempPosition.X] = currentStep + 1;
                                positions.Add(tempPosition);
                            }

                            if (unlimitedDistance)
                            {
                                tempPosition += moveDirections[j];
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }

                return -1;
            }

            static Vector2 FindMinManhattanDistance(List<Vector2> steps, Vector2 end)
            {
                float minDistance = int.MaxValue;
                Vector2 nextStep = new Vector2(-1, -1);

                for (int i = 0; i < steps.Count; i++)
                {
                    float distance = MathF.Abs(end.X - steps[i].X) + MathF.Abs(end.Y - steps[i].Y);

                    if (minDistance > distance)
                    {
                        minDistance = distance;
                        nextStep = steps[i];
                    }
                }

                return nextStep;
            }

            static int[][] GetMatrix(int n, int m, int defaultValue = 0)
            {
                int[][] matrix = new int[n][];

                for (int i = 0; i < n; i++)
                {
                    matrix[i] = new int[m];

                    if (defaultValue != 0)
                    {
                        for (int j = 0; j < m; j++)
                        {
                            matrix[i][j] = defaultValue;
                        }
                    }
                }

                return matrix;
            }

            static Vector2 FindChar(char[][] grid, char ch)
            {
                for (int i = 0; i < grid.GetLength(0); i++)
                {
                    for (int j = 0; j < grid[i].Length; j++)
                    {
                        if (grid[i][j] == ch)
                        {
                            return new Vector2(j, i);
                        }
                    }
                }

                return new Vector2(-1, -1);
            }

            static bool IsValidPlace(char[][] grid, Vector2 position)
            {
                if (position.X < 0 || position.X >= grid[0].Length)
                {
                    return false;
                }

                if (position.Y < 0 || position.Y >= grid.GetLength(0))
                {
                    return false;
                }

                if (grid[(int)position.Y][(int)position.X] == 'x')
                {
                    return false;
                }

                return true;
            }
        }

        /// задание 5) Жадина
        public static long CalculateMaxCoins(int[][] mapData, int idStart, int idFinish)
        {
            City startCity = GenerateMap(mapData, idStart);

            return FindMaxCoins(startCity, idFinish);

            // Nested Methods
            int FindMaxCoins(City startCity, int finishId)
            {
                int maxCollectedCoins = -1;

                Queue<CityWalker> walkers = new Queue<CityWalker>();
                walkers.Enqueue(new CityWalker(startCity));

                while (walkers.Count > 0)
                {
                    CityWalker walker = walkers.Dequeue();

                    foreach (var transition in walker.CurrentCity.Transitions)
                    {
                        if (!walker.PassedCities.Contains(transition.NextCity.Id))
                        {
                            CityWalker clone = walker.GetCopy();
                            clone.Move(transition);

                            if (clone.CurrentCity.Id == finishId && clone.Wallet > maxCollectedCoins)
                            {
                                maxCollectedCoins = clone.Wallet;
                            }
                            else
                            {
                                walkers.Enqueue(clone);
                            }
                        }
                    }
                }

                return maxCollectedCoins;
            }

            static City GenerateMap(int[][] mapData, int startId)
            {
                List<City> cities = new List<City>();

                foreach (var transit in mapData)
                {
                    int firstCityId = transit[0];
                    int secondCityId = transit[1];
                    int coins = transit[2];

                    City firstCity = cities.FirstOrDefault(city => city.Id == firstCityId) ?? new City(firstCityId);
                    City secondCity = cities.FirstOrDefault(city => city.Id == secondCityId) ?? new City(secondCityId);

                    Transition transitionTo = new Transition(secondCity, coins);
                    Transition transitionFrom = new Transition(firstCity, coins);

                    firstCity.AddTransition(transitionTo);
                    secondCity.AddTransition(transitionFrom);

                    cities.Add(firstCity);
                    cities.Add(secondCity);
                }

                return cities.FirstOrDefault(city => city.Id == startId);
            }
        }

        #region Task 5 Structs
        class City
        {
            private readonly List<Transition> transitions = new List<Transition>();

            public int Id { get; }
            public IEnumerable<Transition> Transitions => transitions.Where(_ => true); // Where - Защита от явного привидения к List и нарушения приватности
            public int TransitionCount => transitions.Count;

            public City(int id)
            {
                this.Id = id;
            }

            public void AddTransition(Transition transition)
            {
                transitions.Add(transition);
            }
        }

        struct Transition
        {
            public int Coins { get; }
            public City NextCity { get; }

            public Transition(City city, int coins)
            {
                this.NextCity = city;
                this.Coins = coins;
            }
        }

        struct CityWalker
        {
            private readonly List<int> passedCitiesId;

            public City CurrentCity { get; private set; }
            public int Wallet { get; private set; }
            public IEnumerable<int> PassedCities => passedCitiesId.Where(_ => true); // Where - Защита от явного привидения к List и нарушения приватности

            public CityWalker(City startCity)
            {
                this.CurrentCity = startCity;
                this.passedCitiesId = new List<int> { this.CurrentCity.Id };
                this.Wallet = default;
            }

            private CityWalker(IEnumerable<int> passedCitiesId, City currentCity, int wallet)
            {
                this.passedCitiesId = new List<int>(passedCitiesId);
                this.CurrentCity = currentCity;
                this.Wallet = wallet;
            }

            public void Move(Transition transition)
            {
                this.Wallet += transition.Coins;
                this.passedCitiesId.Add(transition.NextCity.Id);
                this.CurrentCity = transition.NextCity;
            }

            public CityWalker GetCopy()
            {
                return new CityWalker(passedCitiesId, CurrentCity, Wallet);
            }
        }
        #endregion

        /// Тесты (можно/нужно добавлять свои тесты) 

        private static void TestGenerateWordsFromWord()
        {
            var wordsList = new List<string>
            {
                "кот", "ток", "око", "мимо", "гром", "ром", "мама",
                "рог", "морг", "огр", "мор", "порог", "бра", "раб", "зубр"
            };

            AssertSequenceEqual(GenerateWordsFromWord("арбуз", wordsList), new[] { "бра", "зубр", "раб" });
            AssertSequenceEqual(GenerateWordsFromWord("лист", wordsList), new List<string>());
            AssertSequenceEqual(GenerateWordsFromWord("маг", wordsList), new List<string>());
            AssertSequenceEqual(GenerateWordsFromWord("погром", wordsList), new List<string> { "гром", "мор", "морг", "огр", "порог", "рог", "ром" });
        }

        private static void TestMaxLengthTwoChar()
        {
            AssertEqual(MaxLengthTwoChar("beabeeab"), 5);
            AssertEqual(MaxLengthTwoChar("а"), 0);
            AssertEqual(MaxLengthTwoChar("ab"), 2);

            AssertEqual(MaxLengthTwoChar("aabbcc"), 0);
            AssertEqual(MaxLengthTwoChar("abcabcab"), 6);
            AssertEqual(MaxLengthTwoChar("abcabcabc"), 6);
            AssertEqual(MaxLengthTwoChar("adbdcabc"), 4);
            AssertEqual(MaxLengthTwoChar("abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyza"), 7);
        }

        private static void TestGetPreviousMaxDigital()
        {
            AssertEqual(GetPreviousMaxDigital(21), 12L);
            AssertEqual(GetPreviousMaxDigital(531), 513L);
            AssertEqual(GetPreviousMaxDigital(1027), -1L);
            AssertEqual(GetPreviousMaxDigital(2071), 2017L);
            AssertEqual(GetPreviousMaxDigital(207034), 204730L);
            AssertEqual(GetPreviousMaxDigital(135), -1L);
        }

        private static void TestSearchQueenOrHorse()
        {
            char[][] gridA =
            {
                new[] {'s', '#', '#', '#', '#', '#'},
                new[] {'#', 'x', 'x', 'x', 'x', '#'},
                new[] {'#', '#', '#', '#', 'x', '#'},
                new[] {'#', '#', '#', '#', 'x', '#'},
                new[] {'#', '#', '#', '#', '#', 'e'},
            };

            AssertSequenceEqual(SearchQueenOrHorse(gridA), new[] { 3, 2 });

            char[][] gridB =
            {
                new[] {'s', '#', '#', '#', '#', 'x'},
                new[] {'#', 'x', 'x', 'x', 'x', '#'},
                new[] {'#', 'x', '#', '#', 'x', '#'},
                new[] {'#', '#', '#', '#', 'x', '#'},
                new[] {'x', '#', '#', '#', '#', 'e'},
            };

            AssertSequenceEqual(SearchQueenOrHorse(gridB), new[] { -1, 3 });

            char[][] gridC =
            {
                new[] {'s', '#', '#', '#', '#', 'x'},
                new[] {'x', 'x', 'x', 'x', 'x', 'x'},
                new[] {'#', '#', '#', '#', 'x', '#'},
                new[] {'#', '#', '#', 'e', 'x', '#'},
                new[] {'x', '#', '#', '#', '#', '#'},
            };

            AssertSequenceEqual(SearchQueenOrHorse(gridC), new[] { 2, -1 });


            char[][] gridD =
            {
                new[] {'e', '#'},
                new[] {'x', 's'},
            };

            AssertSequenceEqual(SearchQueenOrHorse(gridD), new[] { -1, 1 });

            char[][] gridE =
            {
                new[] {'e', '#'},
                new[] {'x', 'x'},
                new[] {'#', 's'},
            };

            AssertSequenceEqual(SearchQueenOrHorse(gridE), new[] { 1, -1 });

            char[][] gridF =
            {
                new[] {'x', '#', '#', 'x'},
                new[] {'#', 'x', 'x', '#'},
                new[] {'#', 'x', '#', 'x'},
                new[] {'e', 'x', 'x', 's'},
                new[] {'#', 'x', 'x', '#'},
                new[] {'x', '#', '#', 'x'},
            };

            AssertSequenceEqual(SearchQueenOrHorse(gridF), new[] { -1, 5 });

            char[][] gridAA =
            {
                new[] {'s', '#', '#', '#', '#', '#'},
                new[] {'#', 'x', 'x', 'x', 'x', '#'},
                new[] {'#', '#', '#', '#', 'x', '#'},
                new[] {'#', '#', 'e', '#', 'x', '#'},
                new[] {'#', '#', '#', '#', '#', '#'},
            };

            AssertSequenceEqual(SearchQueenOrHorse(gridAA), new[] { 3, 2 });
        }

        private static void TestCalculateMaxCoins()
        {
            var mapA = new[]
            {
                new []{0, 1, 1},
                new []{0, 2, 4},
                new []{0, 3, 3},
                new []{1, 3, 10},
                new []{2, 3, 6},
            };

            AssertEqual(CalculateMaxCoins(mapA, 0, 3), 11L);

            var mapB = new[]
            {
                new []{0, 1, 1},
                new []{1, 2, 53},
                new []{2, 3, 5},
                new []{5, 4, 10}
            };

            AssertEqual(CalculateMaxCoins(mapB, 0, 5), -1L);

            var mapC = new[]
            {
                new []{0, 1, 1},
                new []{0, 3, 2},
                new []{0, 5, 10},
                new []{1, 2, 3},
                new []{2, 3, 2},
                new []{2, 4, 7},
                new []{3, 5, 3},
                new []{4, 5, 8}
            };

            AssertEqual(CalculateMaxCoins(mapC, 0, 5), 19L);
        }

        /// Тестирующая система, лучше не трогать этот код

        private static void Assert(bool value)
        {
            if (value)
            {
                return;
            }

            throw new Exception("Assertion failed");
        }

        private static void AssertEqual(object value, object expectedValue)
        {
            if (value.Equals(expectedValue))
            {
                return;
            }

            throw new Exception($"Assertion failed expected = {expectedValue} actual = {value}");
        }

        private static void AssertSequenceEqual<T>(IEnumerable<T> value, IEnumerable<T> expectedValue)
        {
            if (ReferenceEquals(value, expectedValue))
            {
                return;
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (expectedValue is null)
            {
                throw new ArgumentNullException(nameof(expectedValue));
            }

            var valueList = value.ToList();
            var expectedValueList = expectedValue.ToList();

            if (valueList.Count != expectedValueList.Count)
            {
                throw new Exception($"Assertion failed expected count = {expectedValueList.Count} actual count = {valueList.Count}");
            }

            for (var i = 0; i < valueList.Count; i++)
            {
                if (!valueList[i].Equals(expectedValueList[i]))
                {
                    throw new Exception($"Assertion failed expected value at {i} = {expectedValueList[i]} actual = {valueList[i]}");
                }
            }
        }

    }
}