﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;

namespace RainbowMage.OverlayPlugin
{
    public partial class ControlPanel : UserControl
    {
        PluginMain pluginMain;
        PluginConfig config;

        static readonly List<KeyValuePair<string, MiniParseSortType>> sortTypeDict = new List<KeyValuePair<string, MiniParseSortType>>()
        {
            new KeyValuePair<string, MiniParseSortType>(Localization.GetText(TextItem.DoNotSort), MiniParseSortType.None),
            new KeyValuePair<string, MiniParseSortType>(Localization.GetText(TextItem.SortStringAscending), MiniParseSortType.StringAscending),
            new KeyValuePair<string, MiniParseSortType>(Localization.GetText(TextItem.SortStringDescending), MiniParseSortType.StringDescending),
            new KeyValuePair<string, MiniParseSortType>(Localization.GetText(TextItem.SortNumberAscending), MiniParseSortType.NumericAscending),
            new KeyValuePair<string, MiniParseSortType>(Localization.GetText(TextItem.SortNumberDescending), MiniParseSortType.NumericDescending)
        };

        public ControlPanel(PluginMain pluginMain, PluginConfig config)
        {
            InitializeComponent();

            this.pluginMain = pluginMain;
            this.config = config;

            SetupMiniParseConfigHandlers();
            SetupSpellTimerConfigHandlers();

            SetupMiniParseTab();
            SetupSpellTimerTab();

            this.menuFollowLatestLog.Checked = this.config.FollowLatestLog;
            this.listViewLog.VirtualListSize = pluginMain.Logs.Count;
            this.pluginMain.Logs.ListChanged += (o, e) =>
            {
                this.listViewLog.BeginUpdate();
                this.listViewLog.VirtualListSize = pluginMain.Logs.Count;
                if (this.config.FollowLatestLog && this.pluginMain.Logs.Count > 0)
                {
                    this.listViewLog.EnsureVisible(this.pluginMain.Logs.Count - 1);
                }
                this.listViewLog.EndUpdate();
            };
        }

        private void SetupMiniParseTab()
        {
            this.checkMiniParseVisible.Checked = config.MiniParseOverlay.IsVisible;
            this.checkMiniParseClickthru.Checked = config.MiniParseOverlay.IsClickThru;
            this.textMiniParseUrl.Text = config.MiniParseOverlay.Url;
            this.textMiniParseSortKey.Text = config.MiniParseOverlay.SortKey;
            this.comboMiniParseSortType.DisplayMember = "Key";
            this.comboMiniParseSortType.ValueMember = "Value";
            this.comboMiniParseSortType.DataSource = sortTypeDict;
            this.comboMiniParseSortType.SelectedValue = config.MiniParseOverlay.SortType;
            this.comboMiniParseSortType.SelectedIndexChanged += comboSortType_SelectedIndexChanged;
            this.checkEnableGlobalHotkey.Checked = config.MiniParseOverlay.GlobalHotkeyEnabled;
            this.textGlobalHotkey.Enabled = this.checkEnableGlobalHotkey.Checked;
            this.textGlobalHotkey.Text = GetHotkeyString(config.MiniParseOverlay.GlobalHotkeyModifiers, config.MiniParseOverlay.GlobalHotkey);
            this.nudMiniParseMaxFrameRate.Value = config.MiniParseOverlay.MaxFrameRate;
        }

        private void SetupSpellTimerTab()
        {
            this.checkSpellTimerVisible.Checked = config.SpellTimerOverlay.IsVisible;
            this.checkSpellTimerClickThru.Checked = config.SpellTimerOverlay.IsClickThru;
            this.textSpellTimerUrl.Text = config.SpellTimerOverlay.Url;
            this.checkSpellTimerEnableGlobalHotkey.Checked = config.SpellTimerOverlay.GlobalHotkeyEnabled;
            this.textSpellTimerGlobalHotkey.Enabled = this.checkSpellTimerEnableGlobalHotkey.Checked;
            this.textSpellTimerGlobalHotkey.Text = GetHotkeyString(config.SpellTimerOverlay.GlobalHotkeyModifiers, config.SpellTimerOverlay.GlobalHotkey);
            this.nudSpellTimerMaxFrameRate.Value = config.SpellTimerOverlay.MaxFrameRate;
        }

