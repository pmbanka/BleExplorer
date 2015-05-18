using System;
using ReactiveUI;

namespace BleExplorer.Core.Views
{
    public class NegatingConverter : IBindingTypeConverter
    {
        private static readonly Lazy<NegatingConverter> instance = new Lazy<NegatingConverter>();

        public static NegatingConverter Instance
        {
            get { return instance.Value; }
        }

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