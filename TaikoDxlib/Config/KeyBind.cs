using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DxLibDLL.DX;
using Amaoto;

namespace TaikoDxlib.TaikoDxlib.Config
{
    //シミュレーターの操作に必要なキーのバインドに関するクラス。
    internal static class KeyBind
    {
        public static int[] DON_LEFT_1P = { KEY_INPUT_F, -1, -1, -1 };
        public static int[] DON_RIGHT_1P = { KEY_INPUT_J, -1, -1, -1 };
        public static int[] KA_LEFT_1P = { KEY_INPUT_D, -1, -1, -1 };
        public static int[] KA_RIGHT_1P = { KEY_INPUT_K, -1, -1, -1 };

        public static int[] DON_LEFT_2P = { KEY_INPUT_C, -1, -1, -1 };
        public static int[] DON_RIGHT_2P = { KEY_INPUT_N, -1, -1, -1 };
        public static int[] KA_LEFT_2P = { KEY_INPUT_X, -1, -1, -1 };
        public static int[] KA_RIGHT_2P = { KEY_INPUT_M, -1, -1, -1 };

        public static bool IsPushedSystemKey(int[] skt)
        {
            bool ret = false;

            for (int i = 0; i < 4; i++)
            {
                if (skt[i] != -1 && Key.IsPushed(skt[i]) == true)
                {
                    ret = true;
                }
            }

            return ret;
        }
    }
}
