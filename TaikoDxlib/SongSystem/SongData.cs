using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaikoDxlib.TaikoDxlib.SongSystem
{
    internal class SongData
    {
        public class Song
        {
            public SongHeader Header;
            public SongCourse[] SongCourses;
        }

        public class SongHeader
        {
            public string Path;
            public string Title;
            public string SubTitle;
            public double BPM;
            public string Wave;
            public double OFFSET;
            public int SONGVOL;
            public int SEVOL;
            public double DemoStart;
            public string Genre;
        }

        public class SongCourse
        {
            public int Diffculty;
            public int Level;
            public List<int> Balloon;
            public List<int> Balloon_Normal;
            public List<int> Balloon_Expert;
            public List<int> Balloon_Master;
            public List<Chip> Chips;
        }

        public class Chip
        {
            public double Time;

            public NoteType NoteType;
            public SENoteType SENoteType;
            public Branch Branch;

            public double BPM;
            public double Scroll;
            public double Measure;

            public bool IsHit;

            public bool IsBarVisible;
            public int Balloon_Count;
            public Chip RollEnd;
        }

        public class Command
        {
            public double Time;
            public CommandType CMDType;
        }

        public enum Branch
        {
            Normal,
            Expert,
            Master
        }

        public enum SENoteType
        {
            None,
            Don,
            Do,
            Ko,
            Ka,
            Katsu,
            DON,
            KA,
            RollStart,
            ROLLSTART,
            Rolling,
            RollEnd,
            ROLLEND,
            Balloon
        }

        public enum NoteType
        {
            None,
            Don,
            Ka,
            DON,
            KA,
            RollStart,
            RollBigStart,
            BalloonStart,
            RollEnd,
            Kusudama,
            Measure
        }

        public enum CommandType
        {
            GOGOSTART,
            GOGOEND,
            SCROLL,
            BPMCHANGE,
            MEASURE,
            BARLINEOFF,
            BARLINEON,
            LEVELHOLD,
        }

        public enum Course
        {
            Easy,
            Normal,
            Hard,
            Oni,
            Edit,
            Dan
        }
    }
}
