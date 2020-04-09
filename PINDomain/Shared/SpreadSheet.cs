using PINDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PINDomain.Shared
{
    public class SpreadSheet : ISpreadSheet
    {
        readonly ICollection<SpreadSheet> spreadSheetRows = new List<SpreadSheet>();

        public int Sequence { get; private set; }
        public int Amid0_9 { get; private set; }
        public int Amid10_19 { get; private set; }
        public int Amid20_29 { get; private set; }
        public int Amid30_39 { get; private set; }
        public int Amid40_49 { get; private set; }
        public int Amid50_59 { get; private set; }

        public int? GetValueSpreadSheet(int sequence, int second)
        {
            MountSpreadSheet();
            var result = spreadSheetRows.FirstOrDefault(x => x.Sequence == sequence);
            var propertyName = GetPropertyFromSecond(second);
            var value = result.GetType().GetProperty(propertyName)?.GetValue(result);
            return value == null ? (int?)null : (int)value;
        }

        void MountSpreadSheet()
        {
            if (spreadSheetRows.Count() > 0)
                return;
            spreadSheetRows.Add(new SpreadSheet { Sequence = 1, Amid0_9 = 22, Amid10_19 = 32, Amid20_29 = 42, Amid30_39 = 52, Amid40_49 = 62, Amid50_59 = 72 });
            spreadSheetRows.Add(new SpreadSheet { Sequence = 2, Amid0_9 = 33, Amid10_19 = 43, Amid20_29 = 53, Amid30_39 = 63, Amid40_49 = 73, Amid50_59 = 83 });
            spreadSheetRows.Add(new SpreadSheet { Sequence = 3, Amid0_9 = 44, Amid10_19 = 54, Amid20_29 = 64, Amid30_39 = 74, Amid40_49 = 84, Amid50_59 = 94 });
            spreadSheetRows.Add(new SpreadSheet { Sequence = 4, Amid0_9 = 16, Amid10_19 = 26, Amid20_29 = 36, Amid30_39 = 46, Amid40_49 = 56, Amid50_59 = 66 });
            spreadSheetRows.Add(new SpreadSheet { Sequence = 5, Amid0_9 = 75, Amid10_19 = 85, Amid20_29 = 95, Amid30_39 = 105, Amid40_49 = 115, Amid50_59 = 125 });
            spreadSheetRows.Add(new SpreadSheet { Sequence = 6, Amid0_9 = 57, Amid10_19 = 67, Amid20_29 = 77, Amid30_39 = 87, Amid40_49 = 97, Amid50_59 = 107 });
            spreadSheetRows.Add(new SpreadSheet { Sequence = 7, Amid0_9 = 81, Amid10_19 = 91, Amid20_29 = 101, Amid30_39 = 111, Amid40_49 = 121, Amid50_59 = 131 });
            spreadSheetRows.Add(new SpreadSheet { Sequence = 8, Amid0_9 = 69, Amid10_19 = 79, Amid20_29 = 89, Amid30_39 = 99, Amid40_49 = 109, Amid50_59 = 119 });
            spreadSheetRows.Add(new SpreadSheet { Sequence = 9, Amid0_9 = 98, Amid10_19 = 108, Amid20_29 = 108, Amid30_39 = 128, Amid40_49 = 139, Amid50_59 = 148 });
        }

        string GetPropertyFromSecond(int second)
        {
            var propertyName = "Amid";
            if (Enumerable.Range(0, 10).Contains(second))
                propertyName += "0_9";
            else if (Enumerable.Range(10, 10).Contains(second))
                propertyName += "10_19";
            else if (Enumerable.Range(20, 10).Contains(second))
                propertyName += "20_29";
            else if (Enumerable.Range(30, 10).Contains(second))
                propertyName += "30_39";
            else if (Enumerable.Range(40, 10).Contains(second))
                propertyName += "40_49";
            else if (Enumerable.Range(50, 10).Contains(second))
                propertyName += "50_59";
            return propertyName;
        }
    }
}
