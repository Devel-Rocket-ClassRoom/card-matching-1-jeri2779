using System;
using System.Data;
using System.Data.Common;
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

    Difficulty difficulty;
    while (true)
    {
        Console.WriteLine("=== 카드 짝 맞추기 게임 ===");
        Console.WriteLine();
        Console.WriteLine("난이도를 선택하세요: \n 1. 쉬움 (2x4)\n 2. 보통 (4x4)\n 3. 어려움 (4x6)");
        Console.Write("선택: ");
        string input = Console.ReadLine();
        if (input == "1")
        {
            difficulty = Difficulty.Easy;
            break;
        }
        else if (input == "2")
        {
            difficulty = Difficulty.Normal;
            break;
        }
        else if (input == "3")
        {
            difficulty = Difficulty.Hard;
            break;
        }
        else
            Console.WriteLine("잘못된 입력입니다. 다시 입력해주세요.");

    }
    Board board = new Board(difficulty);
    GameState state = board.Play();
    board.PrintResult(state);

    Console.WriteLine();
    Console.WriteLine("새 게임을 하시겠습니까?(Y/N): ");
    string input2 = Console.ReadLine();
    if (input2 == "Y")
    {
        continue;
    }
    else if (input2 == "N")
    {
        Console.WriteLine("게임을 종료합니다.");
        Thread.Sleep(1500);
        break;
    }
     
    else
    {
        Console.WriteLine("잘못된 입력입니다. ");
        
    }
}

//void InitializeSelect()
//{
//    while (true)
//    {
//        Difficulty difficulty;
//        Console.WriteLine("=== 카드 짝 맞추기 게임 ===");
//        Console.WriteLine();
//        Console.WriteLine("난이도를 선택하세요: \n 1. 쉬움 (2x4)\n 2. 보통 (4x4)\n 3. 어려움 (4x6)");
//        Console.Write("선택: ");
//        string input = Console.ReadLine();
//        if (input == "1")
//        {
//            difficulty = Difficulty.Easy;
//            break;
//        }
//        else if (input == "2")
//        {
//            difficulty = Difficulty.Normal;
//            break;
//        }
//        else if (input == "3")
//        {
//            difficulty = Difficulty.Hard;
//            break;
//        }
//        else
//            Console.WriteLine("잘못된 입력입니다. 다시 입력해주세요.");
//    }
//}



enum GameState
{
    Playing,
    GameOver,
    Clear
}

enum Difficulty
{
    
    Easy,
    Normal,
    Hard
}





class Card
{
    public int[] numbers;
    public Card(int totalCards)
    {
        numbers = new int[totalCards];//
        int pairCount = totalCards / 2;
        for (int i = 0; i < pairCount; i++)
        {
            numbers[i] = i + 1;// 1부터 시작하는 숫자 쌍 생성
            numbers[i + pairCount] = i + 1;// 쌍의 두 번째 카드에 같은 숫자 할당
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
    protected int[,] numbers;//카드의 숫자를 저장하는 2차원 배열
    protected bool[,] isFlipped;//카드가 뒤집혔는지 여부를 저장하는 2차원 배열
    protected int rows;//행  
    protected int cols;//열
    private int tryCount = 0;
    private int findCount = 0;
    private int MaxTryCount = 1;
    private int MaxFindCount;

    public Board(Difficulty difficulty)
    {

        switch(difficulty)
        {
       

            case Difficulty.Easy:
                MaxTryCount = 1;
                
                rows = 2; 
                cols = 4;//2행 4열
                break;
            case Difficulty.Normal:
                MaxTryCount = 20;
                
                rows = 4;// 4행 4열
                cols = 4;
                break;
            case Difficulty.Hard:
                MaxTryCount = 30;
                
                rows = 4;//4행 6열
                cols = 6;
                break;
        }
        MaxFindCount = (rows * cols) / 2;
        InitializeBoard();//난이도 설정 후 보드 초기화

    }

    public void PrintResult(GameState state)//게임 결과 출력
    {
        if (state == GameState.Clear)
        {
            Console.WriteLine("=== 게임 클리어! ===");
            Console.WriteLine($"총 시도 횟수: {tryCount}");
        }
        else if (state == GameState.GameOver)
        {
            Console.WriteLine("=== 게임 오버! ===");
            Console.WriteLine("시도 횟수를 모두 사용했습니다.");
            Console.WriteLine($"찾은 쌍: {findCount}/{MaxFindCount}");
        }
    }


    public GameState Play()//게임 진행
    {
        while (true)
        {
            PrintBoard();
            if (tryCount >= MaxTryCount) return GameState.GameOver;//Gaemover state로 넘어감
            if (findCount >= MaxFindCount) return GameState.Clear;//Clear state로 넘어감
            FlipCard();
        }
    }



    void InitializeBoard()//보드 초기화
    {


        //카드 생성
        Card card = new Card(rows * cols);
        numbers = new int[rows, cols];
        isFlipped = new bool[rows, cols];
        int index = 0;
        for (int i = 0; i < rows; i++)
        {
            //Console.Write($"{i + 1}행  ");
            for (int j = 0; j < cols; j++)
            {
                numbers[i, j] = card.numbers[index++];
                //Console.Write($"** ");//생성된 카드는 **로 표시

            }
            
        }
    }

    void PrintBoard()//보드 출력
    {
        Console.Clear(); // 화면을 지우고 새로 그림
        

        Console.Write("\t");
        for(int j = 0; j < cols; j++)
        {
            Console.Write($"{j + 1}열\t");
        }
        Console.WriteLine();

        for (int i = 0; i < rows; i++)//열?
        {
            Console.Write($"{i + 1}행\t");
            for (int j = 0; j < cols; j++)
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

    
    }





    void FlipCard()//카드 뒤집기
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

    //예외 처리 및 유효성 검사
    bool TryGetValidInput(string prompt, // 입력 프롬프트
         out int row, out int col, // 행과 열을 반환하는 out 매개변수
         int excludeRow = -1, // 첫 번째 카드의 행을 제외하는 매개변수(기본값 -1)
         int excludeCol = -1)// 첫 번째 카드의 열을 제외하는 매개변수(기본값 -1)
    {
        row = -1; col = -1;
        Console.Write(prompt);
        string[] input = Console.ReadLine().Split(' ');

        if (input.Length != 2 ||// 입력이 2개X
            !int.TryParse(input[0], out row) ||// 행이 숫자X
            !int.TryParse(input[1], out col) ||// 열이 숫자X
            row < 1 || row > rows ||// 행X
            col < 1 || col > cols)// 열X
            return false;// 유효X

        row--; col--;

        if (isFlipped[row, col]) return false;// 뒤집힌 카드 선택X
        if (row == excludeRow &&
            col == excludeCol) return false;// 첫 번째 카드와 같은 위치 선택X
        return true;// 유효한 입력 반환
    }

}