using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using CommandLine;

namespace programming
{
    class Program
    {
        static void Main(string[] args)
        {
            List<byte[]> sudokus = new List<byte[]>();
            int maxSudokus;

            //parse arguments
            var options = Parser.Default.ParseArguments<ArgOptions>(args)
                .WithParsed(options => {
                    //set maxSudokus
                    maxSudokus = options.maxSudokus;

                    //parse sudokus
                    if(options.sudoku != null){
                        byte[] sudoku = ParseSudoku(options.sudoku);
                        if(sudoku != null) sudokus.Add(sudoku);
                        else Console.WriteLine("[ERROR] invalid sudoku: " + options.sudoku);
                    }
                    if(options.file != null){
                        if(File.Exists(options.file)){
                            string[] lines = File.ReadAllLines(options.file);
                            for(int i = 0; i < lines.Length; i++){
                                //start parsing each line for sudokus
                                byte[] sudoku = ParseSudoku(lines[i]);
                                if(sudoku != null) sudokus.Add(sudoku);
                                else Console.WriteLine("[ERROR] invalid sudoku: " + lines[i]);

                                //stop adding if we have reached max sudokus
                                if(sudokus.Count == maxSudokus) break;
                            }
                        } else {
                            Console.WriteLine("[ERROR] Invalid file path: " + options.file);
                        }
                    }

                });

            //start solving sudokus
            // if(sudokus.Count != 0){ //make sure there are sudokus to solve
            for(int i = 0; i < sudokus.Count; i++){
                //start processing the sudoku
                byte[] initialSudoku = sudokus[i]; //The initial state of the sudoku
                byte[] unsolvedSudoku = new byte[81]; //the sudoku variable that will be worked on
                Array.Copy(initialSudoku, 0, unsolvedSudoku, 0, initialSudoku.Length); //copy the sudoku to the work-sudoku
   
                //start solving the sudoku
                sbyte currentPos = 0; //the current position the solver is working on
                sbyte direction = 1; // 1 forward, -1 backward
                while(currentPos < 81 && currentPos >=0){
                    if(initialSudoku[currentPos] != 0) currentPos += direction; //skip if there is a constant number here
                    else {
                        //set the direction to forward
                        direction = 1;
                        unsolvedSudoku[currentPos] += 1;
                        while(!IsValid(unsolvedSudoku, currentPos) && unsolvedSudoku[currentPos] <= 9){
                            unsolvedSudoku[currentPos]++;
                        }
                        if(unsolvedSudoku[currentPos] <= 9) {currentPos++; continue;}
                        else {unsolvedSudoku[currentPos] = 0; direction = -1; currentPos--;}
                        
                    }
                }
                //write the solution to console
                Console.WriteLine($"[{i}]: {SudokuToString(initialSudoku)} {SudokuToString(unsolvedSudoku)}");
            }
        }

        //parses the read string into a byte[]
        static Byte[] ParseSudoku(string input){
            byte[] parsedSudoku = new byte[81];
            if(input.Length != 81 /* 9*9 */) { Console.WriteLine("[ERROR] not enough characters"); return null; }
            else {
                for (int i = 0; i < 81; i++){
                    parsedSudoku[i] = Byte.Parse(input[i].ToString());
                }
            }
            return parsedSudoku;
        }

        //expects to get byte[81] as input
        static string SudokuToString(byte[] b){
            StringBuilder sb = new StringBuilder(81);
            for(int i = 0; i < 81; i++) sb.Append(b[i]);
            return sb.ToString();
        }
    
        static bool IsValid (byte[] sudoku, int pos){
            byte num = sudoku[pos];
            //check if the number is too high
            if(num > 9) return false;

            //validate horizontal axis
            int horizontalOffset = pos % 9;
            int horizontalBase = pos - horizontalOffset;

            for (int x = horizontalBase; x < horizontalBase + 9; x++){
                if(num == sudoku[x] && x != pos) return false;
            }

            //validate vertical axis
            for(int y = horizontalOffset; y < 81; y += 9){
                if(num == sudoku[y] && pos != y) return false;
            }

            //validate section
            int verticalSection = pos - (pos % 27);
            int horizontalSection = (pos % 9) - (pos % 3);

            int startpos = verticalSection + horizontalSection;

            for (int y = startpos; y < startpos + 27; y += 9){
                for (int x = 0; x < 3; x++){
                    if(sudoku[x + y] == num && x + y != pos) return false;
                }
            }

            return true;
        }
    }

    //options that can be given which affect how the program works
    class ArgOptions {
        //Either input a sudoku or file
        [Option('s', "sudoku", Group="input")]
        public string sudoku {get; set;}
        [Option('f', "file"  , Group="input")]
        public string file   {get; set;}

        //max amount of sudokus it will process
        [Option('m', "max-sudokus", Default=100)]
        public int maxSudokus {get; set;}
    }
}

