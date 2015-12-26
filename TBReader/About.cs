using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TBReader
{
	partial class About : Tools
	{
		public About()
		{
			InitializeComponent();
			/*this.Text = String.Format("关于 {0}", AssemblyTitle);
			this.labelProductName.Text = AssemblyProduct;
			this.labelVersion.Text = String.Format("版本 {0}", AssemblyVersion);
			this.labelCopyright.Text = AssemblyCopyright;
			this.labelCompanyName.Text = AssemblyCompany;
			this.textBoxDescription.Text = AssemblyDescription;*/

			label1.Text = "Ctrl+Shift+Right: Next Line\r\nCtrl+Shift+Left: Previous line\r\nCtrl+Shift+Up: Bookmark current line\r\nCtrl+Shift+Down: Jump through bookmarks\r\nCtrl+Shift+P: Jump to previous location\r\n                    (ONLY under bookmark view)\r\nCtrl+Shift+R: Toggle default title/current line\r\nCtrl+Shift+Space: Toggle hide/show\r\nCtrl+Shift+Q: Quit";
			label2.Text = "Version: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + "\r\nAuthor: Henry Xu\r\nEmail: henryxrl@gmail.com\r\n\r\nThank you for your support! :)";
		}

		#region 程序集特性访问器

		public string AssemblyTitle
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				if (attributes.Length > 0)
				{
					AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
					if (titleAttribute.Title != "")
					{
						return titleAttribute.Title;
					}
				}
				return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}

		public string AssemblyVersion
		{
			get
			{
				return Assembly.GetExecutingAssembly().GetName().Version.ToString();
			}
		}

		public string AssemblyDescription
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyDescriptionAttribute)attributes[0]).Description;
			}
		}

		public string AssemblyProduct
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyProductAttribute)attributes[0]).Product;
			}
		}

		public string AssemblyCopyright
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
			}
		}

		public string AssemblyCompany
		{
			get
			{
				object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
				if (attributes.Length == 0)
				{
					return "";
				}
				return ((AssemblyCompanyAttribute)attributes[0]).Company;
			}
		}
		#endregion

		private void About_Load(object sender, EventArgs e)
		{
			Tuple<FontFamily, Font> result = loadFont();
			AllocFont(tabControl1, result.Item1, result.Item2, 14f, FontStyle.Bold);
			AllocFont(label1, result.Item1, result.Item2, 13f, FontStyle.Regular);
			AllocFont(label2, result.Item1, result.Item2, 13f, FontStyle.Regular);
		}
	}
}
