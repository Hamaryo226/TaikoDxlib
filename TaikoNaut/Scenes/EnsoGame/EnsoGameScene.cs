using Amaoto;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaikoNaut.TaikoNaut.Common;
using TaikoNaut.TaikoNaut.Config;
using TaikoNaut.TaikoNaut.SongSystem;

namespace TaikoNaut.TaikoNaut.Scenes.EnsoGame
{
    internal class EnsoGameScene : Scene
    {
        public SongData.Song NowPlayingSong;
        public Sound PlaySound;
        public int NowPlayingCourse = 3;

        public override void Enable()
        {
            ctGameCounter = new Counter(-3000, 100000000000, 1000, false);
            ctGameCounter.Start();

            string TJADirectory = NowPlayingSong.Header.Path.Replace(Path.GetFileName(NowPlayingSong.Header.Path), "\\");
            PlaySound = new Sound(TJADirectory + NowPlayingSong.Header.Wave);

            AddChildScene(Lane = new EnsoGame_Lane());
            AddChildScene(Gauge = new EnsoGame_Gauge());
            AddChildScene(MiniTaiko = new EnsoGame_MiniTaiko());

            base.Enable();
        }

        public override void Disable()
        {
            base.Disable();
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
                        if(chip_x >= 0)
                        {
                            ResourceLoader.EnsoGame_Notes.Draw(notes_x + chip_x, notes_y, new Rectangle(195 * 11, 0, 195 * 2, 195));
                        }
                        else if(chip_x <= 0 && chip_end_x >= 0)
                        {
                            ResourceLoader.EnsoGame_Notes.Draw(notes_x, notes_y, new Rectangle(195 * 11, 0, 195 * 2, 195));
                        }
                        else
                        {
                            ResourceLoader.EnsoGame_Notes.Draw(notes_x + chip_end_x, notes_y, new Rectangle(195 * 11, 0, 195 * 2, 195));
                        }
                    }
                }

                if(chip_x <= 0)
                {
                    if(IsAuto)
                    {
                        if(!now_chip.IsHit)
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
                }
            }

            #endregion

            MiniTaiko.Draw();

            base.Draw();
        }

        public override void Update()
        {
            Lane.Update();
            MiniTaiko.Update();

            ctGameCounter.Tick();
            NowGameTime = ctGameCounter.Value + (long)(NowPlayingSong.Header.OFFSET * 1000);

            if (KeyBind.IsPushedSystemKey(KeyBind.DON_LEFT_1P) || KeyBind.IsPushedSystemKey(KeyBind.DON_RIGHT_1P))
            {
                ResourceLoader.sndDon.Play();
            }
            if (KeyBind.IsPushedSystemKey(KeyBind.KA_LEFT_1P) || KeyBind.IsPushedSystemKey(KeyBind.KA_RIGHT_1P))
            {
                ResourceLoader.sndKa.Play();
            }

            if(!isMusicPlayed)
            {
                if(ctGameCounter.Value >= 0)
                {
                    this.PlaySound.Play(true);
                    isMusicPlayed = true;
                }
            }

            base.Update();
        }

        private bool IsAuto = true;

        private Counter ctGameCounter;
        private long NowGameTime;

        private bool isMusicPlayed;

        public EnsoGame_Lane Lane;
        public EnsoGame_Gauge Gauge;
        public EnsoGame_MiniTaiko MiniTaiko;
    }
}
