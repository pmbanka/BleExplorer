using System;
using ReactiveUI;

namespace BleExplorer.Core.Views
{
    public class NegatingConverter : IBindingTypeConverter
    {
        internal static Lazy<NegatingConverter> Instance = new Lazy<NegatingConverter>();

        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            if (fromType == typeof(bool) && toType == typeof(bool))
            {
                return 1;
            }
            return -1;
        }

        public bool TryConvert(object @from, Type toType, object conversionHint, out object result)
        {
            result = null;
            if (@from is bool && toType == typeof(bool))
            {
                result = !(bool) @from;
                return true;
            }
            return false;
        }
    }
}