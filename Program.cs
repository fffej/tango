public class Program
{
    public static void Main(string[] args) {      
        var possibleSolutions = TangoSolver.GetMinimalSolutions(new Tango());
        Console.Out.WriteLine($"Canonical Number of possible solutions with zero constraints: {possibleSolutions.Count()}");              
        
        var hardestGridWithNoConstraints = possibleSolutions.MinBy(x => 
            {
                var minPuzzle = PuzzleMinimizer.Minimize(x, true);
                return minPuzzle.CellsSet + minPuzzle.ConstraintsSet;
            }
        ); 

        Console.Out.WriteLine("Hardest grid with fewest constraints and symbols:");
        Console.Out.WriteLine(hardestGridWithNoConstraints);
    }
}