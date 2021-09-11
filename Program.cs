using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using ConsoleTables;

namespace ex3
{
class Program
    {

    public static void Main(string[] args)
    {
        var key = new byte[128];
        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        rng.GetBytes(key);        

        if(args.Length % 2 == 0 || args.Length < 3 || args.Distinct().Count() != args.Count()){
        Console.Write("Invalid data entered, enter more than 2 odd non-repeating lines");
        return;
        }

        int RandomGen = RandomNumberGenerator.GetInt32(args.Length);
        using (HMACSHA256 hmac = new HMACSHA256(key)){
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(args[RandomGen]));
            Console.WriteLine($"Hashed Selection CPU: {Convert.ToBase64String(hash)}");
        }
        
        Console.WriteLine("Available moves:");
        for (int i = 0; i < args.Length; i++){
            Console.WriteLine($"{i + 1}) {args[i]}");
        }
        Console.WriteLine("0) Exit");
        Console.WriteLine("? - help");
        Console.Write("Enter your move: ");
        string inputSelection = Console.ReadLine();
        int selectedIndex = -1;
        while(selectedIndex == -1){
           if(int.TryParse(inputSelection, out int selectedMove) && selectedMove > 0  && selectedMove <= args.Length){
               selectedIndex = selectedMove - 1;
           } else if (inputSelection == "0"){
               return;    
           }else if(inputSelection == "?"){
               getHelpDesk(args);
               inputSelection = Console.ReadLine();
           }
           else{
                  Console.WriteLine($"Enter your move correct from 1 to {args.Length}: ");
           }
        }

        Console.WriteLine($"Your move is {args[selectedIndex]}");
        Console.WriteLine($"CPU answer: {args[RandomGen]}\nKey:{Convert.ToBase64String(key)};");

        var resultGame = getResultGame(args, selectedIndex, RandomGen);
        Console.WriteLine(resultGame);
    }

    public static string getResultGame(string[] movesArray, int selectedIndex, int RandomGen){
        List<string> ListItems = new List<string>(movesArray);
        int ListItemsCount = ListItems.Count;
        if(selectedIndex <= ListItems.Count/2){
            for(int j = 0; j < (ListItemsCount/2 - selectedIndex); j++){
                ListItems.Insert(0, ListItems[ListItemsCount - 1]);
                ListItems.RemoveAt(ListItemsCount);
            }
        } else {
            for(int j = 0; j < (selectedIndex - ListItemsCount/2); j++){
                ListItems.Insert(ListItemsCount, ListItems[0]);
                ListItems.RemoveAt(0);
             }
        }
        int indexCPU = ListItems.IndexOf(movesArray[RandomGen]);
        if(ListItemsCount/2 < indexCPU){
            return("You win");
        } else if (ListItemsCount/2 == indexCPU){
            return("Draw");
        } else {
            return("You lose");
        }

    }

    public static void getHelpDesk(string[] movesArray){
        var table = new ConsoleTable("    User vs   PC ");
        table.AddColumn(movesArray);
        string[][] rowForTable = new string[movesArray.Length][];
        for(int j = 0; j < movesArray.Length; j++){
            rowForTable[j] = new String[movesArray.Length + 1];
            rowForTable[j][0] = movesArray[j];  
            for (int i = 0; i < movesArray.Length; i++){
            rowForTable[j][i + 1] = getResultGame(movesArray, j,  i);     
            }
            table.AddRow(rowForTable[j]);
        }
        table.Write();
    }

    }
}