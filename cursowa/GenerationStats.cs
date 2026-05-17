namespace cursowa
{
    public class GenerationStats
    {
        public int StepNumber { get; set; }
        public int EmptyCount { get; set; }
        public int YinCount { get; set; }
        public int YangCount { get; set; }

        public int LiveCount
        {
            get { return YinCount + YangCount; }
        }

        public bool IsExtinct
        {
            get { return LiveCount == 0; }
        }

        public bool IsOnePopulationDead
        {
            get { return LiveCount > 0 && (YinCount == 0 || YangCount == 0); }
        }
    }
}