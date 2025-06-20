using Amaoto;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaikoDxlib.TaikoDxlib.Common;
using TaikoDxlib.TaikoDxlib.SongSystem;
using static DxLibDLL.DX;

namespace TaikoDxlib.TaikoDxlib.Scenes.SongLoading
{
    /// <summary>
    /// 曲読み込み中シーン
    /// </summary>
    internal class SongLoadingScene : Scene, IDisposable
    {
        public override void Enable()
        {
            // 初期化
            loadingCounter = new Counter(0, 3000, 1000, false); // 3秒間の表示
            loadingCounter.Start();
            
            // テクスチャのクリア
            CleanupTextures();
            
            // フォントレンダラーの初期化
            titleFontRenderer = new FontRender(new FontFamily("MS Gothic"), 48, 2, FontStyle.Bold);
            subtitleFontRenderer = new FontRender(new FontFamily("MS Gothic"), 28, 2, FontStyle.Regular);
            
            // タイトルとサブタイトルのテクスチャを生成
            titleFontRenderer.ForeColor = Color.White;
            subtitleFontRenderer.ForeColor = Color.LightGray;
            
            titleTexture = titleFontRenderer.GetTexture(currentSong?.Header?.Title ?? "タイトル不明");
            
            if (!string.IsNullOrEmpty(currentSong?.Header?.SubTitle))
            {
                subtitleTexture = subtitleFontRenderer.GetTexture(currentSong.Header.SubTitle);
            }
            
            base.Enable();
        }

        public override void Disable()
        {
            CleanupTextures();
            base.Disable();
        }

        public void Dispose()
        {
            CleanupTextures();
            titleFontRenderer = null;
            subtitleFontRenderer = null;
            currentSong = null;
            loadingCounter = null;
        }

        private void CleanupTextures()
        {
            // テクスチャのクリーンアップ
            if (titleTexture != null)
            {
                titleTexture.Dispose();
                titleTexture = null;
            }
            
            if (subtitleTexture != null)
            {
                subtitleTexture.Dispose();
                subtitleTexture = null;
            }
            
            // 一時的なテクスチャを格納するためのディクショナリ
            if (tempTextures != null)
            {
                foreach (var texture in tempTextures.Values)
                {
                    texture?.Dispose();
                }
                tempTextures.Clear();
            }
            else
            {
                tempTextures = new Dictionary<string, Texture>();
            }
        }

        public override void Draw()
        {
            // 背景色
            DrawBox(0, 0, 1920, 1080, GetColor(0, 0, 30), TRUE);
            
            // タイトルを中央に表示
            if (titleTexture != null)
            {
                int titleX = (1920 - titleTexture.TextureSize.Width) / 2;
                int titleY = 400;
                titleTexture.Draw(titleX, titleY);
            }
            
            // サブタイトルを表示
            if (subtitleTexture != null)
            {
                int subtitleX = (1920 - subtitleTexture.TextureSize.Width) / 2;
                int subtitleY = 470;
                subtitleTexture.Draw(subtitleX, subtitleY);
            }
            
            // 難易度表示
            DrawDifficulty();
            
            // 進行状況バーの描画
            int progressWidth = (int)(1920 * 0.6);  // 画面幅の60%
            int progressX = (1920 - progressWidth) / 2;
            int progressY = 600;
            int progressHeight = 20;
            
            // 外枠
            DrawBox(progressX - 2, progressY - 2, progressX + progressWidth + 2, progressY + progressHeight + 2, GetColor(150, 150, 150), TRUE);
            
            // 内側の進行状況
            double progress = loadingCounter.Value / (double)loadingCounter.End;
            int filledWidth = (int)(progressWidth * progress);
            DrawBox(progressX, progressY, progressX + filledWidth, progressY + progressHeight, GetColor(255, 100, 100), TRUE);
            
            // 読み込み中テキスト
            subtitleFontRenderer.ForeColor = Color.White;
            
            // 既存のテクスチャがあればそれを再利用し、なければ新規に作成
            string loadingKey = "loading_text";
            Texture loadingText;
            
            if (!tempTextures.TryGetValue(loadingKey, out loadingText) || loadingText == null)
            {
                loadingText = subtitleFontRenderer.GetTexture("読み込み中...");
                tempTextures[loadingKey] = loadingText;
            }
            
            loadingText.Draw(progressX, progressY + 40);
            
            base.Draw();
        }

        public override void Update()
        {
            // カウンターの更新
            loadingCounter.Tick();
            
            // 3秒経過したら演奏シーンに切り替え
            if (loadingCounter.Value >= loadingCounter.End)
            {
                MoveToEnsoGameScene();
            }
            
            base.Update();
        }
        
        /// <summary>
        /// 演奏シーンに移行する
        /// </summary>
        private void MoveToEnsoGameScene()
        {
            // 演奏シーンの初期化
            if (currentSong != null)
            {
                Program.EnsoGameScene.NowPlayingSong = currentSong;
                Program.EnsoGameScene.NowPlayingCourse = currentDifficulty;
                Program.EnsoGameScene.Enable();
                
                // シーン切り替え
                Program.nowScene = Program.NowScene.EnsoGame;
            }
            else
            {
                // 曲データがない場合は曲選択画面に戻る
                Program.nowScene = Program.NowScene.SongSelect;
            }
        }
        
        /// <summary>
        /// 曲データと難易度をセットする
        /// </summary>
        public void SetSongData(SongData.Song song, int difficulty)
        {
            currentSong = song;
            currentDifficulty = difficulty;
        }
        
        /// <summary>
        /// 難易度表示を描画する
        /// </summary>
        private void DrawDifficulty()
        {
            // 難易度に対応する色とテキスト
            Color diffColor;
            string diffName;
            
            switch (currentDifficulty)
            {
                case 0:
                    diffColor = Color.LightGreen;
                    diffName = "かんたん";
                    break;
                case 1:
                    diffColor = Color.LightBlue;
                    diffName = "ふつう";
                    break;
                case 2:
                    diffColor = Color.Orange;
                    diffName = "むずかしい";
                    break;
                case 3:
                    diffColor = Color.Red;
                    diffName = "おに";
                    break;
                case 4:
                    diffColor = Color.Purple;
                    diffName = "うら";
                    break;
                default:
                    diffColor = Color.White;
                    diffName = "不明";
                    break;
            }
            
            // 難易度レベル
            int level = 0;
            if (currentSong?.SongCourses != null && 
                currentDifficulty >= 0 && 
                currentDifficulty < currentSong.SongCourses.Length && 
                currentSong.SongCourses[currentDifficulty] != null)
            {
                level = currentSong.SongCourses[currentDifficulty].Level;
            }
            
            // 難易度表示
            subtitleFontRenderer.ForeColor = diffColor;
            
            // 難易度テキスト用のキー
            string diffKey = $"diff_{diffName}_{level}";
            Texture diffText;
            
            if (!tempTextures.TryGetValue(diffKey, out diffText) || diffText == null)
            {
                diffText = subtitleFontRenderer.GetTexture($"難易度: {diffName} ★{level}");
                tempTextures[diffKey] = diffText;
            }
            
            int diffX = (1920 - diffText.TextureSize.Width) / 2;
            diffText.Draw(diffX, 530);
        }
        
        private SongData.Song currentSong;
        private int currentDifficulty;
        private Counter loadingCounter;
        private FontRender titleFontRenderer;
        private FontRender subtitleFontRenderer;
        private Texture titleTexture;
        private Texture subtitleTexture;
        private Dictionary<string, Texture> tempTextures = new Dictionary<string, Texture>();
    }
}
