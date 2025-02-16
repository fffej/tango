using System.Text;

public static class TangoSolver
{
    public static IEnumerable<Tango> EnumerateSolutions(Tango tango)
    {
        return EnumerateSolutions(0, 0, tango);
    }    

    private static IEnumerable<Tango> EnumerateSolutions(
        int row, 
        int col, 
        Tango tango)
    {
        if (row == Tango.GRID_SIZE)
        {
            // Finished grid; yield a copy of the solved grid.
            var solution = new char[Tango.GRID_SIZE, Tango.GRID_SIZE];
            Array.Copy(tango.Grid, solution, tango.Grid.Length);
            yield return new Tango(solution, tango.Constraints);
            yield break;
        }

        // Compute next cell coordinates.
        int nextRow = row, nextCol = col + 1;
        if (nextCol == Tango.GRID_SIZE)
        {
            nextRow++;
            nextCol = 0;
        }

        // If cell is fixed, skip to the next.
        if (tango.IsFixed[row, col])
        {
            foreach (var sol in EnumerateSolutions(nextRow, nextCol, tango))
                yield return sol;
            yield break;
        }

        // Try both possible symbols.
        foreach (var symbol in new char[] { 'X', 'Y' })
        {
            tango.Grid[row, col] = symbol;
            // Only continue if the partial grid is still valid.
            if (tango.IsValid())
            {
                foreach (var sol in EnumerateSolutions(nextRow, nextCol, tango))
                    yield return sol;
            }
        }

        // Backtrack.
        tango.Grid[row, col] = ' ';
    }

    public static IEnumerable<Tango> GetMinimalSolutions(Tango puzzle)
    {
        var seen = new HashSet<string>();

        // Enumerate all valid solutions
        foreach (var solution in EnumerateSolutions(puzzle))
        {
            var canonicalGrid = Canonicalize(solution);
            var key = GridToString(canonicalGrid);

            // Yield only if we haven't seen this canonical arrangement before
            if (!seen.Contains(key))
            {
                seen.Add(key);

                // Construct a Tango with all cells fixed
                yield return new Tango(canonicalGrid, []);
            }
        }
    }

    private static char[,] Canonicalize(Tango solution)
    {
        // Extract the solution’s 6x6 arrangement
        char[,] original = new char[6, 6];
        for (int r = 0; r < 6; r++)
            for (int c = 0; c < 6; c++)
                original[r, c] = solution.GetCell(r, c);

        // Generate the 4 possible rotations
        var rotations = new List<char[,]>(4)
        {
            original,
            Rotate90(original),
            Rotate90(Rotate90(original)),
            Rotate90(Rotate90(Rotate90(original)))
        };

        return rotations
            .Select(x => (x[0, 0] == 'Y') ? 
                Flip(x) : CopyGrid(x)).MinBy(GridToString);
    }

    private static char[,] Rotate90(char[,] grid)
    {
        int n = grid.GetLength(0); // Should be 6
        char[,] rotated = new char[n, n];
        for (int r = 0; r < n; r++)
        {
            for (int c = 0; c < n; c++)
            {
                rotated[c, n - 1 - r] = grid[r, c];
            }
        }
        return rotated;
    }

    private static char[,] Flip(char[,] grid)
    {
        int n = grid.GetLength(0);
        char[,] flipped = new char[n, n];
        for (int r = 0; r < n; r++)
        {
            for (int c = 0; c < n; c++)
            {
                flipped[r, c] = (grid[r, c] == 'X') ? 'Y'
                               : (grid[r, c] == 'Y') ? 'X'
                               : grid[r, c];
            }
        }
        return flipped;
    }

    private static char[,] CopyGrid(char[,] grid)
    {
        int n = grid.GetLength(0);
        char[,] copy = new char[n, n];
        for (int r = 0; r < n; r++)
        {
            for (int c = 0; c < n; c++)
            {
                copy[r, c] = grid[r, c];
            }
        }
        return copy;
    }

    /// <summary>
    /// Convert a 6x6 grid to a single multi-line string so we can 
    /// stick it in a HashSet to detect duplicates.
    /// </summary>
    private static string GridToString(char[,] grid)
    {
        int n = grid.GetLength(0);
        var sb = new StringBuilder(n * (n+1));
        for (int r = 0; r < n; r++)
        {
            for (int c = 0; c < n; c++)
            {
                sb.Append(grid[r, c]);
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
