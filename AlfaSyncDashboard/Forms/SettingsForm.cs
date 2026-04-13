using AlfaSyncDashboard.Models;

namespace AlfaSyncDashboard.Forms;

public sealed class SettingsForm : Form
{
    private readonly TextBox _txtScriptsPath = new() { Dock = DockStyle.Top };
    private readonly TextBox _txtCentralConnection = new() { Dock = DockStyle.Fill, Multiline = true, ScrollBars = ScrollBars.Vertical };

    public AppSettings Settings { get; }

    public SettingsForm(AppSettings settings)
    {
        Settings = settings;
        Text = "Configuración";
        Width = 900;
        Height = 380;
        StartPosition = FormStartPosition.CenterParent;

        var btnBrowse = new Button { Text = "Seleccionar carpeta...", Dock = DockStyle.Right, Width = 160 };
        btnBrowse.Click += (_, _) => BrowseFolder();

        var pathPanel = new Panel { Dock = DockStyle.Top, Height = 34 };
        pathPanel.Controls.Add(_txtScriptsPath);
        pathPanel.Controls.Add(btnBrowse);

        var btnSave = new Button { Text = "Guardar", Dock = DockStyle.Right, Width = 120 };
        btnSave.Click += (_, _) => SaveAndClose();
        var btnCancel = new Button { Text = "Cancelar", Dock = DockStyle.Right, Width = 120 };
        btnCancel.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

        var bottom = new Panel { Dock = DockStyle.Bottom, Height = 44 };
        bottom.Controls.Add(btnSave);
        bottom.Controls.Add(btnCancel);

        var lbl1 = new Label { Text = "Ruta base de scripts", Dock = DockStyle.Top, Height = 20 };
        var lbl2 = new Label { Text = "Connection string central", Dock = DockStyle.Top, Height = 20 };

        Controls.Add(_txtCentralConnection);
        Controls.Add(lbl2);
        Controls.Add(pathPanel);
        Controls.Add(lbl1);
        Controls.Add(bottom);

        _txtScriptsPath.Text = settings.DefaultScriptsPath;
        _txtCentralConnection.Text = settings.CentralConnectionString;
    }

    private void BrowseFolder()
    {
        using var dialog = new FolderBrowserDialog();
        dialog.SelectedPath = _txtScriptsPath.Text;
        if (dialog.ShowDialog(this) == DialogResult.OK)
            _txtScriptsPath.Text = dialog.SelectedPath;
    }

    private void SaveAndClose()
    {
        Settings.DefaultScriptsPath = _txtScriptsPath.Text.Trim();
        Settings.CentralConnectionString = _txtCentralConnection.Text.Trim();
        DialogResult = DialogResult.OK;
        Close();
    }
}
