namespace HammingsCode;
using System.Collections.Generic;
public class Solver
{
    public static List<int> GenerateRandomMessage(int k)
    {
        Random random = new Random();
        List<int> message = new List<int>();

        for (int i = 0; i < k; i++)
        {
            message.Add(random.Next(0, 2)); // 0 или 1
        }

        return message;
    }

    // Вычисление количества контрольных битов
    public static int CalculateParityBits(int messageLength)
    {
        int p = 0;
        while ((1 << p) < (messageLength + p + 1))
        {
            p++;
        }
        return p;
    }

    // Вставка контрольных битов в нужные позиции
    public static List<int> InsertParityBits(List<int> dataBits, int p)
    {
        List<int> hammingCode = new List<int>();
        int dataIdx = 0, parityIdx = 0;

        for (int i = 1; i <= dataBits.Count + p; i++)
        {
            if (IsPowerOfTwo(i))
            {
                // Если позиция степени двойки, добавляем контрольный бит (изначально 0)
                hammingCode.Add(0);
                parityIdx++;
            }
            else
            {
                // Иначе добавляем информационные биты
                hammingCode.Add(dataBits[dataIdx]);
                dataIdx++;
            }
        }

        return hammingCode;
    }

    // Проверка, является ли число степенью двойки
    public static bool IsPowerOfTwo(int x)
    {
        return (x & (x - 1)) == 0;
    }

    // Расчет значений контрольных битов
    public static void CalculateCheckBits(List<int> hammingCode)
    {
        int n = hammingCode.Count;
        int p = CalculateParityBits(n);

        for (int i = 0; i < p; i++)
        {
            int parityPos = 1 << i;
            int paritySum = 0;

            // Подсчитываем сумму битов, контролируемых текущим контрольным битом
            for (int j = parityPos - 1; j < n; j += 2 * parityPos)
            {
                for (int k = 0; k < parityPos && j + k < n; k++)
                {
                    paritySum += hammingCode[j + k];
                }
            }

            // Устанавливаем значение контрольного бита
            hammingCode[parityPos - 1] = paritySum % 2;
        }
    }

    // Создание кода Хэмминга
    public static List<int> GenerateHammingCode(List<int> dataBits)
    {
        int p = CalculateParityBits(dataBits.Count);
        List<int> hammingCode = InsertParityBits(dataBits, p);
        CalculateCheckBits(hammingCode);

        return hammingCode;
    }

    // Добавление дополнительного бита для обнаружения двухкратных ошибок
    public static List<int> AddParityBitForDoubleErrorDetection(List<int> hammingCode)
    {
        int parityBit = 0;
        foreach (int bit in hammingCode)
        {
            parityBit ^= bit;
        }

        hammingCode.Add(parityBit); // Добавляем бит четности в конец
        return hammingCode;
    }

    public static List<int> IntroduceRandomError(List<int> hammingCode, int maxErrors)
    {
        Random random = new Random();
        int errorCount = random.Next(0, maxErrors + 1);
        List<int> errorPositions = new List<int>();

        for (int i = 0; i < errorCount; i++)
        {
            int pos = random.Next(0, hammingCode.Count);
            if (!errorPositions.Contains(pos))
            {
                hammingCode[pos] ^= 1; // Инвертируем бит
                errorPositions.Add(pos);
            }
        }

        // Выводим позиции ошибок или сообщение о том, что ошибок нет
        if (errorPositions.Count > 0)
        {
            Console.WriteLine($"Позиции ошибок: {string.Join(", ", errorPositions)}");
        }
        else
        {
            Console.WriteLine("Ошибки не были сгенерированы.");
        }

        return hammingCode;
    }

    // Расчет синдрома ошибки
    public static (int, int) CalculateSyndrome(List<int> hammingCode)
    {
        int n = hammingCode.Count;
        int p = CalculateParityBits(n - 1); // исключаем дополнительный бит
        int syndrome = 0;

        // Подсчет синдрома на основе контрольных битов
        for (int i = 0; i < p; i++)
        {
            int parityPos = 1 << i;
            int paritySum = 0;

            // Подсчет суммы битов для текущего контрольного бита
            for (int j = parityPos - 1; j < n; j += 2 * parityPos)
            {
                for (int k = 0; k < parityPos && j + k < n; k++)
                {
                    paritySum += hammingCode[j + k];
                }
            }

            if (paritySum % 2 != 0)
            {
                syndrome += parityPos;
            }
        }

        // Проверка на наличие однократной или двукратной ошибки
        bool overallParityMatches = hammingCode[^1] == CalculateOverallParity(hammingCode.GetRange(0, n - 1));

        // Определяем количество ошибок
        int errorType;
        if (syndrome == 0 && overallParityMatches)
        {
            errorType = 0; // Нет ошибок
        }
        else if (syndrome != 0 && overallParityMatches)
        {
            errorType = 2; // Двукратная ошибка
        }
        else
        {
            errorType = 1; // Однократная ошибка
        }

        return (syndrome, errorType);
    }

    // Метод для расчета общего контрольного бита
    public static int CalculateOverallParity(List<int> hammingCode)
    {
        int parity = 0;
        foreach (int bit in hammingCode)
        {
            parity ^= bit;
        }
        return parity;
    }

    public static void CorrectError(List<int> hammingCode, int syndrome)
    {
        if (syndrome > 0 && syndrome <= hammingCode.Count)
        {
            hammingCode[syndrome - 1] ^= 1; // Инвертируем ошибочный бит
        }
    }
}