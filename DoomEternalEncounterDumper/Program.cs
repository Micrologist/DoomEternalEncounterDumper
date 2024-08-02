using MemUtil;
using System.Diagnostics;

Process? proc = Process.GetProcesses().ToList().Find(x => x.ProcessName == "DOOMEternalx64vk");
if (proc == null)
{
	Console.WriteLine("Game process not found...");
	Console.ReadKey();
	return;
}

var scn = new SignatureScanner(proc, proc.MainModule.BaseAddress, proc.MainModule.ModuleMemorySize);
var encounterListPtr = new DeepPointer(0x6B07CB0 + 0xB0, 0x2C8, 0x8, 0x28, 0x0).Deref<IntPtr>(proc);

if (encounterListPtr == IntPtr.Zero)
{
	Console.WriteLine("Encounter list not found...");
	Console.ReadKey();
	return;
}


var listMaxSize = proc.ReadValue<int>(encounterListPtr - 0x8);

Console.WriteLine($"{"Address",-18} {"Name",-64} active");
Console.WriteLine("------------------------------------------------------------------------------------------");
for (int i = 1; i < listMaxSize / 0x8; i++)
{
	var encounter = proc.ReadValue<IntPtr>(encounterListPtr + (i * 0x8));
	var encounterNamePtr = proc.ReadValue<IntPtr>(encounter + 0x48);
	if ((string?)proc.ReadString(encounterNamePtr, 64) != "player1")
	{
		var encounterActive = proc.ReadValue<bool>(encounter + 0x640);
		Console.WriteLine($"[{encounter:X16}] {proc.ReadString(proc.ReadValue<IntPtr>(encounter + 0x48), 255),-64} {encounterActive,-5}");
	}
	else
	{
		break;
	}
}

Console.ReadKey();
return;