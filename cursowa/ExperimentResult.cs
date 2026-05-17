using System.Text;

namespace cursowa
{
    public class ExperimentResult
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        public int Steps { get; set; }
        public int Runs { get; set; }

        public double YinProbability { get; set; }
        public double YangProbability { get; set; }

        public int ExtinctRuns { get; set; }
        public int OnePopulationDeadRuns { get; set; }

        public double AverageFinalYin { get; set; }
        public double AverageFinalYang { get; set; }
        public double AverageFinalLive { get; set; }
        public double AverageFinalEmpty { get; set; }

        public double ExtinctionProbability
        {
            get
            {
                if (Runs == 0) return 0;
                return (double)ExtinctRuns / Runs;
            }
        }

        public double OnePopulationDeadProbability
        {
            get
            {
                if (Runs == 0) return 0;
                return (double)OnePopulationDeadRuns / Runs;
            }
        }

        public string ToReport()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Результати експерименту");
            builder.AppendLine("----------------------");
            builder.AppendLine("Розмір поля: " + Rows + " x " + Cols);
            builder.AppendLine("Кількість кроків в одному запуску: " + Steps);
            builder.AppendLine("Кількість запусків: " + Runs);
            builder.AppendLine("Ймовірність Інь: " + YinProbability.ToString("0.00"));
            builder.AppendLine("Ймовірність Ян: " + YangProbability.ToString("0.00"));
            builder.AppendLine();

            builder.AppendLine("Повне виродження автомата: " + ExtinctRuns);
            builder.AppendLine("Ймовірність повного виродження: " + ExtinctionProbability.ToString("P2"));
            builder.AppendLine();

            builder.AppendLine("Зникнення однієї з популяцій: " + OnePopulationDeadRuns);
            builder.AppendLine("Ймовірність зникнення однієї з популяцій: " + OnePopulationDeadProbability.ToString("P2"));
            builder.AppendLine();

            builder.AppendLine("Середні значення після останнього кроку");
            builder.AppendLine("--------------------------------------");
            builder.AppendLine("Середня кількість Інь: " + AverageFinalYin.ToString("0.00"));
            builder.AppendLine("Середня кількість Ян: " + AverageFinalYang.ToString("0.00"));
            builder.AppendLine("Середня кількість живих клітинок: " + AverageFinalLive.ToString("0.00"));
            builder.AppendLine("Середня кількість порожніх клітинок: " + AverageFinalEmpty.ToString("0.00"));

            return builder.ToString();
        }
    }
}