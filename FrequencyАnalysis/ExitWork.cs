using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FrequencyАnalysis
{
    public class ExitWork
    {
        private readonly CancellationTokenSource _cancellTokenSourceForMainTask;
        public ExitWork(CancellationTokenSource cancellationTokenSource)
        {
            _cancellTokenSourceForMainTask = cancellationTokenSource;
        }

        public async void WorkAsync()
        {
            await Task.Run(() =>
            {
                Console.WriteLine("Нажмите на любую клавишу для остановки частотного анализа");
                Console.ReadKey();
                // Отмена
                _cancellTokenSourceForMainTask.Cancel();
            });
            return;
        }
    }
}
