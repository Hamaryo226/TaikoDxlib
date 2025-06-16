using Amaoto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaikoNaut.TaikoNaut.Common;

namespace TaikoNaut.TaikoNaut.Scenes.EnsoGame
{
    internal class EnsoGame_MiniTaiko : Scene
    {
        public override void Enable()
        {
            base.Enable();
        }

        public override void Disable()
        {
            base.Disable();
        }

        public override void Draw()
        {
            ResourceLoader.EnsoGame_MiniTaiko_Background_1P.Draw(0, 276);

            base.Draw();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
