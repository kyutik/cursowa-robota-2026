using System;

namespace cursowa
{
    public class ExperimentRunner
    {
        private readonly Random random;

        public ExperimentRunner()
        {
            random = new Random();
        }

        public ExperimentResult RunExperiment(
            int rows,
            int cols,
            double yinProbability,
            double yangProbability,
            int steps,
            int runs)
        {
            if (rows <= 0 || cols <= 0)
                throw new ArgumentException("Розміри поля мають бути більшими за нуль.");

            if (steps <= 0)
                throw new ArgumentException("Кількість кроків має бути більшою за нуль.");

            if (runs <= 0)
                throw new ArgumentException("Кількість запусків має бути більшою за нуль.");

            if (yinProbability < 0 || yangProbability < 0)
                throw new ArgumentException("Ймовірності не можуть бути від’ємними.");

            if (yinProbability + yangProbability > 1)
                throw new ArgumentException("Сума ймовірностей Інь та Ян не може перевищувати 1.");

            ExperimentResult result = new ExperimentResult();

            result.Rows = rows;
            result.Cols = cols;
            result.Steps = steps;
            result.Runs = runs;
            result.YinProbability = yinProbability;
            result.YangProbability = yangProbability;

            double totalFinalYin = 0;
            double totalFinalYang = 0;
            double totalFinalLive = 0;
            double totalFinalEmpty = 0;

            for (int i = 0; i < runs; i++)
            {
                YinYangAutomaton automaton = new YinYangAutomaton(rows, cols, random);
                automaton.Randomize(yinProbability, yangProbability);

                GenerationStats stats = automaton.GetStats();

                bool onePopulationDeadHappened = false;

                for (int step = 0; step < steps; step++)
                {
                    stats = automaton.Step();

                    if (stats.IsOnePopulationDead)
                    {
                        onePopulationDeadHappened = true;
                    }

                    if (stats.IsExtinct)
                    {
                        break;
                    }
                }

                if (stats.IsExtinct)
                {
                    result.ExtinctRuns++;
                }

                if (onePopulationDeadHappened)
                {
                    result.OnePopulationDeadRuns++;
                }

                totalFinalYin += stats.YinCount;
                totalFinalYang += stats.YangCount;
                totalFinalLive += stats.LiveCount;
                totalFinalEmpty += stats.EmptyCount;
            }

            result.AverageFinalYin = totalFinalYin / runs;
            result.AverageFinalYang = totalFinalYang / runs;
            result.AverageFinalLive = totalFinalLive / runs;
            result.AverageFinalEmpty = totalFinalEmpty / runs;

            return result;
        }
    }
}