
using Ini;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing.Text;
using System.Diagnostics;

namespace TBReader
{
	public partial class Main : Tools
	{
		static private String iniPath = Application.StartupPath + "\\bookmarks.ini";
		private IniFile ini = new IniFile(iniPath);
		private List<int> bookmarks = new List<int>();
		private int bookmarks_idx = 0;

		private String filePath = "";
		private String fileName = "";
		//private String iniText;

		//private StreamReader sr;
		//private List<String> history = new List<string>();
		//private int lineNum = -1;

		private String[] lines;
		private int idx = -1;
		private int lineNum = 0;
		private int totalLine;
		private bool isNormalTitle = true;
		private List<String> curTitleText = new List<String>();

		[DllImport("user32.dll")]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
		[DllImport("user32.dll")]
		static extern int GetForegroundWindow();
		[DllImport("user32.dll")]
		static extern int GetWindowText(int hWnd, StringBuilder text, int count);
		[DllImport("user32.dll")]
		static extern int SetWindowText(int hWnd, StringBuilder text);
		[DllImport("user32.dll")]
		static extern int GetActiveWindow();
		[DllImport("gdi32")]
		private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);

		private FontFamily ff;
		private Font font;

		// About to register global hot key
		private KeyboardHook hook = new KeyboardHook();

		private int timerCount;
		private int timerFlag = -1;	// 0: auto read forward; 1: auto read backward; 2: go back to current; -1: default

		private int aptTime = 0;

		private Dictionary<int, String> defaultTitleText = new Dictionary<int, string>();

		private int window = 0;

		public Main()
		{
			InitializeComponent();

			// register the event that is fired after the key press.
			hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);

