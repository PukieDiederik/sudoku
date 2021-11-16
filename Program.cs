using System;
using System.IO;
using System.Diagnostics; //for stopwatch
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

            if(sudokus.Count != 0){
                //start processing the sudoku
                byte[] unsolvedSudoku = sudokus[0]; //SHOULDN'T BE MODIFIED
                byte[] workingSudoku = new byte[81]; //the sudoku that can be modified
                Array.Copy(unsolvedSudoku, 0, workingSudoku, 0, unsolvedSudoku.Length); //copy the sudoku to the work-sudoku

                //write input to console
                Console.Write("Input:  ");
                foreach(byte b in unsolvedSudoku) {Console.Write(b); } Console.Write("\n");
                
                
                //start solving the sudoku
                int curPos = 0; //the current position the solver is working on
                bool direction = true; //true forward, false backward
                while(curPos < 81){
                    //skips over pre-defined numbers
                    if     (unsolvedSudoku[curPos] != 0 && direction)   { curPos++; continue; }
                    else if(unsolvedSudoku[curPos] != 0 && curPos != 0) { curPos--; continue; }
                    else { //if it doesnt need to skip numbers
                        direction = true; //make sure we are moving forward again
                        workingSudoku[curPos]++; //start checking the next number
                        while(!isValid(workingSudoku, curPos) && workingSudoku[curPos] < 9){
                            workingSudoku[curPos]++; //keep increasing until we find a valid number or > 9
                        }
                        //finalize numbers
                        if(workingSudoku[curPos] < 9) curPos++;
                        else if(workingSudoku[curPos] == 9 && isValid(workingSudoku,curPos)) curPos++;
                        else { workingSudoku[curPos] = 0; curPos--; direction = false; }
                    }
                }
                //write the solution in consolse
                Console.Write("output: ");
                foreach(byte b in workingSudoku) { Console.Write(b); } Console.Write("\n"); 

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
    
        static bool isValid (byte[] sudoku, int pos){
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

