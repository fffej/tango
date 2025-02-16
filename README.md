# Tango

inkedIn has a logic game called Tango. I’ve become mildly obsessed with it. The rules of the game are simple.

- It’s played on a 6x6 grid.
- The grid must be filled with symbols (either X or Y)
- No more than 2 X or 2 Y may be next to each other, either vertically or horizontally
- Each row and column must contain the same number of X or Y
- Cells separated by an `=` sign must contain the same type
- Cells separated by an `x` sign must be of the opposite symbol
- Each puzzle has one right answer and can be solved via deduction. No guesses required.

This is some code that simulates it and tries to find the hardest puzzle.

Suggestions for improvement welcome.



It's played on a 6x6 grid.
The grid must be filled with symbols (either X or Y)
No more than 2 X or 2 Y may be next to each other, either vertically or horizontally
Each row and column must contain the same number of X or Y
Cells separated by an = sign must contain the same type
Cells separated by an x sign must be of the opposite symbol
Each puzzle has one right answer and can be solved via deduction. No guesses required.




public class PuzzleMinimizer
{
    // Returns a minimized version of the puzzle.
    public static Tango Minimize(Tango puzzle)
    {
        bool progress = true;
        
        while (progress)
        {
            progress = false;
            // Try to remove fixed cells one by one.
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 6; col++)
                {
                    if (puzzle.IsCellFixed(row, col))
                    {
                        // Create a copy so you can test removal.
                        Tango testPuzzle = puzzle.DeepCopy();
                        testPuzzle.UnfixCell(row, col);
                        
                        if (HasUniqueSolution(testPuzzle))
                        {
                            // Commit removal.
                            puzzle.UnfixCell(row, col);
                            progress = true;
                        }                        
                    }
                }
            }
        }
        return puzzle;
    }
    
   
    private static bool HasUniqueSolution(Tango puzzle)
    {
        // We don't want to calculate all of the values, just check if there is more than one.
        return 1 == TangoSolver.EnumerateSolutions(puzzle).Take(2).Count();
    }
}
