using Amaoto;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaikoDxlib.TaikoDxlib.Common;
using TaikoDxlib.TaikoDxlib.Config;

namespace TaikoDxlib.TaikoDxlib.Scenes.EnsoGame
{
    internal class EnsoGame_Lane : Scene, IDisposable
    {
        private readonly int judgePositionX = 522;
        private readonly int judgePositionY = 290;
        private readonly int judgeSize = 195;

        public override void Enable()
        {
            // Initialize counters for animation duration
            ctDon = new Counter(0, 100, 1000, false);
            ctKa = new Counter(0, 100, 1000, false);
            ctGo = new Counter(0, 100, 1000, false);

            base.Enable();
        }

        public override void Disable()
        {
            base.Disable();
        }

        public void Dispose()
        {
            // Clean up resources
            ctDon = null;
            ctKa = null;
            ctGo = null;
        }

        public override void Draw()
        {
            // Check hits and draw animations
            bool leftDonHit = KeyBind.IsPushedSystemKey(KeyBind.DON_LEFT_1P);
            bool rightDonHit = KeyBind.IsPushedSystemKey(KeyBind.DON_RIGHT_1P);
            bool leftKaHit = KeyBind.IsPushedSystemKey(KeyBind.KA_LEFT_1P);
            bool rightKaHit = KeyBind.IsPushedSystemKey(KeyBind.KA_RIGHT_1P);

            // Draw the lane background first
            ResourceLoader.EnsoGame_Lane_Background_1P.Draw(497, 204);

            // Handle don hits (start animation counters)
            if ((leftDonHit || rightDonHit) && ctDon.State != TimerState.Started)
            {
                ctDon.Reset();
                ctDon.Start();
            }

            // Handle ka hits (start animation counters)
            if ((leftKaHit || rightKaHit) && ctKa.State != TimerState.Started)
            {
                ctKa.Reset();
                ctKa.Start();
            }

            // Draw hit animations on top of the lane background
            // Calculate center of judge position
            int centerX = judgePositionX + (judgeSize / 2);
            int centerY = judgePositionY + (judgeSize / 2);

            // Draw don hit animation using alpha blending and scaling
            if (ctDon.State == TimerState.Started && ctDon.Value < ctDon.End)
            {
                // Calculate animation progress
                double progress = (double)ctDon.Value / ctDon.End;

                // Apply scaling and fading effects for a better visual
                double scale = 1.0 + progress * 0.5; // Slightly grow as the animation progresses
                double opacity = 1.0 - progress;     // Fade out

                // Store original properties
                double originalScaleX = ResourceLoader.EnsoGame_Lane_Don.ScaleX;
                double originalScaleY = ResourceLoader.EnsoGame_Lane_Don.ScaleY;
                double originalOpacity = ResourceLoader.EnsoGame_Lane_Don.Opacity;
                ReferencePoint originalRefPoint = ResourceLoader.EnsoGame_Lane_Don.ReferencePoint;

                // Set properties for this frame
                //ResourceLoader.EnsoGame_Lane_Don.ScaleX = scale;
                //ResourceLoader.EnsoGame_Lane_Don.ScaleY = scale;
                ResourceLoader.EnsoGame_Lane_Don.Opacity = opacity;
                ResourceLoader.EnsoGame_Lane_Don.ReferencePoint = ReferencePoint.Center;

                // Draw the effect centered on the judge position
                ResourceLoader.EnsoGame_Lane_Don.Draw(1212, 386);

                // Restore original properties
                ResourceLoader.EnsoGame_Lane_Don.ScaleX = originalScaleX;
                ResourceLoader.EnsoGame_Lane_Don.ScaleY = originalScaleY;
                ResourceLoader.EnsoGame_Lane_Don.Opacity = originalOpacity;
                ResourceLoader.EnsoGame_Lane_Don.ReferencePoint = originalRefPoint;
            }

            // Draw ka hit animation with similar effects
            if (ctKa.State == TimerState.Started && ctKa.Value < ctKa.End)
            {
                // Calculate animation progress
                double progress = (double)ctKa.Value / ctKa.End;

                // Apply scaling and fading effects
                double scale = 1.0 + progress * 0.5;
                double opacity = 1.0 - progress;

                // Store original properties
                double originalScaleX = ResourceLoader.EnsoGame_Lane_Ka.ScaleX;
                double originalScaleY = ResourceLoader.EnsoGame_Lane_Ka.ScaleY;
                double originalOpacity = ResourceLoader.EnsoGame_Lane_Ka.Opacity;
                ReferencePoint originalRefPoint = ResourceLoader.EnsoGame_Lane_Ka.ReferencePoint;

                // Set properties for this frame
                //ResourceLoader.EnsoGame_Lane_Ka.ScaleX = scale;
                //ResourceLoader.EnsoGame_Lane_Ka.ScaleY = scale;
                ResourceLoader.EnsoGame_Lane_Ka.Opacity = opacity;
                ResourceLoader.EnsoGame_Lane_Ka.ReferencePoint = ReferencePoint.Center;

                // Draw the effect centered on the judge position
                ResourceLoader.EnsoGame_Lane_Ka.Draw(1212, 386);

                // Restore original properties
                ResourceLoader.EnsoGame_Lane_Ka.ScaleX = originalScaleX;
                ResourceLoader.EnsoGame_Lane_Ka.ScaleY = originalScaleY;
                ResourceLoader.EnsoGame_Lane_Ka.Opacity = originalOpacity;
                ResourceLoader.EnsoGame_Lane_Ka.ReferencePoint = originalRefPoint;
            }

            base.Draw();
        }

        public override void Update()
        {
            // Update animation counters
            if (ctDon.State == TimerState.Started)
                ctDon.Tick();

            if (ctKa.State == TimerState.Started)
                ctKa.Tick();

            if (ctGo.State == TimerState.Started)
                ctGo.Tick();

            base.Update();
        }

        // Animation counters
        private Counter ctDon;
        private Counter ctKa;
        private Counter ctGo;
    }
}
