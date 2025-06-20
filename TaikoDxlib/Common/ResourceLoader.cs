using Amaoto;
using DxLibDLL;
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
            
            // Enable alpha channel support before loading textures
            DX.SetUsePremulAlphaConvertLoad(DX.TRUE);
            
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

            #region [ Lane ]
            EnsoGame_Lane_Background_1P = new Texture(@"Skins\DefaultSkin\Image\04.EnsoGame\Lane\Background_1P.png");
            
            // Load hit effect textures with special care for transparency
            EnsoGame_Lane_Don = LoadTextureWithAlpha(@"Skins\DefaultSkin\Image\04.EnsoGame\Lane\Red.png");
            EnsoGame_Lane_Ka = LoadTextureWithAlpha(@"Skins\DefaultSkin\Image\04.EnsoGame\Lane\Blue.png");
            EnsoGame_Lane_Go = LoadTextureWithAlpha(@"Skins\DefaultSkin\Image\04.EnsoGame\Lane\GoGo.png");
            #endregion
        }
        
        /// <summary>
        /// テクスチャをアルファチャンネル付きで読み込みます
        /// </summary>
        private static Texture LoadTextureWithAlpha(string filePath)
        {
            // Save the current state
            int prevFlag1 = DX.GetDrawValidGraphCreateFlag();
            int prevFlag2 = DX.GetDrawValidAlphaChannelGraphCreateFlag();
            
            // Enable alpha channel for loading
            DX.SetDrawValidGraphCreateFlag(DX.TRUE);
            DX.SetDrawValidAlphaChannelGraphCreateFlag(DX.TRUE);
            
            // Load the texture
            Texture tex = new Texture(filePath);
            
            // Restore previous state
            DX.SetDrawValidGraphCreateFlag(prevFlag1);
            DX.SetDrawValidAlphaChannelGraphCreateFlag(prevFlag2);
            
            return tex;
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

            if (EnsoGame_Lane_Don != null)
            {
                EnsoGame_Lane_Don.Dispose();
                EnsoGame_Lane_Don = null;
            }

            if (EnsoGame_Lane_Ka != null)
            {
                EnsoGame_Lane_Ka.Dispose();
                EnsoGame_Lane_Ka = null;
            }

            if (EnsoGame_Lane_Go != null)
            {
                EnsoGame_Lane_Go.Dispose();
                EnsoGame_Lane_Go = null;
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

        #region [ Lane ]
        public static Texture EnsoGame_Lane_Background_1P;
        public static Texture EnsoGame_Lane_Don;
        public static Texture EnsoGame_Lane_Ka;
        public static Texture EnsoGame_Lane_Go;
        #endregion

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
