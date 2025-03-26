using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Lab5
{
    class Program
    {
        static void Main(string[] args)
        {
            var incorrectWords = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"првиет", "привет"},
                {"пирвет", "привет"},
                {"превет", "привет"},
                {"приав", "привет"},
                {"слоово", "слово"},
                {"солво", "слово"},
                {"солово", "слово"}
            };

            // Получаем путь к директории
            string directoryPath;
            do
            {
                Console.WriteLine("Введите путь к директории с файлами для обработки:");
                directoryPath = Console.ReadLine();
            } while (!Directory.Exists(directoryPath));

            // Обрабатываем все текстовые файлы в директории
            ProcessFiles(directoryPath, incorrectWords);

            Console.WriteLine("Обработка завершена. Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static void ProcessFiles(string directoryPath, Dictionary<string, string> incorrectWords)
        {
            var files = Directory.GetFiles(directoryPath, "*.txt");
            if (files.Length == 0)
            {
                Console.WriteLine("Текстовые файлы (*.txt) не найдены в указанной директории.");
                return;
            }

            foreach (var filePath in files)
            {
                try
                {
                    Console.WriteLine($"Обработка файла: {Path.GetFileName(filePath)}");

                    // Читаем содержимое файла
                    string content = File.ReadAllText(filePath);
                    string originalContent = content;

                    // Исправляем ошибочные слова
                    foreach (var pair in incorrectWords)
                    {
                        content = content.Replace(pair.Key, pair.Value);
                    }

                    // Форматируем номера телефонов
                    content = FormatPhoneNumbers(content);

                    // Если были изменения - сохраняем файл
                    if (content != originalContent)
                    {
                        File.WriteAllText(filePath, content);
                        Console.WriteLine("Файл успешно обработан и сохранен.");
                    }
                    else
                    {
                        Console.WriteLine("Изменений не обнаружено.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при обработке файла {filePath}: {ex.Message}");
                }
            }
        }

        static string FormatPhoneNumbers(string input)
        {
            // Регулярное выражение для поиска номеров в формате (012) 345-67-89
            var phonePattern = new Regex(@"\((\d{3})\)\s(\d{3})-(\d{2})-(\d{2})");

            // Замена на формат +380 12 345 67 89
            return phonePattern.Replace(input, match =>
            {
                string code = match.Groups[1].Value;
                string number = $"{match.Groups[2].Value} {match.Groups[3].Value} {match.Groups[4].Value}";
                return $"+380 {code.Substring(1)} {number}";
            });
        }
    }
}