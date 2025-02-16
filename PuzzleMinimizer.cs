public class PuzzleMinimizer
{
    public static Tango Minimize(Tango puzzle, bool includeConstraints)
    {
        while (TrySimplifyPuzzle(puzzle, includeConstraints)) { }
        return puzzle;
    }

    private static bool TrySimplifyPuzzle(Tango puzzle, bool includeConstraints)
    {
        for (int row = 0; row < 6; row++)
        {
            for (int col = 0; col < 6; col++)
            {
                if (!puzzle.IsCellFixed(row, col)) continue;

                // Try removing the fixed cell
                if (CanRemoveCell(puzzle, row, col))
                {
                    puzzle.UnfixCell(row, col);
                    return true;
                }

                // Try replacing with a constraint
                if (includeConstraints && TryReplaceWithConstraint(puzzle, row, col))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static bool CanRemoveCell(Tango puzzle, int row, int col)
    {
        var testPuzzle = puzzle.DeepCopy();
        testPuzzle.UnfixCell(row, col);
        return HasUniqueSolution(testPuzzle);
    }

    private static bool TryReplaceWithConstraint(Tango puzzle, int row, int col)
    {
        var adjacentCells = GetAdjacentFixedCells(puzzle, row, col);
        var constraintTypes = new[] { Tango.ConstraintType.Same, Tango.ConstraintType.Different };

        foreach (var (r2, c2) in adjacentCells)
        {
            foreach (var constraintType in constraintTypes)
            {
                var testPuzzle = puzzle.DeepCopy();
                testPuzzle.UnfixCell(row, col);
                testPuzzle.AddConstraint(row, col, r2, c2, constraintType);

                if (HasUniqueSolution(testPuzzle))
                {
                    puzzle.UnfixCell(row, col);
                    puzzle.AddConstraint(row, col, r2, c2, constraintType);
                    return true;
                }
            }
        }
        return false;
    }

    private static IEnumerable<(int row, int col)> GetAdjacentFixedCells(Tango puzzle, int row, int col)
    {
        var directions = new[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
        
        return directions
            .Select(d => (row: row + d.Item1, col: col + d.Item2))
            .Where(pos => IsValidPosition(pos.row, pos.col) && puzzle.IsCellFixed(pos.row, pos.col));
    }

    private static bool IsValidPosition(int row, int col) =>
        row >= 0 && row < 6 && col >= 0 && col < 6;

    private static bool HasUniqueSolution(Tango puzzle) =>
        TangoSolver.EnumerateSolutions(puzzle).Take(2).Count() == 1;
}
