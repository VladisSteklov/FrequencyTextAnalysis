using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FrequencyАnalysis
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var timer = new Stopwatch();
            timer.Start();

            var cancellTokenSourceForMainTask = new CancellationTokenSource();
            var tokenForMainTask = cancellTokenSourceForMainTask.Token;

            //Console.WriteLine("Введите полное название файла");
            //var filePath = Console.ReadLine();
            //filePath = @"" + filePath;

            var filePath = @"C:/Users/Admin/source/repos/FrequencyАnalysis/FrequencyАnalysis/ExampleBigText.txt";

            // Количество букв - триплеты
            const int countLetters = 3;
            // Сколько нужно взять элементов из частотного словаря
            const int countItems = 10;

            // Передача токена, чтобы обнаружить остановку задачи
            FileWork fileWork = new FileWork(filePath, countLetters, tokenForMainTask);

            // Передача источника токена для того, чтобы остановить задачу после нажатия кнопки
            ExitWork exitWork = new ExitWork(cancellTokenSourceForMainTask);

            // Запуск задач (потоков)
            var taskMain = fileWork.WorkAsync(countItems);
            exitWork.WorkAsync();


            // Вывод результатов
            var freqResults = await taskMain;

            if(freqResults != null)
            {
                OutPutResults(freqResults);
            }

            timer.Stop();
            Console.WriteLine("Время выполнения кода {0} милисекунд", timer.ElapsedMilliseconds);

            Console.ReadKey();
        }
        private static void OutPutResults(IEnumerable<KeyValuePair<string, int>> result)
        {
            var outputString = new StringBuilder();
            foreach (var pair in result)
            {
                outputString.AppendLine($"Элемент {pair.Key} содержится {pair.Value} раз");
            }
            Console.WriteLine(outputString);
        }
    }
}
