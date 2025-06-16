using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DxLibDLL.DX;

namespace Electrhythm.RhythmGame.Common
{
    internal static class Debug
    {
        public static void tprint(int x, int y, string text)
        {
            DrawStringToHandle(x, y, text, GetColor(255, 255, 255), FontHandle, GetColor(0, 0, 0));
        }

        private static int FontHandle = CreateFontToHandle("MS Gothic", 40, 0, DX_FONTTYPE_ANTIALIASING_EDGE_16X16, -1, 3);
    }
}
