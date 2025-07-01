using System;
using System.Collections.Generic;


namespace Bowling_v3
{
    public class Frame
    {
        private bool bonus = false ; // Индикатор бонусных ударов
        private List<int> rolls = new List<int>(); // Список бросков

        public bool get_bonus()
        {
            return bonus;
        }

        public void set_bonus(bool pins)
        {
            bonus = pins;
        }

        public List<int> get_rolls()
        {
            return rolls;
        }

        public void add_roll (int pins)
        {
            rolls.Add(pins);
        }

        public void checking_bonus(int currentFrame) // Проверяем есть ли бонусные удары
        {
            if (currentFrame >= 9)
            {
                if (rolls[^2] == 10 || rolls[^1] + rolls[^2] == 10)
                {
                    bonus = true;
                }
            }
        }

        public void checking_pins(int pins) // Правильно ли ввели количество сбитых кегель
        {
            if (pins < 0 || pins > 10)
            {
                throw new ArgumentException("Количество сбитых кегель должно быть от 0 до 10.");
            }
        }

        public void checking_summ_two_rolls(int pins) // Проверяем, что сумма двух бросков не превышает 10
        {
            if (rolls[^1] + pins > 10 && rolls[^1] < 10)
            {
                throw new ArgumentException("Cумма двух бросков превышает 10 кегель.");
            }
        }
        
    }
    
    
    public class BowlingGame
    {
        private int currentFrame = 0; // Текущий фрейм
        private Frame frame = new Frame();

        // Метод для добавления результата броска
        public void Roll(int pins)
        {
            frame.set_bonus(false); // Снимаем бонусные удары
            
            frame.checking_pins(pins); // Правильно ли ввели количество сбитых кегель

            if (currentFrame < 10)
            {
                // Первый бросок в фрейме
                if (frame.get_rolls().Count % 2 == 0) 
                {
                    frame.add_roll(pins); // Сохраняем бросок
                }
                else // Второй бросок в фрейме
                {
                    // Проверяем, что сумма двух бросков не превышает 10
                    frame.checking_summ_two_rolls(pins);

                    
                    frame.add_roll(pins); // Сохраняем бросок
                    currentFrame++; // Завершаем фрейм
                }
            }
            else if(currentFrame == 10) 
            {
                if (frame.get_rolls()[^2] == 10) // Проверка на страйк в 10 фрейме
                {
                    // Проверяем, что сумма двух бросков не превышает 10
                    frame.checking_summ_two_rolls(pins);
                    
                    frame.add_roll(pins); // Сохраняем бросок
                    currentFrame++; // Завершаем фрейм
                }
                else if (frame.get_rolls()[^1] + frame.get_rolls()[^2] == 10)// Проверка на спар в 10 фрейме
                {
                    frame.add_roll(pins); // Сохраняем бросок
                    currentFrame++; // Завершаем фрейм
                }
                else
                {
                    throw new ArgumentException("Игра не может быть засчитана.");
                }
            }
            else
            {
                throw new ArgumentException("Игра не может быть засчитана.");
            }
            
            
            
            frame.checking_bonus(currentFrame); // Проверяем есть ли бонусные удары
        }

        // Метод для подсчета очков
        public int Score()
        {
            int score = 0;
            int frameIndex = 0;

            if (frame.get_bonus())
            {
                if (frame.get_rolls().Count < 21)
                {
                    throw new ArgumentException("Незавершенная игра не может быть засчитана.");
                }
            }

            if (frame.get_rolls().Count < 10)
            {
                throw new ArgumentException("Незавершенная игра не может быть засчитана.");
            }

            for (int frame = 0; frame < 10; frame++)
            {
                if (IsStrike(frameIndex)) // Проверка на страйк
                {
                    score += 10 + StrikeBonus(frameIndex);
                    frameIndex++;
                }
                else if (IsSpare(frameIndex)) // Проверка на спэр
                {
                    score += 10 + SpareBonus(frameIndex);
                    frameIndex += 2;
                }
                else // Открытый фрейм
                {
                    score += SumOfPinsInFrame(frameIndex);
                    frameIndex += 2;
                }
            }

            return score;
        }

        private bool IsStrike(int frameIndex)
        {
            return frame.get_rolls()[frameIndex] == 10;
        }

        private bool IsSpare(int frameIndex)
        {
            return frame.get_rolls()[frameIndex] + frame.get_rolls()[frameIndex + 1] == 10;
        }

        private int StrikeBonus(int frameIndex)
        {
            return frame.get_rolls()[frameIndex + 1] + frame.get_rolls()[frameIndex + 2];
        }

        private int SpareBonus(int frameIndex)
        {
            return frame.get_rolls()[frameIndex + 2];
        }

        private int SumOfPinsInFrame(int frameIndex)
        {
            return frame.get_rolls()[frameIndex] + frame.get_rolls()[frameIndex + 1];
        }
    }


    public class Program
    {
        public static void Main(string[] args)
        {
            BowlingGame game = new BowlingGame();
            string input;

            Console.WriteLine("Введите количество сбитых кегель (или 'готово' для завершения):");

            while ((input = Console.ReadLine()) != "готово")
            {
                if (int.TryParse(input, out int pins))
                {
                    try
                    {
                        game.Roll(pins);
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Пожалуйста, введите корректное количество сбитых кегель или 'готово'.");
                }
            }

            try
            {
                Console.WriteLine($"Ваш общий счет: {game.Score()}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}