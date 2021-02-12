using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrequencyАnalysis
{
    public class FileWork
    {
        private readonly int _countLetters;
        private readonly string _filePath;
        private readonly CancellationToken _canceltokenForThisWork;

        private FileFreqAnalyzer _fileFreqAnalyzer;

        public FileWork(string filePath, int countLetters, CancellationToken token)
        {
            _countLetters = countLetters;
            _filePath = filePath;
            _canceltokenForThisWork = token;
        }

        public async Task<IEnumerable<KeyValuePair<string, int>>> WorkAsync(int countItemsForTake)
        {
            return await Task.Run(() =>
            {
                try
                {
                    _fileFreqAnalyzer = new FileFreqAnalyzer(_filePath, _countLetters, _canceltokenForThisWork);

                    // Вызвать исключение, если пришла отмена
                    _canceltokenForThisWork.ThrowIfCancellationRequested();

                    // Сортировка частотного словаря
                    _fileFreqAnalyzer.SortSplittedTextFrequency();

                    // Взять countItems элементов
                    return _fileFreqAnalyzer.LetterFreqDictionary.Take(countItemsForTake);
                }
                catch(OperationCanceledException)
                {
                    Console.WriteLine("Работа отменена");
                    // Попробывать получить текущий результат задачи
                    if(
                    _fileFreqAnalyzer != null &&
                    _fileFreqAnalyzer.LetterFreqDictionary != null &&
                    _fileFreqAnalyzer.LetterFreqDictionary.Count >= countItemsForTake
                    )
                    {
                        return _fileFreqAnalyzer.LetterFreqDictionary.Take(countItemsForTake);
                    }
                    return null;
                }
                catch(FileNotFoundException exception)
                {
                    Console.WriteLine("Файл не {0} найден", exception.FileName);
                    return null;
                }
                catch
                {
                    return null;
                }
            });

        }
        
    }
}
