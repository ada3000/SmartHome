using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using SH.TelemetrySource;

namespace SH.Tester
{
	/*
	 itemNode:
{
	type:"cpu|mem|hdd|info",
	subtype:null, //for cpu: now, middle1min, middle5min
	name: null, //string value
	valueMin:0,
	valueMax: 100,
	value: 12.2,
	date: "2015-04-05T18:06:45Z",
	warningMinValue: 90,
}
	 message format: [itemNode, ...]
	 */
	class Program
	{
		static CPUUsage _cpu = new CPUUsage();
		static MemUsage _mem = new MemUsage();
		static MachineInfo _machineInfo = new MachineInfo();

		static void Main(string[] args)
		{
			Console.WriteLine(_machineInfo.SystemManufacturer + " " + _machineInfo.SystemModel);
			Console.WriteLine(_machineInfo.DNSHostName + " " + _machineInfo.Caption);
			Console.WriteLine(_machineInfo.PrimaryOwnerName);
			Console.WriteLine(_machineInfo.TotalPhysicalMemory);
			Console.WriteLine(_machineInfo.CurrentTimeZone);

			Console.ReadKey();
			while (true)
			{
				//Console.WriteLine(_cpu.Current + "% " + _mem.AvaibleMB + "MB " + (_mem.TotalMB - _mem.AvaibleMB) + "MB " + _mem.TotalMB + "MB");
				//Console.WriteLine(_cpu.Current + "% Ave1Min=" + _cpu.Average1Min + "% Ave5Min=" + _cpu.Average5Min + "% ");
				//Console.WriteLine(_cpu.Current + "% " + _mem.AvaibleMB + "MB " + (_mem.TotalMB-_mem.AvaibleMB) + "MB " + _mem.TotalMB + "MB");
				Thread.Sleep(1000);
			}
		}
	}
}
