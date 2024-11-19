using HammingsCode;
class Program
{
    static void Main(string[] args)
    {
        int k = 27; // Длина информационного сообщения
        List<int> dataBits = Solver.GenerateRandomMessage(k);
        Console.WriteLine($"Сгенерированное сообщение: {string.Join("", dataBits)}");

        // Создаем код Хэмминга
        List<int> hammingCode = Solver.GenerateHammingCode(dataBits);
        Console.WriteLine($"Код Хэмминга (без доп. бита): {string.Join("", hammingCode)}");

        // Добавляем дополнительный бит для обнаружения двухкратных ошибок
        List<int> hammingCodeWithParity = Solver.AddParityBitForDoubleErrorDetection(new List<int>(hammingCode));
        Console.WriteLine($"Код Хэмминга с доп. битом:    {string.Join("", hammingCodeWithParity)}");

        // Вносим ошибки
        List<int> receivedCode = Solver.IntroduceRandomError(new List<int>(hammingCodeWithParity), 2);
        Console.WriteLine($"Принятый код с ошибками:      {string.Join("", receivedCode)}");

        // Рассчитываем синдром и корректируем
        var (syndrome, errorType) = Solver.CalculateSyndrome(receivedCode);
        Console.WriteLine($"Синдром ошибки: {syndrome}, Тип ошибки: {errorType}");

        if (errorType == 1 && syndrome != 0)
        {
            // Если обнаружена однократная ошибка, корректируем её
            Solver.CorrectError(receivedCode, syndrome);
            Console.WriteLine($"Исправленный код:             {string.Join("", receivedCode)}");
        }
        else if (errorType == 2)
        {
            // Обнаружена двукратная ошибка, выводим сообщение
            Console.WriteLine("Обнаружена двукратная ошибка. Исправление невозможно.");
        }
        else
        {
            // Ошибок нет
            Console.WriteLine("Ошибок нет.");
        }
    }
}