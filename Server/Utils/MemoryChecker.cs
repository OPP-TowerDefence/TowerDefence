using TowerDefense.Models.Enemies;
using TowerDefense.Models.TileEffects;

namespace TowerDefense.Utils
{
    public class MemoryChecker
{
    // public static void CompareShallowAndDeepCopy()
    // {
    //     FastEnemy originalEnemy = new FastEnemy(5, 10);
    //     originalEnemy.ScheduleEffect(new HealEffect(5), 5);
    //     Console.WriteLine($"Original Enemy Address: {GetMemoryAddress(originalEnemy)}");
    //     Console.WriteLine($"Original Enemy Scheduled Effects Address: {GetMemoryAddress(originalEnemy._scheduledEffects)}");

    //     // Create Shallow Copy
    //     FastEnemy shallowCopyEnemy = originalEnemy.ShallowClone();
    //     Console.WriteLine($"Shallow Copy Enemy Address: {GetMemoryAddress(shallowCopyEnemy)}");
    //     Console.WriteLine($"Shallow Copy Scheduled Effects Address: {GetMemoryAddress(shallowCopyEnemy._scheduledEffects)}");
    //     Console.WriteLine("Original and Shallow Copy have the same Scheduled Effects reference? " + Object.ReferenceEquals(originalEnemy._scheduledEffects, shallowCopyEnemy._scheduledEffects));

    //     // Create Deep Copy
    //     FastEnemy deepCopyEnemy = originalEnemy.DeepClone();
    //     Console.WriteLine($"Deep Copy Enemy Address: {GetMemoryAddress(deepCopyEnemy)}");
    //     Console.WriteLine($"Deep Copy Scheduled Effects Address: {GetMemoryAddress(deepCopyEnemy._scheduledEffects)}");
    //     Console.WriteLine("Original and Deep Copy have the same Scheduled Effects reference? " + Object.ReferenceEquals(originalEnemy._scheduledEffects, deepCopyEnemy._scheduledEffects));
    // }
    // private static string GetMemoryAddress(object obj)
    // {
    //     unsafe
    //     {
    //         TypedReference tr = __makeref(obj);
    //         IntPtr ptr = **(IntPtr**)(&tr);
    //         return ptr.ToString("X");
    //     }
    // }
}
}