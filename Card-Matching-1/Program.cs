using System;
using System.Numerics;
using System.Reflection;
using System.Threading;
using System.Threading.Channels;
//테스트
Console.WriteLine("카드를 섞는중 . . . ");
System.Threading.Thread.Sleep(1500);
//Card card = new Card();
//Board board = new Board();







while (true)
{
    Board board = new Board();
    GameState state = board.Play();
    switch (state)
    {
        case GameState.Clear:
            Console.WriteLine("=== 게임 클리어! ===");
            break;
        case GameState.GameOver:
            Console.WriteLine("게임 오버!");
            break;
    }

     
     
     

        Console.WriteLine();
        Console.WriteLine("새 게임을 하시겠습니까?(Y/N): ");
        string Y = Console.ReadLine();
        if (Y == "Y")
        {
            Console.WriteLine("카드를 섞는중 . . . ");
            Thread.Sleep(1500);
            board = new Board();
        }
        else if (Y == "N")
        {
            Console.WriteLine("게임을 종료합니다.");
            Thread.Sleep(1500);
            break;
        }
    
}



enum GameState
{
    Playing,
    GameOver,
    Clear
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
    private int MaxTryCount = 1;
    private int MaxFindCount = 1;

    public Board()
    {
        InitializeBoard();

    }
    //public void Play()
    //{

    //    // 1. 현재 카드 상태를 화면에 그립니다.
    //    PrintBoard();

    //    // 2. 사용자에게 입력을 받고 판정합니다. (이 안에 Clear와 Sleep이 포함됨)
    //    FlipCard();


    //}

    public GameState Play()
    {
        while (true)
        {
            PrintBoard();
            if (tryCount >= MaxTryCount) return GameState.GameOver;
            if (findCount >= MaxFindCount) return GameState.Clear;
            FlipCard();
        }
    }



    void InitializeBoard()
    {
        //카드 생성
        Card card = new Card();
        int index = 0;
        for (int i = 0; i < 4; i++)
        {
            //Console.Write($"{i + 1}행  ");
            for (int j = 0; j < 4; j++)
            {
                numbers[i, j] = card.numbers[index++];
                //Console.Write($"** ");//생성된 카드는 **로 표시

            }
            //Console.WriteLine();
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
        //if (tryCount >= MaxTryCount)
        //{
        //    Console.WriteLine("게임 오버! 시도 횟수를 초과했습니다.");
             
        //}
        //if (findCount >= MaxFindCount)
        //{
        //    Console.WriteLine("=== 게임 클리어! ===");
        //    Console.WriteLine($"총 시도 횟수: {tryCount}");
             
        //}
    }





    void FlipCard()
    {
        int row, col, row2, col2;

        while (!TryGetValidInput("첫 번째 카드 (행 열): ", out row, out col))
            Console.WriteLine("잘못된 입력입니다.");

        isFlipped[row, col] = true;
        PrintBoard();

        while (!TryGetValidInput("두 번째 카드(행 열): ", out row2, out col2, row, col))
            Console.WriteLine("잘못된 입력입니다.");

        isFlipped[row2, col2] = true;
        PrintBoard();

        tryCount++;

        if (numbers[row, col] == numbers[row2, col2])
        {
            Console.WriteLine("짝을 찾았습니다!");
            Thread.Sleep(1000);
            findCount++;
        }
        else
        {
            Console.WriteLine("짝이 맞지 않습니다!");
            Thread.Sleep(1000);
            isFlipped[row, col] = false;
            isFlipped[row2, col2] = false;
        }
    }
     bool TryGetValidInput(string prompt, 
         out int row, out int col, 
         int excludeRow = -1, 
         int excludeCol = -1)
    {
        row = -1; col = -1;
        Console.Write(prompt);
        string[] input = Console.ReadLine().Split(' ');

        if (input.Length != 2 ||// 입력이 2개가 아니거나
            !int.TryParse(input[0], out row) ||// 행이 숫자가 아니거나
            !int.TryParse(input[1], out col) ||// 열이 숫자가 아니거나
            row < 1 || row > 4 || col < 1 || col > 4)// 행과 열이 1~4 범위를 벗어나거나
            return false;// 유효하지 않은 입력 반환

        row--; col--;

        if (isFlipped[row, col]) return false;// 이미 뒤집힌 카드 선택 방지
        if (row == excludeRow && col == excludeCol) return false;// 첫 번째 카드와 같은 위치 선택 방지
        return true;// 유효한 입력 반환
    }

}