			try
			{
				// ctrl+shift+Q = Quit
				hook.RegisterHotKey(global::ModifierKeys.Control | global::ModifierKeys.Shift, Keys.Q);

				// ctrl+shift+right = next line
				hook.RegisterHotKey(global::ModifierKeys.Control | global::ModifierKeys.Shift, Keys.Right);

				// ctrl+shift+left = prev line
				hook.RegisterHotKey(global::ModifierKeys.Control | global::ModifierKeys.Shift, Keys.Left);

				// ctrl+shift+up = bookmark cur loc
				hook.RegisterHotKey(global::ModifierKeys.Control | global::ModifierKeys.Shift, Keys.Up);

				// ctrl+shift+down = jump to bookmark
				hook.RegisterHotKey(global::ModifierKeys.Control | global::ModifierKeys.Shift, Keys.Down);

				// ctrl+shift+r = toggle to default title text/cur condition
				hook.RegisterHotKey(global::ModifierKeys.Control | global::ModifierKeys.Shift, Keys.R);

				// ctrl+shift+space = toggle hide/show
				hook.RegisterHotKey(global::ModifierKeys.Control | global::ModifierKeys.Shift, Keys.Space);
			}
			catch
			{
				MessageBox.Show("Failed to register some of the hot keys!");
			}

		}

		private void loadFont()
		{
			byte[] fontArray = TBReader.Properties.Resources.rabiohead;
			int dataLength = TBReader.Properties.Resources.rabiohead.Length;

			IntPtr ptrData = Marshal.AllocCoTaskMem(dataLength);

			Marshal.Copy(fontArray, 0, ptrData, dataLength);

			uint cFonts = 0;

			AddFontMemResourceEx(ptrData, (uint)fontArray.Length, IntPtr.Zero, ref cFonts);

			PrivateFontCollection pfc = new PrivateFontCollection();
			pfc.AddMemoryFont(ptrData, dataLength);
			ff = pfc.Families[0];
			font = new Font(ff, 12f, FontStyle.Regular);

			Marshal.FreeCoTaskMem(ptrData);
		}

		private void AllocFont(Control c, Font f, float size, FontStyle fs)
		{
			c.Font = new Font(ff, size, fs);
		}

		private void Main_Load(object sender, EventArgs e)
		{
			((Control)draghere_pbox).AllowDrop = true;

			loadFont();
			dropTXTHere_label.Text = "Drop TXT Here";
			AllocFont(dropTXTHere_label, font, 34f, FontStyle.Bold);
			AllocFont(or_label, font, 16f, FontStyle.Bold);
			AllocFont(selectTXT_button, font, 14f, FontStyle.Regular);
			AllocFont(hotkeys_button, font, 14f, FontStyle.Regular);
			AllocFont(apt_0sec_label, font, 11f, FontStyle.Regular);
			AllocFont(apt_0secManual_label, font, 11f, FontStyle.Regular);
			AllocFont(apt_20sec_label, font, 11f, FontStyle.Regular);
			apt_main_label.Text = "Auto Page Turn";
			AllocFont(apt_main_label, font, 14f, FontStyle.Regular);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (filePath != "")
			{
				DialogResult dialogResult = MessageBox.Show("A TXT has already been opened.\nAre you sure to open another one?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
				if (dialogResult == DialogResult.Yes)
				{
					// do nothing
				}
				else if (dialogResult == DialogResult.No)
				{
					return;
				}
			}

			OpenFileDialog openTXT = new OpenFileDialog();
			openTXT.Filter = "Text Files (.txt)|*.txt";
			openTXT.FilterIndex = 1;
			openTXT.Multiselect = false;

			if (openTXT.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				filePath = openTXT.FileName;
				//MessageBox.Show(filePath);
				//System.Diagnostics.Process.Start(filePath);
				fileName = Path.GetFileNameWithoutExtension(filePath);

				saveCurTitleText();

				//sr = new StreamReader(filePath, Encoding.GetEncoding("GB2312"));
				lines = File.ReadAllLines(filePath, Encoding.Default);
				totalLine = lines.Count();
				//dropTXTHere_label.Text = "Done loading!";
				checkMark_pbox.Visible = true;
			}

		}

		private void button3_Click(object sender, EventArgs e)
		{
			if (filePath == "")
			{
				MessageBox.Show("No TXT file opened!");
				return;
			}

			this.Hide();
			//MessageBox.Show(filePath);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			quitTBReader();
		}

		private void notifyIcon1_DoubleClick(object sender, EventArgs e)
		{
			this.Show();
			this.WindowState = FormWindowState.Normal;
		}

		private void hook_KeyPressed(object sender, KeyPressedEventArgs e)
		{
			// show the keys pressed in a label.
			String key = e.Key.ToString();
			String keyComb = e.Modifier.ToString() + " + " + key;
			//MessageBox.Show(keyComb);

			switch (key)
			{
				case "Up":
					checkOpenTXT();
					addBookmark();
					break;
				case "Down":
					checkOpenTXT();
					jumpToBookmarks();
					break;
				case "Left":
					checkOpenTXT();
					readBackward();
					break;
				case "Right":
					checkOpenTXT();
					readForward();
					break;
				case "Q":
					quitTBReader();
					break;
				case "R":
					toggleTitleText();
					break;
				case "Space":
					hideShow();
					break;
				default:
					MessageBox.Show("Invalid key pressed!");
					break;
			}


		}

		private void saveCurTitleText()
		{
			window = GetForegroundWindow();
			StringBuilder titleText = new StringBuilder();
			GetWindowText(window, titleText, 256);

			//MessageBox.Show(window.ToString());
			//MessageBox.Show(titleText.ToString());

			try
			{
				defaultTitleText.Add(window, titleText.ToString());
			}
			catch (ArgumentException)
			{
				//defaultTitleText.Remove(window);
				//defaultTitleText.Add(window, titleText.ToString());

				// Should do nothing!
			}
			//String result = "";
			//defaultTitleText.TryGetValue(window, out result);
			//MessageBox.Show("Window id: " + window.ToString() + "; Title: " + result);
		}

		private void checkOpenTXT()
		{
			if (filePath == "")
			{
				MessageBox.Show("No TXT file opened!");
				return;
			}
		}

		private void readForward()
		{
			/*
			 * this method is not good for reading backwards
			 * window = GetForegroundWindow();
			 * StringBuilder toDisplay;
			 * //MessageBox.Show(iniText.ToString());
			 * 
			 * String nextLine;
			 * 
			 * if ((nextLine = sr.ReadLine()) != null)
			 * {
			 * 	toDisplay = iniText;
			 * 	//MessageBox.Show(nextLine);
			 * 	toDisplay.Append(nextLine.Trim());
			 * 	history.Add(toDisplay.ToString());
			 * 	lineNum++;
			 * 	//MessageBox.Show(toDisplay.ToString());
			 * 	SetWindowText(window, toDisplay);
			 * 	toDisplay.Clear();
			 * }
			 * 
			 * //sr.Close();
			 */

			/*
			window = GetForegroundWindow();
			StringBuilder toDisplay = new StringBuilder();
			toDisplay.Append(iniText);
			idx++;
			lineNum++;
			toDisplay.Append(" (" + lineNum + "/" + lines.Count() + ")--> " + lines[idx].Trim());
			SetWindowText(window, toDisplay);
			toDisplay.Clear();
			*/

			if (windowSwitched())
			{
				restorePrevWindowTitle();
				//MessageBox.Show("done restoring!");
				saveCurTitleText();
				//MessageBox.Show("done saving!");
			}

			if (idx < totalLine - 1 && lineNum < totalLine)
			{
				idx++;
				lineNum++;
				jumpToLine(idx, lineNum, false, 0);
			}

			if (aptTime != 0)
			{
				// timer here to apt in given secs
				timer.Enabled = true;
				timerCount = aptTime;
				timerFlag = 0;
			}

		}

		private void readBackward()
		{
			if (windowSwitched())
			{
				restorePrevWindowTitle();
				saveCurTitleText();
			}

			if (idx > 0 && lineNum > 1)
			{
				idx--;
				lineNum--;
				jumpToLine(idx, lineNum, false, 0);
			}
		}

		private void quitTBReader()
		{
			window = GetForegroundWindow();
			if (defaultTitleText.ContainsKey(window))
			{
				StringBuilder toDisplay = new StringBuilder();
				String result = "";
				defaultTitleText.TryGetValue(window, out result);
				toDisplay.Append(result);
				SetWindowText(window, toDisplay);
			}

			Application.Exit();
		}

		private void hideShow()
		{
			if (this.Visible == true) this.Hide();
			else this.Show();
		}

		private void addBookmark()
		{
			if (windowSwitched())
			{
				restorePrevWindowTitle();
				saveCurTitleText();
			}

			if (!isNormalTitle)
			{
				isNormalTitle = false;
				window = GetForegroundWindow();
				StringBuilder toDisplay = new StringBuilder();

				//bookmarks.Add(idx);
				//MessageBox.Show("A bookmark has been add at current location!");

				// write into ini
				bool written = writeBookmark(ini, fileName, idx, bookmarks_idx);
				if (!written)
				{
					toDisplay.Append("Bookmark exists!");
					SetWindowText(window, toDisplay);
				}
				else
				{
					bookmarks_idx++;
					toDisplay.Append("Bookmark added!");
					SetWindowText(window, toDisplay);
				}

				// timer here to go back to reading in 3 secs
				timer.Enabled = true;
				timerCount = 3;
				timerFlag = 2;
			}
		}

		private void jumpToLine(int jumpTO, int jumpTOL, bool isBookmark, int bookmark_idx)
		{
			isNormalTitle = false;
			window = GetForegroundWindow();
			StringBuilder toDisplay = new StringBuilder();
			//toDisplay.Append(iniText);

			if (isBookmark) toDisplay.Append("Bookmark (" + (bookmark_idx + 1).ToString() + "/" + bookmarks.Count() + ")--> ");

			if ((jumpTO >= 0 && jumpTOL >= 1) || (jumpTO < totalLine && jumpTOL < totalLine + 1))
			{
				toDisplay.Append(" (" + jumpTOL + "/" + lines.Count() + ")--> " + lines[jumpTO].Trim());
			}
			SetWindowText(window, toDisplay);
		}

		private void jumpToBookmarks()
		{
			if (windowSwitched())
			{
				restorePrevWindowTitle();
				saveCurTitleText();
			}

			bookmarks = loadBookmarks(ini, fileName);
			if (bookmarks.Count() == 0)
			{
				isNormalTitle = false;
				window = GetForegroundWindow();
				StringBuilder toDisplay = new StringBuilder();
				toDisplay.Append("No bookmark found!");
				SetWindowText(window, toDisplay);

				// timer here to go back to reading in 3 secs
				timer.Enabled = true;
				timerCount = 3;
				timerFlag = 2;

				return;
			}

			int bookmark_idx = findLargestSmaller(idx);
			jumpToLine(bookmarks[bookmark_idx], bookmarks[bookmark_idx] + 1, true, bookmark_idx);
			idx = bookmarks[bookmark_idx];
			lineNum = idx + 1;
		}

		private int findLargestSmaller(int idx)
		{
			bookmarks.Sort();

			int bookmarkNumber = bookmarks.Count();
			int bookmark_idx = bookmarkNumber - 1;
			while (bookmark_idx >= 0 && idx <= bookmarks[bookmark_idx])
			{
				bookmark_idx--;
			}
			if (bookmark_idx < 0)
			{
				return bookmarkNumber - 1;
			}
			return bookmark_idx;
		}

		private void toggleTitleText()
		{
			window = GetForegroundWindow();
			StringBuilder toDisplay = new StringBuilder();

			if (!isNormalTitle)
			{
				if (defaultTitleText.ContainsKey(window))
				{
					String result = "";
					defaultTitleText.TryGetValue(window, out result);
					toDisplay.Append(result);
					SetWindowText(window, toDisplay);
				}
				isNormalTitle = true;
			}

			else if (isNormalTitle && filePath != "")
			{
				toDisplay.Append(" (" + lineNum + "/" + lines.Count() + ")--> " + lines[idx].Trim());
				SetWindowText(window, toDisplay);
				isNormalTitle = false;
			}
			else { }
		}

		private void trackBar1_Scroll(object sender, EventArgs e)
		{
			String tooltip = "";

			aptTime = apt_tbar.Value;
			if (aptTime > 0)
			{
				tooltip = aptTime.ToString() + " (sec)";

				timer.Enabled = true;
				timerCount = aptTime;
				timerFlag = 0;	// auto read forward
			}
			else
			{
				tooltip = aptTime.ToString() + " (sec)\nManual";

			}
			toolTip1.SetToolTip(apt_tbar, tooltip);
		}

		private void pictureBox1_DragEnter(object sender, DragEventArgs e)
		{
			dragEnter(sender, e);
		}

		private void pictureBox1_DragDrop(object sender, DragEventArgs e)
		{
			dragDrop(sender, e);
		}

		private void dropTXTHere_label_DragEnter(object sender, DragEventArgs e)
		{
			dragEnter(sender, e);
		}

		private void dropTXTHere_label_DragDrop(object sender, DragEventArgs e)
		{
			dragDrop(sender, e);
		}

		private void or_label_DragEnter(object sender, DragEventArgs e)
		{
			dragEnter(sender, e);
		}

		private void or_label_DragDrop(object sender, DragEventArgs e)
		{
			dragDrop(sender, e);
		}

		private void dragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.All;
			else
				e.Effect = DragDropEffects.None;
		}

		private void dragDrop(object sender, DragEventArgs e)
		{
			String[] test = (String[])e.Data.GetData(DataFormats.FileDrop);
			if (test.Length != 1)
			{
				//MessageBox.Show("只能拖拽一个文件！");
				e.Effect = DragDropEffects.None;
			}
			else if (!test[0].ToLower().EndsWith(".txt"))
			{
				//MessageBox.Show("拖拽进来的不是TXT文件！");
				e.Effect = DragDropEffects.None;
			}
			else
			{
				if (filePath != "")
				{
					DialogResult dialogResult = MessageBox.Show("A TXT has already been opened.\nAre you sure to open another one?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
					if (dialogResult == DialogResult.Yes)
					{
						// do nothing
					}
					else if (dialogResult == DialogResult.No)
					{
						return;
					}
				}

				filePath = test[0];
				//MessageBox.Show(filePath);
				//System.Diagnostics.Process.Start(filePath);
				fileName = Path.GetFileNameWithoutExtension(filePath);

				saveCurTitleText();

				//sr = new StreamReader(filePath, Encoding.GetEncoding("GB2312"));
				lines = File.ReadAllLines(filePath, Encoding.Default);
				totalLine = lines.Count();
				//dropTXTHere_label.Text = "Done loading!!";
				checkMark_pbox.Visible = true;
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
			notifyIcon1.BalloonTipTitle = "Hot Keys";
			notifyIcon1.BalloonTipText = "Ctrl+Shift+Right: Next Line\nCtrl+Shift+Left: Previous line\nCtrl+Shift+Up: Bookmark current line\nCtrl+Shift+Down: Jump to bookmarks\nCtrl+Shift+R: Toggle default title/current line\nCtrl+Shift+Space: Toggle hide/show\nCtrl+Shift+Q: Quit";
			notifyIcon1.ShowBalloonTip(1000);
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			timerCount--;
			if (timerCount == 0)
			{
				if (timerFlag == 0)
				{
					readForward();
				}
				else if (timerFlag == 1)
				{
					// why do we need to auto read backwards? Lol...
					// readBackward();
				}
				else if (timerFlag == 2)
				{
					isNormalTitle = false;
					window = GetForegroundWindow();
					StringBuilder toDisplay = new StringBuilder();
					toDisplay.Append(" (" + lineNum + "/" + lines.Count() + ")--> " + lines[idx].Trim());
					SetWindowText(window, toDisplay);
					timer.Stop();
				}
				else    // timerFlag == -1
				{
					MessageBox.Show("No specified action to perform!");
				}
			}
		}

		private bool windowSwitched()
		{
			int tempWindow = GetForegroundWindow();
			if (tempWindow == window) return false;
			else return true;
		}

		private void restorePrevWindowTitle()
		{
			StringBuilder toDisplay = new StringBuilder();
			String result = "";
			defaultTitleText.TryGetValue(window, out result);
			toDisplay.Append(result);
			SetWindowText(window, toDisplay);
		}

	}
}
