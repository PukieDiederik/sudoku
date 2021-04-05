using System;
using System.IO;

namespace programming
{
    class Program
    {
        static void Main(string[] args)
        {
            //making sure the right arghuments have been given if any
            if(args.Length > 0){
                if(File.Exists(args[0])){
                    //start processing the sudoku
                    string[] lines = File.ReadAllLines(args[0]);
                    byte[] unsolvedSudoku = ParseSudoku(lines[0].Split(',')[0]);

                    foreach(byte b in unsolvedSudoku) {Console.Write(b + " "); } Console.Write("\n");
                    
                    //start solving the sudoku
                    int curPos = 0; //the current position the solver is working on
                    byte[] workingSudoku = unsolvedSudoku;
                    bool direction = true; //true forward, false backward
                    while(curPos < 81){
                        //skips over pre-defined numbers
                        if     (unsolvedSudoku[curPos] != 0 && direction)   {curPos++; continue;}
                        else if(unsolvedSudoku[curPos] != 0 && curPos != 0) {curPos--; Console.WriteLine("UWU " + curPos); continue; }
                        else { //if it doesnt need to skip numbers
                            if(workingSudoku[curPos] >= 9) { //check if we need to abandon this branch
                                Console.WriteLine("working???");
                                workingSudoku[curPos] = 0;
                                curPos--;
                                direction = false;
                            } else {
                                direction = true;
                                workingSudoku[curPos]++;
                                while(!isValid(workingSudoku, curPos) && workingSudoku[curPos] < 9){
                                    workingSudoku[curPos]++;
                                    if(workingSudoku[curPos] == 9) {Console.WriteLine("curpos: " + curPos);}
                                }
                                if(workingSudoku[curPos] < 9) curPos++;
                                else if(workingSudoku[curPos] == 9) {
                                    if(isValid(workingSudoku,curPos)) curPos++;
                                    else {workingSudoku[curPos] = 0; curPos--; direction = false;}
                                } else {workingSudoku[curPos] = 0; curPos--; direction = false;}
                            }
                        }
                    }
                    foreach(byte b in workingSudoku) {Console.Write(b); } Console.Write("\n");

                } else Console.WriteLine("[ERROR] Invalid path or file does not exist");
            } else Console.WriteLine("[ERROR] Didn't specify file path");

        }

        //parses the read string into a byte[]
        static Byte[] ParseSudoku(string input){
            byte[] parsedSudoku = new byte[81];

            if(input.Length != 81 /* 9^2 */) { Console.WriteLine("[ERROR] not enough characters"); return null; }
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
}

