using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Serialization;
using TestTableBinding.Models;
using TestTableBinding.Services;

namespace TestTableBinding.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            TableModel _tableModel = StorageService.LoadOrCreateData();

            return View(_tableModel);
        }

        [HttpGet]
        public IActionResult Edit()
        {
            TableModel _tableModel = StorageService.LoadOrCreateData();

            ViewBag.Message = TempData["Message"] ?? string.Empty;

            return View(_tableModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TableModel model)
        {
            if (model.Rows == null || model.Rows.Count == 0)
            {
                return Content("Binding error! Table is empty");
            }

            if (!ModelState.IsValid)
            {
                return View("Edit", model);
            }

            for (int i = 0; i < model.Rows.Count; i++)
            {
                RowModel row = model.Rows[i];
                string rowNameNormalize = row.Name.ToUpperInvariant();
                foreach (RowModel row2 in model.Rows)
                {
                    if (row2.Id == row.Id) continue;
                    string row2nameNormalize = row2.Name.ToUpperInvariant();
                    if (row2nameNormalize == rowNameNormalize)
                    {
                        ModelState.AddModelError($"Rows[{i}].Name", "Name must be Unique");
                        return View("Edit", model);
                    }
                }
            }

            //так делать нельзя т.к. можно модифицировать таблицу на стороне клиента и нахаляву добавить строки
            //StorageService.SaveData(model); //

            //вместо этого просто обновляю данные по каждой строке
            TableModel savedTable = StorageService.LoadOrCreateData();
            foreach (RowModel row in model.Rows)
            {
                RowModel savedRow = savedTable.Rows.FirstOrDefault(i => i.Id == row.Id)
                    ?? throw new Exception($"Failed to get row id {row.Id}");

                savedRow.Name = row.Name;
                savedRow.Email = row.Email;
            }
            savedTable.Title = model.Title;

            StorageService.SaveData(savedTable);

            TempData["Message"] = "Succesfully saved!";
            return RedirectToAction("Edit");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(TableModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", model);
            }

            TableModel savedTable = StorageService.LoadOrCreateData();

            int rowsDeleted = 0;
            foreach (RowModel row in model.Rows)
            {
                if (!row.Checked) continue;

                RowModel savedRow = savedTable.Rows.FirstOrDefault(i => i.Id == row.Id)
                    ?? throw new Exception($"Failed to get row id {row.Id}");
                savedTable.Rows.Remove(savedRow);
                rowsDeleted++;
            }

            if (rowsDeleted == 0)
            {
                ModelState.AddModelError("", "No selected rows to delete");
                return View("Edit", model);
            }

            TempData["Message"] = $"Succesfully deleted {rowsDeleted} rows!";
            StorageService.SaveData(savedTable);
            return RedirectToAction("Edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(int AddRowsCount)
        {
            TempData["Message"] = $"Succesfully added {AddRowsCount} rows!";

            TableModel savedTable = StorageService.LoadOrCreateData();
            int rowMaxId = savedTable.Rows.Max(i => i.Id);
            for (int i = 0; i < AddRowsCount; i++)
            {
                rowMaxId++;
                RowModel row = new RowModel
                {
                    Id = rowMaxId,
                    Name = $"New row {rowMaxId}",
                };
                savedTable.Rows.Add(row);
            }

            TempData["Message"] = $"Succesfully added {AddRowsCount} rows!";
            StorageService.SaveData(savedTable);
            return RedirectToAction("Edit");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}