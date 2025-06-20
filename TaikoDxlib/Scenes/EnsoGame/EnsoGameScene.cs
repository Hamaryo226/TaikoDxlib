using Amaoto;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaikoDxlib.TaikoDxlib.Common;
using TaikoDxlib.TaikoDxlib.Config;
using TaikoDxlib.TaikoDxlib.SongSystem;

namespace TaikoDxlib.TaikoDxlib.Scenes.EnsoGame
{
    internal class EnsoGameScene : Scene, IDisposable
    {
        public SongData.Song NowPlayingSong;
        public Sound PlaySound;
        public int NowPlayingCourse = 3;

        public override void Enable()
        {
            // リソースのクリーンアップ
            CleanupResources();

            ctGameCounter = new Counter(-3000, 100000000000, 1000, false);
            ctGameCounter.Start();

            ctJudgeDisplayCounter = new Counter(0, 500, 1000, false);

            string TJADirectory = NowPlayingSong.Header.Path.Replace(Path.GetFileName(NowPlayingSong.Header.Path), "\\");
            PlaySound = new Sound(TJADirectory + NowPlayingSong.Header.Wave);

            AddChildScene(Lane = new EnsoGame_Lane());
            AddChildScene(Gauge = new EnsoGame_Gauge());
            AddChildScene(MiniTaiko = new EnsoGame_MiniTaiko());
            AddChildScene(Footer = new EnsoGame_Footer());

            // Initialize FontRender for displaying judgment results
            judgeFontRender = new FontRender(new FontFamily("MS Gothic"), 28, 2, FontStyle.Bold);

            // Initialize texture cache
            textureCache = new Dictionary<string, Texture>();

            isMusicPlayed = false;
            currentCombo = 0;
            currentJudgment = Judgment.None;

            base.Enable();
        }

        public override void Disable()
        {
            // Stop playing sound
            if (PlaySound != null && PlaySound.IsPlaying)
            {
                PlaySound.Stop();
            }

            CleanupResources();

            base.Disable();
        }

        public void Dispose()
        {
            CleanupResources();

            // Clear child scenes explicitly
            Lane = null;
            Gauge = null;
            MiniTaiko = null;
            Footer = null;

            NowPlayingSong = null;
            ctGameCounter = null;
            ctJudgeDisplayCounter = null;
            judgeFontRender = null;
        }

