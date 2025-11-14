using System;
using System.Text;
using System.Runtime.InteropServices;

namespace SystemInfoApp
{
    class Program
    {
        // Импорт необходимых функций из Win32 API
        
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool GetComputerName(StringBuilder lpBuffer, ref uint lpnSize);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint GetWindowsDirectory(StringBuilder lpBuffer, uint uSize);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint GetSystemDirectory(StringBuilder lpBuffer, uint uSize);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint GetTempPath(uint nBufferLength, StringBuilder lpBuffer);

        [DllImport("kernel32.dll")]
        static extern uint GetVersion();

        [DllImport("kernel32.dll")]
        static extern bool GetVersionEx(ref OSVERSIONINFOEX osVersionInfo);

        // Структура для получения подробной информации о версии ОС
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct OSVERSIONINFOEX
        {
            public int dwOSVersionInfoSize;
            public uint dwMajorVersion;
            public uint dwMinorVersion;
            public uint dwBuildNumber;
            public uint dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;
            public ushort wServicePackMajor;
            public ushort wServicePackMinor;
            public ushort wSuiteMask;
            public byte wProductType;
            public byte wReserved;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("=== СИСТЕМНАЯ ИНФОРМАЦИЯ ===");
            Console.WriteLine();

            // 1. Получение имени компьютера
            GetComputerNameInfo();

            // 2. Получение путей к системным каталогам
            GetSystemDirectories();

            // 3. Получение информации о версии ОС
            GetOSVersionInfo();

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        static void GetComputerNameInfo()
        {
            uint bufferSize = 256;
            StringBuilder nameBuffer = new StringBuilder((int)bufferSize);
            
            if (GetComputerName(nameBuffer, ref bufferSize))
            {
                Console.WriteLine($"Имя компьютера: {nameBuffer.ToString()}");
            }
            else
            {
                Console.WriteLine("Ошибка при получении имени компьютера");
            }
        }

        static void GetSystemDirectories()
        {
            StringBuilder pathBuffer = new StringBuilder(256);
            
            // Путь к каталогу Windows
            uint result = GetWindowsDirectory(pathBuffer, (uint)pathBuffer.Capacity);
            if (result > 0)
            {
                Console.WriteLine($"Каталог Windows: {pathBuffer.ToString()}");
            }

            // Системный каталог
            result = GetSystemDirectory(pathBuffer, (uint)pathBuffer.Capacity);
            if (result > 0)
            {
                Console.WriteLine($"Системный каталог: {pathBuffer.ToString()}");
            }

            // Каталог временных файлов
            result = GetTempPath((uint)pathBuffer.Capacity, pathBuffer);
            if (result > 0)
            {
                Console.WriteLine($"Каталог временных файлов: {pathBuffer.ToString()}");
            }
        }

        static void GetOSVersionInfo()
        {
            // Способ 1: Использование GetVersion() (простой способ)
            uint version = GetVersion();
            uint majorVersion = version & 0xFF;
            uint minorVersion = (version >> 8) & 0xFF;
            uint buildNumber = (version >> 16) & 0xFFFF;

            Console.WriteLine($"Версия ОС (через GetVersion): {majorVersion}.{minorVersion}.{buildNumber}");

            // Способ 2: Использование GetVersionEx() (более подробная информация)
            try
            {
                OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();
                osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));
                
                if (GetVersionEx(ref osVersionInfo))
                {
                    Console.WriteLine($"Версия ОС (через GetVersionEx): {osVersionInfo.dwMajorVersion}.{osVersionInfo.dwMinorVersion}.{osVersionInfo.dwBuildNumber}");
                    if (!string.IsNullOrEmpty(osVersionInfo.szCSDVersion))
                    {
                        Console.WriteLine($"Service Pack: {osVersionInfo.szCSDVersion}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении подробной информации о версии: {ex.Message}");
            }

            // Дополнительная информация через Environment
            Console.WriteLine($"Версия .NET: {Environment.Version}");
            Console.WriteLine($"Версия ОС (Environment): {Environment.OSVersion}");
            Console.WriteLine($"64-битная ОС: {Environment.Is64BitOperatingSystem}");
        }
    }
}