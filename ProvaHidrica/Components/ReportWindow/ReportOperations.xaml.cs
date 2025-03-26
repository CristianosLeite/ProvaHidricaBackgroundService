using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using ProvaHidrica.Database;
using ProvaHidrica.Models;

namespace ProvaHidrica.Components
{
    /// <summary>
    /// Interação lógica para ReportOperations.xam
    /// </summary>
    public partial class ReportOperations : UserControl
    {
        public readonly Db db;
        private ExportReport<Operation>? exportReport = null;
        public ObservableCollection<Operation> Operations { get; set; }
        private readonly List<string> Header =
        [
            "ID",
            "VP",
            "CIS",
            "Chassi",
            "Operador",
            "Receita",
            "Início",
            "Fim",
            "Duração",
            "Status",
            "Ponto 1",
            "Ponto 2",
            "Ponto 3",
            "Ponto 4",
            "Ponto 5",
            "Ponto 6",
            "Ponto 7",
            "Ponto 8",
            "Ponto 9",
            "Ponto 10",
            "Ponto 11",
            "Ponto 12",
            "Ponto 13",
            "Ponto 14",
            "Ponto 15",
            "Ponto 16",
            "Ponto 17",
            "Ponto 18",
            "Ponto 19",
            "Ponto 20",
            "Ponto 21",
            "Ponto 22",
            "Ponto 23",
            "Ponto 24",
            "Ponto 25",
            "Ponto 26",
            "Ponto 27",
            "Ponto 28",
            "Ponto 29",
            "Ponto 30",
            "Ponto 31",
            "Ponto 32",
            "Ponto 33",
            "Criado em",
        ];

        private readonly List<string> DataHeader = [];

        public ReportOperations()
        {
            InitializeComponent();

            DgOperations.AutoGenerateColumns = false;
            DgOperations.CanUserAddRows = false;
            DgOperations.CanUserDeleteRows = false;
            DgOperations.CanUserReorderColumns = false;
            DgOperations.CanUserResizeColumns = false;
            DgOperations.CanUserResizeRows = false;
            DgOperations.CanUserSortColumns = false;

            DbConnectionFactory connectionFactory = new();
            db = new(connectionFactory);

            Operations = [];
            DataContext = this;

            SetDataHeader();
            foreach (var header in Header)
            {
                int index = Header.IndexOf(header);
                if (index >= 0 && index < DataHeader.Count)
                {
                    DgOperations.Columns.Add(
                        new DataGridTextColumn
                        {
                            Header = header,
                            Binding = new Binding(DataHeader[index]),
                            Width = new DataGridLength(1, DataGridLengthUnitType.Auto),
                        }
                    );
                }
            }

            InitialDate.SelectedDate = DateTime.Now;
            FinalDate.SelectedDate = DateTime.Now;

            GetOperationsByDate();
        }

        private void SetDataHeader()
        {
            var properties = typeof(Operation).GetProperties();

            foreach (var property in properties)
            {
                DataHeader.Add(property.Name);
            }
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(InitialDate.Text) || string.IsNullOrEmpty(FinalDate.Text))
                return;

            if (TbPartnumber.Text.Length != 0)
            {
                BtnRemoveFilter.IsEnabled = true;
                BtnRemoveFilter.Foreground = Brushes.Red;
            }

            GetOperationsByDate();
        }

        private async void GetOperationsByDate()
        {
            var operations = await db.GetOperationsByDate(
                TbPartnumber.Text,
                InitialDate.SelectedDate.ToString()!,
                FinalDate.SelectedDate.ToString()!
            );

            Operations.Clear();
            foreach (var operation in operations)
            {
                Operations.Add(operation);
            }
        }

        private void ClearFilter(object sender, RoutedEventArgs e)
        {
            TbPartnumber.Clear();
            GetOperationsByDate();
        }

        private void OnTbFilterChange(object sender, RoutedEventArgs e)
        {
            if (TbPartnumber.Text.Length > 0)
            {
                BtnFind.Foreground = Brushes.LawnGreen;
            }
            else
            {
                BtnFind.Foreground = Brushes.DarkGray;
                BtnRemoveFilter.Foreground = Brushes.DarkGray;
                BtnRemoveFilter.IsEnabled = false;
            }
        }

        private void Export(object sender, RoutedEventArgs e)
        {
            exportReport = new(Header, DataHeader, Operations);
            exportReport.ExportExcel();
        }
    }
}
