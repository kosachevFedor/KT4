using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace kt5work;

public class Form1 : Form
{
    private List<PassengerInfo> allPassengers = new List<PassengerInfo>();
    private List<string> flightNumbers = new List<string>();
    private Button btnLoadTxt;
    private Button btnLoadJson;
    private ListBox listBoxResults;
    private ComboBox cmbFlightNumber;
    private Button btnFilterAndSave;
    private Label label1;
    private Label label2;

    public Form1()
    {
        this.Text = "Менеджер авиабилетов";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Size = new System.Drawing.Size(800, 550);
        btnLoadTxt = new Button { Text = "Загрузить .txt", Location = new(12, 12), Size = new(130, 30) };
        btnLoadJson = new Button { Text = "Загрузить .json", Location = new(150, 12), Size = new(130, 30) };
        btnFilterAndSave = new Button { Text = "Фильтр и Сохранить", Location = new(220, 450), Size = new(150, 30) };

        listBoxResults = new ListBox
        {
            Location = new(12, 70),
            Size = new(760, 360),
            Font = new("Consolas", 9)
        };

        cmbFlightNumber = new ComboBox
        {
            Location = new(12, 455),
            Size = new(200, 25),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbFlightNumber.Items.Add("Все пассажиры");
        cmbFlightNumber.SelectedIndex = 0;

        label1 = new Label { Text = "Пассажиры:", Location = new(12, 50), AutoSize = true };
        label2 = new Label { Text = "Рейс:", Location = new(12, 435), AutoSize = true };
        btnLoadTxt.Click += BtnLoadTxt_Click;
        btnLoadJson.Click += BtnLoadJson_Click;
        btnFilterAndSave.Click += BtnFilterAndSave_Click;
        cmbFlightNumber.SelectedIndexChanged += CmbFlightNumber_SelectedIndexChanged;

        Controls.AddRange(new Control[]
        {
            btnLoadTxt, btnLoadJson, listBoxResults,
            cmbFlightNumber, btnFilterAndSave,
            label1, label2
        });
    }

    private void BtnLoadTxt_Click(object sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы|*.*",
            Title = "Выбери .txt"
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                string[] lines = File.ReadAllLines(dialog.FileName);
                allPassengers.Clear();
                flightNumbers.Clear();

                foreach (string line in lines)
                {
                    string trimmed = line.Trim();
                    if (string.IsNullOrWhiteSpace(trimmed)) continue;
                    var parts = trimmed.Split(' ');
                    if (parts.Length < 2) continue;
                    string flight = parts[^1];
                    string fullName = string.Join(" ", parts[..^1]);
                    allPassengers.Add(new PassengerInfo(fullName, flight));
                    if (!flightNumbers.Contains(flight))
                        flightNumbers.Add(flight);
                }

                UpdateList();
                UpdateComboBox();
                MessageBox.Show($"Загружено {allPassengers.Count} пассажиров.", "Nice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка - {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void BtnLoadJson_Click(object sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "JSON файлы (*.json)|*.json|Все файлы|*.*",
            Title = "Выберите tickets.json"
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                string json = File.ReadAllText(dialog.FileName);
                allPassengers = JsonSerializer.Deserialize<List<PassengerInfo>>(json) ?? new();

                flightNumbers = allPassengers.Select(p => p.FlightNumber).Distinct().ToList();

                UpdateList();
                UpdateComboBox();
                MessageBox.Show($"Загружено {allPassengers.Count} пассажиров.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка JSON: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void CmbFlightNumber_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cmbFlightNumber.SelectedItem?.ToString() == "Все пассажиры")
        {
            UpdateList();
        }
        else
        {
            string flight = cmbFlightNumber.SelectedItem.ToString();
            var filtered = allPassengers.Where(p => p.FlightNumber == flight).ToList();
            UpdateList(filtered);
        }
    }

    private void BtnFilterAndSave_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(cmbFlightNumber.Text) || cmbFlightNumber.Text == "Все пассажиры")
        {
            MessageBox.Show("Выберите конкретный рейс для фильтрации.", "Инфо", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string flight = cmbFlightNumber.Text;
        var filtered = allPassengers.Where(p => p.FlightNumber == flight).ToList();
        using var saveDialog = new SaveFileDialog
        {
            Filter = "JSON (*.json)|*.json|Текст (*.txt)|*.txt",
            FileName = $"passengers_{flight}.json"
        };
        if (saveDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                string ext = Path.GetExtension(saveDialog.FileName).ToLowerInvariant();
                if (ext == ".json")
                {
                    string json = JsonSerializer.Serialize(filtered, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(saveDialog.FileName, json);
                }
                else
                {
                    var lines = filtered.Select(p => $"{p.PassengerName} {p.FlightNumber}");
                    File.WriteAllLines(saveDialog.FileName, lines);
                }
                MessageBox.Show($"Сохранено {filtered.Count} записей", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения - {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    private void UpdateList(List<PassengerInfo>? list = null)
    {
        listBoxResults.Items.Clear();
        var source = list ?? allPassengers;
        foreach (var p in source)
            listBoxResults.Items.Add(p.ToString());
    }
    private void UpdateComboBox()
    {
        cmbFlightNumber.Items.Clear();
        cmbFlightNumber.Items.Add("Все пассажиры");
        foreach (string flight in flightNumbers)
            cmbFlightNumber.Items.Add(flight);
        cmbFlightNumber.SelectedIndex = 0;
    }
}