        private void SetupMiniParseConfigHandlers()
        {
            this.config.MiniParseOverlay.VisibleChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.checkMiniParseVisible.Checked = e.IsVisible;
                });
            };
            this.config.MiniParseOverlay.ClickThruChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.checkMiniParseClickthru.Checked = e.IsClickThru;
                });
            };
            this.config.MiniParseOverlay.UrlChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.textMiniParseUrl.Text = e.NewUrl;
                });
            };
            this.config.MiniParseOverlay.SortKeyChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.textMiniParseSortKey.Text = e.NewSortKey;
                });
            };
            this.config.MiniParseOverlay.SortTypeChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.comboMiniParseSortType.SelectedValue = e.NewSortType;
                });
            };
            this.config.MiniParseOverlay.GlobalHotkeyEnabledChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.checkEnableGlobalHotkey.Checked = e.NewGlobalHotkeyEnabled;
                    this.textGlobalHotkey.Enabled = this.checkEnableGlobalHotkey.Checked;
                });
            };
            this.config.MiniParseOverlay.GlobalHotkeyChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.textGlobalHotkey.Text = GetHotkeyString(this.config.MiniParseOverlay.GlobalHotkeyModifiers, e.NewHotkey);
                });
            };
            this.config.MiniParseOverlay.GlobalHotkeyModifiersChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.textGlobalHotkey.Text = GetHotkeyString(e.NewHotkey, this.config.MiniParseOverlay.GlobalHotkey);
                });
            };
            this.config.MiniParseOverlay.MaxFrameRateChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.nudMiniParseMaxFrameRate.Value = e.NewFrameRate;
                });
            };
        }

        private void SetupSpellTimerConfigHandlers()
        {
            this.config.SpellTimerOverlay.VisibleChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.checkSpellTimerVisible.Checked = e.IsVisible;
                });
            };
            this.config.SpellTimerOverlay.ClickThruChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.checkSpellTimerClickThru.Checked = e.IsClickThru;
                });
            };
            this.config.SpellTimerOverlay.UrlChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.textSpellTimerUrl.Text = e.NewUrl;
                });
            };
            this.config.SpellTimerOverlay.GlobalHotkeyEnabledChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.checkSpellTimerEnableGlobalHotkey.Checked = e.NewGlobalHotkeyEnabled;
                    this.textSpellTimerGlobalHotkey.Enabled = this.checkSpellTimerEnableGlobalHotkey.Checked;
                });
            };
            this.config.SpellTimerOverlay.GlobalHotkeyChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.textSpellTimerGlobalHotkey.Text = GetHotkeyString(this.config.SpellTimerOverlay.GlobalHotkeyModifiers, e.NewHotkey);
                });
            };
            this.config.SpellTimerOverlay.GlobalHotkeyModifiersChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.textSpellTimerGlobalHotkey.Text = GetHotkeyString(e.NewHotkey, this.config.SpellTimerOverlay.GlobalHotkey);
                });
            };
            this.config.SpellTimerOverlay.MaxFrameRateChanged += (o, e) =>
            {
                this.InvokeIfRequired(() =>
                {
                    this.nudSpellTimerMaxFrameRate.Value = e.NewFrameRate;
                });
            };
        }

        private void InvokeIfRequired(Action action)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(action);
            }
            else
            {
                action();
            }
        }

        private void checkWindowVisible_CheckedChanged(object sender, EventArgs e)
        {
            this.config.MiniParseOverlay.IsVisible = checkMiniParseVisible.Checked;
        }

        private void checkMouseClickthru_CheckedChanged(object sender, EventArgs e)
        {
            this.config.MiniParseOverlay.IsClickThru = checkMiniParseClickthru.Checked;
        }

        private void textUrl_TextChanged(object sender, EventArgs e)
        {
            this.config.MiniParseOverlay.Url = textMiniParseUrl.Text;
        }

        private void buttonReloadBrowser_Click(object sender, EventArgs e)
        {
            pluginMain.MiniParseOverlay.Navigate(config.MiniParseOverlay.Url);
        }

        private void buttonSelectFile_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.config.MiniParseOverlay.Url = new Uri(ofd.FileName).ToString();
            }
        }

        private void menuLogCopy_Click(object sender, EventArgs e)
        {
            if (listViewLog.SelectedIndices.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (int index in listViewLog.SelectedIndices)
                {
                    sb.AppendFormat(
                        "{0}: {1}: {2}",
                        pluginMain.Logs[index].Time,
                        pluginMain.Logs[index].Level,
                        pluginMain.Logs[index].Message);
                    sb.AppendLine();
                }
                Clipboard.SetText(sb.ToString());
            }
        }

        private void buttonCopyActXiv_Click(object sender, EventArgs e)
        {
            var json = pluginMain.MiniParseOverlay.CreateJsonData();
            if (!string.IsNullOrWhiteSpace(json))
            {
                Clipboard.SetText("var ActXiv = " + json + ";");
            }
        }

        private void textSortKey_TextChanged(object sender, EventArgs e)
        {
            this.config.MiniParseOverlay.SortKey = this.textMiniParseSortKey.Text;
        }

        private void comboSortType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var value = (MiniParseSortType)this.comboMiniParseSortType.SelectedValue;
            this.config.MiniParseOverlay.SortType = value;
        }

        private void checkEnableGlobalHotkey_CheckedChanged(object sender, EventArgs e)
        {
            this.config.MiniParseOverlay.GlobalHotkeyEnabled = this.checkEnableGlobalHotkey.Checked;
            this.textGlobalHotkey.Enabled = this.config.MiniParseOverlay.GlobalHotkeyEnabled;
        }
        private void textGlobalHotkey_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            var key = RemoveModifiers(e.KeyCode, e.Modifiers);
            this.config.MiniParseOverlay.GlobalHotkey = key;
            this.config.MiniParseOverlay.GlobalHotkeyModifiers = e.Modifiers;
        }
        private void nudMiniParseMaxFrameRate_ValueChanged(object sender, EventArgs e)
        {
            this.config.MiniParseOverlay.MaxFrameRate = (int)nudMiniParseMaxFrameRate.Value;
        }

        private void checkSpellTimerVisible_CheckedChanged(object sender, EventArgs e)
        {
            this.config.SpellTimerOverlay.IsVisible = this.checkSpellTimerVisible.Checked;
        }

        private void checkSpellTimerClickThru_CheckedChanged(object sender, EventArgs e)
        {
            this.config.SpellTimerOverlay.IsClickThru = this.checkSpellTimerClickThru.Checked;
        }

        private void textSpellTimerUrl_TextChanged(object sender, EventArgs e)
        {
            this.config.SpellTimerOverlay.Url = this.textSpellTimerUrl.Text;
        }

        private void buttonSpellTimerSelectFile_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.config.SpellTimerOverlay.Url = new Uri(ofd.FileName).ToString();
            }
        }
        private void checkSpelltimerEnableGlobalHotkey_CheckedChanged(object sender, EventArgs e)
        {
            this.config.SpellTimerOverlay.GlobalHotkeyEnabled = this.checkSpellTimerEnableGlobalHotkey.Checked;
            this.textSpellTimerGlobalHotkey.Enabled = this.config.SpellTimerOverlay.GlobalHotkeyEnabled;
        }
        private void textSpellTimerGlobalHotkey_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            var key = RemoveModifiers(e.KeyCode, e.Modifiers);
            this.config.SpellTimerOverlay.GlobalHotkey = key;
            this.config.SpellTimerOverlay.GlobalHotkeyModifiers = e.Modifiers;
        }
        private void buttonSpellTimerCopyActXiv_Click(object sender, EventArgs e)
        {
            var json = pluginMain.SpellTimerOverlay.CreateJsonData();
            if (!string.IsNullOrWhiteSpace(json))
            {
                Clipboard.SetText("var ActXiv = " + json + ";");
            }
        }

        private void buttonSpellTimerReloadBrowser_Click(object sender, EventArgs e)
        {
            pluginMain.SpellTimerOverlay.Navigate(config.SpellTimerOverlay.Url);
        }

        private void nudSpellTimerMaxFrameRate_ValueChanged(object sender, EventArgs e)
        {
            this.config.SpellTimerOverlay.MaxFrameRate = (int)nudSpellTimerMaxFrameRate.Value;
        }

        private void listViewLog_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (e.ItemIndex >= pluginMain.Logs.Count) 
            {
                e.Item = new ListViewItem();
                return;
            };

            var log = this.pluginMain.Logs[e.ItemIndex];
            e.Item = new ListViewItem(log.Time.ToString());
            e.Item.UseItemStyleForSubItems = true;
            e.Item.SubItems.Add(log.Level.ToString());
            e.Item.SubItems.Add(log.Message);

            e.Item.ForeColor = Color.Black;
            if (log.Level == LogLevel.Warning)
            {
                e.Item.BackColor = Color.LightYellow;
            }
            else if (log.Level == LogLevel.Error)
            {
                e.Item.BackColor = Color.LightPink;
            }
            else
            {
                e.Item.BackColor = Color.White;
            }
        }

        private void menuFollowLatestLog_Click(object sender, EventArgs e)
        {
            this.config.FollowLatestLog = menuFollowLatestLog.Checked;
        }

        private void menuClearLog_Click(object sender, EventArgs e)
        {
            this.pluginMain.Logs.Clear();
        }

        private void menuCopyLogAll_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            foreach (var log in this.pluginMain.Logs)
            {
                sb.AppendFormat(
                    "{0}: {1}: {2}",
                    log.Time,
                    log.Level,
                    log.Message);
                sb.AppendLine();
            }
            Clipboard.SetText(sb.ToString());
        }

        //Generates human readable keypress string
        //人間が読めるキー押下文字列を生成します
        private string GetHotkeyString(Keys Modifier, Keys key, String defaultText = "")
        {
            StringBuilder sbKeys = new StringBuilder();
            if ((Modifier & Keys.Shift) == Keys.Shift)
            {
                sbKeys.Append("Shift + ");
            }
            if ((Modifier & Keys.Control) == Keys.Control)
            {
                sbKeys.Append("Ctrl + ");
            }
            if ((Modifier & Keys.Alt) == Keys.Alt)
            {
                sbKeys.Append("Alt + ");
            }
            if ((Modifier & Keys.LWin) == Keys.LWin || (Modifier & Keys.RWin) == Keys.RWin)
            {
                sbKeys.Append("Win + ");
            }
            sbKeys.Append(Enum.ToObject(typeof(Keys), key).ToString());
            return sbKeys.ToString();
        }



        //Removes stray references to Left/Right shifts, etc and modifications of the actual key value caused by bitwise operations
        //ビット単位の操作に起因する左/右シフト、などと実際のキー値の変更に浮遊の参照を削除します。
        private Keys RemoveModifiers(Keys KeyCode, Keys Modifiers)
        {
            var key = KeyCode;
            var modifiers = new List<Keys>() { Keys.ControlKey, Keys.LControlKey, Keys.Alt, Keys.ShiftKey, Keys.Shift, Keys.LShiftKey, Keys.RShiftKey, Keys.Control, Keys.LWin, Keys.RWin };
            foreach (var mod in modifiers)
            {
                if (key.HasFlag(mod))
                {
                    if (key == mod)
                        key &= ~mod;
                }
            }
            return key;
        }
    }
}