        private void CleanupResources()
        {
            // Dispose of the sound
            if (PlaySound != null)
            {
                if (PlaySound.IsPlaying)
                {
                    PlaySound.Stop();
                }
                PlaySound.Dispose();
                PlaySound = null;
            }

            // Dispose of cached textures
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
            Lane.Draw();

            int notes_x = 522;
            int notes_y = 290;

            #region [ 判定枠 ]

            ResourceLoader.EnsoGame_Notes.Draw(notes_x, notes_y, new Rectangle(0, 0, 195, 195));

            #endregion

            #region [ ノーツ描画 / 処理 ]

            for (int i = NowPlayingSong.SongCourses[NowPlayingCourse].Chips.Count - 1; i >= 0; i--)
            {
                var now_chip = NowPlayingSong.SongCourses[NowPlayingCourse].Chips[i];
                int chip_x = (int)((now_chip.Time - NowGameTime) / (240 / now_chip.BPM) * now_chip.Scroll * 1.5);

                if (now_chip.IsHit)
                    continue;

                if (chip_x >= -200 && chip_x <= 1920)
                {
                    if (now_chip.NoteType == SongData.NoteType.Don)
                    {
                        ResourceLoader.EnsoGame_Notes.Draw(notes_x + chip_x, notes_y, new Rectangle(195, 0, 195, 195));
                    }
                    else if (now_chip.NoteType == SongData.NoteType.Ka)
                    {
                        ResourceLoader.EnsoGame_Notes.Draw(notes_x + chip_x, notes_y, new Rectangle(195 * 2, 0, 195, 195));
                    }
                    else if (now_chip.NoteType == SongData.NoteType.DON)
                    {
                        ResourceLoader.EnsoGame_Notes.Draw(notes_x + chip_x, notes_y, new Rectangle(195 * 3, 0, 195, 195));
                    }
                    else if (now_chip.NoteType == SongData.NoteType.KA)
                    {
                        ResourceLoader.EnsoGame_Notes.Draw(notes_x + chip_x, notes_y, new Rectangle(195 * 4, 0, 195, 195));
                    }
                }

                if (now_chip.NoteType == SongData.NoteType.RollStart || now_chip.NoteType == SongData.NoteType.RollBigStart || now_chip.NoteType == SongData.NoteType.BalloonStart)
                {
                    int chip_end_x = (int)((now_chip.RollEnd.Time - NowGameTime) / (240 / now_chip.BPM) * now_chip.Scroll * 1.5);

                    if (now_chip.NoteType == SongData.NoteType.RollStart)
                    {
                        ResourceLoader.EnsoGame_Notes.ScaleX = chip_end_x - chip_x - 195 / 2;
                        ResourceLoader.EnsoGame_Notes.Draw(notes_x + chip_x + 195 / 2, notes_y, new Rectangle(195 * 6, 0, 1, 195));

                        ResourceLoader.EnsoGame_Notes.ScaleX = 1;
                        ResourceLoader.EnsoGame_Notes.Draw(notes_x + chip_x, notes_y, new Rectangle(195 * 5, 0, 195, 195));
                        ResourceLoader.EnsoGame_Notes.Draw(notes_x + chip_end_x, notes_y, new Rectangle(195 * 7, 0, 195, 195));
                    }
                    else if (now_chip.NoteType == SongData.NoteType.RollBigStart)
                    {
                        ResourceLoader.EnsoGame_Notes.ScaleX = chip_end_x - chip_x - 195 / 2;
                        ResourceLoader.EnsoGame_Notes.Draw(notes_x + chip_x + 195 / 2, notes_y, new Rectangle(195 * 9, 0, 1, 195));

                        ResourceLoader.EnsoGame_Notes.ScaleX = 1;
                        ResourceLoader.EnsoGame_Notes.Draw(notes_x + chip_x, notes_y, new Rectangle(195 * 8, 0, 195, 195));
                        ResourceLoader.EnsoGame_Notes.Draw(notes_x + chip_end_x, notes_y, new Rectangle(195 * 10, 0, 195, 195));
                    }
                    else if (now_chip.NoteType == SongData.NoteType.BalloonStart)
                    {
                        if (chip_x >= 0)
                        {
                            ResourceLoader.EnsoGame_Notes.Draw(notes_x + chip_x, notes_y, new Rectangle(195 * 11, 0, 195 * 2, 195));
                        }
                        else if (chip_x <= 0 && chip_end_x >= 0)
                        {
                            ResourceLoader.EnsoGame_Notes.Draw(notes_x, notes_y, new Rectangle(195 * 11, 0, 195 * 2, 195));
                        }
                        else
                        {
                            ResourceLoader.EnsoGame_Notes.Draw(notes_x + chip_end_x, notes_y, new Rectangle(195 * 11, 0, 195 * 2, 195));
                        }
                    }
                }

                if (chip_x <= 0)
                {
                    if (IsAuto)
                    {
                        if (!now_chip.IsHit)
                        {
                            if (now_chip.NoteType == SongData.NoteType.Don || now_chip.NoteType == SongData.NoteType.DON)
                            {
                                ResourceLoader.sndDon.Play();
                                now_chip.IsHit = true;
                            }
                            if (now_chip.NoteType == SongData.NoteType.Ka || now_chip.NoteType == SongData.NoteType.KA)
                            {
                                ResourceLoader.sndKa.Play();
                                now_chip.IsHit = true;
                            }
                        }
                    }
                    else if (chip_x < -JUDGMENT_MISS_RANGE)
                    {
                        // ノートを見逃した時
                        if (!now_chip.IsHit && IsNormalNote(now_chip.NoteType))
                        {
                            currentCombo = 0;
                            now_chip.IsHit = true;

                            // 見逃し表示
                            currentJudgment = Judgment.Bad;
                            ctJudgeDisplayCounter.Reset();
                            ctJudgeDisplayCounter.Start();
                        }
                    }
                }
            }

            #endregion

            MiniTaiko.Draw();

            #region [ 判定表示 ]
            if (ctJudgeDisplayCounter.State == TimerState.Started && ctJudgeDisplayCounter.Value < ctJudgeDisplayCounter.End)
            {
                Color judgementColor;
                switch (currentJudgment)
                {
                    case Judgment.Perfect: // 良
                        judgementColor = Color.Gold;
                        break;
                    case Judgment.Ok: // 可
                        judgementColor = Color.White;
                        break;
                    case Judgment.Bad: // 不可
                        judgementColor = Color.Red;
                        break;
                    default:
                        judgementColor = Color.White;
                        break;
                }

                string judgementText = GetJudgmentText(currentJudgment);

                // 判定表示
                judgeFontRender.ForeColor = judgementColor;

                // キャッシュからテクスチャを取得または作成
                string judgmentKey = $"judgment_{currentJudgment}";
                Texture judgmentTexture;

                if (!textureCache.TryGetValue(judgmentKey, out judgmentTexture) || judgmentTexture == null)
                {
                    judgmentTexture = judgeFontRender.GetTexture(judgementText);
                    textureCache[judgmentKey] = judgmentTexture;
                }

                judgmentTexture.Draw(630, 220);
            }

            // コンボ表示
            if (currentCombo > 1)
            {
                judgeFontRender.ForeColor = Color.White;

                // コンボテキストはコンボ数が変わるたびに更新が必要
                string comboKey = $"combo_{currentCombo}";
                Texture comboTexture;

                if (!textureCache.TryGetValue(comboKey, out comboTexture) || comboTexture == null)
                {
                    comboTexture = judgeFontRender.GetTexture($"{currentCombo} コンボ");

                    // 古いコンボテクスチャをクリーンアップ
                    foreach (var key in new List<string>(textureCache.Keys))
                    {
                        if (key.StartsWith("combo_") && key != comboKey)
                        {
                            if (textureCache[key] != null)
                            {
                                textureCache[key].Dispose();
                            }
                            textureCache.Remove(key);
                        }
                    }

                    textureCache[comboKey] = comboTexture;
                }

                comboTexture.Draw(630, 260);
            }
            #endregion

            Footer.Draw();

            base.Draw();
        }

