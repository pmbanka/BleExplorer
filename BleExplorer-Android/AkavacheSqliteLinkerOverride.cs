using System;
using Akavache.Sqlite3;

// Note: This class file is *required* for iOS to work correctly, and is 
// also a good idea for Android if you enable "Link All Assemblies".
namespace BleExplorer
{
    [Preserve]
    public static class LinkerPreserve
    {
        static LinkerPreserve()
        {
            throw new InvalidOperationException(typeof(SQLitePersistentBlobCache).FullName);
        }
    }


    public class PreserveAttribute : Attribute
    {
    }
}
