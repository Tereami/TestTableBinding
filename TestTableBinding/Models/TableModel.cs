namespace TestTableBinding.Models
{
    public class TableModel
    {
        public string Title { get; set; }
        public List<RowModel> Rows { get; set; }

        public string? Message { get; set; }
        public int AddRowsCount { get; set; }

        public static TableModel GetSample(int RowsCount)
        {
            List<RowModel> rows = new List<RowModel>();
            int id = 0;
            for(int i = 0; i < RowsCount; i++)
            {
                RowModel row = new RowModel()
                {
                    Id = id,
                    Name = $"Row #{i}"
                };
                id = id + new Random().Next(1, 10);
                rows.Add(row);
            }

            TableModel model = new TableModel()
            {
                Title = $"Title {RowsCount} rows",
                Rows = rows,
                AddRowsCount = 1
            };

            return model;
        }
    }
}