        public override void Update()
        {
            Lane.Update();
            MiniTaiko.Update();

            ctGameCounter.Tick();
            NowGameTime = ctGameCounter.Value + (long)(NowPlayingSong.Header.OFFSET * 1000);

            if (ctJudgeDisplayCounter.State == TimerState.Started)
            {
                ctJudgeDisplayCounter.Tick();
            }

            if (!IsAuto)
            {
                // Don note hit
                if (KeyBind.IsPushedSystemKey(KeyBind.DON_LEFT_1P) || KeyBind.IsPushedSystemKey(KeyBind.DON_RIGHT_1P))
                {
                    ResourceLoader.sndDon.Play();
                    ProcessNoteHit(true); // true = Don hit
                }
                // Ka note hit
                if (KeyBind.IsPushedSystemKey(KeyBind.KA_LEFT_1P) || KeyBind.IsPushedSystemKey(KeyBind.KA_RIGHT_1P))
                {
                    ResourceLoader.sndKa.Play();
                    ProcessNoteHit(false); // false = Ka hit
                }
            }
            else
            {
                if (KeyBind.IsPushedSystemKey(KeyBind.DON_LEFT_1P) || KeyBind.IsPushedSystemKey(KeyBind.DON_RIGHT_1P))
                {
                    //ResourceLoader.sndDon.Play();
                }
                if (KeyBind.IsPushedSystemKey(KeyBind.KA_LEFT_1P) || KeyBind.IsPushedSystemKey(KeyBind.KA_RIGHT_1P))
                {
                    //ResourceLoader.sndKa.Play();
                }
            }

            if (!isMusicPlayed)
            {
                if (ctGameCounter.Value >= 0)
                {
                    if (PlaySound != null && !PlaySound.IsPlaying)
                    {
                        PlaySound.Play(true);
                    }
                    isMusicPlayed = true;
                }
            }

            base.Update();
        }

        /// <summary>
        /// 通常ノートかどうかをチェックする
        /// </summary>
        private bool IsNormalNote(SongData.NoteType noteType)
        {
            return noteType == SongData.NoteType.Don ||
                   noteType == SongData.NoteType.Ka ||
                   noteType == SongData.NoteType.DON ||
                   noteType == SongData.NoteType.KA;
        }

