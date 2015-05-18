using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace BleExplorer.Core.Utils
{
    public static class Ensure
    {
        [NotNull]
        public static T NotNull<T>([CanBeNull] [NoEnumeration] T argument, [InvokerParameterName] string argumentName)
        {
            if (argument == null) throw new ArgumentNullException(argumentName);
            return argument;
        }
    }
}