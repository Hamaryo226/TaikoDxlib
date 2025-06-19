using Amaoto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaikoDxlib.TaikoDxlib.Scenes.EnsoGame;

namespace TaikoDxlib.TaikoDxlib.Common
{
    //必要な音声・画像のロード。
    internal static class ResourceLoader
    {
        public static void LoadTexture()
        {
            EnsoGame_Notes = new Texture(@"Skins\DefaultSkin\Image\04.EnsoGame\Notes.png");

            #region [ MiniTaiko ]

            EnsoGame_MiniTaiko_Background_1P = new Texture(@"Skins\DefaultSkin\Image\04.EnsoGame\MiniTaiko\Background_1P.png");

            EnsoGame_MiniTaiko_Taiko = new Texture(@"Skins\DefaultSkin\Image\04.EnsoGame\MiniTaiko\Taiko.png");
            EnsoGame_MiniTaiko_Taiko_Don = new Texture(@"Skins\DefaultSkin\Image\04.EnsoGame\MiniTaiko\Don.png");
            EnsoGame_MiniTaiko_Taiko_Ka = new Texture(@"Skins\DefaultSkin\Image\04.EnsoGame\MiniTaiko\Ka.png");

            #endregion]

            #region [ Footer ]
            EnsoGame_Footer = new Texture(@"Skins\DefaultSkin\Image\04.EnsoGame\Footer\0.png");
            #endregion

            EnsoGame_Lane_Background_1P = new Texture(@"Skins\DefaultSkin\Image\04.EnsoGame\Lane\Background_1P.png");
        }

        #region [ Public ]

        public static Texture EnsoGame_Notes;

        #region [ MiniTaiko ]

        public static Texture EnsoGame_MiniTaiko_Background_1P;
        public static Texture EnsoGame_MiniTaiko_Taiko;
        public static Texture EnsoGame_MiniTaiko_Taiko_Don;
        public static Texture EnsoGame_MiniTaiko_Taiko_Ka;

        #endregion

        #region [ Footer ]
        public static Texture EnsoGame_Footer;
        #endregion

        public static Texture EnsoGame_Lane_Background_1P;

        #endregion

        public static void LoadSound()
        {
            sndDon = new Sound(@"Skins\DefaultSkin\Sound\Taiko\0.Taiko\don.ogg");
            sndKa = new Sound(@"Skins\DefaultSkin\Sound\Taiko\0.Taiko\ka.ogg");
        }

        public static Sound sndDon;
        public static Sound sndKa;
    }
}
