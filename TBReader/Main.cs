using Ini;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TBReader
{
	public partial class Main : Tools
	{
		static private String iniPath = Application.StartupPath + "\\bookmarks.ini";
		private IniFile ini = new IniFile(iniPath);
		private List<int> bookmarks = new List<int>();
		private int bookmarks_idx = 0;
		private int curLoc = -1;

		private String filePath = "";
		private String fileName = "";

		private String[] lines;
		private String wrapped;
		private int idx = -1;
		private int lineNum = 0;
		private int totalLine;
		private bool isNormalTitle = true;

		// About to register global hot key
		private KeyboardHook hook = new KeyboardHook();

		private int timerCount;
		private int timerFlag = -1;	// 0: auto read forward; 1: auto read backward; 2: go back to current; -1: default

		private int aptTime = 0;

		private Dictionary<int, String> defaultTitleText = new Dictionary<int, string>();

		private int window = 0;

		[DllImport("user32.dll")]
		private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);
		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
		/*[DllImport("user32.dll")]
		private static extern int GetWindowTextLength(IntPtr hWnd);*/
		[DllImport("user32.dll")]
		private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
		[DllImport("user32.dll")]
		private static extern int SetWindowText(int hWnd, StringBuilder text);
		[DllImport("user32.dll")]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		private const uint WINEVENT_OUTOFCONTEXT = 0;
		private const uint EVENT_SYSTEM_FOREGROUND = 3;

		private WinEventDelegate dele = null;
		private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

		
		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;        // x position of upper-left corner  
			public int Top;         // y position of upper-left corner  
			public int Right;       // x position of lower-right corner  
			public int Bottom;      // y position of lower-right corner  
		}

		private int dim_w = 0;
		private int dim_h = 0;
		private int possibleTitleByteNum = 0;
		
		//private int activeWindowTextLength = 0;

		int curWordPos = 0;


		public Main()
		{
			InitializeComponent();
			dele = new WinEventDelegate(WinEventProc);
			IntPtr m_hhook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);

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

				// ctrl+shift+delete = delete current bookmark
				hook.RegisterHotKey(global::ModifierKeys.Control | global::ModifierKeys.Shift, Keys.Delete);

				// ctrl+shift+r = toggle to default title text/cur condition
				hook.RegisterHotKey(global::ModifierKeys.Control | global::ModifierKeys.Shift, Keys.R);

				// ctrl+shift+space = toggle hide/show
				hook.RegisterHotKey(global::ModifierKeys.Control | global::ModifierKeys.Shift, Keys.Space);

				// ctrl+shift+p = jump to previous location (only under bookmark view)
				hook.RegisterHotKey(global::ModifierKeys.Control | global::ModifierKeys.Shift, Keys.P);
			}
			catch
			{
				MessageBox.Show("Failed to register some of the hot keys!");
			}

		}

		private void Main_Load(object sender, EventArgs e)
		{
			((Control)draghere_pbox).AllowDrop = true;

			Tuple<FontFamily, Font> result = loadFont();
			dropTXTHere_label.Text = "Drop TXT Here";
			AllocFont(dropTXTHere_label, result.Item1, result.Item2, 34f, FontStyle.Bold);
			AllocFont(or_label, result.Item1, result.Item2, 16f, FontStyle.Bold);
			AllocFont(selectTXT_button, result.Item1, result.Item2, 14f, FontStyle.Regular);
			AllocFont(hotkeys_button, result.Item1, result.Item2, 14f, FontStyle.Regular);
			AllocFont(apt_0sec_label, result.Item1, result.Item2, 11f, FontStyle.Regular);
			AllocFont(apt_0secManual_label, result.Item1, result.Item2, 11f, FontStyle.Regular);
			AllocFont(apt_20sec_label, result.Item1, result.Item2, 11f, FontStyle.Regular);
			apt_main_label.Text = "Auto Page Turn";
			AllocFont(apt_main_label, result.Item1, result.Item2, 14f, FontStyle.Regular);

		}

		private string GetActiveWindowTitle()
		{
			const int nChars = 256;
			IntPtr handle = IntPtr.Zero;
			StringBuilder Buff = new StringBuilder(nChars);
			handle = GetForegroundWindow();

			if (GetWindowText(handle, Buff, nChars) > 0)
			{
				return Buff.ToString();
			}

			return null;
		}

		private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
		{
			// Get active window dimensions
			possibleTitleByteNum = getActiveWindowDim();
		
			/*
			// Get active window current title length, in number of characers
			try
			{
				int tempLength = GetWindowTextLength(hwnd);
				if (tempLength != activeWindowTextLength && tempLength != 0)
				{
					activeWindowTextLength = tempLength;
					MessageBox.Show(activeWindowTextLength.ToString());
				}
			}
			catch
			{
				MessageBox.Show("Failed to get window text length!");
			}
			*/

			// will try to save title text every window switch
			// don't need saveCurTitleText() any more!
			try
			{
				defaultTitleText.Add(hwnd.ToInt32(), GetActiveWindowTitle());
			}
			catch (ArgumentException)
			{
				// Should do nothing!
			}

			// auto load current line after window switch
			if (filePath != "" && idx >= 0)
			{
				if (windowSwitched())
				{
					restorePrevWindowTitle();
				}
				loadCurLine();
			}
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
				fileName = Path.GetFileNameWithoutExtension(filePath);

				//saveCurTitleText();

				//sr = new StreamReader(filePath, Encoding.GetEncoding("GB2312"));
				lines = File.ReadAllLines(filePath, Encoding.Default);
				totalLine = lines.Count();
				//dropTXTHere_label.Text = "Done loading!";
				checkMark_pbox.Visible = true;

				loadCurrentLoc();
			}

		}

		private void button2_Click(object sender, EventArgs e)
		{
			About about = new About();
			about.ShowDialog();
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
				case "P":
					jumpFromBookmarkToCurLoc();
					break;
				case "Delete":
					deleteCurBookmakr();
					break;
				default:
					MessageBox.Show("Invalid key pressed!");
					break;
			}


		}

		private bool windowSwitched()
		{
			int tempWindow = GetForegroundWindow().ToInt32();
			if (tempWindow == window) return false;
			else return true;
		}

		/*private bool windowResized()
		{
			if (!windowSwitched())
			{
				int possibleTitleByteNumTemp = getActiveWindowDim();
				if (possibleTitleByteNumTemp != possibleTitleByteNum) return true;
				else return false;
			}
			else return false;
		}*/

		private void toggleTitleText()
		{
			if (windowSwitched())
			{
				restorePrevWindowTitle();
				//saveCurTitleText();
			}

			window = GetForegroundWindow().ToInt32();
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
				//toDisplay.Append(" (" + lineNum + "/" + lines.Count() + ")--> 111" + lines[idx].Trim());
				/*String s = lines[idx].Trim();
				possibleTitleByteNum = getActiveWindowDim();
				wrapped = stringWrapper(s, curWordPos, possibleTitleByteNum);*/
				toDisplay.Append(" (" + lineNum + "/" + lines.Count() + ")--> 111" + wrapped);
				SetWindowText(window, toDisplay);
				isNormalTitle = false;
			}
			else
			{
			}

			toDisplay.Clear();
		}

		private void restorePrevWindowTitle()
		{
			isNormalTitle = true;
			StringBuilder toDisplay = new StringBuilder();
			String result = "";
			defaultTitleText.TryGetValue(window, out result);
			toDisplay.Append(result);
			SetWindowText(window, toDisplay);
			toDisplay.Clear();
		}

		private void loadCurLine()
		{
			isNormalTitle = false;
			window = GetForegroundWindow().ToInt32();
			StringBuilder toDisplay = new StringBuilder();
			//toDisplay.Append(" (" + lineNum + "/" + lines.Count() + ")--> 222" + lines[idx].Trim());
			/*String s = lines[idx].Trim();
			possibleTitleByteNum = getActiveWindowDim();
			wrapped = stringWrapper(s, curWordPos, possibleTitleByteNum);*/
			toDisplay.Append(" (" + lineNum + "/" + lines.Count() + ")--> 222" + wrapped);
			SetWindowText(window, toDisplay);
			toDisplay.Clear();
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
			if (windowSwitched())
			{
				restorePrevWindowTitle();
				//saveCurTitleText();
			}

			if (idx < totalLine - 1 && lineNum < totalLine)
			{
				if (curWordPos != 0)
				{
					jumpToLine(idx, lineNum, false, 0, false);
				}
				else
				{
					curWordPos = 0;
					idx++;
					lineNum++;
					jumpToLine(idx, lineNum, false, 0, false);
					saveCurrentLoc();
				}
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
				//saveCurTitleText();
			}

			if (idx > 0 && lineNum > 1)
			{
				curWordPos = 0;
				idx--;
				lineNum--;
				jumpToLine(idx, lineNum, false, 0, false);
				saveCurrentLoc();
			}

		}

		private void quitTBReader()
		{
			saveCurrentLoc();

			if (defaultTitleText.ContainsKey(window))
			{
				restorePrevWindowTitle();
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
				//saveCurTitleText();
			}

			if (!isNormalTitle)
			{
				isNormalTitle = false;
				window = GetForegroundWindow().ToInt32();
				StringBuilder toDisplay = new StringBuilder();

				// try to write into ini
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

				toDisplay.Clear();
			}
		}

		private void jumpToLine(int jumpTO, int jumpTOL, bool isBookmark, int bookmark_idx, bool isToPrev)
		{
			//int testL = Encoding.Default.GetByteCount(test);

			isNormalTitle = false;
			window = GetForegroundWindow().ToInt32();
			StringBuilder toDisplay = new StringBuilder();

			if (isBookmark)
			{
				toDisplay.Append("Bookmark (" + (bookmark_idx + 1).ToString() + "/" + (bookmarks.Count()).ToString() + ")--> ");
			}

			if (isToPrev)
			{
				toDisplay.Append("Previous Location ");
			}

			if ((jumpTO >= 0 && jumpTOL >= 1) || (jumpTO < totalLine && jumpTOL < totalLine + 1))
			{
				String s = lines[jumpTO].Trim();
				possibleTitleByteNum = getActiveWindowDim();
				wrapped = stringWrapper(s, curWordPos, possibleTitleByteNum);
				//MessageBox.Show("wrapped: " + wrapped);
				toDisplay.Append("(" + jumpTOL + "/" + lines.Count() + ")--> 000" + wrapped);
			}
			SetWindowText(window, toDisplay);
			toDisplay.Clear();
		}

		private String stringWrapper(String s, int start_pos, int max_width)
		{
			//MessageBox.Show("curWordPos: " + start_pos);
			String result = "";
			StringBuilder builder = new StringBuilder();

			if (s == "")
			{
				//if (updateCurWordPos) 
					curWordPos = 0;
				return result;
			}

			using (Graphics g = this.CreateGraphics())
			{
				for (int i = start_pos, j = start_pos; i < s.Length; i++)
				{
					builder.Append(s[i]);

					if (g.MeasureString(s.Substring(j, i - j + 1), this.Font).Width >= max_width)
					{
						j = i + 1;
						break;
					}
				}

				result = builder.ToString();
				builder.Clear();
				//if (updateCurWordPos)
				//{
					curWordPos += result.Length;
					//MessageBox.Show("curWordPos: " + curWordPos + "s.Length: " + s.Length);
					if (curWordPos == s.Length) curWordPos = 0;
				//}

			}
			//MessageBox.Show("Result: " + result + "New curWordPos: " + curWordPos);
			return result;
		}

		private void jumpToBookmarks()
		{
			if (windowSwitched())
			{
				restorePrevWindowTitle();
				//saveCurTitleText();
			}

			bookmarks = loadBookmarks(ini, fileName);
			if (bookmarks.Count() == 0)
			{
				isNormalTitle = false;
				window = GetForegroundWindow().ToInt32();
				StringBuilder toDisplay = new StringBuilder();
				toDisplay.Append("No bookmark found!");
				SetWindowText(window, toDisplay);

				// timer here to go back to reading in 3 secs
				timer.Enabled = true;
				timerCount = 3;
				timerFlag = 2;

				toDisplay.Clear();
				return;
			}

			int bookmark_idx = findLargestSmaller(idx);
			jumpToLine(bookmarks[bookmark_idx], bookmarks[bookmark_idx] + 1, true, bookmark_idx, false);
			idx = bookmarks[bookmark_idx];
			lineNum = idx + 1;
		}

		private void deleteCurBookmakr()
		{
			// To be implemented
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

		private void jumpFromBookmarkToCurLoc()
		{
			if (windowSwitched())
			{
				restorePrevWindowTitle();
				//saveCurTitleText();
			}

			if (!isNormalTitle)
			{
				isNormalTitle = false;

				loadCurrentLoc();

				String temp = GetActiveWindowTitle();
				if (temp.Contains("Bookmark"))
				{
					//MessageBox.Show("idx: " + idx.ToString() + "; curLoc: " + curLoc.ToString());
					jumpToLine(curLoc, curLoc + 1, false, 0, true);
					idx = curLoc;
					lineNum = idx + 1;
				}
			}
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
				fileName = Path.GetFileNameWithoutExtension(filePath);

				//sr = new StreamReader(filePath, Encoding.GetEncoding("GB2312"));
				lines = File.ReadAllLines(filePath, Encoding.Default);
				totalLine = lines.Count();
				//dropTXTHere_label.Text = "Done loading!!";
				checkMark_pbox.Visible = true;

				loadCurrentLoc();
			}
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			//MessageBox.Show("timerCount: " + timerCount.ToString() + "; aptTime: " + aptTime.ToString());
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
					loadCurLine();
				}
				else    // timerFlag == -1
				{
					MessageBox.Show("No specified action to perform!");
				}
			}
		}

		private void saveCurrentLoc()
		{
			//curLoc = idx;

			if (windowSwitched())
			{
				restorePrevWindowTitle();
				//saveCurTitleText();
			}

			if (!isNormalTitle)
			{
				isNormalTitle = false;

				// try to write into ini
				writeCurLoc(ini, fileName, idx);
			}
		}

		private void loadCurrentLoc()
		{
			if (windowSwitched())
			{
				restorePrevWindowTitle();
				//saveCurTitleText();
			}

			isNormalTitle = true;
			window = GetForegroundWindow().ToInt32();
			StringBuilder toDisplay = new StringBuilder();

			// try to write into ini
			curLoc = loadCurLoc(ini, fileName);
			
			if (curLoc == -1)
			{
				/*
				MessageBox.Show(iniPath);
				toDisplay.Append("Failed to load current location!");
				SetWindowText(window, toDisplay);
				toDisplay.Clear();
				*/
				curLoc = idx;

				// timer here to go back to reading in 3 secs
				/*
				timer.Enabled = true;
				timerCount = 3;
				timerFlag = 2;
				*/
			}
			else
			{
				idx = curLoc;
				lineNum = idx + 1;
			}
			
		}

		private int getActiveWindowDim()
		{
			int result = 0;

			try
			{
				
				RECT dim = new RECT();
				GetWindowRect(GetForegroundWindow(), out dim);
				if (Width != dim.Right - dim.Left || Height != dim.Bottom - dim.Top)
				{
					dim_w = dim.Right - dim.Left;
					dim_h = dim.Bottom - dim.Top;

					//MessageBox.Show("Width: " + Width + "; Height: " + Height);

					result = (dim_w - 200) / 2;
					if (result < 0) result = 0;

				}
			}
			catch
			{
				MessageBox.Show("Failed to get window rect!");
				result = -1;
			}

			return result;
		}
	}
}
