using Ini;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TBReader
{
	public class Tools : Form
	{
		[DllImport("gdi32")]
		private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);

		public Tuple<FontFamily, Font> loadFont()
		{
			byte[] fontArray = TBReader.Properties.Resources.rabiohead;
			int dataLength = TBReader.Properties.Resources.rabiohead.Length;

			IntPtr ptrData = Marshal.AllocCoTaskMem(dataLength);

			Marshal.Copy(fontArray, 0, ptrData, dataLength);

			uint cFonts = 0;

			AddFontMemResourceEx(ptrData, (uint)fontArray.Length, IntPtr.Zero, ref cFonts);

			PrivateFontCollection pfc = new PrivateFontCollection();
			pfc.AddMemoryFont(ptrData, dataLength);
			FontFamily ff = pfc.Families[0];
			Font font = new Font(ff, 12f, FontStyle.Regular);

			Marshal.FreeCoTaskMem(ptrData);

			Tuple<FontFamily, Font> result = new System.Tuple<FontFamily, Font>(ff, font);

			return result;
		}

		public void AllocFont(Control c, FontFamily ff, Font f, float size, FontStyle fs)
		{
			c.Font = new Font(ff, size, fs);
		}

		public List<int> loadBookmarks(IniFile ini, String fileName)
		{
			List<int> result = new List<int>();

			try
			{
				ini.IniReadValue(fileName, "Loc0");
			}
			catch
			{
				return result;
			}

			int i = 0;
			while (true)
			{
				try
				{
					result.Add(Convert.ToInt32(ini.IniReadValue(fileName, "Loc" + i.ToString())));
					i++;
				}
				catch
				{
					break;
				}
			}
			return result;
		}

		public bool writeBookmark(IniFile ini, String fileName, int line_idx, int bookmarks_idx)
		{
			// search for duplicates
			bool found = false;

			int i = 0;
			while (true)
			{
				try
				{
					if (Convert.ToInt32(ini.IniReadValue(fileName, "Loc" + i.ToString())) == line_idx)
					{
						 //MessageBox.Show("found!");
						found = true;
						break;
					}
					i++;
				}
				catch
				{
					break;
				}
			}

			if (found) return false;
			else
			{
				//MessageBox.Show("about to write");
				ini.IniWriteValue(fileName, "Loc" + bookmarks_idx.ToString(), line_idx.ToString());
				return true;
			}
		}

		public bool writeCurLoc(IniFile ini, String fileName, int line_idx)
		{
			try
			{
				ini.IniWriteValue(fileName, "Cur", line_idx.ToString());
				return true;
			}
			catch
			{
				return false;
			}
		}

		public int loadCurLoc(IniFile ini, String fileName)
		{
			int result = -1;

			try
			{
				result = Convert.ToInt32(ini.IniReadValue(fileName, "Cur"));
				//MessageBox.Show(result.ToString());
				return result;
			}
			catch
			{
				return result;
			}
		}
	
	}
}
