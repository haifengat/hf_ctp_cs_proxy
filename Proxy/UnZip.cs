using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Reflection;

namespace ZipHelper
{
	class ZipArchive : IDisposable
	{
		private object external;
		private ZipArchive() { }
		public static ZipArchive OpenOnFile(string path)
		{
			FileMode mode = FileMode.Open;
			FileAccess access = FileAccess.Read;
			FileShare share = FileShare.Read;
			bool streaming = false;
			var type = typeof(Package).Assembly.GetType("MS.Internal.IO.Zip.ZipArchive");
			var meth = type.GetMethod("OpenOnFile", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			return new ZipArchive { external = meth.Invoke(null, new object[] { path, mode, access, share, streaming }) };
		}
		public void Dispose()
		{
			((IDisposable)external).Dispose();
		}
		public IEnumerable<ZipFileInfo> Files
		{
			get
			{
				var meth = external.GetType().GetMethod("GetFiles", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				var coll = meth.Invoke(external, null) as System.Collections.IEnumerable;
				foreach (var p in coll) yield return new ZipFileInfo { external = p };
			}
		}
		public struct ZipFileInfo
		{
			internal object external;
			private object GetProperty(string name)
			{
				return external.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(external, null);
			}
			public string Name
			{
				get { return (string)GetProperty("Name"); }
			}
			public Stream GetStream()
			{
				FileMode mode = FileMode.Open;
				FileAccess access = FileAccess.Read;
				var meth = external.GetType().GetMethod("GetStream", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				return (Stream)meth.Invoke(external, new object[] { mode, access });
			}
		}

		/// <summary>
		/// Zip解压并更新目标文件
		/// </summary>
		/// <param name="ZipFile">Zip压缩包路径</param>
		/// <param name="UnZipDir">解压目标路径</param>
		/// <returns></returns>
		public static bool UnZip2(String ZipFile, String UnZipDir)
		{
			try
			{
				UnZipDir = UnZipDir.EndsWith(@"\") ? UnZipDir : UnZipDir + @"\";
				using (var zipfile = ZipArchive.OpenOnFile(ZipFile))
				{
					foreach (var file in zipfile.Files)
					{
						if (!file.Name.EndsWith("/"))
						{
							string FilePath = UnZipDir + file.Name.Replace("/", @"\");  //设置解压路径
							string GreatFolder = FilePath.Substring(0, FilePath.LastIndexOf(@"\"));
							if (!Directory.Exists(GreatFolder)) Directory.CreateDirectory(GreatFolder);
							byte[] content = new byte[file.GetStream().Length];
							file.GetStream().Read(content, 0, content.Length);
							if (File.Exists(FilePath))                      //跳过相同的文件，否则覆盖更新
							{
								if (content.Length == new FileInfo(FilePath).Length) continue;
							}
							else
							{
								File.WriteAllBytes(FilePath, content);
							}
						}
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				//MessageBox.Show(ex.Message);
				return false;
			}
		}
	}
}
