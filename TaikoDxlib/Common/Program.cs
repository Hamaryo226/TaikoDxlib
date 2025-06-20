using Amaoto;
using Electrhythm.RhythmGame.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaikoDxlib.TaikoDxlib.Config;
using TaikoDxlib.TaikoDxlib.Scenes.EnsoGame;
using TaikoDxlib.TaikoDxlib.SongSystem;
using static DxLibDLL.DX;
using static TaikoDxlib.TaikoDxlib.Common.Program;
using TaikoDxlib.TaikoDxlib.Scenes.SongSelect;
using TaikoDxlib.TaikoDxlib.Scenes.SongLoading;

namespace TaikoDxlib.TaikoDxlib.Common
{
    internal class Program
    {
        static void Main()
        {
            Program_Initialize();

            while (ProcessMessage() == 0)
            {
                ClearDrawScreen();

                DrawBox(0, 0, 1920, 1080, GetColor(0, 0, 0), TRUE);

                Key.Update();

                switch (nowScene)
                {
                    case NowScene.SongSelect:
                        {
                            SongSelectScene.Draw();
                            SongSelectScene.Update();
                        }
                        break;
                    case NowScene.SongLoading:
                        {
                            SongLoadingScene.Draw();
                            SongLoadingScene.Update();
                        }
                        break;
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
                    string screenshotDir = "ScreenShot";

                    // スクリーンショットディレクトリがなければ作成
                    if (!Directory.Exists(screenshotDir))
                    {
                        Directory.CreateDirectory(screenshotDir);
                    }

                    SaveDrawScreenToPNG(0, 0, 1920, 1080, Path.Combine(screenshotDir, dt.ToString("yyyyMMddHHmmss") + ".png"));
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
            SetWindowText("TaikoDxlib β0.0.1");
            SetGraphMode(1920, 1080, 32);
            SetWindowSize(1920, 1080);
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

            // 必要なリソースのロード
            ResourceLoader.LoadTexture();
            ResourceLoader.LoadSound();

            // FPSカウンターの初期化
            fps = new FPSCounter();

            // TJAパーサーの初期化
            TJA = new TJA();

            // シーンの初期化
            SongLoadingScene = new SongLoadingScene();
            SongSelectScene = new SongSelectScene();
            EnsoGameScene = new EnsoGameScene();

            // 最初のシーンをセット
            SongSelectScene.Enable();

            // 曲選択シーンから始める
            nowScene = NowScene.SongSelect;
        }

        static void Program_Finalize()
        {
            // シーンのクリーンアップ
            if (SongSelectScene != null) SongSelectScene.Disable();
            if (SongLoadingScene != null) SongLoadingScene.Disable();
            if (EnsoGameScene != null) EnsoGameScene.Disable();

            DxLib_End();
        }

        public static FPSCounter fps;
        public static SongLoadingScene SongLoadingScene;
        public static SongSelectScene SongSelectScene;
        public static EnsoGameScene EnsoGameScene;

        public static TJA TJA;

        public static NowScene nowScene = NowScene.SongSelect;

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
