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

namespace TaikoDxlib.TaikoDxlib.Scenes.SongSelect
{
    /// <summary>
    /// 曲選択シーン
    /// </summary>
    internal class SongSelectScene : Scene, IDisposable
    {
        // 曲情報を格納するクラス
        private class SongEntry
        {
            public string FilePath { get; set; }
            public string Title { get; set; }
            public string Subtitle { get; set; }
            public bool[] AvailableDifficulties { get; set; } // かんたん、ふつう、むずかしい、おに、うら
        }

        // 曲リスト
        private List<SongEntry> songList;

        // 現在選択中の曲インデックス
        private int currentSongIndex;

        // フォントレンダラー
        private FontRender titleFontRenderer;
        private FontRender subtitleFontRenderer;
        private FontRender messageFontRenderer;

        // テクスチャキャッシュ
        private Dictionary<string, Texture> textureCache = new Dictionary<string, Texture>();

        public SongSelectScene()
        {
            songList = new List<SongEntry>();
            currentSongIndex = 0;
        }

        public override void Enable()
        {
            // 初期化
            songList.Clear();
            textureCache.Clear();
            currentSongIndex = 0;

            // フォントレンダラーの初期化
            titleFontRenderer = new FontRender(new FontFamily("MS Gothic"), 36, 2, FontStyle.Bold);
            subtitleFontRenderer = new FontRender(new FontFamily("MS Gothic"), 24, 2, FontStyle.Regular);
            messageFontRenderer = new FontRender(new FontFamily("MS Gothic"), 32, 2, FontStyle.Bold);

            // 曲リストの読み込み
            LoadSongList();

            base.Enable();
        }

        public override void Disable()
        {
            // リソースの解放
            DisposeCachedTextures();
            songList.Clear();

            base.Disable();
        }

        public void Dispose()
        {
            DisposeCachedTextures();
            songList?.Clear();
            songList = null;
            textureCache = null;
            titleFontRenderer = null;
            subtitleFontRenderer = null;
            messageFontRenderer = null;
        }

        private void DisposeCachedTextures()
        {
            if (textureCache != null)
            {
                foreach (var texture in textureCache.Values)
                {
                    texture?.Dispose();
                }
                textureCache.Clear();
            }
        }

        public override void Draw()
        {
            // 背景色
            DrawBox(0, 0, 1920, 1080, GetColor(20, 20, 50), TRUE);

            // ヘッダータイトル
            titleFontRenderer.ForeColor = Color.White;
            Texture headerTexture = GetCachedTexture("header", () => titleFontRenderer.GetTexture("曲選択"));
            headerTexture.Draw(50, 50);

            if (songList.Count == 0)
            {
                // 曲が見つからない場合のメッセージ
                messageFontRenderer.ForeColor = Color.White;
                Texture noSongsTexture = GetCachedTexture("noSongs", () => messageFontRenderer.GetTexture("曲が見つかりません。Songs/ディレクトリに.tjaファイルを配置してください。"));
                noSongsTexture.Draw((1920 - noSongsTexture.TextureSize.Width) / 2, 400);
                return;
            }

            // 曲一覧の表示
            int startY = 150;
            int itemHeight = 60;

            for (int i = 0; i < songList.Count; i++)
            {
                SongEntry song = songList[i];
                int y = startY + i * itemHeight;

                // 選択中の曲はハイライト表示
                if (i == currentSongIndex)
                {
                    DrawBox(40, y - 5, 1880, y + itemHeight - 5, GetColor(60, 60, 100), TRUE);
                    titleFontRenderer.ForeColor = Color.Yellow;
                }
                else
                {
                    titleFontRenderer.ForeColor = Color.White;
                }

                // 曲タイトル
                Texture songTitleTexture = GetCachedTexture($"title_{i}", () => titleFontRenderer.GetTexture(song.Title));
                songTitleTexture.Draw(60, y);

                // サブタイトルがあれば表示
                if (!string.IsNullOrEmpty(song.Subtitle))
                {
                    subtitleFontRenderer.ForeColor = Color.LightGray;
                    Texture subtitleTexture = GetCachedTexture($"subtitle_{i}", () => subtitleFontRenderer.GetTexture(song.Subtitle));
                    subtitleTexture.Draw(60, y + 38);
                }
            }

            // 操作方法の表示
            subtitleFontRenderer.ForeColor = Color.White;
            Texture instructionTexture = GetCachedTexture("instruction", () => subtitleFontRenderer.GetTexture("↑↓: 選択  Enter: 決定"));
            instructionTexture.Draw(60, 1000);

            base.Draw();
        }

        private Texture GetCachedTexture(string key, Func<Texture> textureFactory)
        {
            if (!textureCache.TryGetValue(key, out Texture texture) || texture == null)
            {
                texture = textureFactory();
                textureCache[key] = texture;
            }
            return texture;
        }

        public override void Update()
        {
            if (songList.Count == 0)
                return;

            // 上下キーで曲選択
            if (Key.IsPushed(KEY_INPUT_UP))
            {
                currentSongIndex = (currentSongIndex > 0) ? currentSongIndex - 1 : songList.Count - 1;
            }
            else if (Key.IsPushed(KEY_INPUT_DOWN))
            {
                currentSongIndex = (currentSongIndex < songList.Count - 1) ? currentSongIndex + 1 : 0;
            }

            // エンターキーで曲決定
            if (Key.IsPushed(KEY_INPUT_RETURN))
            {
                SelectSong(currentSongIndex);
            }

            base.Update();
        }

