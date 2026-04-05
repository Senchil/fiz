using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;  
using System.IO;
using System.Text;
using System.Windows.Forms;
using OfficeOpenXml;
using BCrypt.Net;
using Microsoft.Data.Sqlite;
using fiz.Models;


namespace fiz.Forms
{
	class Reports
	{
		public static string connectionString { get; private set; }

		// Для избежания ошибок при работе с .csv файлами
		public string EscapeCsvField(string field)
		{
			if (string.IsNullOrEmpty(field))
				return "";

			if ((field.Contains(",") || field.Contains("\"") || field.Contains("\n")) &&
				(!field.StartsWith("\"") && !field.EndsWith("\"")))
			{
				field = field.Replace("\"", "\"\"");
				return $"\"{field}\"";
			}
			return field;
		}

		public void Report_query_To_csv(string query)  
		{
			string fileName = $"report_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
			string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
			string dbPath = Path.Combine(Application.StartupPath, "database.db");
			connectionString = $"Data Source={dbPath}";

			try
			{
				using (SQLiteConnection conn = new SQLiteConnection(connectionString))
				{
					conn.Open();
					using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
					{
						using (SQLiteDataReader reader = cmd.ExecuteReader())
						{
							using (StreamWriter writer = new StreamWriter(FilePath, false, Encoding.UTF8))
							{
								// Записываем заголовки
								for (int i = 0; i < reader.FieldCount; i++)
								{
									writer.Write(EscapeCsvField(reader.GetName(i)));
									if (i < reader.FieldCount - 1)
										writer.Write(",");
								}
								writer.WriteLine();

								// Записываем данные
								while (reader.Read())
								{
									for (int i = 0; i < reader.FieldCount; i++)
									{
										string value = reader.IsDBNull(i) ? "" : reader.GetValue(i).ToString();
										writer.Write(EscapeCsvField(value));
										if (i < reader.FieldCount - 1)
											writer.Write(",");
									}
									writer.WriteLine();
								}
							}
						}
					}
				}

				MessageBox.Show($"Файл успешно сохранен:\n{FilePath}", "Успех",
					MessageBoxButtons.OK, MessageBoxIcon.Information);
				System.Diagnostics.Process.Start("explorer.exe", $"/select, \"{FilePath}\"");
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при экспорте в формат .csv: {ex.Message}", "Ошибка",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void Report_query_To_xlsx(string query)
		{
			string fileName = $"report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
			string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
			string dbPath = Path.Combine(Application.StartupPath, "database.db");
			connectionString = $"Data Source={dbPath}";

			try
			{
				using (SQLiteConnection conn = new SQLiteConnection(connectionString))
				{
					conn.Open();
					using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
					{
						DataTable dt = new DataTable();
						using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
						{
							adapter.Fill(dt);
						}

						using (ExcelPackage package = new ExcelPackage())
						{
							ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Report");

							// Записываем заголовки
							for (int i = 0; i < dt.Columns.Count; i++)
							{
								worksheet.Cells[1, i + 1].Value = dt.Columns[i].ColumnName;
								worksheet.Cells[1, i + 1].Style.Font.Bold = true;
								worksheet.Cells[1, i + 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
								worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
							}

							// Записываем данные
							for (int i = 0; i < dt.Rows.Count; i++)
							{
								for (int j = 0; j < dt.Columns.Count; j++)
								{
									worksheet.Cells[i + 2, j + 1].Value = dt.Rows[i][j]?.ToString();
								}
							}

							// Авто-подбор ширины колонок
							worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

							// Сохраняем файл
							FileInfo fileInfo = new FileInfo(FilePath);
							package.SaveAs(fileInfo);

							MessageBox.Show($"Файл успешно сохранен:\n{FilePath}", "Успех",
								MessageBoxButtons.OK, MessageBoxIcon.Information);
							System.Diagnostics.Process.Start("explorer.exe", $"/select, \"{fileInfo.FullName}\"");
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка при экспорте в формат .xlsx: {ex.Message}", "Ошибка",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}