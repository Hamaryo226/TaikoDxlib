using Amaoto.Animation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static TaikoNaut.TaikoNaut.SongSystem.SongData;

namespace TaikoNaut.TaikoNaut.SongSystem
{
    internal class TJA
    {
        private bool IsParsing = false;
        private int NowCourse = 0;
        private double NowTime = 0;
        private double NowBPM = 0;
        private double NowScroll = 1.0;
        private double NowMeasure = 1.0;
        private bool NowBarVisible = true;
        private bool NowGoGo = false;
        private int NowMeasureNotesCount = 0;
        private int NowBalloonCount;
        private SongData.Branch NowBranch;
        private SongData.Chip NowLongStartNote;
        private List<SongData.Chip> NowMeasureChips;

        public int GetCourseFromData(string data)
        {
            string[] courses = { "easy", "normal", "hard", "oni", "edit" };
            int ret = 0;
            var result = 0;

            if(!int.TryParse(data, out result))
            {
                data = data.ToLower();

                for(int i = 0; i < 5; i++)
                {
                    if (courses[i] == data)
                    {
                        ret = i;
                        break;
                    }
                }
            }
            else
            {
                ret = result;
            }

            return ret;
        }

        public SongData.Song GetSongDataFromTJA(string Path)
        {
            SongData.Song song = new SongData.Song();
            song.Header = new SongData.SongHeader();
            song.SongCourses = new SongData.SongCourse[5];

            song.Header.Path = Path;

            //まずはファイルパスから譜面データを読み込む
            StreamReader sr = new StreamReader(Path, Encoding.GetEncoding("Shift-JIS"));
            string data = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();

            string[] delimiter = { "\n" };
            string[] data_lines = data.Replace(Environment.NewLine, "\n").Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            foreach(string line in data_lines)
            {
                if(!IsParsing)
                {
                    string[] line_split = line.Split(':');
                    string Parameter = line_split.Length >= 2 ? line_split[1] : "";

                    if (line.StartsWith("TITLE:"))
                    {
                        song.Header.Title = Parameter;
                    }
                    else if (line.StartsWith("SUBTITLE:"))
                    {
                        if (Parameter.StartsWith("--") || Parameter.StartsWith("++"))
                            Parameter = Parameter.Remove(0, 2);

                        song.Header.SubTitle = Parameter;
                    }
                    else if (line.StartsWith("BPM:"))
                    {
                        song.Header.BPM = double.Parse(Parameter);
                    }
                    else if (line.StartsWith("WAVE:"))
                    {
                        song.Header.Wave = Parameter;
                    }
                    else if (line.StartsWith("DEMOSTART:"))
                    {
                        song.Header.DemoStart = double.Parse(Parameter);
                    }
                    else if (line.StartsWith("BPM:"))
                    {
                        song.Header.BPM = double.Parse(Parameter);
                    }
                    else if (line.StartsWith("OFFSET:"))
                    {
                        song.Header.OFFSET = double.Parse(Parameter);
                    }
                    else if (line.StartsWith("GENRE:"))
                    {
                        song.Header.Genre = Parameter;
                    }
                    else if (line.StartsWith("COURSE:"))
                    {
                        int course = GetCourseFromData(Parameter);
                        song.SongCourses[course] = new SongData.SongCourse();
                        song.SongCourses[course].Chips = new List<Chip>();
                        this.NowCourse = course;
                    }
                    else if (line.StartsWith("LEVEL:"))
                    {
                        song.SongCourses[NowCourse].Level = int.Parse(Parameter);
                    }
                    else if (line.StartsWith("BALLOON:"))
                    {
                        string[] balloon_counts = Parameter.Split(',');
                        song.SongCourses[NowCourse].Balloon = new List<int>();
                        foreach (var count in balloon_counts)
                        {
                            song.SongCourses[NowCourse].Balloon.Add(int.Parse(count));
                        }
                    }
                    else if(line.StartsWith("#START"))
                    {
                        this.IsParsing = true;
                        this.NowTime = 0;
                        this.NowBPM = song.Header.BPM;
                        this.NowScroll = 1.0;
                        this.NowMeasure = 1.0;
                        this.NowBarVisible = true;
                        this.NowBalloonCount = 0;
                        this.NowGoGo = false;
                        NowMeasureNotesCount = 0;
                        NowBranch = SongData.Branch.Normal;
                        this.NowMeasureChips = new List<Chip>();
                    }
                }
                else
                {
                    //コマンド行以外(=ノーツ)を先に処理
                    if(!line.StartsWith("#"))
                    {
                        string note_line = line.Split(new String[] { "//" }, StringSplitOptions.None)[0];

                        double TimePerNotes = (15000.0 / NowBPM);

                        for (int i = 0; i < note_line.Length; i++)
                        {
                            if (!((note_line[i] >= '0' && note_line[i] <= '9') || note_line[i] == ','))
                                continue;

                            if(note_line[i] == ',')
                            {
                                if(NowMeasureChips.Count == 0)
                                {
                                    NowTime += (15000.0 / NowBPM / NowMeasure) * 16.0;
                                }
                                else
                                {
                                    for (int j = 0; j < NowMeasureChips.Count; j++)
                                    {
                                        if (this.NowMeasureChips[j].NoteType != NoteType.None)
                                        {
                                            this.NowMeasureChips[j].Time = NowTime;
                                            song.SongCourses[NowCourse].Chips.Add(this.NowMeasureChips[j]);

                                            var chiplist = song.SongCourses[NowCourse].Chips;
                                            var nowchip = chiplist[chiplist.Count - 1];

                                            if (nowchip.NoteType == SongData.NoteType.RollStart || nowchip.NoteType == SongData.NoteType.RollBigStart || nowchip.NoteType == SongData.NoteType.BalloonStart)
                                            {
                                                NowLongStartNote = song.SongCourses[NowCourse].Chips[chiplist.Count - 1];
                                            }
                                            else if (nowchip.NoteType == NoteType.RollEnd)
                                            {
                                                NowLongStartNote.RollEnd = song.SongCourses[NowCourse].Chips[chiplist.Count - 1];
                                            }
                                        }
                                        this.NowTime += (15000.0 / this.NowMeasureChips[j].BPM / this.NowMeasureChips[j].Measure) * (16.0 / this.NowMeasureChips.Count);

                                    }
                                    this.NowMeasureChips.Clear();
                                }

                                SongData.Chip measure = new SongData.Chip();
                                measure.NoteType = SongData.NoteType.Measure;
                                measure.Time = NowTime;
                                measure.BPM = NowBPM;
                                measure.Scroll = NowScroll;
                                measure.Measure = NowMeasure;
                                measure.Branch = NowBranch;
                                measure.IsBarVisible = NowBarVisible;
                                song.SongCourses[NowCourse].Chips.Add(measure);
                            }
                            else
                            {
                                //通常ノーツ

                                SongData.Chip chip = new SongData.Chip();
                                chip.NoteType = (SongData.NoteType)(int.Parse(note_line[i].ToString()));
                                chip.Time = NowTime;
                                chip.BPM = NowBPM;
                                chip.Branch = NowBranch;
                                chip.Measure = NowMeasure;
                                chip.Scroll = NowScroll;
                                chip.IsHit = false;

                                if(chip.NoteType == SongData.NoteType.BalloonStart)
                                {
                                    //配列が足りていなかったらとりあえず3打くらい加えとく
                                    if(song.SongCourses[NowCourse].Balloon.Count >= NowBalloonCount)
                                        chip.Balloon_Count = 3;
                                    else
                                        chip.Balloon_Count = song.SongCourses[NowCourse].Balloon[NowBalloonCount];

                                    NowBalloonCount++;
                                }

                                this.NowMeasureChips.Add(chip);
                            }
                        }
                    }
                    else
                    {
                        string[] line_split = line.Split(' ');
                        string Parameter = line_split.Length >= 2 ? line_split[1] : "";

                        if (line.StartsWith("#END"))
                        {
                            IsParsing = false;
                        }
                        else if(line.StartsWith("#GOGOSTART"))
                        {
                            NowGoGo = true;
                        }
                        else if (line.StartsWith("#GOGOEND"))
                        {
                            NowGoGo = false;
                        }
                        else if (line.StartsWith("#SCROLL"))
                        {
                            NowScroll = double.Parse(Parameter);
                        }
                        else if (line.StartsWith("#MEASURE"))
                        {
                            string numerator = Parameter.Split('/')[1];
                            string denominator = Parameter.Split('/')[0];
                            NowMeasure = double.Parse(numerator) / double.Parse(denominator);
                        }
                        else if(line.StartsWith("#BPMCHANGE"))
                        {
                            NowBPM = double.Parse(Parameter);
                        }
                        else if (line.StartsWith("#BARLINEOFF"))
                        {
                            NowBarVisible = false;
                        }
                        else if (line.StartsWith("#BARLINEON"))
                        {
                            NowBarVisible = true;
                        }
                    }
                }
            }

            return song;
        }
    }
}
