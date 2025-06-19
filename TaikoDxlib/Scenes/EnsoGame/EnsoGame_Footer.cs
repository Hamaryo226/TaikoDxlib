using Amaoto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaikoDxlib.TaikoDxlib.Common;

namespace TaikoDxlib.TaikoDxlib.Scenes.EnsoGame
{
    internal class EnsoGame_Footer : Scene
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
            ResourceLoader.EnsoGame_Footer.Draw(0, 1015);

            base.Draw();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
