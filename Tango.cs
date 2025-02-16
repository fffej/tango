public class Tango
{
    public const int GRID_SIZE = 6;
    private readonly char[,] grid = new char[GRID_SIZE, GRID_SIZE];
    private readonly bool[,] isFixed = new bool[GRID_SIZE, GRID_SIZE];
    private readonly List<Constraint> constraints = new();

    public record Constraint(int Row1, int Col1, int Row2, int Col2, ConstraintType Type);
    public enum ConstraintType { Same, Different }

    // Properties with expression-bodied members
    public char[,] Grid => grid;
    public bool[,] IsFixed => isFixed;
    public IReadOnlyList<Constraint> Constraints => constraints;
    public int CellsSet => isFixed.Cast<bool>().Count(x => x);
    public int ConstraintsSet => constraints.Count;

    public Tango()
    {
        // Initialize empty grid with spaces
        for (int i = 0; i < GRID_SIZE; i++)
            for (int j = 0; j < GRID_SIZE; j++)
                grid[i, j] = ' ';
    }

    public Tango(char[,] initialGrid, IEnumerable<Constraint> initialConstraints)
    {
        if (initialGrid.GetLength(0) != GRID_SIZE || initialGrid.GetLength(1) != GRID_SIZE)
            throw new ArgumentException("Invalid grid size");

        Array.Copy(initialGrid, grid, grid.Length);
        for (int i = 0; i < GRID_SIZE; i++)
            for (int j = 0; j < GRID_SIZE; j++)
                isFixed[i, j] = true;

        constraints.AddRange(initialConstraints);
    }

    public void SetCell(int row, int col, char value)
    {
        ValidatePosition(row, col);
        if (value is not 'X' and not 'Y')
            throw new ArgumentException("Invalid symbol - must be X or Y");

        grid[row, col] = value;
        isFixed[row, col] = true;
    }

    public void AddConstraint(int row1, int col1, int row2, int col2, ConstraintType type)
    {
        ValidatePosition(row1, col1);
        ValidatePosition(row2, col2);

        if (Math.Abs(row1 - row2) + Math.Abs(col1 - col2) != 1)
            throw new ArgumentException("Invalid constraint - cells must be adjacent");

        constraints.Add(new Constraint(row1, col1, row2, col2, type));
    }

    public bool IsValid() => 
        !HasThreeConsecutive() && 
        HasValidDistribution() && 
        SatisfiesConstraints();

    private bool HasThreeConsecutive()
    {
        bool CheckLine(Func<int, int, char> getCell)
        {
            for (int i = 0; i < GRID_SIZE; i++)
                for (int j = 0; j < GRID_SIZE - 2; j++)
                    if (getCell(i, j) != ' ' && 
                        getCell(i, j) == getCell(i, j + 1) && 
                        getCell(i, j) == getCell(i, j + 2))
                        return true;
            return false;
        }

        return CheckLine((i, j) => grid[i, j]) ||  // Check rows
               CheckLine((i, j) => grid[j, i]);     // Check columns
    }

    private bool HasValidDistribution()
    {
        bool IsLineValid(Func<int, int, char> getCell)
        {
            for (int i = 0; i < GRID_SIZE; i++)
            {
                var line = Enumerable.Range(0, GRID_SIZE)
                    .Select(j => getCell(i, j))
                    .ToList();

                if (!line.Contains(' ') && 
                    line.Count(c => c == 'X') != line.Count(c => c == 'Y'))
                    return false;
            }
            return true;
        }

        return IsLineValid((i, j) => grid[i, j]) &&  // Check rows
               IsLineValid((i, j) => grid[j, i]);     // Check columns
    }

    private bool SatisfiesConstraints() =>
        constraints.All(c => {
            var cell1 = grid[c.Row1, c.Col1];
            var cell2 = grid[c.Row2, c.Col2];
            if (cell1 == ' ' || cell2 == ' ') return true;
            return c.Type == ConstraintType.Same ? cell1 == cell2 : cell1 != cell2;
        });

    private void ValidatePosition(int row, int col)
    {
        if (row < 0 || row >= GRID_SIZE || col < 0 || col >= GRID_SIZE)
            throw new ArgumentException("Invalid position");
    }

    public char GetCell(int row, int col)
    {
        ValidatePosition(row, col);
        return grid[row, col];
    }

    public bool IsCellFixed(int row, int col)
    {
        ValidatePosition(row, col);
        return isFixed[row, col];
    }

    public void UnfixCell(int row, int col)
    {
        ValidatePosition(row, col);
        isFixed[row, col] = false;
        grid[row, col] = ' ';
    }

    public Tango DeepCopy()
    {
        var copy = new Tango();
        Array.Copy(grid, copy.grid, grid.Length);
        Array.Copy(isFixed, copy.isFixed, isFixed.Length);
        copy.constraints.AddRange(constraints);
        return copy;
    }

    public override string ToString()
    {
        var result = new System.Text.StringBuilder();
        
        for (int i = 0; i < GRID_SIZE; i++)
        {
            // Print cells and horizontal constraints
            for (int j = 0; j < GRID_SIZE; j++)
            {
                result.Append(grid[i, j] == ' ' ? '.' : grid[i, j]);
                
                if (j < GRID_SIZE - 1)
                {
                    var constraint = constraints.FirstOrDefault(c => 
                        (c.Row1 == i && c.Col1 == j && c.Row2 == i && c.Col2 == j + 1) ||
                        (c.Row2 == i && c.Col2 == j && c.Row1 == i && c.Col1 == j + 1));
                    
                    result.Append(constraint?.Type == ConstraintType.Same ? "=" : 
                                constraint != null ? "x" : " ");
                }
            }
            result.AppendLine();
            
            // Print vertical constraints
            if (i < GRID_SIZE - 1)
            {
                for (int j = 0; j < GRID_SIZE; j++)
                {
                    var constraint = constraints.FirstOrDefault(c =>
                        (c.Row1 == i && c.Col1 == j && c.Row2 == i + 1 && c.Col2 == j) ||
                        (c.Row2 == i && c.Col2 == j && c.Row1 == i + 1 && c.Col1 == j));
                    
                    result.Append(constraint?.Type == ConstraintType.Same ? "=" : 
                                constraint != null ? "x" : " ");
                    result.Append(" ");
                }
                result.AppendLine();
            }
        }
        
        return result.ToString();
    }
}