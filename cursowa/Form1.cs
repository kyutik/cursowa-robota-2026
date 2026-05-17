using System;
using System.Drawing;
using System.Windows.Forms;

namespace cursowa
{
    public partial class Form1 : Form
    {
        private YinYangAutomaton automaton;

        private Panel fieldPanel;

        private NumericUpDown rowsNumeric;
        private NumericUpDown colsNumeric;
        private NumericUpDown yinProbabilityNumeric;
        private NumericUpDown yangProbabilityNumeric;
        private NumericUpDown intervalNumeric;

        private NumericUpDown experimentStepsNumeric;
        private NumericUpDown experimentRunsNumeric;

        private Button createButton;
        private Button randomButton;
        private Button stepButton;
        private Button startButton;
        private Button stopButton;
        private Button clearButton;
        private Button runExperimentButton;

        private Label stepLabel;
        private Label yinCountLabel;
        private Label yangCountLabel;
        private Label emptyCountLabel;
        private Label liveCountLabel;
        private Label stateLabel;

        private TextBox experimentResultTextBox;

        private System.Windows.Forms.Timer simulationTimer;

        public Form1()
        {
            InitializeComponent();
            BuildInterface();
        }

        private void BuildInterface()
        {
            Controls.Clear();

            Text = "Курсова робота — гра Інь-Ян";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1250, 800);
            Size = new Size(1400, 900);
            WindowState = FormWindowState.Maximized;

            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 2;
            mainLayout.RowCount = 1;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 430));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            Controls.Add(mainLayout);

            FlowLayoutPanel controlPanel = new FlowLayoutPanel();
            controlPanel.Dock = DockStyle.Fill;
            controlPanel.FlowDirection = FlowDirection.TopDown;
            controlPanel.WrapContents = false;
            controlPanel.AutoScroll = true;
            controlPanel.Padding = new Padding(15);
            mainLayout.Controls.Add(controlPanel, 0, 0);

            fieldPanel = new DoubleBufferedPanel();
            fieldPanel.Dock = DockStyle.Fill;
            fieldPanel.BackColor = Color.Silver;
            fieldPanel.Paint += fieldPanel_Paint;
            fieldPanel.MouseClick += fieldPanel_MouseClick;
            fieldPanel.Resize += fieldPanel_Resize;
            mainLayout.Controls.Add(fieldPanel, 1, 0);

            CreateFieldSettingsGroup(controlPanel);
            CreateSimulationControlGroup(controlPanel);
            CreateStatsGroup(controlPanel);
            CreateLegendGroup(controlPanel);
            CreateExperimentGroup(controlPanel);

            simulationTimer = new System.Windows.Forms.Timer();
            simulationTimer.Interval = 200;
            simulationTimer.Tick += simulationTimer_Tick;

            UpdateStats();
        }

        private void CreateFieldSettingsGroup(FlowLayoutPanel parent)
        {
            GroupBox group = CreateGroupBox("Параметри поля", 370, 240);

            TableLayoutPanel layout = CreateTableLayout(2, 5);
            layout.ColumnStyles[0].Width = 47;
            layout.ColumnStyles[1].Width = 53;
            group.Controls.Add(layout);

            rowsNumeric = CreateIntegerNumeric(10, 200, 40);
            colsNumeric = CreateIntegerNumeric(10, 200, 40);

            yinProbabilityNumeric = CreateProbabilityNumeric(0.25m);
            yangProbabilityNumeric = CreateProbabilityNumeric(0.25m);

            createButton = CreateButton("Створити поле");
            createButton.Click += createButton_Click;

            randomButton = CreateButton("Випадкова генерація");
            randomButton.Click += randomButton_Click;

            AddRow(layout, 0, "Рядки:", rowsNumeric);
            AddRow(layout, 1, "Стовпці:", colsNumeric);
            AddRow(layout, 2, "Ймовірність Інь:", yinProbabilityNumeric);
            AddRow(layout, 3, "Ймовірність Ян:", yangProbabilityNumeric);

            layout.Controls.Add(createButton, 0, 4);
            layout.Controls.Add(randomButton, 1, 4);

            parent.Controls.Add(group);
        }

        private void CreateSimulationControlGroup(FlowLayoutPanel parent)
        {
            GroupBox group = CreateGroupBox("Керування симуляцією", 370, 180);

            TableLayoutPanel layout = CreateTableLayout(2, 3);
            group.Controls.Add(layout);

            stepButton = CreateButton("Один крок");
            stepButton.Click += stepButton_Click;

            startButton = CreateButton("Старт симуляції");
            startButton.Click += startButton_Click;

            stopButton = CreateButton("Стоп");
            stopButton.Click += stopButton_Click;

            clearButton = CreateButton("Очистити поле");
            clearButton.Click += clearButton_Click;

            intervalNumeric = CreateIntegerNumeric(50, 2000, 200);

            layout.Controls.Add(stepButton, 0, 0);
            layout.Controls.Add(startButton, 1, 0);
            layout.Controls.Add(stopButton, 0, 1);
            layout.Controls.Add(clearButton, 1, 1);

            Label intervalLabel = CreateNormalLabel("Інтервал, мс:");
            layout.Controls.Add(intervalLabel, 0, 2);
            layout.Controls.Add(intervalNumeric, 1, 2);

            parent.Controls.Add(group);
        }

        private void CreateStatsGroup(FlowLayoutPanel parent)
        {
            GroupBox group = CreateGroupBox("Статистика", 370, 185);

            FlowLayoutPanel layout = new FlowLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.FlowDirection = FlowDirection.TopDown;
            layout.WrapContents = false;
            layout.Padding = new Padding(15);
            group.Controls.Add(layout);

            stepLabel = CreateStatsLabel("Крок: 0");
            yinCountLabel = CreateStatsLabel("Інь: 0");
            yangCountLabel = CreateStatsLabel("Ян: 0");
            emptyCountLabel = CreateStatsLabel("Порожніх: 0");
            liveCountLabel = CreateStatsLabel("Живих: 0");
            stateLabel = CreateStatsLabel("Стан: поле не створено");

            layout.Controls.Add(stepLabel);
            layout.Controls.Add(yinCountLabel);
            layout.Controls.Add(yangCountLabel);
            layout.Controls.Add(emptyCountLabel);
            layout.Controls.Add(liveCountLabel);
            layout.Controls.Add(stateLabel);

            parent.Controls.Add(group);
        }

        private void CreateLegendGroup(FlowLayoutPanel parent)
        {
            GroupBox group = CreateGroupBox("Позначення та ручне редагування", 370, 145);

            FlowLayoutPanel layout = new FlowLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.FlowDirection = FlowDirection.TopDown;
            layout.WrapContents = false;
            layout.Padding = new Padding(15);
            group.Controls.Add(layout);

            Label yinLabel = CreateStatsLabel("■ Чорний — клітинка Інь");
            yinLabel.ForeColor = Color.Black;

            Label yangLabel = CreateStatsLabel("□ Білий — клітинка Ян");
            yangLabel.ForeColor = Color.Black;

            Label emptyLabel = CreateStatsLabel("Сірий — порожня клітинка");
            Label mouseLabel1 = CreateStatsLabel("ЛКМ — поставити / прибрати Інь");
            Label mouseLabel2 = CreateStatsLabel("ПКМ — поставити / прибрати Ян");

            layout.Controls.Add(yinLabel);
            layout.Controls.Add(yangLabel);
            layout.Controls.Add(emptyLabel);
            layout.Controls.Add(mouseLabel1);
            layout.Controls.Add(mouseLabel2);

            parent.Controls.Add(group);
        }

        private void CreateExperimentGroup(FlowLayoutPanel parent)
        {
            GroupBox group = CreateGroupBox("Експеримент виродження", 370, 330);

            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 2;
            layout.RowCount = 4;
            layout.Padding = new Padding(15);
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            group.Controls.Add(layout);

            experimentStepsNumeric = CreateIntegerNumeric(1, 10000, 100);
            experimentRunsNumeric = CreateIntegerNumeric(1, 10000, 100);

            runExperimentButton = CreateButton("Запустити експеримент");
            runExperimentButton.Click += runExperimentButton_Click;

            experimentResultTextBox = new TextBox();
            experimentResultTextBox.Multiline = true;
            experimentResultTextBox.ScrollBars = ScrollBars.Vertical;
            experimentResultTextBox.ReadOnly = true;
            experimentResultTextBox.Dock = DockStyle.Fill;

            AddRow(layout, 0, "Кроків:", experimentStepsNumeric);
            AddRow(layout, 1, "Запусків:", experimentRunsNumeric);

            layout.Controls.Add(runExperimentButton, 0, 2);
            layout.SetColumnSpan(runExperimentButton, 2);

            layout.Controls.Add(experimentResultTextBox, 0, 3);
            layout.SetColumnSpan(experimentResultTextBox, 2);

            parent.Controls.Add(group);
        }

        private GroupBox CreateGroupBox(string text, int width, int height)
        {
            GroupBox group = new GroupBox();
            group.Text = text;
            group.Width = width;
            group.Height = height;
            group.Margin = new Padding(0, 0, 0, 15);
            return group;
        }

        private TableLayoutPanel CreateTableLayout(int columns, int rows)
        {
            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = columns;
            layout.RowCount = rows;
            layout.Padding = new Padding(15);

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            for (int i = 0; i < rows; i++)
            {
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            }

            return layout;
        }

        private NumericUpDown CreateIntegerNumeric(int min, int max, int value)
        {
            NumericUpDown numeric = new NumericUpDown();
            numeric.Minimum = min;
            numeric.Maximum = max;
            numeric.Value = value;
            numeric.Dock = DockStyle.Fill;
            numeric.Height = 30;
            return numeric;
        }

        private NumericUpDown CreateProbabilityNumeric(decimal value)
        {
            NumericUpDown numeric = new NumericUpDown();
            numeric.Minimum = 0;
            numeric.Maximum = 1;
            numeric.DecimalPlaces = 2;
            numeric.Increment = 0.01m;
            numeric.Value = value;
            numeric.Dock = DockStyle.Fill;
            numeric.Height = 30;
            return numeric;
        }

        private Button CreateButton(string text)
        {
            Button button = new Button();
            button.Text = text;
            button.Dock = DockStyle.Fill;
            button.Height = 34;
            button.MinimumSize = new Size(145, 34);
            button.AutoSize = false;
            return button;
        }

        private Label CreateNormalLabel(string text)
        {
            Label label = new Label();
            label.Text = text;
            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.MiddleLeft;
            return label;
        }

        private Label CreateStatsLabel(string text)
        {
            Label label = new Label();
            label.Text = text;
            label.Width = 330;
            label.Height = 22;
            return label;
        }

        private void AddRow(TableLayoutPanel layout, int row, string text, Control control)
        {
            Label label = CreateNormalLabel(text);

            layout.Controls.Add(label, 0, row);
            layout.Controls.Add(control, 1, row);
        }

        private void createButton_Click(object sender, EventArgs e)
        {
            int rows = (int)rowsNumeric.Value;
            int cols = (int)colsNumeric.Value;

            automaton = new YinYangAutomaton(rows, cols);
            simulationTimer.Stop();

            UpdateStats();
            fieldPanel.Invalidate();
        }

        private void randomButton_Click(object sender, EventArgs e)
        {
            try
            {
                int rows = (int)rowsNumeric.Value;
                int cols = (int)colsNumeric.Value;

                double yinProbability = (double)yinProbabilityNumeric.Value;
                double yangProbability = (double)yangProbabilityNumeric.Value;

                automaton = new YinYangAutomaton(rows, cols);
                automaton.Randomize(yinProbability, yangProbability);

                simulationTimer.Stop();

                UpdateStats();
                fieldPanel.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void stepButton_Click(object sender, EventArgs e)
        {
            DoOneStep();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (automaton == null)
            {
                MessageBox.Show("Спочатку створіть поле або виконайте випадкову генерацію.", "Повідомлення", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            simulationTimer.Interval = (int)intervalNumeric.Value;
            simulationTimer.Start();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            simulationTimer.Stop();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            if (automaton == null)
                return;

            simulationTimer.Stop();
            automaton.Clear();

            UpdateStats();
            fieldPanel.Invalidate();
        }

        private void runExperimentButton_Click(object sender, EventArgs e)
        {
            try
            {
                int rows = (int)rowsNumeric.Value;
                int cols = (int)colsNumeric.Value;
                int steps = (int)experimentStepsNumeric.Value;
                int runs = (int)experimentRunsNumeric.Value;

                double yinProbability = (double)yinProbabilityNumeric.Value;
                double yangProbability = (double)yangProbabilityNumeric.Value;

                ExperimentRunner runner = new ExperimentRunner();

                ExperimentResult result = runner.RunExperiment(
                    rows,
                    cols,
                    yinProbability,
                    yangProbability,
                    steps,
                    runs);

                experimentResultTextBox.Text = result.ToReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка експерименту", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void simulationTimer_Tick(object sender, EventArgs e)
        {
            DoOneStep();
        }

        private void DoOneStep()
        {
            if (automaton == null)
            {
                MessageBox.Show("Спочатку створіть поле або виконайте випадкову генерацію.", "Повідомлення", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            GenerationStats stats = automaton.Step();

            if (stats.IsExtinct)
                simulationTimer.Stop();

            UpdateStats();
            fieldPanel.Invalidate();
        }

        private void UpdateStats()
        {
            if (automaton == null)
            {
                stepLabel.Text = "Крок: 0";
                yinCountLabel.Text = "Інь: 0";
                yangCountLabel.Text = "Ян: 0";
                emptyCountLabel.Text = "Порожніх: 0";
                liveCountLabel.Text = "Живих: 0";
                stateLabel.Text = "Стан: поле не створено";
                return;
            }

            GenerationStats stats = automaton.GetStats();

            stepLabel.Text = "Крок: " + stats.StepNumber;
            yinCountLabel.Text = "Інь: " + stats.YinCount;
            yangCountLabel.Text = "Ян: " + stats.YangCount;
            emptyCountLabel.Text = "Порожніх: " + stats.EmptyCount;
            liveCountLabel.Text = "Живих: " + stats.LiveCount;

            if (stats.IsExtinct && stats.StepNumber == 0)
                stateLabel.Text = "Стан: поле порожнє";
            else if (stats.IsExtinct)
                stateLabel.Text = "Стан: повне виродження";
            else if (stats.IsOnePopulationDead)
                stateLabel.Text = "Стан: одна популяція зникла";
            else
                stateLabel.Text = "Стан: активна еволюція";
        }

        private void fieldPanel_Paint(object sender, PaintEventArgs e)
        {
            if (automaton == null)
                return;

            float cellWidth = (float)fieldPanel.ClientSize.Width / automaton.Cols;
            float cellHeight = (float)fieldPanel.ClientSize.Height / automaton.Rows;

            if (cellWidth <= 0 || cellHeight <= 0)
                return;

            for (int r = 0; r < automaton.Rows; r++)
            {
                for (int c = 0; c < automaton.Cols; c++)
                {
                    CellState state = automaton.GetCell(r, c);

                    Brush brush;

                    if (state == CellState.Yin)
                        brush = Brushes.Black;
                    else if (state == CellState.Yang)
                        brush = Brushes.White;
                    else
                        brush = Brushes.Silver;

                    float x = c * cellWidth;
                    float y = r * cellHeight;

                    e.Graphics.FillRectangle(brush, x, y, cellWidth, cellHeight);

                    if (cellWidth >= 5 && cellHeight >= 5)
                    {
                        e.Graphics.DrawRectangle(
                            Pens.Gray,
                            (int)x,
                            (int)y,
                            (int)cellWidth,
                            (int)cellHeight);
                    }
                }
            }
        }

        private void fieldPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (automaton == null)
                return;

            float cellWidth = (float)fieldPanel.ClientSize.Width / automaton.Cols;
            float cellHeight = (float)fieldPanel.ClientSize.Height / automaton.Rows;

            if (cellWidth <= 0 || cellHeight <= 0)
                return;

            int col = (int)(e.X / cellWidth);
            int row = (int)(e.Y / cellHeight);

            if (row < 0 || row >= automaton.Rows || col < 0 || col >= automaton.Cols)
                return;

            CellState currentState = automaton.GetCell(row, col);

            if (e.Button == MouseButtons.Left)
            {
                if (currentState == CellState.Yin)
                    automaton.SetCell(row, col, CellState.Empty);
                else
                    automaton.SetCell(row, col, CellState.Yin);
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (currentState == CellState.Yang)
                    automaton.SetCell(row, col, CellState.Empty);
                else
                    automaton.SetCell(row, col, CellState.Yang);
            }

            UpdateStats();
            fieldPanel.Invalidate();
        }

        private void fieldPanel_Resize(object sender, EventArgs e)
        {
            fieldPanel.Invalidate();
        }
    }

    public class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
        }
    }
}