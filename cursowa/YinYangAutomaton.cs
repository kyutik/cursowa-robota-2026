using System;

namespace cursowa
{
    public class YinYangAutomaton
    {
        private CellState[,] currentField;
        private readonly Random random;

        public int Rows { get; private set; }
        public int Cols { get; private set; }
        public int StepNumber { get; private set; }

        public YinYangAutomaton(int rows, int cols)
            : this(rows, cols, new Random())
        {
        }

        public YinYangAutomaton(int rows, int cols, Random random)
        {
            if (rows <= 0 || cols <= 0)
                throw new ArgumentException("Розміри поля мають бути більшими за нуль.");

            Rows = rows;
            Cols = cols;
            StepNumber = 0;

            currentField = new CellState[rows, cols];
            this.random = random;
        }

        public CellState GetCell(int row, int col)
        {
            return currentField[row, col];
        }

        public void SetCell(int row, int col, CellState state)
        {
            if (row < 0 || row >= Rows || col < 0 || col >= Cols)
                throw new IndexOutOfRangeException("Координати клітинки виходять за межі поля.");

            currentField[row, col] = state;
        }

        public void Clear()
        {
            currentField = new CellState[Rows, Cols];
            StepNumber = 0;
        }

        public void Randomize(double yinProbability, double yangProbability)
        {
            if (yinProbability < 0 || yangProbability < 0)
                throw new ArgumentException("Ймовірності не можуть бути від’ємними.");

            if (yinProbability + yangProbability > 1)
                throw new ArgumentException("Сума ймовірностей Інь та Ян не може перевищувати 1.");

            StepNumber = 0;

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    double value = random.NextDouble();

                    if (value < yinProbability)
                    {
                        currentField[r, c] = CellState.Yin;
                    }
                    else if (value < yinProbability + yangProbability)
                    {
                        currentField[r, c] = CellState.Yang;
                    }
                    else
                    {
                        currentField[r, c] = CellState.Empty;
                    }
                }
            }
        }

        public GenerationStats Step()
        {
            CellState[,] nextField = new CellState[Rows, Cols];

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    nextField[r, c] = CalculateNextState(r, c);
                }
            }

            currentField = nextField;
            StepNumber++;

            return GetStats();
        }

        private CellState CalculateNextState(int row, int col)
        {
            int yinNeighbors;
            int yangNeighbors;

            CountNeighbors(row, col, out yinNeighbors, out yangNeighbors);

            int liveNeighbors = yinNeighbors + yangNeighbors;
            CellState currentState = currentField[row, col];

            if (currentState == CellState.Empty)
            {
                if (liveNeighbors == 3 && yinNeighbors > 0 && yangNeighbors > 0)
                {
                    if (yangNeighbors == 1)
                        return CellState.Yang;

                    if (yinNeighbors == 1)
                        return CellState.Yin;
                }

                return CellState.Empty;
            }

            if (liveNeighbors < 2 || liveNeighbors > 4)
            {
                return CellState.Empty;
            }

            if (liveNeighbors == 4)
            {
                int sameTypeNeighbors;
                int oppositeTypeNeighbors;

                if (currentState == CellState.Yin)
                {
                    sameTypeNeighbors = yinNeighbors;
                    oppositeTypeNeighbors = yangNeighbors;
                }
                else
                {
                    sameTypeNeighbors = yangNeighbors;
                    oppositeTypeNeighbors = yinNeighbors;
                }

                if (oppositeTypeNeighbors > sameTypeNeighbors)
                {
                    return CellState.Empty;
                }
            }

            return currentState;
        }

        private void CountNeighbors(int row, int col, out int yinCount, out int yangCount)
        {
            yinCount = 0;
            yangCount = 0;

            for (int dr = -1; dr <= 1; dr++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0)
                        continue;

                    int neighborRow = Wrap(row + dr, Rows);
                    int neighborCol = Wrap(col + dc, Cols);

                    CellState neighbor = currentField[neighborRow, neighborCol];

                    if (neighbor == CellState.Yin)
                        yinCount++;
                    else if (neighbor == CellState.Yang)
                        yangCount++;
                }
            }
        }

        private int Wrap(int value, int max)
        {
            if (value < 0)
                return max - 1;

            if (value >= max)
                return 0;

            return value;
        }

        public GenerationStats GetStats()
        {
            GenerationStats stats = new GenerationStats();
            stats.StepNumber = StepNumber;

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (currentField[r, c] == CellState.Empty)
                        stats.EmptyCount++;
                    else if (currentField[r, c] == CellState.Yin)
                        stats.YinCount++;
                    else if (currentField[r, c] == CellState.Yang)
                        stats.YangCount++;
                }
            }

            return stats;
        }
    }
}