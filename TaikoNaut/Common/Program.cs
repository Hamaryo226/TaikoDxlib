using Amaoto;
using Electrhythm.RhythmGame.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaikoNaut.TaikoNaut.Config;
using TaikoNaut.TaikoNaut.Scenes.EnsoGame;
using TaikoNaut.TaikoNaut.SongSystem;
using static DxLibDLL.DX;
using static TaikoNaut.TaikoNaut.Common.Program;

namespace TaikoNaut.TaikoNaut.Common
{
    internal class Program
    {
        static void Main()
        {
            Program_Initialize();

            while(ProcessMessage() == 0)
            {
                ClearDrawScreen();

                DrawBox(0, 0, 1920, 1080, GetColor(0, 0, 0), TRUE);

                Key.Update();

                switch (nowScene)
                {
                    case NowScene.EnsoGame:
                        {
                            EnsoGameScene.Draw();
                            EnsoGameScene.Update();
                        }
                        break;
                }

                fps.Update();

                Debug.tprint(0, 0, fps.FPS.ToString());

                if (Key.IsPushed(KEY_INPUT_F12))
                {
                    DateTime dt = DateTime.Now;
                    SaveDrawScreenToPNG(0, 0, 1920, 1080, "ScreenShot\\" + dt.ToString("yyyyMMddHHmmss") + ".png");
                }

                ScreenFlip();
            }

            Program_Finalize();
        }

        static void Program_Initialize()
        {
            #region [ Initialize DxLib ]

            unsafe
            {
                SetUseASyncChangeWindowModeFunction(TRUE, null, null);
                SetChangeScreenModeGraphicsSystemResetFlag(FALSE);
            }
            SetUseTransColor(FALSE);
            SetWindowText("TaikoNaut β0.0.1");
            SetGraphMode(1920, 1080, 32);
            SetWindowSize(1280, 720);
            ChangeWindowMode(TRUE);
            SetWindowSizeChangeEnableFlag(TRUE);
            SetAlwaysRunFlag(TRUE);
            SetWaitVSyncFlag(FALSE);
            SetOutApplicationLogValidFlag(FALSE);
            SetUseTransColor(FALSE);
            SetDoubleStartValidFlag(TRUE);
            if (DxLib_Init() == -1) return;
            SetDrawScreen(DX_SCREEN_BACK);

            #endregion

            ResourceLoader.LoadTexture();
            ResourceLoader.LoadSound();

            fps = new FPSCounter();

            TJA = new TJA();

            var data = TJA.GetSongDataFromTJA("C:\\Users\\0ren5\\source\\repos\\TaikoNaut\\Build\\Songs\\poxei DOON\\poxei◆DOON.tja");

            EnsoGameScene = new EnsoGameScene();
            EnsoGameScene.NowPlayingSong = data;
            EnsoGameScene.Enable();
        }

        static void Program_Finalize()
        {
            DxLib_End();
        }

        public static FPSCounter fps;
        public static EnsoGameScene EnsoGameScene;

        public static TJA TJA;

        public static NowScene nowScene = NowScene.EnsoGame;

        public enum NowScene
        {
            StartUp,
            Title,
            SongSelect,
            SongLoading,
            EnsoGame,
            Result
        }
    }
}
