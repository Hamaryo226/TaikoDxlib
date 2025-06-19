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
    internal class EnsoGame_MiniTaiko : Scene
    {
        public override void Enable()
        {
            // Initialize counters for animation duration
            ctLeftDon = new Counter(0, 100, 1000, false);
            ctRightDon = new Counter(0, 100, 1000, false);
            ctLeftKa = new Counter(0, 100, 1000, false);
            ctRightKa = new Counter(0, 100, 1000, false);
            
            base.Enable();
        }

        public override void Disable()
        {
            base.Disable();
        }

        public override void Draw()
        {
            // Draw background
            ResourceLoader.EnsoGame_MiniTaiko_Background_1P.Draw(0, 276);
            
            // Draw base taiko
            ResourceLoader.EnsoGame_MiniTaiko_Taiko.Draw(315, 310);
            
            // Get taiko width
            int taikoWidth = 130; // Estimated width, adjust as needed based on actual texture

            // Check hits and draw animations
            bool leftDonHit = KeyBind.IsPushedSystemKey(KeyBind.DON_LEFT_1P);
            bool rightDonHit = KeyBind.IsPushedSystemKey(KeyBind.DON_RIGHT_1P);
            bool leftKaHit = KeyBind.IsPushedSystemKey(KeyBind.KA_LEFT_1P);
            bool rightKaHit = KeyBind.IsPushedSystemKey(KeyBind.KA_RIGHT_1P);

            // Handle don hits (start animation counters)
            if (leftDonHit && ctLeftDon.State != TimerState.Started)
            {
                ctLeftDon.Reset();
                ctLeftDon.Start();
            }
            
            if (rightDonHit && ctRightDon.State != TimerState.Started)
            {
                ctRightDon.Reset();
                ctRightDon.Start();
            }
            
            // Handle ka hits (start animation counters)
            if (leftKaHit && ctLeftKa.State != TimerState.Started)
            {
                ctLeftKa.Reset();
                ctLeftKa.Start();
            }
            
            if (rightKaHit && ctRightKa.State != TimerState.Started)
            {
                ctRightKa.Reset();
                ctRightKa.Start();
            }
            
            // Draw left don hit animation
            if (ctLeftDon.State == TimerState.Started && ctLeftDon.Value < ctLeftDon.End)
            {
                // Draw left half of Don texture
                ResourceLoader.EnsoGame_MiniTaiko_Taiko_Don.Draw(
                    315, 310, 
                    new Rectangle(0, 0, taikoWidth, ResourceLoader.EnsoGame_MiniTaiko_Taiko_Don.TextureSize.Height / 2));
            }
            
            // Draw right don hit animation
            if (ctRightDon.State == TimerState.Started && ctRightDon.Value < ctRightDon.End)
            {
                // Draw right half of Don texture
                ResourceLoader.EnsoGame_MiniTaiko_Taiko_Don.Draw(
                    315, 310, 
                    new Rectangle(0,190,168,190));
            }
            
            // Draw left ka hit animation
            if (ctLeftKa.State == TimerState.Started && ctLeftKa.Value < ctLeftKa.End)
            {
                // Draw left half of Ka texture
                ResourceLoader.EnsoGame_MiniTaiko_Taiko_Ka.Draw(
                    315, 310,
                    new Rectangle(0, 0, taikoWidth, ResourceLoader.EnsoGame_MiniTaiko_Taiko_Ka.TextureSize.Height / 2));
            }
            
            // Draw right ka hit animation
            if (ctRightKa.State == TimerState.Started && ctRightKa.Value < ctRightKa.End)
            {
                // Draw right half of Ka texture
                ResourceLoader.EnsoGame_MiniTaiko_Taiko_Ka.Draw(
                    315, 310,
                    new Rectangle(0, 190, 168, 190));
            }

            base.Draw();
        }

        public override void Update()
        {
            // Update animation counters
            if (ctLeftDon.State == TimerState.Started)
                ctLeftDon.Tick();
                
            if (ctRightDon.State == TimerState.Started)
                ctRightDon.Tick();
                
            if (ctLeftKa.State == TimerState.Started)
                ctLeftKa.Tick();
                
            if (ctRightKa.State == TimerState.Started)
                ctRightKa.Tick();
                
            base.Update();
        }
        
        // Animation counters
        private Counter ctLeftDon;
        private Counter ctRightDon;
        private Counter ctLeftKa;
        private Counter ctRightKa;
    }
}
