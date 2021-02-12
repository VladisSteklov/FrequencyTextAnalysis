using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrequencyАnalysis
{
    public class FileFreqAnalyzer
    {
        // Количество букв для разбиения
        private readonly int _countLetters;
        // Разделители
        private readonly char[] _splitters = { ',', '.', ':', ';', '?', '!', '/', '"', '/', '%', ' ', '\r', '\n' };

        private readonly CancellationToken _canceltokenForThisWork;
        private readonly object _addItemLocker = new object();

        // Частотный словарь
        public Dictionary<string, int> LetterFreqDictionary { get; private set; } = new Dictionary<string, int>();

        public FileFreqAnalyzer(string filePath, int countLetters, CancellationToken cancellationToken)
        {
            _countLetters = countLetters;
            _canceltokenForThisWork = cancellationToken;

            if (File.Exists(filePath))
            {
                // Чтение и разбиение текста в соответствии с разделителями
                // Для корректности требуется, чтобы файл был в кодировке UTF-8
                var splittedText = File.ReadAllText(filePath).Split(_splitters);

                // Разбить каждый элемент массива SplittedText на countLetters (триплеты, если _countLetters = 3) параллельно
                ParallelOptions parallelOptions = new ParallelOptions { CancellationToken = _canceltokenForThisWork };
                Parallel.ForEach(splittedText, parallelOptions, this.SubStringAndAddDictionary);
            }
            else
            {
                throw new FileNotFoundException("Файл не существует", filePath);
            }
        }

        public void SortSplittedTextFrequency()
        {
            List<KeyValuePair<string, int>> items = LetterFreqDictionary.ToList();
            items.Sort(new Comparison<KeyValuePair<string, int>>((first, second) =>
            {
                if (second.Value > first.Value)
                {
                    return 1;
                }
                if (second.Value < first.Value)
                {
                    return -1;
                }
                return 0;
            }));

            LetterFreqDictionary = items.ToDictionary(x => x.Key, y => y.Value);
        }

        private void SubStringAndAddDictionary(string stringFromText)
        {
            if (stringFromText.Length >= _countLetters)
            {
                // Разбить строку, длина которых больше _countLetters
                for (int i = 0; i < stringFromText.Length - _countLetters + 1; i++)
                {
                    // Получение строки
                    var partOfString = stringFromText.Substring(i, _countLetters);
                    if (partOfString.All(ch => char.IsLetter(ch)))
                    {
                        // Строка содержит только буквы
                        lock(_addItemLocker)
                        {
                            // Потокобезопасно
                            if (LetterFreqDictionary.ContainsKey(partOfString))
                            {
                                // Если элемент содержится в словаре, то увеличение частоты повторения
                                LetterFreqDictionary[partOfString]++;
                            }
                            else
                            {
                                // Иначе добавление элемента в словарь
                                LetterFreqDictionary.Add(partOfString, 1);
                            }

                        }
                        
                    }

                }
            }
        }

    }
}