        /// <summary>
        /// ドンのノートかどうかをチェックする
        /// </summary>
        private bool IsDonNote(SongData.NoteType noteType)
        {
            return noteType == SongData.NoteType.Don || noteType == SongData.NoteType.DON;
        }

        /// <summary>
        /// カのノートかどうかをチェックする
        /// </summary>
        private bool IsKaNote(SongData.NoteType noteType)
        {
            return noteType == SongData.NoteType.Ka || noteType == SongData.NoteType.KA;
        }

        /// <summary>
        /// ノートヒット処理
        /// </summary>
        private void ProcessNoteHit(bool isDon)
        {
            SongData.Chip nearestChip = null;
            int nearestDistance = int.MaxValue;

            // 判定範囲内の最も近いノートを探す
            for (int i = 0; i < NowPlayingSong.SongCourses[NowPlayingCourse].Chips.Count; i++)
            {
                var chip = NowPlayingSong.SongCourses[NowPlayingCourse].Chips[i];

                if (chip.IsHit || !IsNormalNote(chip.NoteType))
                    continue;

                // ノートの時間差を計算
                int timeDistance = (int)(chip.Time - NowGameTime);

                // 判定範囲外ならスキップ
                if (Math.Abs(timeDistance) > JUDGMENT_MISS_RANGE)
                    continue;

                // ノートタイプが一致しているかチェック
                bool isCorrectNote = (isDon && IsDonNote(chip.NoteType)) ||
                                    (!isDon && IsKaNote(chip.NoteType));

                if (!isCorrectNote)
                    continue;

                // より近いノートを見つけた場合、更新
                if (Math.Abs(timeDistance) < Math.Abs(nearestDistance))
                {
                    nearestChip = chip;
                    nearestDistance = timeDistance;
                }
            }

            // 最も近いノートが見つかった場合
            if (nearestChip != null)
            {
                // 判定の決定
                Judgment judgment;

                int timeDistance = Math.Abs(nearestDistance);

                if (timeDistance <= JUDGMENT_PERFECT_RANGE)
                {
                    judgment = Judgment.Perfect; // 良
                    currentCombo++;
                }
                else if (timeDistance <= JUDGMENT_OK_RANGE)
                {
                    judgment = Judgment.Ok; // 可
                    currentCombo++;
                }
                else
                {
                    judgment = Judgment.Bad; // 不可
                    currentCombo = 0;
                }

                // 判定結果の表示
                currentJudgment = judgment;
                ctJudgeDisplayCounter.Reset();
                ctJudgeDisplayCounter.Start();

                // ノートを叩いたのでフラグを立てる
                nearestChip.IsHit = true;
            }
        }

        /// <summary>
        /// 判定に応じたテキストを返す
        /// </summary>
        private string GetJudgmentText(Judgment judgment)
        {
            switch (judgment)
            {
                case Judgment.Perfect:
                    return "良";
                case Judgment.Ok:
                    return "可";
                case Judgment.Bad:
                    return "不可";
                default:
                    return "";
            }
        }

        // 判定種類
        private enum Judgment
        {
            None,
            Perfect, // 良
            Ok,      // 可
            Bad      // 不可
        }

        // 判定タイミング (ミリ秒単位)
        private const int JUDGMENT_PERFECT_RANGE = 25;  // 良判定の範囲（±34ms）
        private const int JUDGMENT_OK_RANGE = 75;       // 可判定の範囲（±84ms）
        private const int JUDGMENT_MISS_RANGE = 108;    // 見逃し判定の範囲（±134ms）

        private bool IsAuto = false; // オートプレイをオフにする
        private Counter ctGameCounter;
        private Counter ctJudgeDisplayCounter;
        private long NowGameTime;
        private Judgment currentJudgment = Judgment.None;
        private int currentCombo = 0;
        private FontRender judgeFontRender;

        private bool isMusicPlayed;
        private Dictionary<string, Texture> textureCache;

        public EnsoGame_Lane Lane;
        public EnsoGame_Gauge Gauge;
        public EnsoGame_MiniTaiko MiniTaiko;
        public EnsoGame_Footer Footer;
    }
}
