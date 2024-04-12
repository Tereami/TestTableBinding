using System.Xml.Serialization;
using TestTableBinding.Models;

namespace TestTableBinding.Services
{
    public static class StorageService
    {
        public static TableModel LoadOrCreateData()
        {
            TableModel model;
            if (System.IO.File.Exists(Program.Filename))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TableModel));
                using (StreamReader reader = new StreamReader(Program.Filename))
                {
                    model = serializer.Deserialize(reader) as TableModel
                        ?? TableModel.GetSample(new Random().Next(5, 15));
                }
            }
            else
            {
                model = TableModel.GetSample(new Random().Next(5, 15));
                SaveData(model);

            }
            return model;
        }

        public static void SaveData(TableModel table)
        {
            foreach(RowModel row in table.Rows)
            {
                row.Checked = false;
            }

            if (System.IO.File.Exists(Program.Filename))
            {
                System.IO.File.Delete(Program.Filename);
            }
            XmlSerializer serializer = new XmlSerializer(typeof(TableModel));
            using (FileStream writer = new FileStream(Program.Filename, FileMode.OpenOrCreate))
            {
                serializer.Serialize(writer, table);
            }
        }
    }
}
