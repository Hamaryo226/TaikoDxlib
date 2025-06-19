using Amaoto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaikoDxlib.TaikoDxlib.Common;

namespace TaikoDxlib.TaikoDxlib.Scenes.EnsoGame
{
    internal class EnsoGame_Lane : Scene
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
            ResourceLoader.EnsoGame_Lane_Background_1P.Draw(497, 204);

            base.Draw();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
