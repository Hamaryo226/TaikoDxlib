using Amaoto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaikoNaut.TaikoNaut.Common
{
    //必要な音声・画像のロード。
    internal static class ResourceLoader
    {
        public static void LoadTexture()
        {
            EnsoGame_Notes = new Texture(@"Skins\DefaultSkin\Image\04.EnsoGame\Notes.png");

            EnsoGame_MiniTaiko_Background_1P = new Texture(@"Skins\DefaultSkin\Image\04.EnsoGame\MiniTaiko\Background_1P.png");

            EnsoGame_Lane_Background_1P = new Texture(@"Skins\DefaultSkin\Image\04.EnsoGame\Lane\Background_1P.png");
        }

        public static Texture EnsoGame_Notes;
        public static Texture EnsoGame_MiniTaiko_Background_1P;
        public static Texture EnsoGame_Lane_Background_1P;

        public static void LoadSound()
        {
            sndDon = new Sound(@"Skins\DefaultSkin\Sound\Taiko\0.Taiko\don.ogg");
            sndKa = new Sound(@"Skins\DefaultSkin\Sound\Taiko\0.Taiko\ka.ogg");
        }

        public static Sound sndDon;
        public static Sound sndKa;
    }
}
