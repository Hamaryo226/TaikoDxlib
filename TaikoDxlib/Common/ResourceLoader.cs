using Amaoto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaikoDxlib.TaikoDxlib.Scenes.EnsoGame;

namespace TaikoDxlib.TaikoDxlib.Common
{
    /// <summary>
    /// 必要な音声・画像のロード
    /// </summary>
    internal static class ResourceLoader
    {
        public static void LoadTexture()
        {
            // リソースの再読み込み前に既存のリソースをクリア
            ReleaseTextures();
            
            EnsoGame_Notes = new Texture(@"Skins\DefaultSkin\Image\04.EnsoGame\Notes.png");

            #region [ MiniTaiko ]

            EnsoGame_MiniTaiko_Background_1P = new Texture(@"Skins\DefaultSkin\Image\04.EnsoGame\MiniTaiko\Background_1P.png");

            EnsoGame_MiniTaiko_Taiko = new Texture(@"Skins\DefaultSkin\Image\04.EnsoGame\MiniTaiko\Taiko.png");
            EnsoGame_MiniTaiko_Taiko_Don = new Texture(@"Skins\DefaultSkin\Image\04.EnsoGame\MiniTaiko\Don.png");
            EnsoGame_MiniTaiko_Taiko_Ka = new Texture(@"Skins\DefaultSkin\Image\04.EnsoGame\MiniTaiko\Ka.png");

            #endregion

            #region [ Footer ]
            EnsoGame_Footer = new Texture(@"Skins\DefaultSkin\Image\04.EnsoGame\Footer\0.png");
            #endregion

            EnsoGame_Lane_Background_1P = new Texture(@"Skins\DefaultSkin\Image\04.EnsoGame\Lane\Background_1P.png");
        }

        /// <summary>
        /// すべてのテクスチャリソースを解放します。
        /// </summary>
        public static void ReleaseTextures()
        {
            if (EnsoGame_Notes != null)
            {
                EnsoGame_Notes.Dispose();
                EnsoGame_Notes = null;
            }
            
            if (EnsoGame_MiniTaiko_Background_1P != null)
            {
                EnsoGame_MiniTaiko_Background_1P.Dispose();
                EnsoGame_MiniTaiko_Background_1P = null;
            }
            
            if (EnsoGame_MiniTaiko_Taiko != null)
            {
                EnsoGame_MiniTaiko_Taiko.Dispose();
                EnsoGame_MiniTaiko_Taiko = null;
            }
            
            if (EnsoGame_MiniTaiko_Taiko_Don != null)
            {
                EnsoGame_MiniTaiko_Taiko_Don.Dispose();
                EnsoGame_MiniTaiko_Taiko_Don = null;
            }
            
            if (EnsoGame_MiniTaiko_Taiko_Ka != null)
            {
                EnsoGame_MiniTaiko_Taiko_Ka.Dispose();
                EnsoGame_MiniTaiko_Taiko_Ka = null;
            }
            
            if (EnsoGame_Footer != null)
            {
                EnsoGame_Footer.Dispose();
                EnsoGame_Footer = null;
            }
            
            if (EnsoGame_Lane_Background_1P != null)
            {
                EnsoGame_Lane_Background_1P.Dispose();
                EnsoGame_Lane_Background_1P = null;
            }
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
            // リソースの再読み込み前に既存のリソースをクリア
            ReleaseSounds();
            
            sndDon = new Sound(@"Skins\DefaultSkin\Sound\Taiko\0.Taiko\don.ogg");
            sndKa = new Sound(@"Skins\DefaultSkin\Sound\Taiko\0.Taiko\ka.ogg");
        }
        
        /// <summary>
        /// すべての音声リソースを解放します。
        /// </summary>
        public static void ReleaseSounds()
        {
            if (sndDon != null)
            {
                if (sndDon.IsPlaying)
                {
                    sndDon.Stop();
                }
                sndDon.Dispose();
                sndDon = null;
            }
            
            if (sndKa != null)
            {
                if (sndKa.IsPlaying)
                {
                    sndKa.Stop();
                }
                sndKa.Dispose();
                sndKa = null;
            }
        }
        
        /// <summary>
        /// すべてのリソースを解放します。
        /// </summary>
        public static void ReleaseAllResources()
        {
            ReleaseTextures();
            ReleaseSounds();
        }

        public static Sound sndDon;
        public static Sound sndKa;
    }
}
