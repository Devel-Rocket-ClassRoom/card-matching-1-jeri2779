using System;
using System.Numerics;
using System.Reflection;
using System.Threading.Channels;
//테스트
Console.WriteLine("카드를 섞는중 . . . ");
System.Threading.Thread.Sleep(1500);
Card card = new Card();
Board board = new Board();

 
    
 

while (true)
{
    board.Play();

    bool isPlayAgain = false;
    while (true)
    {

        Console.WriteLine();
        Console.WriteLine("새 게임을 하시겠습니까?(Y/N): ");
        string Y = Console.ReadLine();
        if (Y == "Y")
        {
            
            isPlayAgain = true;
            break;
        }
        else if (Y == "N")
        {
            Console.WriteLine("게임을 종료합니다.");
            break;
        }
    }

    if (!isPlayAgain)
    {
        break;
    }
}


class Card
{
    public int[] numbers = new int[16];
    public Card()
    {
        for (int i = 0; i < 8; i++)
        {
            numbers[i] = i + 1;
            numbers[i + 8] = i + 1;
        }
        Shuffle();
    }

    void Shuffle()
    {
        Random random = new Random();
        random.Shuffle(numbers);
    }
    
}

class Board
{
    private int[,] numbers = new int[4, 4];
    private bool[,] isFlipped = new bool[4, 4]; // 모든 요소는 기본 false(덮임)
    private int tryCount = 0;
    private int findCount = 0;
    private int MaxTryCount = 20;
    private int MaxFindCount = 8;

    public Board()
    {
        InitializeBoard();

    }
    public void Play()
    {

        // 1. 현재 카드 상태를 화면에 그립니다.
        PrintBoard();

        // 2. 사용자에게 입력을 받고 판정합니다. (이 안에 Clear와 Sleep이 포함됨)
        FlipCard();

    }

        void InitializeBoard()
        {
            //카드 생성
            Card card = new Card();
            int index = 0;
            for (int i = 0; i < 4; i++)
            {
                Console.Write($"{i + 1}행  ");
                for (int j = 0; j < 4; j++)
                {
                    numbers[i, j] = card.numbers[index++];
                    Console.Write($"** ");//생성된 카드는 **로 표시

                }
                Console.WriteLine();
            }
        }

        void PrintBoard()
        {
            Console.Clear(); // 화면을 지우고 새로 그림
            Console.WriteLine("\t1열\t2열\t3열\t4열");

            for (int i = 0; i < 4; i++)
            {
                Console.Write($"{i + 1}행\t");
                for (int j = 0; j < 4; j++)
                {

                    if (isFlipped[i, j])
                        Console.Write($"{numbers[i, j]}\t"); // 뒤집혔으면 숫자 출력
                    else
                        Console.Write("**\t"); // 덮였으면 기호 출력
                }
                Console.WriteLine();
            }
            Console.Write($"시도 횟수: {tryCount}/{MaxTryCount} | ");
            Console.WriteLine($"찾은 쌍: {findCount}/{MaxFindCount}");

        //게임 종료 조건
            if (tryCount >= MaxTryCount)
            {
                Console.WriteLine("게임 오버! 시도 횟수를 초과했습니다.");
                Environment.Exit(0);
            }
            if (findCount >= MaxFindCount)
            {
                Console.WriteLine("=== 게임 클리어! ===");
                Console.WriteLine($"총 시도 횟수: {tryCount}");
                Environment.Exit(0);
        }
    }

        void FlipCard()//공개할 카드를 입력받는 메서드
        {

             
            Console.Write("첫 번째 카드를 선택하세요 (행 열): ");
            string[] input1 = Console.ReadLine().Split(' '); // 공백으로 나눔
            int row = int.Parse(input1[0]) - 1;
            int col = int.Parse(input1[1]) - 1;
            isFlipped[row, col] = true;
            PrintBoard();

            Console.Write("두 번째 카드를 선택하세요 (행 열): ");
            string[] input2 = Console.ReadLine().Split(' ');
            int row2 = int.Parse(input2[0]) - 1;
            int col2 = int.Parse(input2[1]) - 1;
            isFlipped[row2, col2] = true;
            PrintBoard();

            if (numbers[row, col] == numbers[row2, col2])
            {
                Console.WriteLine("짝을 찾았습니다!");
                findCount++;
                // 일치하면 true 상태를 유지하므로 다음 PrintBoard에서도 계속 보임
            }
            else
            {
                Console.WriteLine("짝이 맞지 않습니다!");
                tryCount++;
                // 잠시 멈춰서 사용자가 두 번째 카드를 확인하게 함
                System.Threading.Thread.Sleep(1000);

                isFlipped[row, col] = false;
                isFlipped[row2, col2] = false;
            }

        }
    }