using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;

namespace PandaBrowser
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new BrowserWindow());
        }
    }

    public class BrowserWindow : Form
    {
        private readonly WebView2 view = new WebView2();
        private readonly TextBox address = new TextBox();
        private readonly Button go = new Button();

        public BrowserWindow()
        {
            Text = "Panda Browser";
            Width = 1280;
            Height = 820;
            MinimumSize = new System.Drawing.Size(800, 550);
            BackColor = System.Drawing.Color.FromArgb(7, 19, 15);
            ForeColor = System.Drawing.Color.White;

            var bar = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = System.Drawing.Color.FromArgb(10, 30, 21) };
            var brand = new Label {
                Text = "🐼  Panda",
                Dock = DockStyle.Left,
                Width = 145,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Segoe UI", 13, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(86, 214, 139)
            };
            address.Dock = DockStyle.Fill;
            address.BorderStyle = BorderStyle.FixedSingle;
            address.Font = new System.Drawing.Font("Segoe UI", 11);
            address.Text = "https://www.google.com";
            go.Text = "Go";
            go.Dock = DockStyle.Right;
            go.Width = 70;
            go.FlatStyle = FlatStyle.Flat;
            go.BackColor = System.Drawing.Color.FromArgb(31, 122, 77);
            go.ForeColor = System.Drawing.Color.White;
            go.Click += (_, _) => Navigate(address.Text);
            address.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { Navigate(address.Text); e.SuppressKeyPress = true; } };
            bar.Controls.Add(address);
            bar.Controls.Add(go);
            bar.Controls.Add(brand);

            view.Dock = DockStyle.Fill;
            Controls.Add(view);
            Controls.Add(bar);
            Shown += async (_, _) =>
            {
                try
                {
                    await view.EnsureCoreWebView2Async();
                    view.CoreWebView2.NavigationStarting += (_, e) => address.Text = e.Uri;
                    view.CoreWebView2.Navigate("file:///" + Path.Combine(AppContext.BaseDirectory, "panda.html").Replace("\\", "/"));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Panda Browser could not start WebView2.\n\nInstall the Microsoft Edge WebView2 Runtime, then reopen Panda.\n\n" + ex.Message,
                        "Panda Browser", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }

        private void Navigate(string text)
        {
            if (view.CoreWebView2 == null) return;
            text = text.Trim();
            if (text.Length == 0) return;
            if (Uri.TryCreate(text, UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                view.CoreWebView2.Navigate(uri.ToString());
            }
            else if (text.Contains('.') && !text.Contains(' '))
            {
                view.CoreWebView2.Navigate("https://" + text);
            }
            else
            {
                view.CoreWebView2.Navigate("https://www.google.com/search?q=" + Uri.EscapeDataString(text));
            }
        }
    }
}