        /// <summary>
        /// Songs/ディレクトリから.tjaファイルを読み込む
        /// </summary>
        private void LoadSongList()
        {
            // 実行ファイルのディレクトリを取得
            string baseDirectory = Directory.GetCurrentDirectory();

            // Songs ディレクトリのパス
            string songsDirectory = Path.Combine(baseDirectory, "Songs");

            try
            {
                // ディレクトリが存在しなければ作成
                if (!Directory.Exists(songsDirectory))
                {
                    Directory.CreateDirectory(songsDirectory);
                    return;
                }

                // サブディレクトリを取得
                string[] subDirectories = Directory.GetDirectories(songsDirectory);

                // 各サブディレクトリ内の.tjaファイルを検索
                foreach (string subDir in subDirectories)
                {
                    string[] tjaFiles = Directory.GetFiles(subDir, "*.tja");

                    foreach (string tjaFile in tjaFiles)
                    {
                        try
                        {
                            // TJAファイルの基本情報を解析
                            SongEntry entry = ParseTjaBasicInfo(tjaFile);
                            if (entry != null)
                            {
                                songList.Add(entry);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"TJAファイルの読み込みに失敗しました: {tjaFile}, {ex.Message}");
                        }
                    }
                }

                // TJAファイルがサブディレクトリではなくディレクトリ直下にある場合も検索
                string[] rootTjaFiles = Directory.GetFiles(songsDirectory, "*.tja");
                foreach (string tjaFile in rootTjaFiles)
                {
                    try
                    {
                        SongEntry entry = ParseTjaBasicInfo(tjaFile);
                        if (entry != null)
                        {
                            songList.Add(entry);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"TJAファイルの読み込みに失敗しました: {tjaFile}, {ex.Message}");
                    }
                }

                // タイトルでソート
                songList = songList.OrderBy(s => s.Title).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"曲リストの読み込みに失敗しました: {ex.Message}");
            }
        }

        /// <summary>
        /// TJAファイルから基本情報を取得する
        /// </summary>
        private SongEntry ParseTjaBasicInfo(string filePath)
        {
            SongEntry entry = new SongEntry
            {
                FilePath = filePath,
                Title = Path.GetFileNameWithoutExtension(filePath),
                AvailableDifficulties = new bool[5] { false, false, false, false, false } // 5種類の難易度（かんたん、ふつう、むずかしい、おに、うら）
            };

            try
            {
                // ファイルの内容を読み取り
                using (StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding("Shift-JIS")))
                {
                    string line;
                    int currentCourse = -1;

                    while ((line = sr.ReadLine()) != null)
                    {
                        // タイトル情報
                        if (line.StartsWith("TITLE:"))
                        {
                            entry.Title = line.Substring(6).Trim();
                        }
                        // サブタイトル情報
                        else if (line.StartsWith("SUBTITLE:"))
                        {
                            var subtitle = line.Substring(9).Trim();
                            // -- や ++ から始まるサブタイトルは装飾を除去
                            if (subtitle.StartsWith("--") || subtitle.StartsWith("++"))
                            {
                                subtitle = subtitle.Remove(0, 2);
                            }
                            entry.Subtitle = subtitle;
                        }
                        // コース（難易度）情報
                        else if (line.StartsWith("COURSE:"))
                        {
                            string courseStr = line.Substring(7).Trim();
                            currentCourse = Program.TJA.GetCourseFromData(courseStr);

                            if (currentCourse >= 0 && currentCourse < 5) // 0:かんたん, 1:ふつう, 2:むずかしい, 3:おに, 4:うら
                            {
                                entry.AvailableDifficulties[currentCourse] = true;
                            }
                        }
                    }
                }

                return entry;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TJAファイル解析エラー: {filePath}, {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 曲を選択し、難易度選択画面に移行する
        /// </summary>
        private void SelectSong(int index)
        {
            if (index < 0 || index >= songList.Count)
                return;

            SongEntry selectedSong = songList[index];

            try
            {
                // TJAから曲データを読み込む
                SongData.Song songData = Program.TJA.GetSongDataFromTJA(selectedSong.FilePath);

                // 難易度インデックス 3 (おに) またはデフォルト難易度を選択
                int difficultyIndex = 3; // おに難易度をデフォルトに

                // おに難易度がない場合は、利用可能な最も高い難易度を選択
                if (songData.SongCourses[difficultyIndex] == null)
                {
                    for (int i = 4; i >= 0; i--)
                    {
                        if (songData.SongCourses[i] != null)
                        {
                            difficultyIndex = i;
                            break;
                        }
                    }
                }

                // 読み込み画面に遷移
                Program.SongLoadingScene.SetSongData(songData, difficultyIndex);
                Program.SongLoadingScene.Enable();
                Program.nowScene = Program.NowScene.SongLoading;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"曲選択エラー: {ex.Message}");
            }
        }
    }
}
